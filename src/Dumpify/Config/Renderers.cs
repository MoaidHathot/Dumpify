using Dumpify.Renderers;
using Dumpify.Renderers.Spectre.Console.TableRenderer;
using Dumpify.Renderers.Spectre.Console.TextRenderer;

namespace Dumpify.Config;

public static class Renderers
{
    public static IRenderer TableRenderer { get; } = new SpectreConsoleTableRenderer();
    //public static IRenderer TextRenderer { get; } = new SpectreConsoleTextRenderer();
}
