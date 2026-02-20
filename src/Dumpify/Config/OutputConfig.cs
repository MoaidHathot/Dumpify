namespace Dumpify;

public class OutputConfig : IConfigMergeable<OutputConfig>
{
    private static readonly OutputConfig Defaults = new();

    public int? WidthOverride { get; set; }
    public int? HeightOverride { get; set; }

    /// <inheritdoc />
    public OutputConfig MergeWith(OutputConfig? overrideConfig)
    {
        if (overrideConfig is null)
        {
            return this;
        }

        return new OutputConfig
        {
            WidthOverride = ConfigMergeHelper.Merge(WidthOverride, overrideConfig.WidthOverride, Defaults.WidthOverride),
            HeightOverride = ConfigMergeHelper.Merge(HeightOverride, overrideConfig.HeightOverride, Defaults.HeightOverride),
        };
    }
}