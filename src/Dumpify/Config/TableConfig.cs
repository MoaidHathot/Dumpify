namespace Dumpify;

public class TableConfig
{
    public bool ShowArrayIndices { get; set; } = true;
    public bool ShowTableHeaders { get; set; } = true;
    public bool NoColumnWrapping { get; set; } = false;
    public bool ExpandTables { get; set; } = false;
    public bool ShowMemberTypes { get; set; } = false;
    public bool ShowRowSeparators { get; set; } = false;
    public int MaxCollectionCount { get; set; } = int.MaxValue;
}