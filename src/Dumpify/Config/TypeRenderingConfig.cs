namespace Dumpify;

public class TypeRenderingConfig
{
    public bool QuoteStringValues { get; set; } = true;
    public char StringQuotationChar { get; set; } = '"';
    public bool QuoteCharValues { get; set; } = true;
    public char CharQuotationChar { get; set; } = '\'';
}
