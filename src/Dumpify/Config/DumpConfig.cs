using Dumpify.Descriptors.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify;

internal class DumpConfig
{
    public static DumpConfig Default { get; } = new DumpConfig();

    internal IDescriptorGenerator DescriptorGenerator { get; set; } = new CompositeDescriptorGenerator();
    
    public int MaxLevel { get; set; }
}
