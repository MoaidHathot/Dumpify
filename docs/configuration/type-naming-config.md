# TypeNamingConfig

`TypeNamingConfig` controls how type names are displayed in the dump output.

---

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `UseAliases` | `bool` | `true` | Use C# type aliases (e.g., `int` instead of `Int32`) |
| `UseFullName` | `bool` | `false` | Show full type names including namespace |
| `ShowTypeNames` | `bool` | `true` | Show type names at all |
| `SimplifyAnonymousObjectNames` | `bool` | `true` | Simplify anonymous type names |
| `SeparateTypesWithSpace` | `bool` | `true` | Add space between generic type parameters |

---

## Examples

### Use C# Aliases

When enabled (default), shows `int`, `string`, `bool` instead of `Int32`, `String`, `Boolean`:

```csharp
// Default behavior - aliases are used
obj.Dump(); // Shows "int", "string", etc.

// Disable aliases
obj.Dump(typeNames: new TypeNamingConfig { UseAliases = false });
// Shows "Int32", "String", etc.
```

### Show Full Type Names

Display the complete namespace-qualified type name:

```csharp
obj.Dump(typeNames: new TypeNamingConfig { UseFullName = true });
// Shows "System.Collections.Generic.List<System.String>" instead of "List<string>"
```

### Hide Type Names

Remove type names entirely for cleaner output:

```csharp
obj.Dump(typeNames: new TypeNamingConfig { ShowTypeNames = false });
```

![No type names](https://user-images.githubusercontent.com/8770486/232252319-58a98036-5a0e-4514-8d08-df6fdff5a8a7.png)

### Simplify Anonymous Object Names

Anonymous types have compiler-generated names like `<>f__AnonymousType0`. When enabled (default), these are simplified:

```csharp
// Default - simplified name
new { Name = "Test" }.Dump(); // Shows simplified type name

// Show actual compiler name
new { Name = "Test" }.Dump(typeNames: new TypeNamingConfig 
{ 
    SimplifyAnonymousObjectNames = false 
});
```

### Control Type Separator Spacing

Controls spacing in generic type parameters:

```csharp
// With space (default)
// Dictionary<string, int>

// Without space
obj.Dump(typeNames: new TypeNamingConfig { SeparateTypesWithSpace = false });
// Dictionary<string,int>
```

### Global Type Naming Configuration

```csharp
// Set globally
DumpConfig.Default.TypeNamingConfig.UseAliases = true;
DumpConfig.Default.TypeNamingConfig.ShowTypeNames = true;
DumpConfig.Default.TypeNamingConfig.UseFullName = false;
DumpConfig.Default.TypeNamingConfig.SimplifyAnonymousObjectNames = true;

// All dumps use these settings
myObject.Dump();
```

### Combined with TableConfig

Hide both type names and table headers for minimal output:

```csharp
obj.Dump(
    typeNames: new TypeNamingConfig { ShowTypeNames = false },
    tableConfig: new TableConfig { ShowTableHeaders = false }
);
```

---

## Complete Example

```csharp
var typeNamingConfig = new TypeNamingConfig
{
    UseAliases = true,               // int instead of Int32
    UseFullName = false,             // List<T> instead of System.Collections.Generic.List<T>
    ShowTypeNames = true,            // Show type names
    SimplifyAnonymousObjectNames = true,  // Clean anonymous type names
    SeparateTypesWithSpace = true    // Dictionary<string, int>
};

obj.Dump(typeNames: typeNamingConfig);
```

---

## See Also

- [Configuration Overview](index.md)
- [Global Configuration](global-configuration.md)
- [TableConfig](table-config.md)
