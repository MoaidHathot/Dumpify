namespace Dumpify;

/// <summary>
/// Indicates where truncation occurred in a collection.
/// </summary>
public enum TruncationPosition
{
    /// <summary>
    /// Truncation at the end (Head mode - showing first N items).
    /// </summary>
    End,

    /// <summary>
    /// Truncation at the start (Tail mode - showing last N items).
    /// </summary>
    Start,

    /// <summary>
    /// Truncation in the middle (HeadAndTail mode - showing first and last items).
    /// </summary>
    Middle
}

/// <summary>
/// Represents a truncation point in rendered output with metadata about what was truncated.
/// </summary>
public sealed class TruncationMarker
{
    /// <summary>
    /// The number of items that were truncated.
    /// </summary>
    public int TruncatedCount { get; }

    /// <summary>
    /// Where the truncation occurred.
    /// </summary>
    public TruncationPosition Position { get; }

    /// <summary>
    /// Creates a new truncation marker.
    /// </summary>
    public TruncationMarker(int truncatedCount, TruncationPosition position)
    {
        TruncatedCount = truncatedCount;
        Position = position;
    }

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
        return $"... +{TruncatedCount}";
    }
}