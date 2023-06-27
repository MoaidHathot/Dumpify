using Dumpify.Extensions;
using System.Drawing;

namespace Dumpify.Config;

public class DumpColor
{
#if NETSTANDARD2_0
#else
    private static readonly ColorConverter _converter = new ();
#endif
    internal Color Color { get; }
    internal string HexString { get; }

#if NETSTANDARD2_0
public DumpColor(string hexColor)
{
    Color = HexToColor(hexColor);
    HexString = hexColor;
}
#else
    public DumpColor(string hexColor)
    {
        Color = HexToColor(hexColor);
        HexString = hexColor;
    }
#endif

    public DumpColor(Color color)
    {
        Color = color;
        HexString = ColorToHex(color);
    }

#if NETSTANDARD2_0
    private static Color HexToColor(string hexColor)
        => ColorFromHex(hexColor.MustNotBeNull($"Hex Color must not be null")).MustNotBeNull(reason: $"Unable to convert '{hexColor}'");
#else
    private static Color HexToColor(string hexColor)
        => (Color)_converter.ConvertFromString(hexColor.MustNotBeNull($"Hex Color must not be null")).MustNotBeNull(reason: $"Unable to convert '{hexColor}'");
#endif

    private static string ColorToHex(Color color)
        => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    private static Color ColorFromHex(string hexColor)
    {
        if (!(hexColor.Length == 9 || hexColor.Length == 7)
            || hexColor[0] != '#')
        {
            throw new ArgumentException(String.Format("{0} should be in format #AARRGGBB or  #RRGGBB", hexColor), nameof(hexColor));
        }
        byte alpha;
        int alphaOffset = 0;
        if (hexColor.Length == 9)
        {
            alpha = byte.Parse(hexColor[1..3], System.Globalization.NumberStyles.HexNumber);
        } else
        {
            alphaOffset = 2;
            alpha = 255;
        }
        var red = byte.Parse(hexColor[(3 - alphaOffset)..(5 - alphaOffset)], System.Globalization.NumberStyles.HexNumber);
        var green = byte.Parse(hexColor[(5 - alphaOffset)..(7 - alphaOffset)], System.Globalization.NumberStyles.HexNumber);
        var blue = byte.Parse(hexColor[(7 - alphaOffset)..(9 - alphaOffset)], System.Globalization.NumberStyles.HexNumber);
        return Color.FromArgb(alpha, red, green, blue);
    }

    public static DumpColor FromHexString(string hexColor)
        => new (hexColor);

    public static implicit operator DumpColor(string hexColor)
        => new (hexColor);

    public static implicit operator DumpColor(Color color)
        => new (color);
}
