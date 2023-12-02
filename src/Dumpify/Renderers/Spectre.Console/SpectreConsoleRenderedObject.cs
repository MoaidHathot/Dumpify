using Spectre.Console;
using Spectre.Console.Rendering;
using System.Text;

namespace Dumpify;

internal class SpectreConsoleRenderedObject : IRenderedObject
{
    private readonly IRenderable _renderable;

    public SpectreConsoleRenderedObject(IRenderable renderable)
        => _renderable = renderable;

    public void Output(IDumpOutput output, OutputConfig outputConfig)
    {
        var console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.Detect,
            ColorSystem = ColorSystemSupport.Detect,
            Out = CreateOutput(output, outputConfig),
        });

        console.Write(_renderable);
        console.WriteLine();
    }

    private static IAnsiConsoleOutput CreateOutput(IDumpOutput output, OutputConfig config)
    {
        var defaultConsoleSettings = GetDefaultConsoleOutputSettings(output);
        var dimensions = GetRenderAreaDimensions(config, defaultConsoleSettings.width, defaultConsoleSettings.heigth);

        return new CustomConsoleOutput(output.TextWriter, dimensions.width, dimensions.heigth, defaultConsoleSettings.isTerminal, defaultConsoleSettings.enableEncoding);
    }

    private static (int width, int heigth, bool isTerminal, bool enableEncoding) GetDefaultConsoleOutputSettings(IDumpOutput output)
    {
        var consoleOutput = new AnsiConsoleOutput(output.TextWriter);
        var stdSettings = GetSystemStdSettings(output);

        var enableEncoding = stdSettings.isStdOut || stdSettings.isStdErr;

        return (consoleOutput.Width, consoleOutput.Height, consoleOutput.IsTerminal, enableEncoding);
    }

    private static (int width, int heigth) GetRenderAreaDimensions(OutputConfig config, int defaultWidth, int defaultHeight)
    {
        var width = config.WidthOverride ?? defaultWidth;
        var height = config.HeightOverride ?? defaultHeight;

        return (width, height);
    }

    private static (bool isStdOut, bool isStdErr) GetSystemStdSettings(IDumpOutput output)
    {
        bool isStdOut = false;
        bool isStdErr = false;

        try
        {
            isStdOut = output.TextWriter == Console.Out;
        }
        catch
        {

        }

        try
        {

            isStdErr = output.TextWriter == Console.Error;
        }
        catch
        {

        }

        return (isStdOut, isStdErr);
    }
}


file class CustomConsoleOutput : IAnsiConsoleOutput
{
    public TextWriter Writer { get; }

    public bool IsTerminal { get; }
    public int Width { get; }
    public int Height { get; }

    private readonly bool _enableEncoding;

    public CustomConsoleOutput(TextWriter writer, int width, int height, bool isTerminal, bool enableEncoding)
    {
        Writer = writer;
        Width = width;
        Height = height;
        IsTerminal = isTerminal;
        _enableEncoding = enableEncoding;
    }

    public void SetEncoding(Encoding encoding)
    {
        if (_enableEncoding)
        {
            System.Console.OutputEncoding = encoding;
        }
    }
}
