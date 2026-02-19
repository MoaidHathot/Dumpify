using Dumpify.Descriptors.ValueProviders;

namespace Dumpify;

public record RendererConfig
{
    public string? Label { get; init; } = null;
    public int? MaxDepth { get; init; } = null;

    public ColorConfig ColorConfig { get; init; } = ColorConfig.DefaultColors;
    public TableConfig TableConfig { get; init; } = new TableConfig();
    public TypeNamingConfig TypeNamingConfig { get; init; } = new TypeNamingConfig();
    public TypeRenderingConfig TypeRenderingConfig { get; init; } = new TypeRenderingConfig();
    public ReferenceRenderingConfig ReferenceRenderingConfig { get; init; } = new ReferenceRenderingConfig();
    public required IMemberProvider MemberProvider { get; init; }
    public required ITypeNameProvider TypeNameProvider { get; init; }

    /// <summary>
    /// Gets the reference rendering strategy based on the current configuration.
    /// </summary>
    public IReferenceRenderingStrategy ReferenceRenderingStrategy =>
        ReferenceRenderingStrategyFactory.Create(ReferenceRenderingConfig);
}
