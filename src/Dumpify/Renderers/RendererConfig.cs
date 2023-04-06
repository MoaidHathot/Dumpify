using Dumpify.Config;

namespace Dumpify.Renderers;

public struct RendererConfig
{
    public string? Label { get; init; }
    public int? MaxDepth { get; init; }
    public bool? ShowTypeNames { get; init; }
    public bool? ShowHeaders { get; init; }
    public ColorConfig ColorConfig { get; init; }
    public TableConfig TableConfig { get; init; }
}
