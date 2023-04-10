using Dumpify.Outputs;

namespace Dumpify.Renderers;

public interface IRenderedObject
{
    public void Output(IDumpOutput output);
}
