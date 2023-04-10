using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Extensions;
internal static class CollectionExtensions
{
    internal static bool None<T>(this IEnumerable<T> source)
        => !source.Any();
}
