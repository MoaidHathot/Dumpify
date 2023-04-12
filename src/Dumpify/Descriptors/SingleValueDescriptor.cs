using Dumpify.Descriptors.ValueProviders;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dumpify.Descriptors;

internal record SingleValueDescriptor(Type Type, IValueProvider? ValueProvider) : ValueDescriptorBase(Type, ValueProvider);

