using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

public interface IRenderableBuilder
{
    public IRenderableBuilder WithLabel(string? label);
    public IRenderableBuilder WithTitle(string? title, Style? style = null);
    public IRenderableBuilder WithHeader(string header, Style? style = null);
    public IRenderableBuilder WithEntry(IDescriptor? descriptor, object? obj, IRenderable? renderedObject, string header, IEnumerable<IRenderable> entry, Style? headerStyle = null);

    public IRenderable Build();
}