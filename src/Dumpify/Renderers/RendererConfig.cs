using Dumpify.Config;
using Dumpify.Descriptors.ValueProviders;
using Dumpify.Outputs;

namespace Dumpify.Renderers;

public record RendererConfig
{
    public string? Label { get; init; } = null;
    public int? MaxDepth { get; init; } = null;

    public ColorConfig ColorConfig { get; init; } = ColorConfig.DefaultColors;
    public TableConfig TableConfig { get; init; } = new TableConfig();
    public TypeNamingConfig TypeNamingConfig { get; init; } = new TypeNamingConfig();
    public required IMemberProvider MemberProvider { get; init; }
    public required ITypeNameProvider TypeNameProvider { get; init; }
}
