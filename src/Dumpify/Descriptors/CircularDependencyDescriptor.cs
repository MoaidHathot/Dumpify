using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors;
internal record CircularDependencyDescriptor(Type Type, PropertyInfo? PropertyInfo, IDescriptor? Descriptor) : IDescriptor
{
    public string Name => PropertyInfo?.Name ?? Type.Name;

    public IDescriptor? Descriptor { get; set; } = Descriptor;
}
