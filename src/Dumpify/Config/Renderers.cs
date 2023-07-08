namespace Dumpify;

public static class Renderers
{
    public static IRenderer Table { get; } = new SpectreConsoleTableRenderer();

    //Still work in progress
    //public static IRenderer Text { get; } = new SpectreConsoleTextRenderer();
}
