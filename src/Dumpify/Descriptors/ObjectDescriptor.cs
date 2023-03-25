using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors;

internal record ObjectDescriptor(Type Type, PropertyInfo? ParentPropertyInfo, IEnumerable<IDescriptor> Properties) : IDescriptor
{
    public string Name => ParentPropertyInfo?.Name ?? Type.Name;
}
