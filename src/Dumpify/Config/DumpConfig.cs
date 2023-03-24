using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Config;

internal class DumpConfig
{
    public static DumpConfig Default { get; } = new DumpConfig();
    
    public int MaxLevel { get; set; }
}
