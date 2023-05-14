using Dumpify.Descriptors;

namespace Dumpify.Renderers;
internal interface ICustomTypeRenderer<TRenderable>
{
    Type DescriptorType { get; }

    TRenderable Render(IDescriptor descriptor, object obj, RenderContext context, object? handleContext);
    (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj);
}