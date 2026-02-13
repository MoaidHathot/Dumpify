# API Reference

Complete reference for Dumpify's public API.

## Table of Contents

- [Extension Methods](#extension-methods)
- [DumpConfig Class](#dumpconfig-class)
- [DumpColor Class](#dumpcolor-class)
- [Configuration Classes](#configuration-classes)

## Extension Methods

All extension methods are available on any object after adding `using Dumpify;`.

### Dump

Dumps an object to the console with optional configuration.

```csharp
// Basic overloads
public static T Dump<T>(this T obj);
public static T Dump<T>(this T obj, string? label);
public static T Dump<T>(this T obj, DumpConfig? config);
public static T Dump<T>(this T obj, Action<DumpConfig> configBuilder);
public static T Dump<T>(this T obj, string? label, Action<DumpConfig> configBuilder);
```

**Returns:** The original object (enables chaining).

**Examples:**

```csharp
// Simple dump
person.Dump();

// With label
person.Dump("Current User");

// With configuration
person.Dump(config => config.SetMaxDepth(2));

// With label and configuration
person.Dump("User", config => config.SetMaxDepth(2));

// With DumpConfig instance
var cfg = new DumpConfig { MaxDepth = 2 };
person.Dump(cfg);
```

### DumpConsole

Explicitly dumps to console output.

```csharp
public static T DumpConsole<T>(this T obj);
public static T DumpConsole<T>(this T obj, string? label);
public static T DumpConsole<T>(this T obj, DumpConfig? config);
public static T DumpConsole<T>(this T obj, Action<DumpConfig> configBuilder);
```

**Example:**

```csharp
data.DumpConsole("Console Output");
```

### DumpDebug

Dumps to Debug output (Visual Studio Output window).

```csharp
public static T DumpDebug<T>(this T obj);
public static T DumpDebug<T>(this T obj, string? label);
public static T DumpDebug<T>(this T obj, DumpConfig? config);
public static T DumpDebug<T>(this T obj, Action<DumpConfig> configBuilder);
```

**Example:**

```csharp
data.DumpDebug("Debug Output");
```

### DumpTrace

Dumps to Trace output.

```csharp
public static T DumpTrace<T>(this T obj);
public static T DumpTrace<T>(this T obj, string? label);
public static T DumpTrace<T>(this T obj, DumpConfig? config);
public static T DumpTrace<T>(this T obj, Action<DumpConfig> configBuilder);
```

**Example:**

```csharp
data.DumpTrace("Trace Output");
```

### DumpText

Returns the dump as a string without outputting.

```csharp
public static string DumpText<T>(this T obj);
public static string DumpText<T>(this T obj, string? label);
public static string DumpText<T>(this T obj, DumpConfig? config);
public static string DumpText<T>(this T obj, Action<DumpConfig> configBuilder);
```

**Returns:** String representation of the dump.

**Example:**

```csharp
var text = data.DumpText();
logger.LogInformation("Data: {Data}", text);
```

## DumpConfig Class

Central configuration class for customizing dump behavior.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Label` | `string?` | Label displayed above the output |
| `MaxDepth` | `int` | Maximum nesting depth (default: varies) |
| `ColorConfig` | `ColorConfig` | Color settings |
| `TableConfig` | `TableConfig` | Table rendering settings |
| `MembersConfig` | `MembersConfig` | Member inclusion settings |
| `TypeNamingConfig` | `TypeNamingConfig` | Type name display settings |
| `TypeRenderingConfig` | `TypeRenderingConfig` | Custom type rendering |
| `OutputConfig` | `OutputConfig` | Output behavior settings |

### Static Properties

| Property | Type | Description |
|----------|------|-------------|
| `Default` | `DumpConfig` | Global default configuration |

### Fluent Methods

```csharp
public DumpConfig SetLabel(string? label);
public DumpConfig SetMaxDepth(int depth);
public DumpConfig UseColorConfig(Action<ColorConfig> configure);
public DumpConfig UseTableConfig(Action<TableConfig> configure);
public DumpConfig UseMembersConfig(Action<MembersConfig> configure);
public DumpConfig UseTypeNamingConfig(Action<TypeNamingConfig> configure);
public DumpConfig UseOutputConfig(Action<OutputConfig> configure);
public DumpConfig SetColorConfig(ColorConfig config);
public DumpConfig SetTableConfig(TableConfig config);
public DumpConfig SetMembersConfig(MembersConfig config);
public DumpConfig SetTypeNamingConfig(TypeNamingConfig config);
```

### Example

```csharp
var config = new DumpConfig()
    .SetLabel("My Data")
    .SetMaxDepth(3)
    .UseColorConfig(c => c.SetLabelColor(Color.Yellow))
    .UseTableConfig(t => t.ShowTableHeaders(false));
```

## DumpColor Class

Represents a color for dump output.

### Constructors

```csharp
public DumpColor(Color color);  // System.Drawing.Color
```

### Implicit Conversions

```csharp
// From string (hex color)
public static implicit operator DumpColor?(string? hex);

// From System.Drawing.Color
public static implicit operator DumpColor?(Color? color);
```

### Examples

```csharp
// From System.Drawing.Color
var color1 = new DumpColor(Color.Red);

// From hex string (implicit)
DumpColor color2 = "#FF5733";

// Assignment
config.ColorConfig.LabelColor = "#FFD700";
config.ColorConfig.PropertyValueColor = new DumpColor(Color.White);
```

## Configuration Classes

### ColorConfig

| Property | Type | Description |
|----------|------|-------------|
| `LabelColor` | `DumpColor?` | Color for labels |
| `TypeNameColor` | `DumpColor?` | Color for type names |
| `PropertyNameColor` | `DumpColor?` | Color for property names |
| `PropertyValueColor` | `DumpColor?` | Color for property values |
| `NullValueColor` | `DumpColor?` | Color for null values |
| `MetadataInfoColor` | `DumpColor?` | Color for metadata |
| `MetadataErrorColor` | `DumpColor?` | Color for errors |
| `ColumnNameColor` | `DumpColor?` | Color for column headers |

**Fluent Methods:**
- `SetLabelColor(Color/string)`
- `SetTypeNameColor(Color/string)`
- `SetPropertyNameColor(Color/string)`
- `SetPropertyValueColor(Color/string)`
- `SetNullValueColor(Color/string)`
- `SetMetadataInfoColor(Color/string)`
- `SetMetadataErrorColor(Color/string)`
- `SetColumnNameColor(Color/string)`

### TableConfig

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowHeaders` | `bool` | `true` | Show table headers |
| `ShowMemberTypes` | `bool` | `false` | Show member types |
| `ExpandTables` | `bool` | `false` | Expand to full width |
| `ColumnSeparator` | `string?` | `null` | Column separator |
| `RowSeparator` | `string?` | `null` | Row separator |

**Fluent Methods:**
- `ShowTableHeaders(bool)`
- `ShowMemberTypes(bool)` 
- `SetExpandTables(bool)`
- `SetColumnSeparator(string)`
- `SetRowSeparator(string)`

### MembersConfig

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IncludeFields` | `bool` | `true` | Include fields |
| `IncludePublicMembers` | `bool` | `true` | Include public members |
| `IncludeNonPublicMembers` | `bool` | `false` | Include private members |
| `MemberFilter` | `Func<MemberInfo, bool>?` | `null` | Custom filter |

**Fluent Methods:**
- `IncludeFields(bool)`
- `IncludePublicMembers(bool)`
- `IncludeNonPublicMembers(bool)`
- `SetMemberFilter(Func<MemberInfo, bool>)`

### TypeNamingConfig

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowTypeNames` | `bool` | `true` | Show type names |
| `UseAliases` | `bool` | `true` | Use C# aliases |
| `UseFullTypeNames` | `bool` | `false` | Use full names |

**Fluent Methods:**
- `SetShowTypeNames(bool)`
- `SetUseAliases(bool)`
- `SetUseFullTypeNames(bool)`

### TypeRenderingConfig

| Property | Type | Description |
|----------|------|-------------|
| `CustomTypeHandlers` | `Dictionary<Type, Func<object?, Type, DumpConfig, string?>>` | Custom handlers |

**Usage:**

```csharp
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(SecureString)] = 
    (obj, type, config) => "****";
```

## See Also

- [Configuration](configuration.md) - Configuration guide
- [Features](features.md) - Feature details
- [Examples](examples.md) - Usage examples
