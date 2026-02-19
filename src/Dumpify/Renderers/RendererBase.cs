using Dumpify.Descriptors;
using Dumpify.Descriptors.ValueProviders;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Dumpify;

internal abstract class RendererBase<TRenderable, TState> : IRenderer, IRendererHandler<TRenderable, TState>
{
    private readonly ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<TRenderable>>> _customTypeRenderers;

    protected RendererBase(ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<TRenderable>>>? customTypeRenderers)
    {
        _customTypeRenderers = customTypeRenderers ?? new ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<TRenderable>>>();
    }

    protected void AddCustomTypeDescriptor(ICustomTypeRenderer<TRenderable> handler)
    {
        _customTypeRenderers.AddOrUpdate(handler.DescriptorType.TypeHandle, new List<ICustomTypeRenderer<TRenderable>>() { handler }, (k, list) =>
        {
            list.Add(handler);
            return list;
        });
    }

    public IRenderedObject Render(object? obj, IDescriptor? descriptor, RendererConfig config)
    {
        var state = CreateState(obj, descriptor, config);
        var idGenerator = new ObjectIdReferenceTracker();
        var ancestors = ImmutableHashSet.Create<object>(ReferenceEqualityComparer.Instance);
        
        // Get the strategy to determine what needs to be tracked
        var strategy = config.ReferenceRenderingStrategy;
        
        // Initialize tracking collections based on what strategy needs
        var objectIds = strategy.TracksIds
            ? ImmutableDictionary.Create<object, int>(ReferenceEqualityComparer.Instance)
            : ImmutableDictionary<object, int>.Empty;
        var objectPaths = strategy.TracksPaths
            ? ImmutableDictionary.Create<object, string>(ReferenceEqualityComparer.Instance)
            : ImmutableDictionary<object, string>.Empty;

        var context = new RenderContext<TState>(
            config, idGenerator, 0, obj, null, state, ancestors,
            CurrentPath: "",
            ObjectIds: objectIds,
            ObjectPaths: objectPaths);

        var renderable = obj switch
        {
            null => RenderNullValue(descriptor, context),
            _ => RenderDescriptor(obj, descriptor, context),
        };

        return CreateRenderedObject(renderable);
    }

    public TRenderable RenderDescriptor(object? @object, IDescriptor? descriptor, RenderContext<TState> context)
    {
        if (@object is null)
        {
            return RenderNullValue(descriptor, context);
        }

        if (context.Config.MaxDepth is not null && context.CurrentDepth > context.Config.MaxDepth)
        {
            return RenderExceededDepth(@object, descriptor, context);
        }

        // Get the actual descriptor for this object instance
        descriptor = GetObjectInstanceDescriptor(@object, descriptor, context);

        // Unwrap CircularDependencyDescriptor before cycle checking
        // (This is a compile-time sentinel for recursive types, not a runtime cycle)
        while (descriptor is CircularDependencyDescriptor circularDescriptor)
        {
            descriptor = circularDescriptor.Descriptor;
        }

        // Cycle and shared reference detection for reference types
        // Exclude strings - they are immutable and often interned, so the same instance appearing
        // multiple times is not a cycle (and is expected behavior)
        var objectType = @object.GetType();
        var shouldTrack = !objectType.IsValueType && objectType != typeof(string);
        
        if (shouldTrack)
        {
            var strategy = context.Config.ReferenceRenderingStrategy;
            
            // Check for cycle (already in ancestor chain)
            if (context.Ancestors.Contains(@object))
            {
                var id = strategy.TracksIds && context.ObjectIds.TryGetValue(@object, out var objId) ? objId : (int?)null;
                var path = strategy.TracksPaths && context.ObjectPaths.TryGetValue(@object, out var objPath) ? objPath : null;
                
                var label = strategy.FormatCircularReference(objectType, id, path);
                return RenderCircularDependency(@object, descriptor, label, context);
            }
            
            // Check for shared reference (seen before but not in ancestor chain)
            if (strategy.TracksSharedReferences && context.ObjectIds.ContainsKey(@object))
            {
                var id = context.ObjectIds[@object];
                var path = strategy.TracksPaths ? context.ObjectPaths[@object] : null;
                
                var label = strategy.FormatSharedReference(objectType, id, path);
                if (label != null)
                {
                    return RenderSharedReference(@object, descriptor, label, context);
                }
                // else: label is null, meaning render fully
            }
            
            // First time seeing this object - update tracking
            var newContext = context with { Ancestors = context.Ancestors.Add(@object) };
            
            if (strategy.TracksIds && !context.ObjectIds.ContainsKey(@object))
            {
                var newId = context.ObjectIds.Count + 1;
                newContext = newContext with { ObjectIds = context.ObjectIds.Add(@object, newId) };
            }
            if (strategy.TracksPaths && !context.ObjectPaths.ContainsKey(@object))
            {
                newContext = newContext with { ObjectPaths = context.ObjectPaths.Add(@object, context.CurrentPath) };
            }
            
            context = newContext;
        }

        return descriptor switch
        {
            null => RenderNullDescriptor(@object, context),
            IgnoredDescriptor ignoredDescriptor => TryRenderCustomTypeDescriptor(@object, ignoredDescriptor, context, RenderIgnoredDescriptor),
            SingleValueDescriptor singleDescriptor => TryRenderCustomTypeDescriptor(@object, singleDescriptor, context, RenderSingleValueDescriptor),
            ObjectDescriptor objDescriptor => TryRenderCustomTypeDescriptor(@object, objDescriptor, context, TryRenderObjectDescriptor),
            MultiValueDescriptor multiDescriptor => TryRenderCustomTypeDescriptor(@object, multiDescriptor, context, RenderMultiValueDescriptor),
            LabelDescriptor labelDescriptor => TryRenderCustomTypeDescriptor(@object, labelDescriptor, context, RenderLabelDescriptor),
            CustomDescriptor customDescriptor => TryRenderCustomTypeDescriptor(@object, customDescriptor, context, RenderCustomDescriptor),
            _ => RenderUnsupportedDescriptor(@object, descriptor, context),
        };
    }

