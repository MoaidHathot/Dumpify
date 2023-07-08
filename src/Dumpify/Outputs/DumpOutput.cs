using Dumpify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify;

public class DumpOutput : IDumpOutput
{
    private readonly Func<RendererConfig, RendererConfig> _configFactory;

    public TextWriter TextWriter { get; }

    public DumpOutput(TextWriter writer, Func<RendererConfig, RendererConfig>? configFactory = null)
    {
        _configFactory = configFactory ?? (c => c);
        TextWriter = writer;
    }

    public RendererConfig AdjustConfig(in RendererConfig config)
        => _configFactory(config);
}
