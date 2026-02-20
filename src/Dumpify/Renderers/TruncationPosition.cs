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
