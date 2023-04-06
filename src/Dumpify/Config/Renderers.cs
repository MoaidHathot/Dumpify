using Dumpify.Renderers;
using Dumpify.Renderers.Spectre.Console.SimpleRenderer;
using Dumpify.Renderers.Spectre.Console.TableRenderer;

namespace Dumpify.Config;

public static class Renderers
{
    public static IRenderer TableRenderer { get; } = new SpectreConsoleTableRenderer();
    public static IRenderer SimpleRenderer { get; } = new SpectreConsoleSimpleRenderer();
}
