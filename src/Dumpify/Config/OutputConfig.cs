namespace Dumpify;

public class OutputConfig : ConfigBase<OutputConfig>
{
    private static readonly OutputConfig Defaults = new();

    public int? WidthOverride { get; set; }
    public int? HeightOverride { get; set; }

    /// <inheritdoc />
    protected override OutputConfig MergeOverride(OutputConfig overrideConfig)
    {
        return new OutputConfig
        {
            WidthOverride = Merge(WidthOverride, overrideConfig.WidthOverride, Defaults.WidthOverride),
            HeightOverride = Merge(HeightOverride, overrideConfig.HeightOverride, Defaults.HeightOverride),
        };
    }
}
