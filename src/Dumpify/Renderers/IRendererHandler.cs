using Dumpify.Descriptors;

namespace Dumpify.Renderers;

internal interface IRendererHandler<TRenderable>
{
    TRenderable RenderDescriptor(object? @object, IDescriptor? descriptor, RenderContext context);
    TRenderable RenderNullValue(IDescriptor? descriptor, RenderContext context);
    TRenderable RenderExceededDepth(object obj, IDescriptor? descriptor, RenderContext context);
}