using Dumpify.Descriptors.ValueProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors;

internal record ObjectDescriptor(Type Type, IValueProvider? ValueProvider, IEnumerable<IDescriptor> Properties) : IDescriptor
{
    public string Name => ValueProvider?.Name ?? Type.Name;
}
