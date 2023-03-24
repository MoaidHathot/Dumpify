using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dumpify.Descriptors;

internal record SingleValueDescriptor(Type Type, PropertyInfo PropertyInfo, int NestingLevel) : IDescriptor 
{
    public string Name => PropertyInfo.Name;
}

