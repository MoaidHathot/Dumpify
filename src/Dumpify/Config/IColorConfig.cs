namespace Dumpify;

public interface IColorConfig<TColor>
{
    TColor? TypeNameColor { get; set; }
    TColor? ColumnNameColor { get; set; }
    TColor? PropertyValueColor { get; set; }
    TColor? PropertyNameColor { get; set; }
    TColor? NullValueColor { get; set; }
    TColor? IgnoredValueColor { get; set; }
    TColor? MetadataInfoColor { get; set; }
    TColor? MetadataErrorColor { get; set; }
    TColor? LabelValueColor { get; set; }
}