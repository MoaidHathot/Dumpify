using Spectre.Console;

namespace Dumpify.Extensions;

internal static class TableBorderStyleExtensions
{
    internal static TableBorder ToSpectreBorder(this TableBorderStyle style) => style switch
    {
        TableBorderStyle.Rounded => TableBorder.Rounded,
        TableBorderStyle.Square => TableBorder.Square,
        TableBorderStyle.Ascii => TableBorder.Ascii,
        TableBorderStyle.None => TableBorder.None,
        TableBorderStyle.Heavy => TableBorder.Heavy,
        TableBorderStyle.Double => TableBorder.Double,
        TableBorderStyle.Minimal => TableBorder.Minimal,
        TableBorderStyle.Markdown => TableBorder.Markdown,
        _ => TableBorder.Rounded,
    };
}
