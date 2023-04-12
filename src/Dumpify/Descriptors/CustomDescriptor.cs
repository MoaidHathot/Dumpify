using Dumpify.Descriptors.ValueProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors;

internal record CustomDescriptor(Type Type, IValueProvider? ValueProvider) : IDescriptor
{
    public string Name => ValueProvider?.Name ?? Type.Name;
}
