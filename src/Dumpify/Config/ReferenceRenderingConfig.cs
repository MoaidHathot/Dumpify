namespace Dumpify;

/// <summary>
/// Configuration for how circular references and shared references are rendered.
/// </summary>
public class ReferenceRenderingConfig
{
    /// <summary>
    /// Default configuration: TypeName | Id for circular, RenderFully for shared.
    /// </summary>
    public static ReferenceRenderingConfig Default { get; } = new();

    /// <summary>
    /// Controls what information is displayed for circular references.
    /// Default: TypeName | Id (e.g., [Cycle #Person:1])
    /// </summary>
    public CircularReferenceDisplay CircularReferenceDisplay { get; set; } = CircularReferenceDisplay.TypeName | CircularReferenceDisplay.Id;

    /// <summary>
    /// Controls how shared references are displayed.
    /// Default: RenderFully (render the full object each time)
    /// </summary>
    public SharedReferenceDisplay SharedReferenceDisplay { get; set; } = SharedReferenceDisplay.RenderFully;
}