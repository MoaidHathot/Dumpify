using Dumpify.Extensions;
using System.Drawing;

namespace Dumpify;

public class DumpColor
{
    private static readonly System.ComponentModel.TypeConverter _converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Color));

    internal Color Color { get; }
    internal string HexString { get; }

    public DumpColor(string hexColor)
    {
        Color = HexToColor(hexColor);
        HexString = hexColor;
    }

    public DumpColor(Color color)
    {
        Color = color;
        HexString = ColorToHex(color);
    }

    private static Color HexToColor(string hexColor)
        => (Color)_converter.ConvertFromString(hexColor.MustNotBeNull($"Hex Color must not be null")).MustNotBeNull(reason: $"Unable to convert '{hexColor}'");

    private static string ColorToHex(Color color)
        => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    public static DumpColor FromHexString(string hexColor)
        => new (hexColor);

    public static implicit operator DumpColor(string hexColor)
        => new (hexColor);

    public static implicit operator DumpColor(Color color)
        => new (color);
}