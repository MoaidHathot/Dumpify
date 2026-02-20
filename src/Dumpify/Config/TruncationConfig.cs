namespace Dumpify;

/// <summary>
/// Configuration for truncating collections and other large data structures.
/// </summary>
public class TruncationConfig : ConfigBase<TruncationConfig>
{
    /// <summary>
    /// Maximum number of elements to render for collections.
    /// Default is null (no limit).
    /// </summary>
    public TrackableProperty<int?> MaxCollectionCount { get; set; } = new(null);

    /// <summary>
    /// How to truncate: Head (first N), Tail (last N), or HeadAndTail (first N/2 + last N/2).
    /// Default is Head.
    /// </summary>
    public TrackableProperty<TruncationMode> Mode { get; set; } = new(TruncationMode.Head);

    /// <summary>
    /// Whether to apply truncation per dimension for multi-dimensional arrays.
    /// Default is true.
    /// </summary>
    public TrackableProperty<bool> PerDimension { get; set; } = new(true);

    /// <inheritdoc />
    protected override TruncationConfig MergeOverride(TruncationConfig overrideConfig)
    {
        return new TruncationConfig
        {
            MaxCollectionCount = Merge(MaxCollectionCount, overrideConfig.MaxCollectionCount),
            Mode = Merge(Mode, overrideConfig.Mode),
            PerDimension = Merge(PerDimension, overrideConfig.PerDimension),
        };
    }
}
