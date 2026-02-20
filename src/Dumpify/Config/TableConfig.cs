namespace Dumpify;

public class TableConfig : IConfigMergeable<TableConfig>
{
    private static readonly TableConfig Defaults = new();

    public bool ShowArrayIndices { get; set; } = true;
    public bool ShowTableHeaders { get; set; } = true;
    public bool NoColumnWrapping { get; set; } = false;
    public bool ExpandTables { get; set; } = false;
    public bool ShowMemberTypes { get; set; } = false;
    public bool ShowRowSeparators { get; set; } = false;
    public int MaxCollectionCount { get; set; } = int.MaxValue;

    /// <summary>
    /// The border style for tables. Default is Rounded.
    /// Use Ascii or Square for better terminal compatibility.
    /// </summary>
    public TableBorderStyle BorderStyle { get; set; } = TableBorderStyle.Rounded;

    /// <inheritdoc />
    public TableConfig MergeWith(TableConfig? overrideConfig)
    {
        if (overrideConfig is null)
        {
            return this;
        }

        return new TableConfig
        {
            ShowArrayIndices = ConfigMergeHelper.Merge(ShowArrayIndices, overrideConfig.ShowArrayIndices, Defaults.ShowArrayIndices),
            ShowTableHeaders = ConfigMergeHelper.Merge(ShowTableHeaders, overrideConfig.ShowTableHeaders, Defaults.ShowTableHeaders),
            NoColumnWrapping = ConfigMergeHelper.Merge(NoColumnWrapping, overrideConfig.NoColumnWrapping, Defaults.NoColumnWrapping),
            ExpandTables = ConfigMergeHelper.Merge(ExpandTables, overrideConfig.ExpandTables, Defaults.ExpandTables),
            ShowMemberTypes = ConfigMergeHelper.Merge(ShowMemberTypes, overrideConfig.ShowMemberTypes, Defaults.ShowMemberTypes),
            ShowRowSeparators = ConfigMergeHelper.Merge(ShowRowSeparators, overrideConfig.ShowRowSeparators, Defaults.ShowRowSeparators),
            MaxCollectionCount = ConfigMergeHelper.Merge(MaxCollectionCount, overrideConfig.MaxCollectionCount, Defaults.MaxCollectionCount),
            BorderStyle = ConfigMergeHelper.Merge(BorderStyle, overrideConfig.BorderStyle, Defaults.BorderStyle),
        };
    }
}