using Dumpify.Descriptors.ValueProviders;
using System.Collections.Concurrent;

namespace Dumpify.Descriptors.Generators;

internal class CompositeDescriptorGenerator : IDescriptorGenerator
{
    // Cache stores either the final descriptor OR a CircularDependencyDescriptor placeholder
    private readonly ConcurrentDictionary<(RuntimeTypeHandle, RuntimeTypeHandle?, string?, IMemberProvider), IDescriptor> _descriptorsCache = new();

    // Track which keys are currently being generated, with their placeholder descriptors
    // Key: cache key, Value: (generatingThreadId, CircularDependencyDescriptor placeholder)
    private readonly ConcurrentDictionary<(RuntimeTypeHandle, RuntimeTypeHandle?, string?, IMemberProvider), (int threadId, CircularDependencyDescriptor placeholder)> _generating = new();

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

        // Fast path: check if already fully generated and cached
        if (_descriptorsCache.TryGetValue(cacheKey, out var cachedDescriptor))
        {
            return cachedDescriptor;
        }

        // Check if another thread is currently generating this descriptor
        // or if this is a re-entrant call on the same thread (circular reference)
        if (_generating.TryGetValue(cacheKey, out var generatingInfo))
        {
            if (generatingInfo.threadId == Environment.CurrentManagedThreadId)
            {
                // Re-entrant call on same thread - return the placeholder for circular reference handling
                return generatingInfo.placeholder;
            }
            else
            {
                // Another thread is generating - spin-wait for it to complete
                // This is a simple approach; could use more sophisticated synchronization if needed
                SpinWait spinner = default;
                while (_generating.ContainsKey(cacheKey) && !_descriptorsCache.ContainsKey(cacheKey))
                {
                    spinner.SpinOnce();
                }

                // Now check cache again - should have the generated descriptor
                if (_descriptorsCache.TryGetValue(cacheKey, out cachedDescriptor))
                {
                    return cachedDescriptor;
                }
                // Fall through to generate if still not present (edge case)
            }
        }

        // Start generating - create placeholder for circular reference handling
        var placeholder = new CircularDependencyDescriptor(type, valueProvider, null);
        var thisThread = Environment.CurrentManagedThreadId;

        // Try to claim generation of this key
        if (!_generating.TryAdd(cacheKey, (thisThread, placeholder)))
        {
            // Another thread just started generating - retry from the top
            return Generate(type, valueProvider, memberProvider);
        }

        try
        {
            var generatedDescriptor = GenerateDescriptor(type, valueProvider, memberProvider);

            if (generatedDescriptor is null)
            {
                throw new NotSupportedException($"Could not generate a Descriptor for type '{type.FullName}'");
            }

            // Link the placeholder to the real descriptor (for any code that received the placeholder)
            placeholder.Descriptor = generatedDescriptor;

            // Store in cache
            _descriptorsCache[cacheKey] = generatedDescriptor;

            return generatedDescriptor;
        }
        finally
        {
            // Remove from generating set
            _generating.TryRemove(cacheKey, out _);
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