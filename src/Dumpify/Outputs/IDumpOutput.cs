using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify;
public interface IDumpOutput
{
    public TextWriter TextWriter { get; }

    RendererConfig AdjustConfig(in RendererConfig config);
}
