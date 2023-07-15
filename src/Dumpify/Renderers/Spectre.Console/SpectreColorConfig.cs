using Spectre.Console;

namespace Dumpify;

public class SpectreColorConfig : IColorConfig<Color?>
{
    public required Color? TypeNameColor { get; set; }
    public required Color? ColumnNameColor { get; set; }
    public required Color? PropertyNameColor { get; set; }
    public required Color? PropertyValueColor { get; set; }
    public required Color? NullValueColor { get; set; }
    public required Color? IgnoredValueColor { get; set; }
    public required Color? MetadataInfoColor { get; set; }
    public required Color? MetadataErrorColor { get; set; }
    public required Color? LabelValueColor { get; set; }
}