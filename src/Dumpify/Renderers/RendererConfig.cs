using Dumpify.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers;

public struct RendererConfig
{
    public string? Label { get; init; }
    public int? MaxDepth { get; init; }
    public bool? ShowTypeNames { get; init; }
    public bool? ShowHeaders { get; init; }
    public ColorConfig ColorConfig { get; set; } = new ColorConfig();

    public RendererConfig()
    {
        
    }
}
