# DumpConfig Reference

`DumpConfig` is the global singleton class that controls default behavior for all Dumpify operations.

## Table of Contents

- [Overview](#overview)
- [Accessing DumpConfig](#accessing-dumpconfig)
- [Properties](#properties)
- [Methods](#methods)
- [Configuration Objects](#configuration-objects)
- [Examples](#examples)

---

## Overview

```csharp
namespace Dumpify;

public class DumpConfig
{
    public static DumpConfig Default { get; }
    
    // Core settings
    public int MaxDepth { get; set; }
    public bool UseDescriptors { get; set; }
    public bool ShowHeaders { get; set; }
    public bool UseAutoLabels { get; set; }
    
    // Component settings
    public IDescriptorGenerator Generator { get; set; }
    public IRenderer Renderer { get; set; }
    public IDumpOutput Output { get; set; }
    
    // Configuration objects
    public ColorConfig ColorConfig { get; }
    public TableConfig TableConfig { get; }
    public MembersConfig MembersConfig { get; }
    public TypeNamingConfig TypeNamingConfig { get; }
    public OutputConfig OutputConfig { get; }
    public TypeRenderingConfig TypeRenderingConfig { get; }
    
    // Methods
    public void AddCustomTypeHandler(Type type, Func<...> valueFactory);
    public void RemoveCustomTypeHandler(Type type);
}
```

---

## Accessing DumpConfig

Access the global configuration through the static `Default` property:

```csharp
// Access global config
var config = DumpConfig.Default;

// Modify settings
DumpConfig.Default.MaxDepth = 5;
DumpConfig.Default.UseAutoLabels = true;
```

---

## Properties

### MaxDepth

```csharp
public int MaxDepth { get; set; }
```

Maximum nesting depth for object rendering.

| | |
|---|---|
| **Type** | `int` |
| **Default** | `7` |
| **Minimum** | `1` (values < 1 are ignored) |

```csharp
// Limit to 3 levels deep
DumpConfig.Default.MaxDepth = 3;
```

### UseDescriptors

```csharp
public bool UseDescriptors { get; set; }
```

Whether to use reflection to inspect object structure.

| | |
|---|---|
| **Type** | `bool` |
| **Default** | `true` |

```csharp
// Disable reflection, use ToString() instead
DumpConfig.Default.UseDescriptors = false;
```

### ShowHeaders

```csharp
public bool ShowHeaders { get; set; }
```

Whether to display table headers.

| | |
|---|---|
| **Type** | `bool` |
| **Default** | `true` |

```csharp
// Hide table headers
DumpConfig.Default.ShowHeaders = false;
```

### UseAutoLabels

```csharp
public bool UseAutoLabels { get; set; }
```

Whether to automatically use the calling expression as a label when no explicit label is provided.

| | |
|---|---|
| **Type** | `bool` |
| **Default** | `false` |

```csharp
DumpConfig.Default.UseAutoLabels = true;

// Now this will show "myVariable" as the label
myVariable.Dump();
```

### Generator

```csharp
public IDescriptorGenerator Generator { get; set; }
```

The descriptor generator used for object inspection.

| | |
|---|---|
| **Type** | `IDescriptorGenerator` |
| **Default** | `CompositeDescriptorGenerator` |

### Renderer

```csharp
public IRenderer Renderer { get; set; }
```

The renderer used for output formatting.

| | |
|---|---|
| **Type** | `IRenderer` |
| **Default** | `Dumpify.Renderers.Table` |

```csharp
// Use custom renderer
DumpConfig.Default.Renderer = myCustomRenderer;
```

### Output

```csharp
public IDumpOutput Output { get; set; }
```

The default output target for `Dump()` calls.

| | |
|---|---|
| **Type** | `IDumpOutput` |
| **Default** | `Dumpify.Outputs.Console` |

```csharp
// Change default output to Debug
DumpConfig.Default.Output = Dumpify.Outputs.Debug;

// Now Dump() goes to Debug by default
myObject.Dump();  // Goes to Debug
```

---

## Methods

### AddCustomTypeHandler

```csharp
public void AddCustomTypeHandler(
    Type type, 
    Func<object, Type, IValueProvider?, IMemberProvider, object?> valueFactory
)
```

Registers a custom handler for rendering specific types.

```csharp
// Custom handler for DateTime
DumpConfig.Default.AddCustomTypeHandler(
    typeof(DateTime),
    (obj, type, valueProvider, memberProvider) => 
    {
        var dt = (DateTime)obj;
        return dt.ToString("yyyy-MM-dd HH:mm:ss");
    }
);
```

See [Custom Type Handlers](../features/custom-type-handlers.md) for detailed documentation.

### RemoveCustomTypeHandler

```csharp
public void RemoveCustomTypeHandler(Type type)
```

Removes a previously registered custom type handler.

```csharp
// Remove DateTime handler
DumpConfig.Default.RemoveCustomTypeHandler(typeof(DateTime));
```

---

## Configuration Objects

DumpConfig provides access to several configuration objects. These are read-only properties that return mutable objects:

### ColorConfig

```csharp
public ColorConfig ColorConfig { get; }
```

Controls colors for different value types. See [Color Configuration](../configuration/color-config.md).

```csharp
DumpConfig.Default.ColorConfig.NullValueColor = "#FF0000";
```

### TableConfig

```csharp
public TableConfig TableConfig { get; }
```

Controls table appearance. See [Table Configuration](../configuration/table-config.md).

```csharp
DumpConfig.Default.TableConfig.ShowTableHeaders = false;
```

### MembersConfig

```csharp
public MembersConfig MembersConfig { get; }
```

Controls which members are included. See [Members Configuration](../configuration/members-config.md).

```csharp
DumpConfig.Default.MembersConfig.IncludeFields = true;
```

### TypeNamingConfig

```csharp
public TypeNamingConfig TypeNamingConfig { get; }
```

Controls type name display. See [Type Naming Configuration](../configuration/type-naming-config.md).

```csharp
DumpConfig.Default.TypeNamingConfig.UseFullName = true;
```

### OutputConfig

```csharp
public OutputConfig OutputConfig { get; }
```

Controls output dimensions. See [Output Configuration](../configuration/output-config.md).

```csharp
DumpConfig.Default.OutputConfig.WidthOverride = 200;
```

### TypeRenderingConfig

```csharp
public TypeRenderingConfig TypeRenderingConfig { get; }
```

Controls custom type rendering. See [Type Rendering Configuration](../configuration/type-rendering-config.md).

---

## Examples

### Basic Configuration

```csharp
// Configure at application startup
DumpConfig.Default.MaxDepth = 5;
DumpConfig.Default.UseAutoLabels = true;
DumpConfig.Default.ColorConfig.NullValueColor = "#FF6B6B";
DumpConfig.Default.TableConfig.ShowTableHeaders = false;
```

### Change Default Output

```csharp
// All Dump() calls go to Debug by default
DumpConfig.Default.Output = Dumpify.Outputs.Debug;

myObject.Dump();        // Goes to Debug
myObject.DumpConsole(); // Explicitly goes to Console
```

### Show Private Members

```csharp
DumpConfig.Default.MembersConfig.IncludeNonPublicMembers = true;
DumpConfig.Default.MembersConfig.IncludeFields = true;
```

### Custom Type Handler

```csharp
// Show only date part of DateTime
DumpConfig.Default.AddCustomTypeHandler(
    typeof(DateTime),
    (obj, type, vp, mp) => ((DateTime)obj).ToShortDateString()
);
```

### Thread Safety

`DumpConfig.Default` is a singleton and its configuration objects are shared across threads. Be aware of race conditions when modifying configuration from multiple threads.

```csharp
// Safe: configure at startup before other threads access
DumpConfig.Default.MaxDepth = 5;

// Potentially unsafe: modifying during concurrent Dump() calls
Task.Run(() => DumpConfig.Default.MaxDepth = 10);  // Race condition
```

---

## See Also

- [Global Configuration Guide](../configuration/global-configuration.md)
- [Configuration Overview](../configuration/index.md)
- [Extension Methods](./dump-extensions.md)
- [Custom Type Handlers](../features/custom-type-handlers.md)
