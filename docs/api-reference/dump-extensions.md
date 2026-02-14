---
layout: default
title: Extension Methods
parent: API Reference
nav_order: 1
---

# Extension Methods Reference

Dumpify provides extension methods that can be called on any object to display its contents in a formatted, readable way.

## Table of Contents

- [Dump()](#dump)
- [DumpConsole()](#dumpconsole)
- [DumpDebug()](#dumpdebug)
- [DumpTrace()](#dumptrace)
- [DumpText()](#dumptext)
- [Common Parameters](#common-parameters)
- [Parameter Details](#parameter-details)
- [Return Values](#return-values)

---

## Dump()

The primary extension method. Outputs to the configured default output (Console by default).

```csharp
public static T? Dump<T>(
    this T? obj,
    string? label = null,
    int? maxDepth = null,
    IRenderer? renderer = null,
    bool? useDescriptors = null,
    ColorConfig? colors = null,
    IDumpOutput? output = null,
    MembersConfig? members = null,
    TypeNamingConfig? typeNames = null,
    TableConfig? tableConfig = null,
    OutputConfig? outputConfig = null,
    TypeRenderingConfig? typeRenderingConfig = null
)
```

### Basic Usage

```csharp
// Simple dump
myObject.Dump();

// With label
myObject.Dump("My Object");

// With options
myObject.Dump(label: "Data", maxDepth: 3);
```

### Unique Parameter

| Parameter | Type | Description |
|-----------|------|-------------|
| `output` | `IDumpOutput?` | Custom output target. Defaults to `DumpConfig.Default.Output` |

---

## DumpConsole()

Always outputs to the console, regardless of global configuration.

```csharp
public static T? DumpConsole<T>(
    this T? obj,
    string? label = null,
    int? maxDepth = null,
    IRenderer? renderer = null,
    bool? useDescriptors = null,
    ColorConfig? colors = null,
    MembersConfig? members = null,
    TypeNamingConfig? typeNames = null,
    TableConfig? tableConfig = null,
    OutputConfig? outputConfig = null,
    TypeRenderingConfig? typeRenderingConfig = null
)
```

### Usage

```csharp
// Always goes to Console.WriteLine
myObject.DumpConsole();

// Even if global output is set to Debug
DumpConfig.Default.Output = Outputs.Debug;
myObject.DumpConsole();  // Still outputs to console
```

---

## DumpDebug()

Outputs to `System.Diagnostics.Debug`. Useful for debugging in IDEs that capture debug output.

```csharp
public static T? DumpDebug<T>(
    this T? obj,
    string? label = null,
    int? maxDepth = null,
    IRenderer? renderer = null,
    bool? useDescriptors = null,
    ColorConfig? colors = null,
    MembersConfig? members = null,
    TypeNamingConfig? typeNames = null,
    TableConfig? tableConfig = null,
    OutputConfig? outputConfig = null,
    TypeRenderingConfig? typeRenderingConfig = null
)
```

### Usage

```csharp
// Output appears in Debug window (Visual Studio, Rider, etc.)
myObject.DumpDebug();
```

### Default Behavior

- `OutputConfig.WidthOverride` defaults to `250` (wider than console default)

---

## DumpTrace()

Outputs to `System.Diagnostics.Trace`. Useful for trace listeners and logging frameworks.

```csharp
public static T? DumpTrace<T>(
    this T? obj,
    string? label = null,
    int? maxDepth = null,
    IRenderer? renderer = null,
    bool? useDescriptors = null,
    ColorConfig? colors = null,
    MembersConfig? members = null,
    TypeNamingConfig? typeNames = null,
    TableConfig? tableConfig = null,
    OutputConfig? outputConfig = null,
    TypeRenderingConfig? typeRenderingConfig = null
)
```

### Usage

```csharp
// Output goes to configured trace listeners
myObject.DumpTrace();
```

### Default Behavior

- `OutputConfig.WidthOverride` defaults to `250` (wider than console default)

---

## DumpText()

Returns the formatted output as a string instead of writing to an output target.

```csharp
public static string DumpText<T>(
    this T? obj,
    string? label = null,
    int? maxDepth = null,
    IRenderer? renderer = null,
    bool? useDescriptors = null,
    ColorConfig? colors = null,
    MembersConfig? members = null,
    TypeNamingConfig? typeNames = null,
    TableConfig? tableConfig = null,
    OutputConfig? outputConfig = null,
    TypeRenderingConfig? typeRenderingConfig = null
)
```

### Usage

```csharp
// Get formatted string
string output = myObject.DumpText();

// Use in logging
logger.Info(myObject.DumpText());

// Write to file
File.WriteAllText("dump.txt", myObject.DumpText());
```

### Default Behavior

- `ColorConfig` defaults to `ColorConfig.NoColors` (no ANSI color codes)
- `OutputConfig.WidthOverride` defaults to `1000`
- `OutputConfig.HeightOverride` defaults to `1000`

### Important

Unlike other Dump methods, `DumpText()` returns `string` (not the original object), so it cannot be chained in the same way.

---

## Common Parameters

All extension methods share these parameters:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `label` | `string?` | `null` | Display label above the output |
| `maxDepth` | `int?` | `7` | Maximum nesting depth to render |
| `renderer` | `IRenderer?` | Table renderer | Rendering engine to use |
| `useDescriptors` | `bool?` | `true` | Whether to use reflection descriptors |
| `colors` | `ColorConfig?` | Default colors | Color scheme configuration |
| `members` | `MembersConfig?` | Default members | Member filtering configuration |
| `typeNames` | `TypeNamingConfig?` | Default naming | Type name display configuration |
| `tableConfig` | `TableConfig?` | Default table | Table rendering configuration |
| `outputConfig` | `OutputConfig?` | Default output | Output dimensions configuration |
| `typeRenderingConfig` | `TypeRenderingConfig?` | Default type rendering | Custom type handlers |

---

## Parameter Details

### label

Displays a header label above the output.

```csharp
myObject.Dump("Customer Data");
```

Output:
```
┌───────────────┐
│ Customer Data │
├───────┬───────┤
│ Name  │ John  │
│ Age   │ 30    │
└───────┴───────┘
```

### maxDepth

Controls how deep nested objects are rendered.

```csharp
// Only render 2 levels deep
complexObject.Dump(maxDepth: 2);
```

Values less than 1 are ignored.

### useDescriptors

When `false`, renders the object's `ToString()` value instead of using reflection.

```csharp
myObject.Dump(useDescriptors: false);  // Uses ToString()
```

### colors

Override color settings for this call only.

```csharp
var customColors = new ColorConfig();
customColors.NullValueColor = "#FF0000";
myObject.Dump(colors: customColors);
```

### members

Filter which members are displayed.

```csharp
var membersConfig = new MembersConfig 
{ 
    IncludeFields = true,
    IncludeNonPublicMembers = true 
};
myObject.Dump(members: membersConfig);
```

### typeNames

Control how type names are displayed.

```csharp
var typeConfig = new TypeNamingConfig 
{ 
    UseFullName = true 
};
myObject.Dump(typeNames: typeConfig);
```

### tableConfig

Configure table appearance.

```csharp
var tableConfig = new TableConfig 
{ 
    ShowTableHeaders = false 
};
myObject.Dump(tableConfig: tableConfig);
```

### outputConfig

Configure output dimensions.

```csharp
var outputConfig = new OutputConfig 
{ 
    WidthOverride = 200 
};
myObject.Dump(outputConfig: outputConfig);
```

---

## Return Values

### Dump(), DumpConsole(), DumpDebug(), DumpTrace()

All return the original object (`T?`), enabling method chaining:

```csharp
var result = GetUsers()
    .Dump("Raw users")
    .Where(u => u.IsActive)
    .Dump("Active users")
    .ToList();
```

These methods use `[return: NotNullIfNotNull(nameof(obj))]` to preserve nullability information.

### DumpText()

Returns `string` containing the formatted output. Cannot be chained in the same way as other methods.

---

## Auto Labels

When `DumpConfig.Default.UseAutoLabels` is `true` and no explicit label is provided, Dumpify automatically uses the expression being dumped as the label:

```csharp
DumpConfig.Default.UseAutoLabels = true;

myObject.Dump();  // Label will be "myObject"
GetUser().Dump(); // Label will be "GetUser()"
```

This feature uses `[CallerArgumentExpression]` to capture the calling expression.

---

## See Also

- [DumpConfig Reference](./dump-config.md)
- [Configuration Overview](../configuration/index.md)
- [Labels Feature](../features/labels.md)
- [Output Targets](../features/output-targets.md)
