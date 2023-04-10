using Dumpify.Outputs;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify.Renderers.Spectre.Console;

internal class SpectreConsoleRenderedObject : IRenderedObject
{
    private readonly IRenderable _renderable;

    public SpectreConsoleRenderedObject(IRenderable renderable)
        => _renderable = renderable;

    public void Output(IDumpOutput output)
    {
        var console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.Detect,
            ColorSystem = ColorSystemSupport.Detect,
            Out = new AnsiConsoleOutput(output.TextWriter),
        });

        console.Write(_renderable);
        console.WriteLine();
    }
}
