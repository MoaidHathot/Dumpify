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
        return type.IsArray switch
        {
            true => GenerateArrayDescriptor(type, propertyInfo),
            _ => GenerateEnumerableDescriptor(type, propertyInfo),
        };
    }

    private MultiValueDescriptor? GenerateArrayDescriptor(Type type, PropertyInfo? propertyInfo) 
        => new MultiValueDescriptor(type, propertyInfo, type.GetElementType());

    private MultiValueDescriptor? GenerateEnumerableDescriptor(Type type, PropertyInfo? propertyInfo)
    {
        bool isEnumerable = false;

        foreach(var i in type.GetInterfaces()) 
        {
            if(i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return new MultiValueDescriptor(type, propertyInfo, i.GenericTypeArguments[0]);
            }
            else if(i == typeof(IEnumerable))
            {
                isEnumerable = true;
            }
        }

        if(isEnumerable is false)
        {
            return null;
        }

        return new MultiValueDescriptor(type, propertyInfo, null);
    }
}
