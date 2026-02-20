namespace Dumpify;

/// <summary>
/// Specifies how collections should be truncated when they exceed MaxCollectionCount.
/// </summary>
public enum TruncationMode
{
    /// <summary>
    /// Show the first N elements, truncate the rest at the end.
    /// </summary>
    Head,

    /// <summary>
    /// Show the last N elements, truncate the beginning.
    /// </summary>
    Tail,

    /// <summary>
    /// Show first N/2 elements and last N/2 elements, with truncation marker in the middle.
    /// </summary>
    HeadAndTail
}
