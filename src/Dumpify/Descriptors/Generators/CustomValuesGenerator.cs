using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.Generators;

internal class CustomValuesGenerator : IDescriptorGenerator
{
    private readonly Dictionary<RuntimeTypeHandle, Func<object, Type, PropertyInfo?, object>> _customTypeHandlers;

    public CustomValuesGenerator(Dictionary<RuntimeTypeHandle, Func<object, Type, PropertyInfo?, object>> customTypeHandlers)
    {
        _customTypeHandlers = customTypeHandlers;

        _customTypeHandlers.TryAdd(typeof(Type).TypeHandle, (obj, type, propertyInfo) => $"Typeof({((Type)obj).Name})");
        _customTypeHandlers.TryAdd(typeof(PropertyInfo).TypeHandle, (obj, type, propertyInfo) => $"{((PropertyInfo)obj).PropertyType} {((PropertyInfo)obj).Name} {{ {(((PropertyInfo)obj).SetMethod is not null ? "set; " : "" )}{(((PropertyInfo)obj).GetMethod is not null ? "get; " : "")}}}");
        _customTypeHandlers.TryAdd(typeof(StringBuilder).TypeHandle, (obj, type, propertyInfo) => ((StringBuilder)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(DateTime).TypeHandle, (obj, type, propertyInfo) => ((DateTime)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(DateTimeOffset).TypeHandle, (obj, type, propertyInfo) => ((DateTimeOffset)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(DateOnly).TypeHandle, (obj, type, propertyInfo) => ((DateOnly)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(TimeOnly).TypeHandle, (obj, type, propertyInfo) => ((TimeOnly)obj).ToString());
        _customTypeHandlers.TryAdd(typeof(TimeSpan).TypeHandle, (obj, type, propertyInfo) => ((TimeSpan)obj).ToString());
    }

    public IDescriptor? Generate(Type type, PropertyInfo? propertyInfo)
    {
        if(_customTypeHandlers.ContainsKey(type.TypeHandle))
        {
            return new CustomDescriptor(type, propertyInfo);
        }

        return null;
    }
}
