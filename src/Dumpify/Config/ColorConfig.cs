using System.Drawing;

namespace Dumpify;

public class ColorConfig : IColorConfig<DumpColor>
{
    public static DumpColor? DefaultTypeNameColor { get; } = new(Color.White);
    public static DumpColor? DefaultColumnNameColor { get; } = new("#87D7D7");
    public static DumpColor? DefaultPropertyNameColor { get; } = null;
    public static DumpColor? DefaultPropertyValueColor { get; } = new("#D7D787");
    public static DumpColor? DefaultNullValueColor { get; } = new("#87D7D7");
    public static DumpColor? DefaultIgnoredValueColor { get; } = null;
    public static DumpColor? DefaultMetadataInfoColor { get; } = new("#87D7D7");
    public static DumpColor? DefaultMetadataErrorColor { get; } = new("#D78700");
    public static DumpColor? DefaultLabelValueColor { get; } = new("#d7afd7");

    public DumpColor? TypeNameColor { get; set; } = DefaultTypeNameColor;
    public DumpColor? ColumnNameColor { get; set; } = DefaultColumnNameColor;
    public DumpColor? PropertyValueColor { get; set; } = DefaultPropertyValueColor;
    public DumpColor? PropertyNameColor { get; set; } = DefaultPropertyNameColor;
    public DumpColor? NullValueColor { get; set; } = DefaultNullValueColor;
    public DumpColor? IgnoredValueColor { get; set; } = DefaultIgnoredValueColor;
    public DumpColor? MetadataInfoColor { get; set; } = DefaultMetadataInfoColor;
    public DumpColor? MetadataErrorColor { get; set; } = DefaultMetadataErrorColor;
    public DumpColor? LabelValueColor { get; set; } = DefaultLabelValueColor;

    public static ColorConfig DefaultColors => new();
    public static ColorConfig NoColors => new(null);

    public ColorConfig(DumpColor? value)
    {
        TypeNameColor = value;
        ColumnNameColor = value;
        PropertyValueColor = value;
        PropertyNameColor = value;
        NullValueColor = value;
        IgnoredValueColor = value;
        MetadataInfoColor = value;
        MetadataErrorColor = value;
        LabelValueColor = value;
    }

    public ColorConfig()
    {

    }
}