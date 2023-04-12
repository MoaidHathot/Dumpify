using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.ValueProviders;

internal record class FieldValueProvider(FieldInfo FieldInfo) : IValueProvider
{
    public string Name => FieldInfo.Name;
    public MemberInfo Info => FieldInfo;
    public Type MemberType => FieldInfo.FieldType;

    public object? GetValue(object source)
        => FieldInfo.GetValue(source);
}
