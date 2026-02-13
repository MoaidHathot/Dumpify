# DumpColor Reference

`DumpColor` is a utility class for representing colors in Dumpify configuration.

## Overview

```csharp
namespace Dumpify;

public class DumpColor
{
    // Constructors
    public DumpColor(string hexColor);
    public DumpColor(Color color);
    
    // Factory method
    public static DumpColor FromHexString(string hexColor);
    
    // Implicit conversions
    public static implicit operator DumpColor(string hexColor);
    public static implicit operator DumpColor(Color color);
}
```

---

## Creating DumpColor Instances

### From Hex String

```csharp
// Using constructor
var color1 = new DumpColor("#FF5733");

// Using factory method
var color2 = DumpColor.FromHexString("#FF5733");

// Using implicit conversion (most common)
DumpColor color3 = "#FF5733";
```

### From System.Drawing.Color

```csharp
using System.Drawing;

// Using constructor
var color1 = new DumpColor(Color.Red);

// Using implicit conversion
DumpColor color2 = Color.Blue;
```

---

## Hex String Format

Dumpify accepts standard hex color formats:

```csharp
// With hash prefix (recommended)
"#FF5733"
"#ff5733"  // Case insensitive

// Named colors (supported by System.Drawing)
"Red"
"Blue"
"Green"
```

---

## Usage with ColorConfig

The most common use of `DumpColor` is configuring colors in `ColorConfig`:

```csharp
// Implicit conversion from string - cleanest syntax
DumpConfig.Default.ColorConfig.NullValueColor = "#FF6B6B";
DumpConfig.Default.ColorConfig.StringValueColor = "#98C379";
DumpConfig.Default.ColorConfig.NumericValueColor = "#E5C07B";

// Using System.Drawing.Color
DumpConfig.Default.ColorConfig.BoolTrueColor = Color.Green;
DumpConfig.Default.ColorConfig.BoolFalseColor = Color.Red;

// Explicit construction
DumpConfig.Default.ColorConfig.PropertyValueColor = new DumpColor("#61AFEF");
```

---

## Implicit Conversions

`DumpColor` supports implicit conversion from both `string` and `System.Drawing.Color`, making it convenient to use:

```csharp
// All of these are equivalent
ColorConfig config = new ColorConfig();

config.NullValueColor = "#FF0000";                    // string
config.NullValueColor = new DumpColor("#FF0000");     // explicit
config.NullValueColor = Color.Red;                    // Color
config.NullValueColor = new DumpColor(Color.Red);    // explicit from Color
```

---

## Error Handling

Invalid hex strings will throw an exception:

```csharp
// This will throw
var invalid = new DumpColor("not-a-color");

// This will also throw
DumpColor color = "invalid";
```

Valid formats:
- `#RGB` (short form)
- `#RRGGBB` (full form)
- Named colors: `Red`, `Blue`, `Green`, etc.

---

## Internal Properties

While not part of the public API, `DumpColor` internally maintains:

- `Color` - The `System.Drawing.Color` representation
- `HexString` - The hex string representation

These are used internally by Dumpify's rendering system.

---

## Examples

### Configure Multiple Colors

```csharp
// Set up a dark theme
var colorConfig = DumpConfig.Default.ColorConfig;

colorConfig.NullValueColor = "#6B7280";      // Gray for null
colorConfig.StringValueColor = "#10B981";    // Green for strings
colorConfig.NumericValueColor = "#F59E0B";   // Amber for numbers
colorConfig.BoolTrueColor = "#22C55E";       // Green for true
colorConfig.BoolFalseColor = "#EF4444";      // Red for false
colorConfig.PropertyValueColor = "#3B82F6";  // Blue for properties
colorConfig.MetadataInfoColor = "#8B5CF6";   // Purple for metadata
```

### Using System.Drawing.Color Constants

```csharp
using System.Drawing;

var colorConfig = DumpConfig.Default.ColorConfig;

colorConfig.NullValueColor = Color.Gray;
colorConfig.StringValueColor = Color.ForestGreen;
colorConfig.NumericValueColor = Color.DarkGoldenrod;
colorConfig.BoolTrueColor = Color.Green;
colorConfig.BoolFalseColor = Color.Red;
```

### Per-Call Color Override

```csharp
var customColors = new ColorConfig
{
    NullValueColor = "#FF0000",  // Highlight nulls in red
};

myObject.Dump(colors: customColors);
```

---

## See Also

- [Color Configuration](../configuration/color-config.md)
- [DumpConfig Reference](./dump-config.md)
- [Configuration Overview](../configuration/index.md)
