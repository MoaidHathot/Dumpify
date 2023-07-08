using Dumpify.Descriptors;
using Spectre.Console.Rendering;

namespace Dumpify;

internal interface IDescriptorRenderer
{
    IRenderable RenderCircularDependency(object @object, IDescriptor? descriptor, RendererConfig config);
    IRenderable RenderNull(IDescriptor? descriptor, RendererConfig config);
    IRenderable RenderExeededDepth(object obj, IDescriptor? descriptor, RendererConfig config);
    IRenderable RenderObject(object obj, ObjectDescriptor descriptor, in RendererConfig config);
    IRenderable RenderMultiValue(object? obj, MultiValueDescriptor descriptor, in RendererConfig config);
}