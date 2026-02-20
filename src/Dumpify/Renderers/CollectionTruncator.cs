using System.Collections;

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

/// <summary>
/// Utility class for efficiently truncating collections.
/// </summary>
public static class CollectionTruncator
{
    /// <summary>
    /// Truncates an enumerable according to the specified configuration.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source enumerable.</param>
    /// <param name="config">The truncation configuration.</param>
    /// <returns>A TruncatedEnumerable containing the visible items and truncation metadata.</returns>
    public static TruncatedEnumerable<T> Truncate<T>(IEnumerable<T> source, TruncationConfig? config)
    {
        var maxCount = config?.MaxCollectionCount.Value;
        var mode = config?.Mode.Value ?? TruncationMode.Head;

        return Truncate(source, maxCount, mode);
    }

    /// <summary>
    /// Truncates an enumerable to the specified maximum count.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source enumerable.</param>
    /// <param name="maxCount">Maximum number of elements to keep. Null means no limit.</param>
    /// <param name="mode">The truncation mode.</param>
    /// <returns>A TruncatedEnumerable containing the visible items and truncation metadata.</returns>
    public static TruncatedEnumerable<T> Truncate<T>(
        IEnumerable<T> source,
        int? maxCount,
        TruncationMode mode = TruncationMode.Head)
    {
        if (maxCount is null or <= 0)
        {
            // No truncation - materialize and return all
            var all = source as IReadOnlyList<T> ?? source.ToList();
            return new TruncatedEnumerable<T>(all, all.Count, mode);
        }

        return mode switch
        {
            TruncationMode.Head => TruncateHead(source, maxCount.Value),
            TruncationMode.Tail => TruncateTail(source, maxCount.Value),
            TruncationMode.HeadAndTail => TruncateHeadAndTail(source, maxCount.Value),
            _ => TruncateHead(source, maxCount.Value)
        };
    }

    /// <summary>
    /// Truncates an Array to the specified maximum count (non-generic version for runtime arrays).
    /// </summary>
    public static TruncatedEnumerable<object?> TruncateArray(
        Array source,
        int? maxCount,
        TruncationMode mode = TruncationMode.Head)
    {
        return Truncate(EnumerateArray(source), maxCount, mode);
    }

    private static IEnumerable<object?> EnumerateArray(Array array)
    {
        foreach (var item in array)
        {
            yield return item;
        }
    }

    private static TruncatedEnumerable<T> TruncateHead<T>(IEnumerable<T> source, int maxCount)
    {
        // Optimization: if source has known count, use it directly
        if (source is ICollection<T> collection)
        {
            var items = source.Take(maxCount).ToList();
            var endMarker = collection.Count > maxCount
                ? new TruncationMarker(collection.Count - maxCount, TruncationPosition.End)
                : null;
            return new TruncatedEnumerable<T>(items, collection.Count, TruncationMode.Head, endMarker: endMarker);
        }

        if (source is IReadOnlyCollection<T> readOnlyCollection)
        {
            var items = source.Take(maxCount).ToList();
            var endMarker = readOnlyCollection.Count > maxCount
                ? new TruncationMarker(readOnlyCollection.Count - maxCount, TruncationPosition.End)
                : null;
            return new TruncatedEnumerable<T>(items, readOnlyCollection.Count, TruncationMode.Head, endMarker: endMarker);
        }

        // For pure IEnumerable: enumerate once, counting as we go
        var result = new List<T>(Math.Min(maxCount, 1024));
        int totalCount = 0;

        foreach (var item in source)
        {
            totalCount++;
            if (result.Count < maxCount)
            {
                result.Add(item);
            }
            // Continue counting even after we have enough items
        }

        var marker = totalCount > maxCount
            ? new TruncationMarker(totalCount - maxCount, TruncationPosition.End)
            : null;

        return new TruncatedEnumerable<T>(result, totalCount, TruncationMode.Head, endMarker: marker);
    }

    private static TruncatedEnumerable<T> TruncateTail<T>(IEnumerable<T> source, int maxCount)
    {
        // We need to know the total count first, so materialize
        var all = source as IList<T> ?? source.ToList();
        var totalCount = all.Count;

        if (totalCount <= maxCount)
        {
            return new TruncatedEnumerable<T>(
                all as IReadOnlyList<T> ?? all.ToList(),
                totalCount,
                TruncationMode.Tail);
        }

        var skipCount = totalCount - maxCount;
        var items = all.Skip(skipCount).ToList();
        var startMarker = new TruncationMarker(skipCount, TruncationPosition.Start);

        return new TruncatedEnumerable<T>(items, totalCount, TruncationMode.Tail, startMarker: startMarker);
    }

    private static TruncatedEnumerable<T> TruncateHeadAndTail<T>(IEnumerable<T> source, int maxCount)
    {
        // We need to know the total count first, so materialize
        var all = source as IList<T> ?? source.ToList();
        var totalCount = all.Count;

        if (totalCount <= maxCount)
        {
            return new TruncatedEnumerable<T>(
                all as IReadOnlyList<T> ?? all.ToList(),
                totalCount,
                TruncationMode.HeadAndTail);
        }

        // Split: take half from head, half from tail
        var headCount = maxCount / 2;
        var tailCount = maxCount - headCount;

        var headItems = all.Take(headCount);
        var tailItems = all.Skip(totalCount - tailCount);
        var items = headItems.Concat(tailItems).ToList();

        var truncatedInMiddle = totalCount - headCount - tailCount;
        var middleMarker = new TruncationMarker(truncatedInMiddle, TruncationPosition.Middle);

        return new TruncatedEnumerable<T>(
            items,
            totalCount,
            TruncationMode.HeadAndTail,
            middleMarker: middleMarker,
            middleMarkerIndex: headCount);
    }
}
