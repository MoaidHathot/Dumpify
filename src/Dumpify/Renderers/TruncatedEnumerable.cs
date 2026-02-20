namespace Dumpify;

/// <summary>
/// Represents the result of truncating a collection, containing the visible items
/// and metadata about what was truncated.
/// </summary>
public sealed class TruncatedEnumerable<T>
{
    /// <summary>
    /// The items to display (after truncation).
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// The total count of items in the original collection.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// The truncation mode that was applied.
    /// </summary>
    public TruncationMode Mode { get; }

    /// <summary>
    /// Marker for truncation at the start (for Tail or HeadAndTail modes).
    /// Null if no truncation at start.
    /// </summary>
    public TruncationMarker? StartMarker { get; }

    /// <summary>
    /// Marker for truncation at the end (for Head or HeadAndTail modes).
    /// Null if no truncation at end.
    /// </summary>
    public TruncationMarker? EndMarker { get; }

    /// <summary>
    /// For HeadAndTail mode: the index in Items where the middle marker should be inserted.
    /// Null for other modes.
    /// </summary>
    public int? MiddleMarkerIndex { get; }

    /// <summary>
    /// Marker for truncation in the middle (for HeadAndTail mode).
    /// Null for other modes.
    /// </summary>
    public TruncationMarker? MiddleMarker { get; }

    /// <summary>
    /// The number of items that were truncated.
    /// </summary>
    public int TruncatedCount => TotalCount - Items.Count;

    /// <summary>
    /// Whether any truncation occurred.
    /// </summary>
    public bool WasTruncated => TruncatedCount > 0;

    /// <summary>
    /// Iterates through all items and markers in the correct order,
    /// invoking the appropriate callback for each.
    /// </summary>
    /// <param name="onMarker">Called for each truncation marker (start, middle, end).</param>
    /// <param name="onItem">Called for each item with its index in the Items list.</param>
    public void ForEachWithMarkers(
        Action<TruncationMarker> onMarker,
        Action<T, int> onItem)
    {
        if (StartMarker is { } startMarker)
        {
            onMarker(startMarker);
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (MiddleMarkerIndex == i && MiddleMarker is { } middleMarker)
            {
                onMarker(middleMarker);
            }

            onItem(Items[i], i);
        }

        if (EndMarker is { } endMarker)
        {
            onMarker(endMarker);
        }
    }

    internal TruncatedEnumerable(
        IReadOnlyList<T> items,
        int totalCount,
        TruncationMode mode,
        TruncationMarker? startMarker = null,
        TruncationMarker? endMarker = null,
        TruncationMarker? middleMarker = null,
        int? middleMarkerIndex = null)
    {
        Items = items;
        TotalCount = totalCount;
        Mode = mode;
        StartMarker = startMarker;
        EndMarker = endMarker;
        MiddleMarker = middleMarker;
        MiddleMarkerIndex = middleMarkerIndex;
    }
}
