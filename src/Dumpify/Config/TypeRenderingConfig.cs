namespace Dumpify;

public class TypeRenderingConfig : ConfigBase<TypeRenderingConfig>
{
    public TrackableProperty<bool> QuoteStringValues { get; set; } = new(true);
    public TrackableProperty<char> StringQuotationChar { get; set; } = new('"');
    public TrackableProperty<bool> QuoteCharValues { get; set; } = new(true);
    public TrackableProperty<char> CharQuotationChar { get; set; } = new('\'');

    /// <inheritdoc />
    protected override TypeRenderingConfig MergeOverride(TypeRenderingConfig overrideConfig)
    {
        return new TypeRenderingConfig
        {
            QuoteStringValues = Merge(QuoteStringValues, overrideConfig.QuoteStringValues),
            StringQuotationChar = Merge(StringQuotationChar, overrideConfig.StringQuotationChar),
            QuoteCharValues = Merge(QuoteCharValues, overrideConfig.QuoteCharValues),
            CharQuotationChar = Merge(CharQuotationChar, overrideConfig.CharQuotationChar),
        };
    }
}
