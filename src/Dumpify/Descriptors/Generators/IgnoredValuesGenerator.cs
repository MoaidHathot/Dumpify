using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.Generators;
internal class IgnoredValuesGenerator : IDescriptorGenerator
{
    private readonly HashSet<RuntimeTypeHandle> _ignoredTypes = new HashSet<RuntimeTypeHandle> { };

    public IDescriptor? Generate(Type type, PropertyInfo? propertyInfo)
    {
        if(_ignoredTypes.Contains(type.TypeHandle))
        {
            return new IgnoredDescriptor(type, propertyInfo);
        }

        return null;
    }
}
