using Spectre.Console;

namespace Dumpify;

public class SpectreRendererState
{
    public IColorConfig<Color?> Colors { get; }

    public SpectreRendererState(RendererConfig config)
    {
        Colors = new SpectreColorConfig
        {
            TypeNameColor = config.ColorConfig.TypeNameColor.ToSpectreColor(),
            NullValueColor = config.ColorConfig.NullValueColor.ToSpectreColor(),
            ColumnNameColor = config.ColorConfig.ColumnNameColor.ToSpectreColor(),
            IgnoredValueColor = config.ColorConfig.IgnoredValueColor.ToSpectreColor(),
            MetadataInfoColor = config.ColorConfig.MetadataInfoColor.ToSpectreColor(),
            PropertyNameColor = config.ColorConfig.PropertyNameColor.ToSpectreColor(),
            MetadataErrorColor = config.ColorConfig.MetadataErrorColor.ToSpectreColor(),
            PropertyValueColor = config.ColorConfig.PropertyValueColor.ToSpectreColor(),
            LabelValueColor = config.ColorConfig.LabelValueColor.ToSpectreColor(),
        };
    }
}