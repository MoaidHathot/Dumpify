using Dumpify.Descriptors;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers;

internal class CustomTypeRenderer<TRenderable>
{
    public Type DescriptorType { get; init; }

    private readonly Func<IDescriptor, object, bool> _shouldHandleFunc;
    private readonly Func<IDescriptor, object, RenderContext, TRenderable> _rendererFunc;

    public CustomTypeRenderer(Type descriptorType, Func<IDescriptor, object, bool> shouldHandleFunc, Func<IDescriptor, object, RenderContext, TRenderable> rendererFunc)
    {
        DescriptorType = descriptorType;
        _shouldHandleFunc = shouldHandleFunc;
        _rendererFunc = rendererFunc;
    }

    public bool ShouldHandle(IDescriptor descriptor, object obj)
        => _shouldHandleFunc(descriptor, obj);

    public TRenderable Render(IDescriptor descriptor, object obj, RenderContext context)
        => _rendererFunc(descriptor, obj, context);
}
