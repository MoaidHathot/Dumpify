namespace Dumpify;

public class TypeRenderingConfig : IConfigMergeable<TypeRenderingConfig>
{
    private static readonly TypeRenderingConfig Defaults = new();

    public bool QuoteStringValues { get; set; } = true;
    public char StringQuotationChar { get; set; } = '"';
    public bool QuoteCharValues { get; set; } = true;
    public char CharQuotationChar { get; set; } = '\'';

    /// <inheritdoc />
    public TypeRenderingConfig MergeWith(TypeRenderingConfig? overrideConfig)
    {
        if (overrideConfig is null)
        {
            return this;
        }

        return new TypeRenderingConfig
        {
            QuoteStringValues = ConfigMergeHelper.Merge(QuoteStringValues, overrideConfig.QuoteStringValues, Defaults.QuoteStringValues),
            StringQuotationChar = ConfigMergeHelper.Merge(StringQuotationChar, overrideConfig.StringQuotationChar, Defaults.StringQuotationChar),
            QuoteCharValues = ConfigMergeHelper.Merge(QuoteCharValues, overrideConfig.QuoteCharValues, Defaults.QuoteCharValues),
            CharQuotationChar = ConfigMergeHelper.Merge(CharQuotationChar, overrideConfig.CharQuotationChar, Defaults.CharQuotationChar),
        };
    }
}
