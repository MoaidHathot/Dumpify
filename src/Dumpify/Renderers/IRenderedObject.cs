namespace Dumpify;

public interface IRenderedObject
{
    public void Output(IDumpOutput output, OutputConfig config);
}