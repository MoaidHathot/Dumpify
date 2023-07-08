using Dumpify;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify;

internal static class MappingExtensions
{
    public static Color? ToSpectreColor(this DumpColor? color)
        => color is null ? null : new Color(color.Color.R, color.Color.G, color.Color.B);
}
