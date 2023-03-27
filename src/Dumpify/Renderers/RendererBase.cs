using Dumpify.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers;

internal abstract class RendererBase<TRenderable> : IRenderer
{
    public void Render(object? obj, IDescriptor? descriptor, RendererConfig config)
    {
        var idGenerator = new ObjectIDGenerator();

        var renderable = obj switch
        {
            null => RenderNullValue(descriptor, config),
            _ => RenderDescriptor(obj, descriptor, config, idGenerator, 0),
        };

        PublishRenderables(renderable);
    }

    protected TRenderable RenderDescriptor(object? @object, IDescriptor? descriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentDepth)
    {
        if (@object is null)
        {
            return RenderNullValue(descriptor, config);
        }

        if (config.MaxDepth is not null && currentDepth > config.MaxDepth)
        {
            return RenderExeededDepth(@object, descriptor, config);
        }

        return descriptor switch
        {
            null => RenderNullDescriptor(@object),
            CircularDependencyDescriptor circularDescriptor => RenderDescriptor(@object, circularDescriptor.Descriptor, config, tracker, currentDepth),
            IgnoredDescriptor ignoredDescriptor => RenderIgnoredDescriptor(@object, ignoredDescriptor),
            SingleValueDescriptor singleDescriptor => RenderSingleValueDescriptor(@object, singleDescriptor),
            ObjectDescriptor objDescriptor => RenderObjectDescriptor(@object, objDescriptor, config, tracker, currentDepth),
            MultiValueDescriptor multiDescriptor => RenderMultiValueDescriptor(@object, multiDescriptor, config, tracker, currentDepth),
            CustomDescriptor customDescriptor => RenderCustomDescriptor(@object, customDescriptor, config, tracker, currentDepth),
            _ => RenderUnsupportedDescriptor(@object, descriptor),
        }; ;
    }


    private TRenderable RenderCustomDescriptor(object obj, CustomDescriptor customDescriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentDepth)
    {

        if (!DumpConfig.Default.CustomDescriptorHandlers.TryGetValue(customDescriptor.Type.TypeHandle, out var valueFactory))
        {
            return RenderUnfamiliarCustomDescriptor(obj, customDescriptor, config);
        }

        var customValue = valueFactory(obj, customDescriptor.Type, customDescriptor.PropertyInfo);

        if (customValue is null)
        {
            return RenderNullValue(customDescriptor, config);
        }

        var customValueDescriptor = DumpConfig.Default.Generator.Generate(customValue.GetType(), null);

        return RenderDescriptor(customValue, customValueDescriptor, config, tracker, currentDepth);
    }

    protected bool ObjectAlreadyRendered(object @object, ObjectIDGenerator tracker)
    {
        tracker.GetId(@object, out var firstTime);

        return firstTime is false;
    }

    protected abstract void PublishRenderables(TRenderable renderable);

    protected abstract TRenderable RenderNullValue(IDescriptor? descriptor, in RendererConfig config);

    protected abstract TRenderable RenderUnfamiliarCustomDescriptor(object obj, CustomDescriptor descriptor, in RendererConfig config);

    protected abstract TRenderable RenderExeededDepth(object obj, IDescriptor? descriptor, in RendererConfig config);
    protected abstract TRenderable RenderCircularDependency(object @object, IDescriptor? descriptor, in RendererConfig config);

    protected abstract TRenderable RenderNullDescriptor(object obj);
    protected abstract TRenderable RenderIgnoredDescriptor(object obj, IgnoredDescriptor descriptor);
    protected abstract TRenderable RenderSingleValueDescriptor(object obj, SingleValueDescriptor descriptor);
    protected abstract TRenderable RenderUnsupportedDescriptor(object obj, IDescriptor descriptor);
    protected abstract TRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentDepth);
    protected abstract TRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentDepth);
}
