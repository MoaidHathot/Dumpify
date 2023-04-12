using Dumpify.Descriptors.ValueProviders;
using System.Collections;

namespace Dumpify.Descriptors.Generators;

internal class MultiValueGenerator : IDescriptorGenerator
{
    public IDescriptor? Generate(Type type, IValueProvider? valueProvider, IMemberProvider memberProvider)
    {
        return type.IsArray switch
        {
            true => GenerateArrayDescriptor(type, valueProvider),
            _ => GenerateEnumerableDescriptor(type, valueProvider),
        };
    }

    private MultiValueDescriptor? GenerateArrayDescriptor(Type type, IValueProvider? valueProvider) 
        => new MultiValueDescriptor(type, valueProvider, type.GetElementType());

    private MultiValueDescriptor? GenerateEnumerableDescriptor(Type type, IValueProvider? valueProvider)
    {
        bool isEnumerable = false;

        foreach(var i in type.GetInterfaces()) 
        {
            if(i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return new MultiValueDescriptor(type, valueProvider, i.GenericTypeArguments[0]);
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

        return new MultiValueDescriptor(type, valueProvider, null);
    }
}
