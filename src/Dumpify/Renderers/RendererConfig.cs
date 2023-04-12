using Dumpify.Config;
using Dumpify.Descriptors.ValueProviders;
using Dumpify.Outputs;

namespace Dumpify.Renderers;

public record struct RendererConfig
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
    public required IMemberProvider MemberProvider { get; init; }
}
