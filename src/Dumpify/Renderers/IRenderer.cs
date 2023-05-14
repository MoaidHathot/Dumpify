using Dumpify.Descriptors;

namespace Dumpify.Renderers;

public interface IRenderer
{
    IRenderedObject Render(object? obj, IDescriptor? descriptor, RendererConfig config);
}