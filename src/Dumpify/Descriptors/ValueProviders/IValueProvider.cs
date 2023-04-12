using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.ValueProviders;

public interface IValueProvider
{
    public string Name { get; }
    MemberInfo Info { get; }
    public Type MemberType { get; }
    public object? GetValue(object source);
}
