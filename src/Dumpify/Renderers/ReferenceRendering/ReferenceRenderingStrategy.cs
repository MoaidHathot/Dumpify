namespace Dumpify;

/// <summary>
/// Implementation of IReferenceRenderingStrategy.
/// Stateless - determines what to track and how to format based on configuration.
/// </summary>
internal class ReferenceRenderingStrategy(CircularReferenceDisplay circularDisplay, SharedReferenceDisplay sharedDisplay, bool tracksIds, bool tracksPaths, bool tracksSharedReferences) : IReferenceRenderingStrategy
{
    private readonly CircularReferenceDisplay _circularDisplay = circularDisplay;
    private readonly SharedReferenceDisplay _sharedDisplay = sharedDisplay;

    public bool TracksIds { get; } = tracksIds;
    public bool TracksPaths { get; } = tracksPaths;
    public bool TracksSharedReferences { get; } = tracksSharedReferences;

    public ReferenceLabel FormatCircularReference(Type type, int? id, string? path)
    {
        return new ReferenceLabel(
            _circularDisplay.HasFlag(CircularReferenceDisplay.TypeName) ? type.Name : null,
            _circularDisplay.HasFlag(CircularReferenceDisplay.Id) ? id : null,
            _circularDisplay.HasFlag(CircularReferenceDisplay.Path) ? path : null
        );
    }

    public ReferenceLabel? FormatSharedReference(Type type, int? id, string? path)
    {
        if (_sharedDisplay == SharedReferenceDisplay.RenderFully)
        {
            return null;
        }

        // Use same format flags as circular for shared references
        return FormatCircularReference(type, id, path);
    }
}