# ColorConfig

`ColorConfig` controls the colors used when rendering dumped objects. You can customize individual colors or use predefined color schemes.

---

## Table of Contents

- [Properties](#properties)
- [Predefined Configurations](#predefined-configurations)
- [Using DumpColor](#using-dumpcolor)
- [Examples](#examples)

---

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `TypeNameColor` | `DumpColor?` | Color for type names |
| `ColumnNameColor` | `DumpColor?` | Color for column headers |
| `PropertyValueColor` | `DumpColor?` | Color for property values |
| `PropertyNameColor` | `DumpColor?` | Color for property names |
| `NullValueColor` | `DumpColor?` | Color for null values |
| `IgnoredValueColor` | `DumpColor?` | Color for ignored/filtered values |
| `MetadataInfoColor` | `DumpColor?` | Color for metadata information |
| `MetadataErrorColor` | `DumpColor?` | Color for error metadata |
| `LabelValueColor` | `DumpColor?` | Color for label text |

---

## Predefined Configurations

### DefaultColors

The default color scheme used by Dumpify:

```csharp
var defaultColors = ColorConfig.DefaultColors;
```

### NoColors

Disables all colors for plain text output:

```csharp
obj.Dump(colors: ColorConfig.NoColors);
```

![No colors](https://user-images.githubusercontent.com/8770486/232252235-18d43c3a-0b54-475a-befc-0f957777f150.png)

---

## Using DumpColor

Colors are specified using the `DumpColor` class. You can create colors in several ways:

### From Hex String

```csharp
var color = new DumpColor("#FF5733");
// or
var color = DumpColor.FromHexString("#FF5733");
```

### From System.Drawing.Color

```csharp
using System.Drawing;

var color = new DumpColor(Color.RoyalBlue1);
```

### Implicit Conversion

```csharp
// From string
DumpColor color = "#FF5733";

// From System.Drawing.Color
DumpColor color = Color.Gold;
```

---

## Examples

### Custom Type Name Color

```csharp
obj.Dump(colors: new ColorConfig 
{ 
    TypeNameColor = new DumpColor("#FFD700") // Gold
});
```

### Custom Property Colors

```csharp
obj.Dump(colors: new ColorConfig 
{ 
    PropertyNameColor = new DumpColor("#87CEEB"),  // Sky blue
    PropertyValueColor = new DumpColor("#98FB98")  // Pale green
});
```

### Highlight Null Values

```csharp
obj.Dump(colors: new ColorConfig 
{ 
    NullValueColor = new DumpColor("#FF0000") // Red
});
```

### Global Color Configuration

```csharp
// Set colors globally
DumpConfig.Default.ColorConfig.TypeNameColor = new DumpColor("#FFD700");
DumpConfig.Default.ColorConfig.PropertyNameColor = new DumpColor("#87CEEB");
DumpConfig.Default.ColorConfig.PropertyValueColor = new DumpColor("#98FB98");
DumpConfig.Default.ColorConfig.NullValueColor = new DumpColor("#FF6347");

// All subsequent dumps will use these colors
obj.Dump();
```

### Using System.Drawing.Color Constants

```csharp
using System.Drawing;

obj.Dump(colors: new ColorConfig 
{ 
    TypeNameColor = new DumpColor(Color.Gold),
    PropertyValueColor = new DumpColor(Color.RoyalBlue),
    NullValueColor = new DumpColor(Color.Red)
});
```

![Custom colors](https://user-images.githubusercontent.com/8770486/232252235-18d43c3a-0b54-475a-befc-0f957777f150.png)

### Complete Custom Color Scheme

```csharp
var darkTheme = new ColorConfig
{
    TypeNameColor = new DumpColor("#E5C07B"),      // Muted yellow
    ColumnNameColor = new DumpColor("#61AFEF"),    // Blue
    PropertyNameColor = new DumpColor("#C678DD"),  // Purple
    PropertyValueColor = new DumpColor("#98C379"), // Green
    NullValueColor = new DumpColor("#E06C75"),     // Red
    MetadataInfoColor = new DumpColor("#5C6370"),  // Gray
    LabelValueColor = new DumpColor("#56B6C2")     // Cyan
};

obj.Dump(colors: darkTheme);
```

---

## See Also

- [DumpColor API Reference](../api-reference/dump-color.md)
- [Global Configuration](global-configuration.md)
- [Configuration Overview](index.md)
