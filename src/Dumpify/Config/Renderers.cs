using Dumpify.Renderers;
using Dumpify.Renderers.Spectre.Console.TableRenderer;
using Dumpify.Renderers.Spectre.Console.TextRenderer;

namespace Dumpify.Config;

public static class Renderers
{
    public static IRenderer Table { get; } = new SpectreConsoleTableRenderer();
    public static IRenderer Text { get; } = new SpectreConsoleTextRenderer();
}
