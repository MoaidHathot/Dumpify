---
layout: default
title: DumpOutput
parent: API Reference
nav_order: 9
---

# DumpOutput Class

Default implementation of `IDumpOutput` that wraps a `TextWriter`.

## Definition

```csharp
namespace Dumpify;

public class DumpOutput : IDumpOutput
{
    public TextWriter TextWriter { get; }
    
    public DumpOutput(TextWriter writer, Func<RendererConfig, RendererConfig>? configFactory = null);
    
    public RendererConfig AdjustConfig(in RendererConfig config);
}
```

---

## Constructors

### DumpOutput(TextWriter, Func&lt;RendererConfig, RendererConfig&gt;?)

Creates a new `DumpOutput` instance.

| Parameter | Type | Description |
|-----------|------|-------------|
| `writer` | `TextWriter` | The `TextWriter` to output to. |
| `configFactory` | `Func<RendererConfig, RendererConfig>?` | Optional function to modify the renderer configuration. |

---

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `TextWriter` | `TextWriter` | The underlying `TextWriter` that output will be written to. |

---

## Methods

### AdjustConfig

```csharp
public RendererConfig AdjustConfig(in RendererConfig config)
```

Applies the `configFactory` (if provided) to modify the renderer configuration.

---

## Usage Examples

### Simple Output to TextWriter

```csharp
// Output to a StringWriter
var stringWriter = new StringWriter();
var output = new DumpOutput(stringWriter);

myObject.Dump(output: output);
string result = stringWriter.ToString();
```

### Output to File

```csharp
using var fileWriter = new StreamWriter("dump.txt");
var output = new DumpOutput(fileWriter);

myObject.Dump(output: output);
```

### With Config Adjustment

```csharp
// Create output that disables colors
var output = new DumpOutput(
    writer: Console.Out,
    configFactory: config => config with
    {
        ColorConfig = ColorConfig.NoColors
    }
);

myObject.Dump(output: output);
```

### Custom Log Output

```csharp
// Output that forces ASCII borders for log compatibility
var logOutput = new DumpOutput(
    writer: logWriter,
    configFactory: config => config with
    {
        ColorConfig = ColorConfig.NoColors,
        TableConfig = config.TableConfig with
        {
            BorderStyle = TableBorderStyle.Ascii
        }
    }
);

DumpConfig.Default.Output = logOutput;
```

---

## Built-in Outputs

Instead of creating `DumpOutput` instances directly, you can use the pre-configured outputs:

```csharp
// These are pre-configured DumpOutput instances
Dumpify.Outputs.Console  // Console.Out
Dumpify.Outputs.Debug    // Debug output
Dumpify.Outputs.Trace    // Trace output
```

---

## When to Use DumpOutput vs IDumpOutput

| Scenario | Recommendation |
|----------|----------------|
| Simple output to any `TextWriter` | Use `DumpOutput` |
| Need config adjustments | Use `DumpOutput` with `configFactory` |
| Complex custom behavior | Implement `IDumpOutput` directly |
| Built-in targets (Console, Debug, Trace) | Use `Dumpify.Outputs.*` |

---

## See Also

- [IDumpOutput](./idump-output.md) - Interface definition
- [DumpConfig Reference](./dump-config.md) - Global configuration
- [Output Configuration](../configuration/output-config.md)
