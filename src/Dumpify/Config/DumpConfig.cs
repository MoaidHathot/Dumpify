using Dumpify.Descriptors.Generators;
using Dumpify.Renderers;
using Dumpify.Renderers.Json;
using Dumpify.Renderers.Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify;

public class DumpConfig
{
    public static DumpConfig Default { get; } = new DumpConfig();

    public IDescriptorGenerator Generator { get; set; } = new CompositeDescriptorGenerator();
    public IRenderer Renderer { get; set; } = new SpectreTableRenderer();

    public bool useDescriptors = true;
    public int MaxNestingLevel { get; set; }
}
