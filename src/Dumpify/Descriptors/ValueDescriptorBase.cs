using Dumpify.Descriptors.ValueProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors;

internal abstract record ValueDescriptorBase(Type Type, IValueProvider? ValueProvider) : IDescriptor
{
    public string Name { get; } = (ValueProvider?.Name ?? Type.Name);
}
