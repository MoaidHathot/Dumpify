using System.Diagnostics;
using System.Text;

namespace Dumpify.Outputs.TextWriters;

internal class DebugTextWriter : TextWriter
{
    public override Encoding Encoding { get; } = Encoding.UTF8;

    public override void Write(char value)
        => Debug.Write(value);

    public override void WriteLine()
        => Debug.WriteLine("");

    public override void Write(string? value)
        => Debug.Write(value);

    public override void WriteLine(string? value)
        => Debug.WriteLine(value);
}
