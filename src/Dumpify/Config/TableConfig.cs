namespace Dumpify;

public class TableConfig : ConfigBase<TableConfig>
{
    public TrackableProperty<bool> ShowArrayIndices { get; set; } = new(true);
    public TrackableProperty<bool> ShowTableHeaders { get; set; } = new(true);
    public TrackableProperty<bool> NoColumnWrapping { get; set; } = new(false);
    public TrackableProperty<bool> ExpandTables { get; set; } = new(false);
    public TrackableProperty<bool> ShowMemberTypes { get; set; } = new(false);
    public TrackableProperty<bool> ShowRowSeparators { get; set; } = new(false);
    public TrackableProperty<int> MaxCollectionCount { get; set; } = new(int.MaxValue);

    /// <summary>
    /// The border style for tables. Default is Rounded.
    /// Use Ascii or Square for better terminal compatibility.
    /// </summary>
    public TrackableProperty<TableBorderStyle> BorderStyle { get; set; } = new(TableBorderStyle.Rounded);

    /// <inheritdoc />
    protected override TableConfig MergeOverride(TableConfig overrideConfig)
    {
        return new TableConfig
        {
            ShowArrayIndices = Merge(ShowArrayIndices, overrideConfig.ShowArrayIndices),
            ShowTableHeaders = Merge(ShowTableHeaders, overrideConfig.ShowTableHeaders),
            NoColumnWrapping = Merge(NoColumnWrapping, overrideConfig.NoColumnWrapping),
            ExpandTables = Merge(ExpandTables, overrideConfig.ExpandTables),
            ShowMemberTypes = Merge(ShowMemberTypes, overrideConfig.ShowMemberTypes),
            ShowRowSeparators = Merge(ShowRowSeparators, overrideConfig.ShowRowSeparators),
            MaxCollectionCount = Merge(MaxCollectionCount, overrideConfig.MaxCollectionCount),
            BorderStyle = Merge(BorderStyle, overrideConfig.BorderStyle),
        };
    }
}
