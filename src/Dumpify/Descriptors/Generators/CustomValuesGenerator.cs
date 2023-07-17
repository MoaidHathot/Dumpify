using Dumpify.Descriptors.ValueProviders;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using System.Text;

namespace Dumpify.Descriptors.Generators;

internal class CustomValuesGenerator : IDescriptorGenerator
{
    private readonly ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>> _customTypeHandlers;

    public CustomValuesGenerator(ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>> customTypeHandlers)
    {
        _customTypeHandlers = customTypeHandlers;

        //Todo: find a better way to register types that have a specific CustomTypeRenderer, instead of adding them here.

        _customTypeHandlers.TryAdd(typeof(Type).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);
        _customTypeHandlers.TryAdd(typeof(PropertyInfo).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);
        _customTypeHandlers.TryAdd(typeof(FieldInfo).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);
        _customTypeHandlers.TryAdd(typeof(MethodInfo).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);
        _customTypeHandlers.TryAdd(typeof(ConstructorInfo).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);

        _customTypeHandlers.TryAdd(typeof(Enum).TypeHandle, (obj, type, e, memberProvider) => obj);
        _customTypeHandlers.TryAdd(typeof(DataTable).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);
        _customTypeHandlers.TryAdd(typeof(DataSet).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);

        _customTypeHandlers.TryAdd(typeof(StringBuilder).TypeHandle, (obj, type, valueProvider, memberProvider) => ((StringBuilder)obj).ToString());

        _customTypeHandlers.TryAdd(typeof(Guid).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);

        _customTypeHandlers.TryAdd(typeof(DateTime).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);
        _customTypeHandlers.TryAdd(typeof(DateTimeOffset).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);
        _customTypeHandlers.TryAdd(typeof(TimeSpan).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);
#if NET6_0_OR_GREATER
        _customTypeHandlers.TryAdd(typeof(DateOnly).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);
        _customTypeHandlers.TryAdd(typeof(TimeOnly).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);
#endif
    }

    public IDescriptor? Generate(Type type, IValueProvider? valueProvider, IMemberProvider memberProvider)
    {
        if (type.FullName == "System.RuntimeType")
        {
            return new CustomDescriptor(type, valueProvider);
        }

        if (type.IsEnum)
        {
            return new CustomDescriptor(type, valueProvider);
        }

        if (_customTypeHandlers.ContainsKey(type.TypeHandle))
        {
            return new CustomDescriptor(type, valueProvider);
        }

        if ((type.Namespace?.StartsWith("System.Reflection") ?? false) && !type.IsArray)
        {
            return new CustomDescriptor(type, valueProvider);
        }

        return null;
    }
}