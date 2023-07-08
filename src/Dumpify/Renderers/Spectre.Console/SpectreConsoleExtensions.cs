using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify;

internal static class SpectreConsoleExtensions
{
    public static TableColumn ZeroPadding(this TableColumn column)
    {
        column.Padding = new Padding(0);
        return column;
    }
}
