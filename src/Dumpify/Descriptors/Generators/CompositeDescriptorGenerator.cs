using Dumpify.Descriptors.ValueProviders;
using System.Collections.Concurrent;

namespace Dumpify.Descriptors.Generators;

internal class CompositeDescriptorGenerator : IDescriptorGenerator
{
    private readonly ConcurrentDictionary<(RuntimeTypeHandle, RuntimeTypeHandle?, string?, IMemberProvider), IDescriptor> _descriptorsCache = new();
    //Used to resolve race conditions when generating descriptors for self-referencing types.
    //The key is the same as the cache key, and the value is a placeholder descriptor that will be linked to the real descriptor once generation completes.
    private readonly ThreadLocal<Dictionary<(RuntimeTypeHandle, RuntimeTypeHandle?, string?, IMemberProvider), CircularDependencyDescriptor>> _threadLocalGenerating = new(() => new());

    private readonly IDescriptorGenerator[] _generatorsChain;

    public CompositeDescriptorGenerator(ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>> customDescriptorHandlers)
    {
        _generatorsChain =
        [
            new IgnoredValuesGenerator(),
            new CustomValuesGenerator(customDescriptorHandlers),
            new KnownSingleValueGenerator(),
            new MultiValueGenerator(),
        ];
    }

    public IDescriptor? Generate(Type type, IValueProvider? valueProvider, IMemberProvider memberProvider)
    {
        var cacheKey = CreateCacheKey(type, valueProvider, memberProvider);

        if (_descriptorsCache.TryGetValue(cacheKey, out IDescriptor? cachedDescriptor))
        {
            return cachedDescriptor;
        }

        return GenerateWithCircularReferenceTracking(cacheKey, type, valueProvider, memberProvider);
    }

    private IDescriptor GenerateWithCircularReferenceTracking(
        (RuntimeTypeHandle, RuntimeTypeHandle?, string?, IMemberProvider) cacheKey,
        Type type,
        IValueProvider? valueProvider,
        IMemberProvider memberProvider)
    {
        var generating = _threadLocalGenerating.Value!;

        if (generating.TryGetValue(cacheKey, out var placeholder))
        {
            return placeholder;
        }

        // Use a placeholder to handle self-referencing types (e.g., class Node { Node? Parent; }).
        // When generating nested descriptors, if we encounter the same type we're currently generating,
        // we return this placeholder instead of recursing infinitely. After generation completes,
        // we link the placeholder to the real descriptor so rendering can unwrap it.
        placeholder = new CircularDependencyDescriptor(type, valueProvider, null);
        generating[cacheKey] = placeholder;

        try
        {
            var generatedDescriptor = GenerateDescriptor(type, valueProvider, memberProvider);

            if (generatedDescriptor is null)
            {
                throw new NotSupportedException($"Could not generate a Descriptor for type '{type.FullName}'");
            }

            placeholder.Descriptor = generatedDescriptor;
            _descriptorsCache.TryAdd(cacheKey, generatedDescriptor);

            return generatedDescriptor;
        }
        finally
        {
            generating.Remove(cacheKey);
        }
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