using Spectre.Console;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.Generators;

internal class CustomValuesGenerator : IDescriptorGenerator
{
    private readonly ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, PropertyInfo?, object>> _customTypeHandlers;

    public CustomValuesGenerator(ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, PropertyInfo?, object>> customTypeHandlers)
    {
        _customTypeHandlers = customTypeHandlers;

        _customTypeHandlers.TryAdd(typeof(Type).TypeHandle, (obj, type, propertyInfo) => $"Typeof({((Type)obj).Name})");

        _customTypeHandlers.TryAdd(typeof(PropertyInfo).TypeHandle, (obj, type, propertyInfo) =>
        {
            var property = ((PropertyInfo)obj);
            return $"{property.PropertyType.Name} {property.Name} {{ {(property.SetMethod is not null ? "set; " : "")}{(property.GetMethod is not null ? "get; " : "")}}}";
        });

        _customTypeHandlers.TryAdd(typeof(MethodInfo).TypeHandle, (obj, type, propertyInfo) =>
        {
            var method = ((MethodInfo)obj);

            return $"{method.ReturnType.Name} {method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType} {p.Name}"))})";
        });

        _customTypeHandlers.TryAdd(typeof(ConstructorInfo).TypeHandle, (obj, type, propertyInfo) =>
        {
            var ctor = ((ConstructorInfo)obj);

            return $"Constructor {ctor.Name}({string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType} {p.Name}"))})";
        });

        _customTypeHandlers.TryAdd(typeof(Enum).TypeHandle, (obj, type, e) =>
        {
            return $"{type.Name}{e}";
        });

        _customTypeHandlers.TryAdd(typeof(StringBuilder).TypeHandle, (obj, type, propertyInfo) => ((StringBuilder)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(DateTime).TypeHandle, (obj, type, propertyInfo) => ((DateTime)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(DateTimeOffset).TypeHandle, (obj, type, propertyInfo) => ((DateTimeOffset)obj).ToString());
#if NET6_0_OR_GREATER
        _customTypeHandlers.TryAdd(typeof(DateOnly).TypeHandle, (obj, type, propertyInfo) => ((DateOnly)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(TimeOnly).TypeHandle, (obj, type, propertyInfo) => ((TimeOnly)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(TimeSpan).TypeHandle, (obj, type, propertyInfo) => ((TimeSpan)obj).ToString());
#endif
    }

    public IDescriptor? Generate(Type type, PropertyInfo? propertyInfo)
    {
        if(type.IsEnum)
        {
            _customTypeHandlers.TryAdd(type.TypeHandle, (obj, type, propertyInfo) => $"{type.Name}.{obj}");
            return new CustomDescriptor(type, propertyInfo);
        }

        if(_customTypeHandlers.ContainsKey(type.TypeHandle))
        {
            return new CustomDescriptor(type, propertyInfo);
        }

        return null;
    }
}
