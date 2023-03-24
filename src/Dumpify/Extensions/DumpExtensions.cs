using Dumpify.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Extensions;
public static class DumpExtensions
{
    public static T Dump<T>(this T obj, string? label = null, int? maxNestingLevel = null, IRenderer? renderer = null)
    {
        return obj;
    }
}
