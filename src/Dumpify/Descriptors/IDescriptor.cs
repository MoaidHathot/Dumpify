using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Descriptors;

internal interface IDescriptor
{
    Type Type { get; }
    string Name { get; }
    int NestingLevel { get; }
}
