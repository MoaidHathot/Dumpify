using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.Generators;

internal class CustomValuesGenerator : IDescriptorGenerator
{
    private readonly HashSet<RuntimeTypeHandle> _customTypes = new HashSet<RuntimeTypeHandle> { typeof(Type).TypeHandle, typeof(StringBuilder).TypeHandle };

    public IDescriptor? Generate(Type type, PropertyInfo? propertyInfo)
    {
        if(_customTypes.Contains(type.TypeHandle))
        {
            return new CustomDescriptor(type, propertyInfo);
        }

        return null;
    }
}
