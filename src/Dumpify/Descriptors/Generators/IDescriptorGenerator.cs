using Dumpify.Descriptors.ValueProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors.Generators;

public interface IDescriptorGenerator
{
    IDescriptor? Generate(Type type, IValueProvider? valueProvider, IMemberProvider memberProvider);
}
