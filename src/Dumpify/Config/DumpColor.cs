using Dumpify.Extensions;
using System.Drawing;

namespace Dumpify.Config;

public class DumpColor
{
    private readonly static ColorConverter _converter = new ColorConverter();

    internal readonly Color Color;

    public DumpColor(string hexColor)
    {
        Color = HexToColor(hexColor);
    }

    public DumpColor(Color color)
        => Color = color;

    private static Color HexToColor(string hexColor)
        => (Color)_converter.ConvertFromString(hexColor.MustNotBeNull($"Hex Color must not be null")).MustNotBeNull(reason: $"Unable to convert '{hexColor}'");

    public static DumpColor FromHexString(string hexColor) 
        => new DumpColor(hexColor);

    public static implicit operator DumpColor(string hexColor)
        => new DumpColor(hexColor);

    public static implicit operator DumpColor(Color color)
        => new DumpColor(color);
}
