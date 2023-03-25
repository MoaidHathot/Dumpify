using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.Generators;

internal class CompositeDescriptorGenerator : IDescriptorGenerator
{
    private readonly Dictionary<RuntimeTypeHandle, IDescriptor> _descriptorsCache = new();

    private readonly IDescriptorGenerator[] _generatorsChain = new IDescriptorGenerator[]
    {
        new IgnoredValuesGenerator(),
        new CustomValuesGenerator(),
        new KnownSingleValueGenerator(),
        new MultiValueGenerator(),
        //new DefaultDescriptorGenerator(),
    };

    public IDescriptor? Generate(Type type, PropertyInfo? propertyInfo)
    {
        if (_descriptorsCache.TryGetValue(type.TypeHandle, out IDescriptor? cachedDescriptor))
        {
            return cachedDescriptor;
        }

        var generatedDescriptor = GenerateDescriptor(type, propertyInfo);

        if (generatedDescriptor is null)
        {
            throw new NotSupportedException($"Could not generate a Descriptor for type '{type.FullName}'");
        }

        _descriptorsCache.Add(type.TypeHandle, generatedDescriptor);

        return generatedDescriptor;
    }

    private IDescriptor? GenerateDescriptor(Type type, PropertyInfo? propertyInfo)
    { 
        var descriptor = GetSpecialyHandledDescriptor(type, propertyInfo);

        if(descriptor is not null)
        {
            return descriptor;
        }

        var nestedDescriptor = GetNestedDescriptors(type);
        return new ObjectDescriptor(type, propertyInfo, nestedDescriptor);
    }

    private IEnumerable<IDescriptor> GetNestedDescriptors(Type type)
    {
        var list = new List<IDescriptor>();

        foreach(var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetIndexParameters().Length == 0))
        {
            var descriptor = Generate(property.PropertyType, property);

            if(descriptor is not null)
            {
                list.Add(descriptor);
            }
        }

        return list;
    }

    private IDescriptor? GetSpecialyHandledDescriptor(Type type, PropertyInfo? propertyInfo)
    {
        foreach(var generator in _generatorsChain)
        {
            var descriptor = generator.Generate(type, propertyInfo);

            if(descriptor is not null)
            {
                return descriptor;
            }
        }

        return null;
    }
}
