using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.Generators;

internal class MultiValueGenerator : IDescriptorGenerator
{
    public IDescriptor? Generate(Type type, PropertyInfo? propertyInfo)
    {
        var enumerableList = new List<Type?>(2);

        foreach(var i in type.GetInterfaces()) 
        {
            if(i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                enumerableList.Add(i.GenericTypeArguments[0]);
            }
            else if(i == typeof(IEnumerable))
            {
                enumerableList.Add(null);
            }
        }

        if(enumerableList.Count == 0)
        {
            return null;
        }

        var genericParameterType = enumerableList.FirstOrDefault(t => t is not null);

        return new MultiValueDescriptor(type, propertyInfo, genericParameterType);
    }
}
