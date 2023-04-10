using Dumpify.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Outputs;
public interface IDumpOutput
{
    public TextWriter TextWriter { get; }

    RendererConfig AdjustConfig(in RendererConfig config);
}
