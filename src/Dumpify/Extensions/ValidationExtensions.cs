using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Extensions;
internal static class ValidationExtensions
{
    public static int? MustBeGreaterThan(this int? value, int greater)
    {
        if(value is null)
        {
            return null;
        }

        return value.Value.MustBeGreaterThan(greater);
    }

    public static int MustBeGreaterThan(this int value, int greater)
    {
        if(value <= greater)
        {
            throw new ArgumentException($"{value} must be greater than {greater}");
        }

        return value;
    }
}
