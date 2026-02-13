# Configuration

Dumpify provides extensive configuration options to customize how objects are rendered.

## Table of Contents

- [Configuration Methods](#configuration-methods)
- [Global Configuration](#global-configuration)
- [Per-Dump Configuration](#per-dump-configuration)
- [Color Configuration](#color-configuration)
- [Table Configuration](#table-configuration)
- [Members Configuration](#members-configuration)
- [Type Naming Configuration](#type-naming-configuration)
- [Type Rendering Configuration](#type-rendering-configuration)
- [Output Configuration](#output-configuration)

## Configuration Methods

### Global Configuration

Set defaults that apply to all dumps:

```csharp
DumpConfig.Default.MaxDepth = 5;
DumpConfig.Default.ColorConfig.PropertyValueColor = new DumpColor(Color.Aqua);
```

### Per-Dump Configuration

Override for a single dump:

```csharp
// Using config builder
data.Dump(config => config
    .SetMaxDepth(3)
    .UseColorConfig(c => c.SetLabelColor(Color.Yellow)));

// Using DumpConfig instance
var config = new DumpConfig { MaxDepth = 3 };
data.Dump(config);
```

## Color Configuration

`ColorConfig` controls colors used in output.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `LabelColor` | `DumpColor?` | Color for labels |
| `TypeNameColor` | `DumpColor?` | Color for type names |
| `PropertyNameColor` | `DumpColor?` | Color for property names |
| `PropertyValueColor` | `DumpColor?` | Color for property values |
| `NullValueColor` | `DumpColor?` | Color for null values |
| `MetadataInfoColor` | `DumpColor?` | Color for metadata information |
| `MetadataErrorColor` | `DumpColor?` | Color for error metadata |
| `ColumnNameColor` | `DumpColor?` | Color for column names |

### Usage

```csharp
// Using System.Drawing.Color
config.UseColorConfig(c => c
    .SetLabelColor(Color.Yellow)
    .SetTypeNameColor(Color.Cyan)
    .SetPropertyValueColor(Color.White));

// Using hex strings
config.UseColorConfig(c => c
    .SetLabelColor("#FFD700")
    .SetPropertyValueColor("#FFFFFF"));
```

### DumpColor Class

`DumpColor` supports multiple construction methods:

```csharp
// From System.Drawing.Color
var color1 = new DumpColor(Color.Red);

// From hex string (implicit conversion)
DumpColor color2 = "#FF5733";

// From RGB values (via System.Drawing.Color)
var color3 = new DumpColor(Color.FromArgb(255, 87, 51));
```

## Table Configuration

`TableConfig` controls table rendering.

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowHeaders` | `bool` | `true` | Show table headers |
| `ShowMemberTypes` | `bool` | `false` | Show type of each member |
| `ExpandTables` | `bool` | `false` | Expand tables to full width |
| `ColumnSeparator` | `string?` | `null` | Custom column separator |
| `RowSeparator` | `string?` | `null` | Custom row separator |

### Usage

```csharp
data.Dump(config => config.UseTableConfig(t => t
    .ShowTableHeaders(false)
    .SetExpandTables(true)
    .SetColumnSeparator(" | ")));
```

## Members Configuration

`MembersConfig` controls which members to include.

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IncludeFields` | `bool` | `true` | Include fields |
| `IncludePublicMembers` | `bool` | `true` | Include public members |
| `IncludeNonPublicMembers` | `bool` | `false` | Include non-public members |
| `MemberFilter` | `Func<MemberInfo, bool>?` | `null` | Custom filter function |

### Usage

```csharp
data.Dump(config => config.UseMembersConfig(m => m
    .IncludeFields(false)
    .IncludeNonPublicMembers(true)
    .SetMemberFilter(member => member.Name != "Password")));
```

### Filtering Examples

```csharp
// Exclude specific properties
config.UseMembersConfig(m => m.SetMemberFilter(
    member => !new[] { "Password", "Secret" }.Contains(member.Name)));

// Only include certain properties
config.UseMembersConfig(m => m.SetMemberFilter(
    member => new[] { "Id", "Name", "Email" }.Contains(member.Name)));

// Filter by attribute
config.UseMembersConfig(m => m.SetMemberFilter(
    member => !member.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any()));
```

## Type Naming Configuration

`TypeNamingConfig` controls how type names are displayed.

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowTypeNames` | `bool` | `true` | Display type names |
| `UseAliases` | `bool` | `true` | Use C# aliases (int vs Int32) |
| `UseFullTypeNames` | `bool` | `false` | Use full namespace-qualified names |

### Usage

```csharp
data.Dump(config => config.UseTypeNamingConfig(t => t
    .SetShowTypeNames(true)
    .SetUseAliases(true)
    .SetUseFullTypeNames(false)));
```

## Type Rendering Configuration

`TypeRenderingConfig` controls how specific types are rendered.

### Custom Type Handlers

Register custom handlers for specific types:

```csharp
// Global registration
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(SecureString)] = 
    (obj, type, config) => "****";

// Custom DateTime format
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(DateTime)] = 
    (obj, type, config) => ((DateTime)obj).ToString("yyyy-MM-dd HH:mm");

// Domain-specific formatting
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(Money)] = 
    (obj, type, config) => 
    {
        var m = (Money)obj;
        return $"{m.Currency} {m.Amount:N2}";
    };
```

### Handler Signature

```csharp
Func<object?, Type, DumpConfig, string?>
```

Parameters:
- `object?` - The value being rendered
- `Type` - The type of the value
- `DumpConfig` - Current configuration
- Returns: `string?` - The string representation, or null to use default

## Output Configuration

`OutputConfig` controls output behavior (available through `DumpConfig`).

### DumpConfig Properties

| Property | Type | Description |
|----------|------|-------------|
| `Label` | `string?` | Label for the dump output |
| `MaxDepth` | `int` | Maximum depth for nested objects |

### Usage

```csharp
data.Dump(config => config
    .SetLabel("My Label")
    .SetMaxDepth(5));
```

## Complete Configuration Example

```csharp
var fullConfig = new DumpConfig
{
    Label = "Debug Output",
    MaxDepth = 4,
    
    ColorConfig = new ColorConfig
    {
        LabelColor = new DumpColor(Color.Gold),
        TypeNameColor = new DumpColor("#00CED1"),
        PropertyNameColor = new DumpColor(Color.LightGreen),
        PropertyValueColor = new DumpColor("#FFFFFF"),
        NullValueColor = new DumpColor(Color.Gray)
    },
    
    TableConfig = new TableConfig
    {
        ShowHeaders = true,
        ExpandTables = false,
        ShowMemberTypes = false
    },
    
    MembersConfig = new MembersConfig
    {
        IncludeFields = false,
        IncludePublicMembers = true,
        IncludeNonPublicMembers = false
    },
    
    TypeNamingConfig = new TypeNamingConfig
    {
        ShowTypeNames = true,
        UseAliases = true,
        UseFullTypeNames = false
    }
};

data.Dump(fullConfig);
```

## See Also

- [API Reference](api-reference.md) - Method signatures
- [Features](features.md) - Feature details
- [Examples](examples.md) - Usage examples
