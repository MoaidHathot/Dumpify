using System.Diagnostics;
using System.Text;

namespace Dumpify.Outputs.TextWriters;

internal class TraceTextWriter : TextWriter
{
    public override Encoding Encoding { get; } = Encoding.ASCII;

    public override void Write(char value)
        => Trace.Write(value);

    public override void WriteLine()
        => Trace.WriteLine("");

    public override void Write(string? value)
        => Trace.Write(value);

    public override void WriteLine(string? value)
        => Trace.WriteLine(value);
}
