---
layout: default
title: IDumpOutput
parent: API Reference
nav_order: 7
---

# IDumpOutput Interface

Defines an output target for Dumpify's rendered objects.

## Definition

```csharp
namespace Dumpify;

public interface IDumpOutput
{
    TextWriter TextWriter { get; }
    
    RendererConfig AdjustConfig(in RendererConfig config);
}
```

---

## Members

| Member | Type | Description |
|--------|------|-------------|
| `TextWriter` | `TextWriter` | The underlying `TextWriter` that output will be written to. |
| `AdjustConfig` | Method | Allows the output to modify the renderer configuration before rendering. |

---

## Built-in Implementations

Dumpify provides several pre-configured outputs via the `Outputs` static class:

```csharp
// Console output (default)
Dumpify.Outputs.Console

// Debug output (System.Diagnostics.Debug)
Dumpify.Outputs.Debug

// Trace output (System.Diagnostics.Trace)
Dumpify.Outputs.Trace
```

---

## Implementing Custom Outputs

You can create custom output targets by implementing `IDumpOutput`:

```csharp
public class FileOutput : IDumpOutput
{
    private readonly StreamWriter _writer;
    
    public TextWriter TextWriter => _writer;
    
    public FileOutput(string filePath)
    {
        _writer = new StreamWriter(filePath, append: true);
    }
    
    public RendererConfig AdjustConfig(in RendererConfig config)
    {
        // Optionally modify the config for file output
        // For example, disable colors for plain text files
        return config with
        {
            ColorConfig = ColorConfig.NoColors
        };
    }
}
```

### Using Custom Output

```csharp
// Set as global default
DumpConfig.Default.Output = new FileOutput("dump.log");

// Or use per-call
myObject.Dump(output: new FileOutput("dump.log"));
```

---

## Using DumpOutput Class

For simpler cases, use the built-in `DumpOutput` class instead of implementing the interface directly:

```csharp
// Simple output to any TextWriter
var output = new DumpOutput(Console.Out);

// With config adjustment
var output = new DumpOutput(
    writer: myTextWriter,
    configFactory: config => config with { ColorConfig = ColorConfig.NoColors }
);
```

---

## The AdjustConfig Method

The `AdjustConfig` method allows outputs to modify rendering behavior. Common use cases:

1. **Disable colors** for plain text outputs (files, logs)
2. **Adjust dimensions** based on the output medium
3. **Change table styles** for compatibility

```csharp
public RendererConfig AdjustConfig(in RendererConfig config)
{
    // Example: Force ASCII borders and no colors for log files
    return config with
    {
        ColorConfig = ColorConfig.NoColors,
        TableConfig = config.TableConfig with
        {
            BorderStyle = TableBorderStyle.Ascii
        }
    };
}
```

---

## See Also

- [DumpOutput](./dump-output.md) - Default implementation of `IDumpOutput`
- [IRenderer](./irenderer.md) - Interface for custom renderers
- [DumpConfig Reference](./dump-config.md) - Global configuration
- [Output Configuration](../configuration/output-config.md)
