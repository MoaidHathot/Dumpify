using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors;

public interface IDescriptor
{
    Type Type { get; }
    PropertyInfo? PropertyInfo { get; }
    string Name { get; }
}
