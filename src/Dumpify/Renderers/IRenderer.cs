using Dumpify.Descriptors;

namespace Dumpify;

public interface IRenderer
{
    IRenderedObject Render(object? obj, IDescriptor? descriptor, RendererConfig config);
}