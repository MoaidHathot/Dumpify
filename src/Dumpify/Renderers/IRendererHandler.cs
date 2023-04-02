using Dumpify.Descriptors;

namespace Dumpify.Renderers;

internal interface IRendererHandler<TRenderable>
{
    TRenderable RenderDescriptor(object? @object, IDescriptor? descriptor, in RenderContext context);
    TRenderable RenderNullValue(IDescriptor? descriptor, in RendererConfig config);
    TRenderable RenderExeededDepth(object obj, IDescriptor? descriptor, in RendererConfig config);
}