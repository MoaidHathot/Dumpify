using Dumpify.Descriptors.ValueProviders;
using System.Collections.Concurrent;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Dumpify.Descriptors.Generators;

internal class CustomValuesGenerator : IDescriptorGenerator
{
    private readonly ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>> _customTypeHandlers;

    public CustomValuesGenerator(ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>> customTypeHandlers)
    {
        _customTypeHandlers = customTypeHandlers;

        _customTypeHandlers.TryAdd(typeof(Type).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);

        _customTypeHandlers.TryAdd(typeof(PropertyInfo).TypeHandle, (obj, type, valueProvider, memberProvider) =>
        {
            var property = ((PropertyInfo)obj);
            return $"{property.PropertyType.Name} {property.Name} {{ {(property.SetMethod is not null ? "set; " : "")}{(property.GetMethod is not null ? "get; " : "")}}}";
        });

        _customTypeHandlers.TryAdd(typeof(FieldInfo).TypeHandle, (obj, type, valueProvider, memberProvider) =>
        {
            var field = ((FieldInfo)obj);
            return $"{field.FieldType.Name} {field.Name}";
        });

        _customTypeHandlers.TryAdd(typeof(MethodInfo).TypeHandle, (obj, type, valueProvider, memberProvider) =>
        {
            var method = ((MethodInfo)obj);

            return $"{method.ReturnType.Name} {method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType} {p.Name}"))})";
        });

        _customTypeHandlers.TryAdd(typeof(ConstructorInfo).TypeHandle, (obj, type, valueProvider, memberProvider) =>
        {
            var ctor = ((ConstructorInfo)obj);

            return $"Constructor {ctor.Name}({string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType} {p.Name}"))})";
        });

        _customTypeHandlers.TryAdd(typeof(Enum).TypeHandle, (obj, type, e, memberProvider) => $"{type.Name}{e}");
        _customTypeHandlers.TryAdd(typeof(DataTable).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);
        _customTypeHandlers.TryAdd(typeof(DataSet).TypeHandle, (obj, type, valueProvider, memberProvider) => obj);

        _customTypeHandlers.TryAdd(typeof(StringBuilder).TypeHandle, (obj, type, valueProvider, memberProvider) => ((StringBuilder)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(DateTime).TypeHandle, (obj, type, valueProvider, memberProvider) => ((DateTime)obj).ToString(CultureInfo.CurrentCulture));
        _customTypeHandlers.TryAdd(typeof(DateTimeOffset).TypeHandle, (obj, type, valueProvider, memberProvider) => ((DateTimeOffset)obj).ToString());

#if NET6_0_OR_GREATER
        _customTypeHandlers.TryAdd(typeof(DateOnly).TypeHandle, (obj, type, valueProvider, memberProvider) => ((DateOnly)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(TimeOnly).TypeHandle, (obj, type, valueProvider, memberProvider) => ((TimeOnly)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(TimeSpan).TypeHandle, (obj, type, valueProvider, memberProvider) => ((TimeSpan)obj).ToString());
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

        if(_customTypeHandlers.ContainsKey(type.TypeHandle))
        {
            return new CustomDescriptor(type, valueProvider);
        }

        return null;
    }
}
