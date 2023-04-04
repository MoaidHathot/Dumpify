using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.Generators;

internal class CompositeDescriptorGenerator : IDescriptorGenerator
{
    private readonly ConcurrentDictionary<(RuntimeTypeHandle, RuntimeTypeHandle?, string?), IDescriptor> _descriptorsCache = new();

    private readonly IDescriptorGenerator[] _generatorsChain;

    public CompositeDescriptorGenerator(ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, PropertyInfo?, object>> customDescriptorHandlers)
    {
        _generatorsChain = new IDescriptorGenerator[]
        {
            new IgnoredValuesGenerator(),
            new CustomValuesGenerator(customDescriptorHandlers),
            new KnownSingleValueGenerator(),
            new MultiValueGenerator(),
        };
    }

    public IDescriptor? Generate(Type type, PropertyInfo? propertyInfo)
    {
        var cacheKey = CreateCacheKey(type, propertyInfo);

        if (_descriptorsCache.TryGetValue(cacheKey, out IDescriptor? cachedDescriptor))
        {
            return cachedDescriptor;
        }

        _descriptorsCache.TryAdd(cacheKey, new CircularDependencyDescriptor(type, propertyInfo, null));

        var generatedDescriptor = GenerateDescriptor(type, propertyInfo);

        if (generatedDescriptor is null)
        {
            throw new NotSupportedException($"Could not generate a Descriptor for type '{type.FullName}'");
        }

        _descriptorsCache.AddOrUpdate(cacheKey, generatedDescriptor, (key, descriptor) =>
        {
            if(descriptor is CircularDependencyDescriptor cdd)
            {
                cdd.Descriptor = generatedDescriptor;
            }

            return generatedDescriptor;
        });

        return generatedDescriptor;
    }

    (RuntimeTypeHandle, RuntimeTypeHandle?, string?) CreateCacheKey(Type type, PropertyInfo? propertyInfo)
        => (type.TypeHandle, propertyInfo?.DeclaringType?.TypeHandle, propertyInfo?.Name);

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

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetIndexParameters().Length == 0);

        foreach(var property in properties)
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
