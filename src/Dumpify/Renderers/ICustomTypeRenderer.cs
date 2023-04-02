using Dumpify.Descriptors;

namespace Dumpify.Renderers;
internal interface ICustomTypeRenderer<TRenderable>
{
    Type DescriptorType { get; }

    TRenderable Render(IDescriptor descriptor, object obj, RenderContext context);
    bool ShouldHandle(IDescriptor descriptor, object obj);
}