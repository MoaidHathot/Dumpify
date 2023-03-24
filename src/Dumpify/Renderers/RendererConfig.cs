using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers;

public struct RendererConfig
{
    string? Label { get; init; }
    int? MaxNestingLevel { get; init; }
}
