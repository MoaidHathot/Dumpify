# TypeRenderingConfig

`TypeRenderingConfig` controls how specific value types are rendered in the output.

---

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `QuoteStringValues` | `bool` | `true` | Wrap string values in quotation marks |
| `StringQuotationChar` | `char` | `"` | Character used to quote strings |
| `QuoteCharValues` | `bool` | `true` | Wrap char values in quotation marks |
| `CharQuotationChar` | `char` | `'` | Character used to quote chars |

---

## Examples

### String Quotation

By default, strings are wrapped in double quotes:

```csharp
new { Name = "John" }.Dump();
// Output: Name = "John"
```

Disable string quotation:

```csharp
new { Name = "John" }.Dump(typeRenderingConfig: new TypeRenderingConfig 
{ 
    QuoteStringValues = false 
});
// Output: Name = John
```

### Custom String Quote Character

Use single quotes for strings:

```csharp
new { Name = "John" }.Dump(typeRenderingConfig: new TypeRenderingConfig 
{ 
    StringQuotationChar = '\'' 
});
// Output: Name = 'John'
```

### Char Quotation

By default, chars are wrapped in single quotes:

```csharp
new { Letter = 'A' }.Dump();
// Output: Letter = 'A'
```

Disable char quotation:

```csharp
new { Letter = 'A' }.Dump(typeRenderingConfig: new TypeRenderingConfig 
{ 
    QuoteCharValues = false 
});
// Output: Letter = A
```

### Custom Char Quote Character

Use backticks for chars:

```csharp
new { Letter = 'A' }.Dump(typeRenderingConfig: new TypeRenderingConfig 
{ 
    CharQuotationChar = '`' 
});
// Output: Letter = `A`
```

### Global Type Rendering Configuration

```csharp
// Set globally
DumpConfig.Default.TypeRenderingConfig.QuoteStringValues = true;
DumpConfig.Default.TypeRenderingConfig.StringQuotationChar = '"';
DumpConfig.Default.TypeRenderingConfig.QuoteCharValues = true;
DumpConfig.Default.TypeRenderingConfig.CharQuotationChar = '\'';

// All dumps use these settings
myObject.Dump();
```

### Complete Example

```csharp
var typeRenderingConfig = new TypeRenderingConfig
{
    QuoteStringValues = true,
    StringQuotationChar = '"',
    QuoteCharValues = true,
    CharQuotationChar = '\''
};

obj.Dump(typeRenderingConfig: typeRenderingConfig);
```

---

## Use Cases

### JSON-like Output

For output that resembles JSON:

```csharp
DumpConfig.Default.TypeRenderingConfig.QuoteStringValues = true;
DumpConfig.Default.TypeRenderingConfig.StringQuotationChar = '"';
```

### Plain Text Output

For output without any quotation:

```csharp
obj.Dump(typeRenderingConfig: new TypeRenderingConfig 
{ 
    QuoteStringValues = false,
    QuoteCharValues = false 
});
```

---

## See Also

- [Configuration Overview](index.md)
- [Global Configuration](global-configuration.md)
