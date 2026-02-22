namespace Dumpify;

/// <summary>
/// Strategy interface for reference rendering.
/// Determines what to track and how to format reference labels.
/// </summary>
public interface IReferenceRenderingStrategy
{
    /// <summary>
    /// Whether this strategy needs to track object IDs.
    /// </summary>
    bool TracksIds { get; }

    /// <summary>
    /// Whether this strategy needs to track paths.
    /// </summary>
    bool TracksPaths { get; }

    /// <summary>
    /// Whether this strategy tracks shared references (not just cycles).
    /// </summary>
    bool TracksSharedReferences { get; }

    /// <summary>
    /// Format the label for a circular reference.
    /// </summary>
    /// <param name="type">The type of the object</param>
    /// <param name="id">The object ID (if tracked)</param>
    /// <param name="path">The path where first seen (if tracked)</param>
    /// <returns>A structured label for rendering</returns>
    ReferenceLabel FormatCircularReference(Type type, int? id, string? path);

    /// <summary>
    /// Format the label for a shared reference (same object appearing multiple times, not in a cycle).
    /// </summary>
    /// <param name="type">The type of the object</param>
    /// <param name="id">The object ID (if tracked)</param>
    /// <param name="path">The path where first seen (if tracked)</param>
    /// <returns>A structured label for rendering, or null if the object should be rendered fully</returns>
    ReferenceLabel? FormatSharedReference(Type type, int? id, string? path);
}
