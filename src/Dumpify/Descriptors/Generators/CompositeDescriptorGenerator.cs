using Dumpify.Descriptors.ValueProviders;
using System.Collections.Concurrent;

namespace Dumpify.Descriptors.Generators;

internal class CompositeDescriptorGenerator : IDescriptorGenerator
{
    private readonly ConcurrentDictionary<(RuntimeTypeHandle, RuntimeTypeHandle?, string?, IMemberProvider), IDescriptor> _descriptorsCache = new();

    private readonly IDescriptorGenerator[] _generatorsChain;

    public CompositeDescriptorGenerator(ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>> customDescriptorHandlers)
    {
        _generatorsChain = new IDescriptorGenerator[]
        {
            new IgnoredValuesGenerator(),
            new CustomValuesGenerator(customDescriptorHandlers),
            new KnownSingleValueGenerator(),
            new MultiValueGenerator(),
        };
    }

    public IDescriptor? Generate(Type type, IValueProvider? valueProvider, IMemberProvider memberProvider)
    {
        var cacheKey = CreateCacheKey(type, valueProvider, memberProvider);

        if (_descriptorsCache.TryGetValue(cacheKey, out IDescriptor? cachedDescriptor))
        {
            return cachedDescriptor;
        }

        _descriptorsCache.TryAdd(cacheKey, new CircularDependencyDescriptor(type, valueProvider, null));

        var generatedDescriptor = GenerateDescriptor(type, valueProvider, memberProvider);

        if (generatedDescriptor is null)
        {
            throw new NotSupportedException($"Could not generate a Descriptor for type '{type.FullName}'");
        }

        _descriptorsCache.AddOrUpdate(cacheKey, generatedDescriptor, (key, descriptor) =>
        {
            if (descriptor is CircularDependencyDescriptor cdd)
            {
                cdd.Descriptor = generatedDescriptor;
            }

            return generatedDescriptor;
        });

        return generatedDescriptor;
    }

    (RuntimeTypeHandle, RuntimeTypeHandle?, string?, IMemberProvider) CreateCacheKey(Type type, IValueProvider? valueProvider, IMemberProvider memberProvider)
        => (type.TypeHandle, valueProvider?.Info.DeclaringType?.TypeHandle, valueProvider?.Name, memberProvider);

    private IDescriptor? GenerateDescriptor(Type type, IValueProvider? valueProvider, IMemberProvider memberProvider)
    {
        var descriptor = GetSpeciallyHandledDescriptor(type, valueProvider, memberProvider);

        if (descriptor is not null)
        {
            return descriptor;
        }

        var nestedDescriptor = GetNestedDescriptors(type, memberProvider);
        return new ObjectDescriptor(type, valueProvider, nestedDescriptor);
    }

    private IEnumerable<IDescriptor> GetNestedDescriptors(Type type, IMemberProvider memberProvider)
    {
        var list = new List<IDescriptor>();

        var members = memberProvider.GetMembers(type);

        foreach (var member in members)
        {
            var descriptor = Generate(member.MemberType, member, memberProvider);

            if (descriptor is not null)
            {
                list.Add(descriptor);
            }
        }

        return list;
    }

    private IDescriptor? GetSpeciallyHandledDescriptor(Type type, IValueProvider? valueProvider, IMemberProvider memberProvider)
    {
        foreach (var generator in _generatorsChain)
        {
            var descriptor = generator.Generate(type, valueProvider, memberProvider);

            if (descriptor is not null)
            {
                return descriptor;
            }
        }

        return null;
    }
}