namespace Dumpify;

/// <summary>
/// Represents a truncation point in rendered output with metadata about what was truncated.
/// </summary>
/// <remarks>
/// Creates a new truncation marker.
/// </remarks>
public sealed class TruncationMarker(int truncatedCount, TruncationPosition position)
{
    /// <summary>
    /// The number of items that were truncated.
    /// </summary>
    public int TruncatedCount { get; } = truncatedCount;

    /// <summary>
    /// Where the truncation occurred.
    /// </summary>
    public TruncationPosition Position { get; } = position;

    /// <summary>
    /// Gets a default message describing the truncation.
    /// </summary>
    public string GetDefaultMessage()
    {
        return Position switch
        {
            TruncationPosition.End => $"[... {TruncatedCount} more]",
            TruncationPosition.Start => $"[{TruncatedCount} previous ...]",
            TruncationPosition.Middle => $"[... {TruncatedCount} more ...]",
            _ => $"[... {TruncatedCount} more]"
        };
    }

    /// <summary>
    /// Gets a compact message (useful for column/row headers).
    /// </summary>
    public string GetCompactMessage()
    {
        return $"[... +{TruncatedCount}]";
    }
}
