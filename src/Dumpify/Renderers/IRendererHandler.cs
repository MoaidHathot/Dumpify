using Dumpify.Descriptors;

namespace Dumpify;

internal interface IRendererHandler<TRenderable, TState>
{
    TRenderable RenderDescriptor(object? @object, IDescriptor? descriptor, RenderContext<TState> context);
    TRenderable RenderNullValue(IDescriptor? descriptor, RenderContext<TState> context);
    TRenderable RenderExceededDepth(object obj, IDescriptor? descriptor, RenderContext<TState> context);
}