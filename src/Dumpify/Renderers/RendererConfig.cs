using Dumpify.Config;

namespace Dumpify.Renderers;

public struct RendererConfig
{
    public RendererConfig()
    {
    }

    public string? Label { get; init; } = null;
    public int? MaxDepth { get; init; } = null;
    public bool? ShowTypeNames { get; init; } = null;
    public bool? ShowHeaders { get; init; } = null;

    public ColorConfig ColorConfig { get; init; } = ColorConfig.DefaultColors;
    public TableConfig TableConfig { get; init; } = new TableConfig();
}
