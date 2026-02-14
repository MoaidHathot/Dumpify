---
layout: default
title: Advanced Usage
parent: Examples
nav_order: 2
---

# Advanced Usage Examples

This guide covers advanced Dumpify usage patterns for complex scenarios.

## Table of Contents

- [Custom Configuration Per Dump](#custom-configuration-per-dump)
- [Color Customization](#color-customization)
- [Table Styling](#table-styling)
- [Member Filtering](#member-filtering)
- [Custom Type Handlers](#custom-type-handlers)
- [Handling Circular References](#handling-circular-references)
- [Depth Control](#depth-control)
- [Output Redirection](#output-redirection)
- [Combining Configurations](#combining-configurations)

## Custom Configuration Per Dump

### Inline Configuration

Pass configuration parameters directly to customize a single dump:

```csharp
var data = GetComplexData();

data.Dump(
    maxDepth: 2,
    tableConfig: new TableConfig
    {
        ShowTableHeaders = false
    },
    colors: new ColorConfig
    {
        PropertyValueColor = "#00FFFF"
    }
);
```

### Global Configuration

Configure `DumpConfig.Default` for settings that apply to all dumps:

```csharp
// Configure globally at application startup
DumpConfig.Default.MaxDepth = 5;
DumpConfig.Default.TableConfig.ShowTableHeaders = true;
DumpConfig.Default.TableConfig.ExpandTables = true;
DumpConfig.Default.ColorConfig.PropertyValueColor = "#00FFFF";

// All subsequent dumps use these settings
item1.Dump();
item2.Dump();
item3.Dump();
```

## Color Customization

### Using System.Drawing.Color

```csharp
using System.Drawing;

var colorConfig = new ColorConfig
{
    LabelValueColor = new DumpColor(Color.Yellow),
    TypeNameColor = new DumpColor(Color.Cyan),
    PropertyNameColor = new DumpColor(Color.LightGreen),
    PropertyValueColor = new DumpColor(Color.White),
    NullValueColor = new DumpColor(Color.Gray),
    MetadataInfoColor = new DumpColor(Color.DarkGray),
    MetadataErrorColor = new DumpColor(Color.Red)
};

data.Dump(colors: colorConfig);
```

### Using Hex Strings

```csharp
var colorConfig = new ColorConfig
{
    LabelValueColor = "#FFD700",      // Gold
    TypeNameColor = "#00CED1",        // Dark Turquoise
    PropertyNameColor = "#98FB98",    // Pale Green
    PropertyValueColor = "#FFFFFF",
    NullValueColor = "#808080"
};

data.Dump(colors: colorConfig);
```

### Using DumpColor

```csharp
using System.Drawing;

var accentColor = new DumpColor(Color.Coral);
var dimColor = new DumpColor("#666666");

var colorConfig = new ColorConfig
{
    PropertyValueColor = accentColor,
    MetadataInfoColor = dimColor
};

data.Dump(colors: colorConfig);
```

### Themed Configurations

```csharp
// Dark theme
var darkTheme = new ColorConfig
{
    LabelValueColor = "#BB86FC",
    TypeNameColor = "#03DAC6",
    PropertyNameColor = "#CF6679",
    PropertyValueColor = "#FFFFFF",
    NullValueColor = "#666666"
};

// Light theme (for light terminal backgrounds)
var lightTheme = new ColorConfig
{
    LabelValueColor = "#6200EE",
    TypeNameColor = "#018786",
    PropertyNameColor = "#B00020",
    PropertyValueColor = "#000000",
    NullValueColor = "#999999"
};

data.Dump(colors: darkTheme);
```

## Table Styling

### Header Control

```csharp
// Hide headers for cleaner output
data.Dump(tableConfig: new TableConfig { ShowTableHeaders = false });

// Show member types column
data.Dump(tableConfig: new TableConfig { ShowMemberTypes = true });

// Show row separators
data.Dump(tableConfig: new TableConfig { ShowRowSeparators = true });
```

### Table Expansion

```csharp
// Force tables to expand to full width
data.Dump(tableConfig: new TableConfig { ExpandTables = true });

// Compact tables (default)
data.Dump(tableConfig: new TableConfig { ExpandTables = false });
```

### Combined Table Configuration

```csharp
var tableConfig = new TableConfig
{
    ShowTableHeaders = true,
    ShowRowSeparators = true,
    ShowMemberTypes = true,
    ShowArrayIndices = true,
    ExpandTables = false,
    MaxCollectionCount = 100
};

data.Dump(tableConfig: tableConfig);
```

## Member Filtering

### Including Private Members and Fields

```csharp
var membersConfig = new MembersConfig
{
    IncludeFields = true,
    IncludePublicMembers = true,
    IncludeNonPublicMembers = true
};

data.Dump(members: membersConfig);
```

### Using Custom Filters

```csharp
// Exclude properties by name
data.Dump(members: new MembersConfig
{
    MemberFilter = member => !member.Name.StartsWith("_")
});

// Exclude password properties
data.Dump(members: new MembersConfig
{
    MemberFilter = member => member.Name != "Password"
});

// Only include specific properties
var allowedProps = new[] { "Id", "Name", "Email" };
data.Dump(members: new MembersConfig
{
    MemberFilter = member => allowedProps.Contains(member.Name)
});
```

### Filtering by Attribute

```csharp
using System.Text.Json.Serialization;

// Exclude properties with [JsonIgnore]
data.Dump(members: new MembersConfig
{
    MemberFilter = member => !member.Info.CustomAttributes
        .Any(a => a.AttributeType == typeof(JsonIgnoreAttribute))
});
```

## Custom Type Handlers

Custom type handlers allow you to control how specific types are rendered.

### Basic Type Handler

```csharp
// Register a handler for SecureString
DumpConfig.Default.AddCustomTypeHandler(
    typeof(SecureString),
    (obj, type, valueProvider, memberProvider) => "****"
);

var secure = new SecureString();
// Adds characters...
secure.Dump(); // Outputs: ****
```

### Handler with Formatting

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(TimeSpan),
    (obj, type, valueProvider, memberProvider) =>
    {
        var ts = (TimeSpan)obj;
        if (ts.TotalDays >= 1)
            return $"{ts.Days}d {ts.Hours}h {ts.Minutes}m";
        if (ts.TotalHours >= 1)
            return $"{ts.Hours}h {ts.Minutes}m {ts.Seconds}s";
        return $"{ts.Minutes}m {ts.Seconds}s";
    }
);

TimeSpan.FromHours(25.5).Dump(); // Outputs: 1d 1h 30m
```

### Handler for Domain Types

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(Money),
    (obj, type, valueProvider, memberProvider) =>
    {
        var money = (Money)obj;
        return $"{money.Currency} {money.Amount:N2}";
    }
);

new Money { Currency = "USD", Amount = 1234.56m }.Dump(); // Outputs: USD 1,234.56
```

### Removing Handlers

```csharp
// Remove a previously registered handler
DumpConfig.Default.RemoveCustomTypeHandler(typeof(TimeSpan));
```

## Handling Circular References

### Understanding Circular Reference Detection

Dumpify automatically detects circular references and displays them safely:

```csharp
public class Node
{
    public string Name { get; set; }
    public Node? Next { get; set; }
}

var node1 = new Node { Name = "First" };
var node2 = new Node { Name = "Second" };
node1.Next = node2;
node2.Next = node1; // Circular reference

node1.Dump(); // Safely displays without infinite loop
```

### Parent-Child Relationships

```csharp
public class TreeNode
{
    public string Value { get; set; }
    public TreeNode? Parent { get; set; }
    public List<TreeNode> Children { get; set; } = new();
}

var root = new TreeNode { Value = "Root" };
var child1 = new TreeNode { Value = "Child 1", Parent = root };
var child2 = new TreeNode { Value = "Child 2", Parent = root };
root.Children.AddRange(new[] { child1, child2 });

root.Dump(); // Shows structure with circular reference markers
```

## Depth Control

### Limiting Depth

```csharp
// Shallow dump - only 2 levels deep
deepNestedObject.Dump(maxDepth: 2);

// Deep inspection
deepNestedObject.Dump(maxDepth: 10);
```

### Combining with Filters

```csharp
// Show shallow with only specific members
complexObject.Dump(
    maxDepth: 1,
    members: new MembersConfig
    {
        MemberFilter = member =>
            member.Name == "Id" ||
            member.Name == "Name" ||
            member.Name == "Status"
    }
);
```

## Output Redirection

### Multiple Output Targets

```csharp
// Console (default)
data.DumpConsole("Console Output");

// Debug (Visual Studio Output)
data.DumpDebug("Debug Output");

// Trace
data.DumpTrace("Trace Output");

// String (for logging)
var text = data.DumpText();
logger.Information("Data dump: {Data}", text);
```

### Conditional Output

```csharp
// Only dump in debug builds
#if DEBUG
data.Dump("Debug Mode Data");
#endif

// Conditional based on configuration
if (config.VerboseLogging)
{
    data.DumpDebug("Verbose");
}
```

### Changing Default Output

```csharp
// All Dump() calls will go to Debug by default
DumpConfig.Default.Output = Dumpify.Outputs.Debug;

data.Dump(); // Goes to Debug
data.DumpConsole(); // Explicitly goes to Console
```

## Combining Configurations

### Complete Configuration Example

```csharp
// Configure globally
DumpConfig.Default.MaxDepth = 3;

DumpConfig.Default.ColorConfig.LabelValueColor = "#FFD700";
DumpConfig.Default.ColorConfig.TypeNameColor = "#00CED1";
DumpConfig.Default.ColorConfig.PropertyNameColor = "#90EE90";
DumpConfig.Default.ColorConfig.PropertyValueColor = "#FFFFFF";
DumpConfig.Default.ColorConfig.NullValueColor = "#808080";
DumpConfig.Default.ColorConfig.MetadataInfoColor = "#666666";

DumpConfig.Default.TableConfig.ShowTableHeaders = true;
DumpConfig.Default.TableConfig.ExpandTables = true;
DumpConfig.Default.TableConfig.ShowMemberTypes = false;

DumpConfig.Default.MembersConfig.IncludeFields = false;
DumpConfig.Default.MembersConfig.IncludePublicMembers = true;
DumpConfig.Default.MembersConfig.IncludeNonPublicMembers = false;

DumpConfig.Default.TypeNamingConfig.ShowTypeNames = true;
DumpConfig.Default.TypeNamingConfig.UseAliases = true;
DumpConfig.Default.TypeNamingConfig.UseFullName = false;

// All dumps now use these settings
data.Dump("Full Config Example");
```

### Per-Call Overrides

```csharp
// Use global config but override specific settings for this call
data.Dump(
    label: "Special Output",
    maxDepth: 10,
    colors: new ColorConfig { NullValueColor = "#FF0000" },
    tableConfig: new TableConfig { ShowRowSeparators = true }
);
```

## See Also

- [Basic Usage](basic-usage.md) - Fundamental examples
- [Real-World Scenarios](real-world-scenarios.md) - Practical applications
- [Configuration Reference](../configuration/index.md) - All configuration options
- [Custom Type Handlers](../features/custom-type-handlers.md) - Detailed handler guide
