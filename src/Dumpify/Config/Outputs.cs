using Dumpify.Outputs;
using Dumpify.Outputs.TextWriters;

namespace Dumpify.Config;

public static class Outputs
{
    public static IDumpOutput Console { get;  } = new DumpOutput(System.Console.Out, c => c);
    public static IDumpOutput Trace { get;  } = new DumpOutput(new TraceTextWriter(), c => c with { ColorConfig = ColorConfig.NoColors });
    public static IDumpOutput Debug { get;  } = new DumpOutput(new DebugTextWriter(), c => c with { ColorConfig = ColorConfig.NoColors });
}