    private IDescriptor? GetObjectInstanceDescriptor(object @object, IDescriptor? descriptor, RenderContext<TState> context)
    {
        var type = descriptor?.Type switch
        {
            null => @object.GetType(),
            var elementType when elementType == typeof(Type) || @object.GetType() == elementType => elementType,
            _ => @object.GetType()
        };

        if (type != descriptor?.Type)
        {
            var actualDescriptor = DumpConfig.Default.Generator.Generate(type, null, context.Config.MemberProvider);
            return actualDescriptor;
        }

        return descriptor;
    }

    private TRenderable TryRenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext<TState> context)
    {
        // Cycle check is now handled in RenderDescriptor for all reference types
        return RenderObjectDescriptor(obj, descriptor, context);
    }

    private TRenderable TryRenderCustomTypeDescriptor<TDescriptor>(object obj, TDescriptor descriptor, in RenderContext<TState> context, Func<object, TDescriptor, RenderContext<TState>, TRenderable> defaultRenderer)
        where TDescriptor : IDescriptor
    {
        if (_customTypeRenderers.TryGetValue(descriptor.GetType().TypeHandle, out var renderList))
        {
            var (customRenderer, result) = renderList
                .Select(r => (customRenderer: r, result: r.ShouldHandle(descriptor, obj)))
                .FirstOrDefault(r => r.result.shouldHandle);

            if (result.shouldHandle)
            {
                return customRenderer.Render(descriptor, obj, context, result.handleContext);
            }
        }

        return defaultRenderer(obj, descriptor, context);
    }

    private TRenderable RenderCustomDescriptor(object obj, CustomDescriptor customDescriptor, RenderContext<TState> context)
    {
        if (!DumpConfig.Default.CustomDescriptorHandlers.TryGetValue(customDescriptor.Type.TypeHandle, out var valueFactory))
        {
            return RenderUnfamiliarCustomDescriptor(obj, customDescriptor, context);
        }

        var memberProvider = context.Config.MemberProvider;

        var customValue = valueFactory(obj, customDescriptor.Type, customDescriptor.ValueProvider, memberProvider);

        if (customValue is null)
        {
            return RenderNullValue(customDescriptor, context);
        }

        context = context with { RootObjectTransform = customValue };

        var customValueDescriptor = DumpConfig.Default.Generator.Generate(customValue.GetType(), null, memberProvider);

        return RenderDescriptor(customValue, customValueDescriptor, context);
    }

    private bool ObjectAlreadyRendered(object @object, IObjectIdTracker tracker)
    {
        // tracker.GetId(@object, out var firstTime);
        var (firstTime, id) = tracker.Track(@object);

        return firstTime is false;
    }

    protected virtual (bool success, object? value, TRenderable renderedValue) GetValueAndRender(object source, IValueProvider valueProvider, IDescriptor? descriptor, RenderContext<TState> context, object? providedValue = null)
    {
        try
        {
            var value = providedValue ?? valueProvider.GetValue(source);
            return (true, value, RenderDescriptor(value, descriptor, context));
        }
        catch (Exception ex)
        {
            return (false, default, RenderFailedValueReading(ex, valueProvider, descriptor, context));
        }
    }

    protected abstract TRenderable RenderFailedValueReading(Exception ex, IValueProvider valueProvider, IDescriptor? descriptor, RenderContext<TState> context);
    protected abstract IRenderedObject CreateRenderedObject(TRenderable rendered);

    public abstract TRenderable RenderNullValue(IDescriptor? descriptor, RenderContext<TState> context);

    protected abstract TRenderable RenderUnfamiliarCustomDescriptor(object obj, CustomDescriptor descriptor, RenderContext<TState> context);

    public abstract TRenderable RenderExceededDepth(object obj, IDescriptor? descriptor, RenderContext<TState> context);
    
    protected abstract TRenderable RenderCircularDependency(object obj, IDescriptor? descriptor, ReferenceLabel label, RenderContext<TState> context);
    protected abstract TRenderable RenderSharedReference(object obj, IDescriptor? descriptor, ReferenceLabel label, RenderContext<TState> context);

    protected abstract TRenderable RenderNullDescriptor(object obj, RenderContext<TState> context);
    protected abstract TRenderable RenderIgnoredDescriptor(object obj, IgnoredDescriptor descriptor, RenderContext<TState> context);
    protected abstract TRenderable RenderSingleValueDescriptor(object obj, SingleValueDescriptor descriptor, RenderContext<TState> context);
    protected abstract TRenderable RenderUnsupportedDescriptor(object obj, IDescriptor descriptor, RenderContext<TState> context);
    protected abstract TRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext<TState> context);
    protected abstract TRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext<TState> context);
    protected abstract TRenderable RenderLabelDescriptor(object obj, LabelDescriptor descriptor, RenderContext<TState> context);
    protected abstract TState CreateState(object? obj, IDescriptor? descriptor, RendererConfig config);
}
