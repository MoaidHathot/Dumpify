using Dumpify.Descriptors;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace Dumpify.Renderers;

internal abstract class RendererBase<TRenderable> : IRenderer, IRendererHandler<TRenderable>
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
        var idGenerator = new ObjectIDGenerator();
        var context = new RenderContext(config, idGenerator, 0);

        var renderable = obj switch
        {
            null => RenderNullValue(descriptor, context),
            _ => RenderDescriptor(obj, descriptor, context),
        };

        return CreateRenderedObject(renderable);
    }

    public TRenderable RenderDescriptor(object? @object, IDescriptor? descriptor, RenderContext context)
    {
        if (@object is null)
        {
            return RenderNullValue(descriptor, context);
        }

        if (context.Config.MaxDepth is not null && context.CurrentDepth > context.Config.MaxDepth)
        {
            return RenderExceededDepth(@object, descriptor, context);
        }

        return descriptor switch
        {
            null => RenderNullDescriptor(@object, context),
            CircularDependencyDescriptor circularDescriptor => RenderDescriptor(@object, circularDescriptor.Descriptor, context),
            IgnoredDescriptor ignoredDescriptor => TryRenderCustomTypeDescriptor(@object, ignoredDescriptor, context, RenderIgnoredDescriptor),
            SingleValueDescriptor singleDescriptor => TryRenderCustomTypeDescriptor(@object, singleDescriptor, context, RenderSingleValueDescriptor),
            ObjectDescriptor objDescriptor => TryRenderCustomTypeDescriptor(@object, objDescriptor, context, TryRenderObjectDescriptor),
            MultiValueDescriptor multiDescriptor => TryRenderCustomTypeDescriptor(@object, multiDescriptor, context, RenderMultiValueDescriptor),
            CustomDescriptor customDescriptor => TryRenderCustomTypeDescriptor(@object, customDescriptor, context, RenderCustomDescriptor),
            _ => RenderUnsupportedDescriptor(@object, descriptor, context),
        };
    }

    private TRenderable TryRenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext context)
    {
        if (ObjectAlreadyRendered(obj, context.ObjectTracker))
        {
            return RenderCircularDependency(obj, descriptor, context);
        }

        return RenderObjectDescriptor(obj, descriptor, context);
    }

    private TRenderable TryRenderCustomTypeDescriptor<TDescriptor>(object obj, TDescriptor descriptor, in RenderContext context, Func<object, TDescriptor, RenderContext, TRenderable> defaultRenderer)
        where TDescriptor : IDescriptor
    {
        if (_customTypeRenderers.TryGetValue(descriptor.GetType().TypeHandle, out var renderList))
        {
            var renderer = renderList.FirstOrDefault(r => r.ShouldHandle(descriptor, obj));

            if (renderer is not null)
            {
                return renderer.Render(descriptor, obj, context);
            }
        }

        return defaultRenderer(obj, descriptor, context);
    }

    private TRenderable RenderCustomDescriptor(object obj, CustomDescriptor customDescriptor, RenderContext context)
    {
        if (!DumpConfig.Default.CustomDescriptorHandlers.TryGetValue(customDescriptor.Type.TypeHandle, out var valueFactory))
        {
            return RenderUnfamiliarCustomDescriptor(obj, customDescriptor, context);
        }

        var customValue = valueFactory(obj, customDescriptor.Type, customDescriptor.PropertyInfo);

        if (customValue is null)
        {
            return RenderNullValue(customDescriptor, context);
        }

        var customValueDescriptor = DumpConfig.Default.Generator.Generate(customValue.GetType(), null);

        return RenderDescriptor(customValue, customValueDescriptor, context);
    }

    private bool ObjectAlreadyRendered(object @object, ObjectIDGenerator tracker)
    {
        tracker.GetId(@object, out var firstTime);

        return firstTime is false;
    }

    protected abstract IRenderedObject CreateRenderedObject(TRenderable rendered);

    public abstract TRenderable RenderNullValue(IDescriptor? descriptor, RenderContext context);

    protected abstract TRenderable RenderUnfamiliarCustomDescriptor(object obj, CustomDescriptor descriptor, RenderContext context);

    public abstract TRenderable RenderExceededDepth(object obj, IDescriptor? descriptor, RenderContext context);
    protected abstract TRenderable RenderCircularDependency(object obj, IDescriptor? descriptor, RenderContext context);

    protected abstract TRenderable RenderNullDescriptor(object obj, RenderContext context);
    protected abstract TRenderable RenderIgnoredDescriptor(object obj, IgnoredDescriptor descriptor, RenderContext context);
    protected abstract TRenderable RenderSingleValueDescriptor(object obj, SingleValueDescriptor descriptor, RenderContext context);
    protected abstract TRenderable RenderUnsupportedDescriptor(object obj, IDescriptor descriptor, RenderContext context);
    protected abstract TRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext context);
    protected abstract TRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext context);
}
