using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.ValueProviders;
internal record PropertyValueProvider(PropertyInfo PropertyInfo) : IValueProvider
{
    public string Name => PropertyInfo.Name;
    public MemberInfo Info => PropertyInfo;
    public Type MemberType => PropertyInfo.PropertyType;

    public object? GetValue(object source)
        => PropertyInfo.GetValue(source);
}
