namespace Dumpify;

/// <summary>
/// Controls how shared references (same object appearing multiple times, but not in a cycle) are displayed.
/// </summary>
public enum SharedReferenceDisplay
{
    /// <summary>
    /// Render the full object each time it appears (default behavior).
    /// </summary>
    RenderFully,

    /// <summary>
    /// Show a reference indicator on subsequent appearances (e.g., [Identical #Person:1]).
    /// Uses the same display flags as CircularReferenceDisplay.
    /// </summary>
    ShowReference,
}
