# Output Targets

Dumpify supports multiple output destinations, allowing you to send dump output to the console, debug window, trace listeners, or capture it as a string.

## Table of Contents

- [Built-in Output Targets](#built-in-output-targets)
- [Console Output](#console-output)
- [Debug Output](#debug-output)
- [Trace Output](#trace-output)
- [Text Output](#text-output)
- [Custom Output](#custom-output)
- [Default Output Configuration](#default-output-configuration)

---

## Built-in Output Targets

| Method | Output Target | Use Case |
|--------|---------------|----------|
| `Dump()` | Configured default | General use (Console by default) |
| `DumpConsole()` | `Console.WriteLine` | Terminal/console applications |
| `DumpDebug()` | `System.Diagnostics.Debug` | IDE debug windows |
| `DumpTrace()` | `System.Diagnostics.Trace` | Trace listeners, logging |
| `DumpText()` | Returns `string` | Logging, file output, testing |

---

## Console Output

### Using DumpConsole

```csharp
myObject.DumpConsole();
myObject.DumpConsole("My Label");
```

Always outputs to `Console.WriteLine`, regardless of the global default.

### Console Features

- Full color support (ANSI escape codes)
- Respects console width
- Works in terminals that support colors

---

## Debug Output

### Using DumpDebug

```csharp
myObject.DumpDebug();
myObject.DumpDebug("Debug Data");
```

Outputs to `System.Diagnostics.Debug`, which appears in:
- Visual Studio Debug Output window
- JetBrains Rider Debug Output
- VS Code Debug Console
- Other debugger-attached outputs

### Debug-Specific Settings

`DumpDebug` uses a wider default width (250 characters) since debug windows typically support wider output:

```csharp
// Default behavior
myObject.DumpDebug();  // WidthOverride = 250

// Custom width
myObject.DumpDebug(outputConfig: new OutputConfig { WidthOverride = 300 });
```

### Conditional Compilation

Debug output is typically removed in Release builds:

```csharp
#if DEBUG
myObject.DumpDebug("Development Only");
#endif
```

---

## Trace Output

### Using DumpTrace

```csharp
myObject.DumpTrace();
myObject.DumpTrace("Trace Data");
```

Outputs to `System.Diagnostics.Trace`, which can be captured by:
- Configured trace listeners
- Logging frameworks
- Diagnostic tools

### Trace-Specific Settings

Like `DumpDebug`, `DumpTrace` uses a wider default width (250 characters).

### Adding Trace Listeners

```csharp
// Add a file listener
Trace.Listeners.Add(new TextWriterTraceListener("trace.log"));

// Now DumpTrace output goes to the file
myObject.DumpTrace("Logged to file");

// Don't forget to flush
Trace.Flush();
```

---

## Text Output

### Using DumpText

```csharp
string output = myObject.DumpText();
string labeledOutput = myObject.DumpText("My Object");
```

Returns the formatted output as a string instead of writing it anywhere.

### Text-Specific Settings

`DumpText` has special defaults:

```csharp
// Default behavior
string text = myObject.DumpText();
// ColorConfig = NoColors (no ANSI codes)
// WidthOverride = 1000
// HeightOverride = 1000
```

### Use Cases

#### Logging

```csharp
logger.Info(myObject.DumpText("Request Data"));
logger.Error(exception.DumpText("Exception Details"));
```

#### File Output

```csharp
File.WriteAllText("dump.txt", myObject.DumpText());
File.AppendAllText("log.txt", $"{DateTime.Now}: {myObject.DumpText()}\n");
```

#### Testing Assertions

```csharp
var output = myObject.DumpText();
Assert.Contains("Expected Value", output);
```

#### String Building

```csharp
var sb = new StringBuilder();
sb.AppendLine("=== Report ===");
sb.AppendLine(data.DumpText("Current State"));
sb.AppendLine(metrics.DumpText("Metrics"));
return sb.ToString();
```

---

## Custom Output

### Setting the Default Output

```csharp
// Change default output to Debug
DumpConfig.Default.Output = Dumpify.Outputs.Debug;

// Now Dump() goes to Debug
myObject.Dump();        // Goes to Debug
myObject.DumpConsole(); // Still goes to Console
```

### Custom IDumpOutput Implementation

Create your own output target by implementing `IDumpOutput`:

```csharp
public class FileOutput : IDumpOutput
{
    private readonly string _path;
    
    public FileOutput(string path)
    {
        _path = path;
    }
    
    public void WriteRenderedObject(IRenderedObject renderedObject, OutputConfig config)
    {
        using var writer = new StreamWriter(_path, append: true);
        // Write to file
    }
}

// Use custom output
DumpConfig.Default.Output = new FileOutput("dumps.log");
```

### Per-Call Custom Output

```csharp
// Use custom output for specific call
myObject.Dump(output: new FileOutput("special.log"));
```

---

## Default Output Configuration

### Accessing Built-in Outputs

```csharp
// Built-in outputs
Dumpify.Outputs.Console  // Console output
Dumpify.Outputs.Debug    // Debug output
Dumpify.Outputs.Trace    // Trace output
```

### Changing the Default

```csharp
// At application startup
DumpConfig.Default.Output = Dumpify.Outputs.Debug;

// Later, all Dump() calls go to Debug
myObject.Dump();  // Debug output
```

### Checking Current Default

```csharp
var currentOutput = DumpConfig.Default.Output;
```

---

## Output Dimensions

Control output dimensions via `OutputConfig`:

```csharp
// Global settings
DumpConfig.Default.OutputConfig.WidthOverride = 200;
DumpConfig.Default.OutputConfig.HeightOverride = 500;

// Per-call settings
myObject.Dump(outputConfig: new OutputConfig 
{ 
    WidthOverride = 300,
    HeightOverride = 1000 
});
```

See [Output Configuration](../configuration/output-config.md) for details.

---

## Output and Colors

Different outputs handle colors differently:

| Output | Colors |
|--------|--------|
| Console | Full ANSI color support |
| Debug | ANSI codes may not render |
| Trace | ANSI codes may not render |
| Text | No colors by default |

### Disabling Colors

```csharp
// For Debug/Trace where colors don't render well
myObject.DumpDebug(colors: ColorConfig.NoColors);
```

---

## Examples

### Multi-Output Debugging

```csharp
// Output to multiple destinations
myObject.Dump("Console");
myObject.DumpDebug("Debug Window");

// Save to file for later analysis
File.WriteAllText("debug.txt", myObject.DumpText("Full Details"));
```

### Conditional Output

```csharp
public void Process(Data data)
{
#if DEBUG
    data.DumpDebug("Input");
#endif

    var result = DoProcessing(data);
    
#if DEBUG
    result.DumpDebug("Output");
#endif

    return result;
}
```

### Logging Integration

```csharp
public class DumpifyLogger : IDumpOutput
{
    private readonly ILogger _logger;
    
    public DumpifyLogger(ILogger logger)
    {
        _logger = logger;
    }
    
    public void WriteRenderedObject(IRenderedObject renderedObject, OutputConfig config)
    {
        // Convert to string and log
        using var writer = new StringWriter();
        renderedObject.Output(new DumpOutput(writer), config);
        _logger.LogDebug(writer.ToString());
    }
}
```

---

## See Also

- [Extension Methods](../api-reference/dump-extensions.md)
- [Output Configuration](../configuration/output-config.md)
- [Color Configuration](../configuration/color-config.md)
- [Features Overview](./index.md)
