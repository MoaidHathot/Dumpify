# OutputConfig

`OutputConfig` controls the dimensions of the output, allowing you to override the default console width and height.

---

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `WidthOverride` | `int?` | `null` | Override the output width in characters |
| `HeightOverride` | `int?` | `null` | Override the output height in lines |

When set to `null`, Dumpify uses the console's actual dimensions.

---

## Examples

### Override Output Width

Force a specific width regardless of console size:

```csharp
obj.Dump(outputConfig: new OutputConfig { WidthOverride = 120 });
```

### Override Output Height

Force a specific height:

```csharp
obj.Dump(outputConfig: new OutputConfig { HeightOverride = 50 });
```

### Override Both Dimensions

```csharp
obj.Dump(outputConfig: new OutputConfig 
{ 
    WidthOverride = 120,
    HeightOverride = 50 
});
```

### Global Output Configuration

```csharp
// Set globally
DumpConfig.Default.OutputConfig.WidthOverride = 150;
DumpConfig.Default.OutputConfig.HeightOverride = null; // Use console height

// All dumps use this width
myObject.Dump();
```

### Reset to Console Defaults

```csharp
DumpConfig.Default.OutputConfig.WidthOverride = null;
DumpConfig.Default.OutputConfig.HeightOverride = null;
```

---

## Use Cases

### Consistent Output in CI/CD

In continuous integration environments where console dimensions may vary:

```csharp
DumpConfig.Default.OutputConfig.WidthOverride = 120;
```

### Wide Tables

When dumping objects with many properties, increase width:

```csharp
wideObject.Dump(outputConfig: new OutputConfig { WidthOverride = 200 });
```

### Narrow Output for Side-by-Side Viewing

When viewing output alongside code:

```csharp
obj.Dump(outputConfig: new OutputConfig { WidthOverride = 80 });
```

### File Output

When redirecting to a file where console width doesn't apply:

```csharp
DumpConfig.Default.OutputConfig.WidthOverride = 120;
var text = obj.DumpText();
File.WriteAllText("dump.txt", text);
```

---

## Complete Example

```csharp
var outputConfig = new OutputConfig
{
    WidthOverride = 120,
    HeightOverride = null  // Use console height
};

obj.Dump(outputConfig: outputConfig);
```

---

## See Also

- [Configuration Overview](index.md)
- [Global Configuration](global-configuration.md)
- [Output Targets](../features/output-targets.md)
