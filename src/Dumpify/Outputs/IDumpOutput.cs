namespace Dumpify;
public interface IDumpOutput
{
    public TextWriter TextWriter { get; }

    RendererConfig AdjustConfig(in RendererConfig config);
}