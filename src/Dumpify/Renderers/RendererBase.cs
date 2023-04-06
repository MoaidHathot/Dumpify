using Dumpify.Descriptors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers;

internal abstract class RendererBase<TRenderable> : IRenderer, IRendererHandler<TRenderable>
{
    protected readonly ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<TRenderable>>> _customTypeRenderers;

    public RendererBase(ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<TRenderable>>>? customTypeRenderers)
    {
        _customTypeRenderers = customTypeRenderers ?? new ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<TRenderable>>>();
    }

    public void Render(object? obj, IDescriptor? descriptor, RendererConfig config)
    {
        var idGenerator = new ObjectIDGenerator();

        var renderable = obj switch
        {
            null => RenderNullValue(descriptor, config),
            _ => RenderDescriptor(obj, descriptor, new RenderContext(config, idGenerator, 0)),
        };

        PublishRenderables(renderable);
    }

    public TRenderable RenderDescriptor(object? @object, IDescriptor? descriptor, in RenderContext context)
    {
        if (@object is null)
        {
            return RenderNullValue(descriptor, context.Config);
        }

        if (context.Config.MaxDepth is not null && context.CurrentDepth > context.Config.MaxDepth)
        {
            return RenderExeededDepth(@object, descriptor, context.Config);
        }

        return descriptor switch
        {
            null => RenderNullDescriptor(@object, context),
            CircularDependencyDescriptor circularDescriptor => RenderDescriptor(@object, circularDescriptor.Descriptor, context),
            IgnoredDescriptor ignoredDescriptor => TryRenderCustomTypeDescriptor(@object, ignoredDescriptor, context, RenderIgnoredDescriptor),
            SingleValueDescriptor singleDescriptor => TryRenderCustomTypeDescriptor(@object, singleDescriptor, context, RenderSingleValueDescriptor),
            ObjectDescriptor objDescriptor => TryRenderCustomTypeDescriptor(@object, objDescriptor, context, RenderObjectDescriptor),
            MultiValueDescriptor multiDescriptor => TryRenderCustomTypeDescriptor(@object, multiDescriptor, context, RenderMultiValueDescriptor),
            CustomDescriptor customDescriptor => TryRenderCustomTypeDescriptor(@object, customDescriptor, context, RenderCustomDescriptor),
            _ => RenderUnsupportedDescriptor(@object, descriptor, context),
        };
    }

    private TRenderable TryRenderCustomTypeDescriptor<TDescriptor>(object obj, TDescriptor descriptor, in RenderContext context, Func<object, TDescriptor, RenderContext, TRenderable> defaultRenderer)
        where TDescriptor : notnull, IDescriptor
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
            return RenderUnfamiliarCustomDescriptor(obj, customDescriptor, context.Config);
        }

        var customValue = valueFactory(obj, customDescriptor.Type, customDescriptor.PropertyInfo);

        if (customValue is null)
        {
            return RenderNullValue(customDescriptor, context.Config);
        }

        var customValueDescriptor = DumpConfig.Default.Generator.Generate(customValue.GetType(), null);

        return RenderDescriptor(customValue, customValueDescriptor, context);
    }

    protected bool ObjectAlreadyRendered(object @object, ObjectIDGenerator tracker)
    {
        tracker.GetId(@object, out var firstTime);

        return firstTime is false;
    }

    protected abstract void PublishRenderables(TRenderable renderable);

    public abstract TRenderable RenderNullValue(IDescriptor? descriptor, in RendererConfig config);

    protected abstract TRenderable RenderUnfamiliarCustomDescriptor(object obj, CustomDescriptor descriptor, in RendererConfig config);

    public abstract TRenderable RenderExeededDepth(object obj, IDescriptor? descriptor, in RendererConfig config);
    protected abstract TRenderable RenderCircularDependency(object @object, IDescriptor? descriptor, in RendererConfig config);

    protected abstract TRenderable RenderNullDescriptor(object obj, RenderContext context);
    protected abstract TRenderable RenderIgnoredDescriptor(object obj, IgnoredDescriptor descriptor, RenderContext context);
    protected abstract TRenderable RenderSingleValueDescriptor(object obj, SingleValueDescriptor descriptor, RenderContext context);
    protected abstract TRenderable RenderUnsupportedDescriptor(object obj, IDescriptor descriptor, RenderContext context);
    protected abstract TRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext context);
    protected abstract TRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext context);
}
