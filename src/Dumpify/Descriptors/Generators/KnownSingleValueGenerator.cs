using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.Generators;
internal class KnownSingleValueGenerator : IDescriptorGenerator
{
    private static readonly HashSet<RuntimeTypeHandle> _singleValueTypes = new HashSet<RuntimeTypeHandle>
    {
        typeof(bool).TypeHandle,
        typeof(byte).TypeHandle,
        typeof(short).TypeHandle,
        typeof(ushort).TypeHandle,
        typeof(int).TypeHandle,
        typeof(uint).TypeHandle,
        typeof(long).TypeHandle,
        typeof(ulong).TypeHandle,
        typeof(float).TypeHandle,
        typeof(double).TypeHandle,
        typeof(decimal).TypeHandle,
        typeof(string).TypeHandle,
#if NET5_0_OR_GREATER
        typeof(Half).TypeHandle,
#endif
    };

    public IDescriptor? Generate(Type type, PropertyInfo? propertyInfo)
    {
        if(!_singleValueTypes.Contains(type.TypeHandle))
        {
            return null;
        }

        return new SingleValueDescriptor(type, propertyInfo);
    }
}
