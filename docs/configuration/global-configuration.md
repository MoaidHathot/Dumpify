---
layout: default
title: Global Configuration
parent: Configuration
nav_order: 1
---

# Global Configuration

The `DumpConfig.Default` static property provides access to global configuration settings that apply to all dumps unless overridden.

---

## Accessing Global Configuration

```csharp
using Dumpify;

// Access the global configuration
var config = DumpConfig.Default;
```

---

## Properties

### MaxDepth

Controls the maximum nesting depth for object rendering.

| Property | Type | Default |
|----------|------|---------|
| `MaxDepth` | `int` | `7` |

```csharp
// Limit nesting to 3 levels
DumpConfig.Default.MaxDepth = 3;
```

### UseDescriptors

Enables or disables the descriptor-based object analysis system.

| Property | Type | Default |
|----------|------|---------|
| `UseDescriptors` | `bool` | `true` |

```csharp
DumpConfig.Default.UseDescriptors = true;
```

### ShowHeaders

Controls whether table headers are displayed.

| Property | Type | Default |
|----------|------|---------|
| `ShowHeaders` | `bool` | `true` |

```csharp
DumpConfig.Default.ShowHeaders = false;
```

### UseAutoLabels

When enabled, automatically uses the variable name as a label if no custom label is provided.

| Property | Type | Default |
|----------|------|---------|
| `UseAutoLabels` | `bool` | `false` |

```csharp
DumpConfig.Default.UseAutoLabels = true;

var myPerson = new Person { Name = "John" };
myPerson.Dump(); // Label will be "myPerson"
```

---

## Nested Configuration Objects

`DumpConfig.Default` contains several nested configuration objects:

| Property | Type | Description |
|----------|------|-------------|
| `ColorConfig` | [ColorConfig](color-config.md) | Color settings |
| `TableConfig` | [TableConfig](table-config.md) | Table display settings |
| `MembersConfig` | [MembersConfig](members-config.md) | Member filtering settings |
| `TypeNamingConfig` | [TypeNamingConfig](type-naming-config.md) | Type name settings |
| `TypeRenderingConfig` | [TypeRenderingConfig](type-rendering-config.md) | Value rendering settings |
| `OutputConfig` | [OutputConfig](output-config.md) | Output dimension settings |

### Accessing Nested Configuration

```csharp
// Color configuration
DumpConfig.Default.ColorConfig.TypeNameColor = new DumpColor("#FFD700");

// Table configuration
DumpConfig.Default.TableConfig.ShowRowSeparators = true;

// Member configuration
DumpConfig.Default.MembersConfig.IncludeFields = true;

// Type naming configuration
DumpConfig.Default.TypeNamingConfig.UseAliases = true;

// Type rendering configuration
DumpConfig.Default.TypeRenderingConfig.QuoteStringValues = true;

// Output configuration
DumpConfig.Default.OutputConfig.WidthOverride = 120;
```

---

## Advanced Properties

### Generator

The descriptor generator used for analyzing object structure.

| Property | Type | Default |
|----------|------|---------|
| `Generator` | `IDescriptorGenerator` | `CompositeDescriptorGenerator` |

### Renderer

The renderer used for producing output.

| Property | Type | Default |
|----------|------|---------|
| `Renderer` | `IRenderer` | Spectre.Console renderer |

### Output

The output target for dumps.

| Property | Type | Default |
|----------|------|---------|
| `Output` | `IDumpOutput` | `Outputs.Console` |

```csharp
// Change default output to Debug
DumpConfig.Default.Output = Outputs.Debug;
```

---

## Custom Type Handlers

You can register custom handlers for specific types:

### AddCustomTypeHandler

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(byte[]), 
    (obj, type, valueProvider, memberProvider) => 
    {
        var bytes = (byte[])obj;
        return $"byte[{bytes.Length}]: {BitConverter.ToString(bytes.Take(10).ToArray())}...";
    }
);
```

### RemoveCustomTypeHandler

```csharp
DumpConfig.Default.RemoveCustomTypeHandler(typeof(byte[]));
```

---

## Example: Complete Configuration

```csharp
// Configure Dumpify globally
DumpConfig.Default.MaxDepth = 5;
DumpConfig.Default.UseAutoLabels = true;

// Colors
DumpConfig.Default.ColorConfig.TypeNameColor = new DumpColor("#FFD700");
DumpConfig.Default.ColorConfig.PropertyNameColor = new DumpColor("#87CEEB");

// Table
DumpConfig.Default.TableConfig.ShowRowSeparators = true;
DumpConfig.Default.TableConfig.ShowMemberTypes = true;

// Members
DumpConfig.Default.MembersConfig.IncludeFields = true;

// Type naming
DumpConfig.Default.TypeNamingConfig.UseAliases = true;
DumpConfig.Default.TypeNamingConfig.ShowTypeNames = true;

// Now all dumps use these settings
myObject.Dump();
```

---

## See Also

- [ColorConfig](color-config.md)
- [TableConfig](table-config.md)
- [MembersConfig](members-config.md)
- [TypeNamingConfig](type-naming-config.md)
- [TypeRenderingConfig](type-rendering-config.md)
- [OutputConfig](output-config.md)
