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

Use the `configBuilder` parameter to customize a single dump:

```csharp
var data = GetComplexData();

data.Dump(config => config
    .SetMaxDepth(2)
    .UseTableConfig(table => table
        .ShowTableHeaders(false)
        .SetColumnSeparator(" | "))
    .UseColorConfig(color => color
        .SetPropertyValueColor(Color.Aqua)));
```

### Reusable Configuration

Create a `DumpConfig` instance for repeated use:

```csharp
var debugConfig = new DumpConfig
{
    MaxDepth = 5,
    TableConfig = new TableConfig
    {
        ShowHeaders = true,
        ExpandTables = true
    },
    ColorConfig = new ColorConfig
    {
        PropertyValueColor = new DumpColor("#00FFFF")
    }
};

// Use the same config multiple times
item1.Dump(debugConfig);
item2.Dump(debugConfig);
item3.Dump(debugConfig);
```

## Color Customization

### Using System.Drawing.Color

```csharp
using System.Drawing;

data.Dump(config => config.UseColorConfig(color => color
    .SetLabelColor(Color.Yellow)
    .SetTypeNameColor(Color.Cyan)
    .SetPropertyNameColor(Color.LightGreen)
    .SetPropertyValueColor(Color.White)
    .SetNullValueColor(Color.Gray)
    .SetMetadataInfoColor(Color.DarkGray)
    .SetMetadataErrorColor(Color.Red)));
```

### Using Hex Strings

```csharp
data.Dump(config => config.UseColorConfig(color => color
    .SetLabelColor("#FFD700")      // Gold
    .SetTypeNameColor("#00CED1")    // Dark Turquoise
    .SetPropertyNameColor("#98FB98") // Pale Green
    .SetPropertyValueColor("#FFFFFF")
    .SetNullValueColor("#808080")));
```

### Using DumpColor

```csharp
var accentColor = new DumpColor(Color.Coral);
var dimColor = new DumpColor("#666666");

data.Dump(config => config.UseColorConfig(color => color
    .SetPropertyValueColor(accentColor)
    .SetMetadataInfoColor(dimColor)));
```

### Themed Configurations

```csharp
// Dark theme
var darkTheme = new ColorConfig
{
    LabelColor = new DumpColor("#BB86FC"),
    TypeNameColor = new DumpColor("#03DAC6"),
    PropertyNameColor = new DumpColor("#CF6679"),
    PropertyValueColor = new DumpColor("#FFFFFF"),
    NullValueColor = new DumpColor("#666666")
};

// Light theme (for light terminal backgrounds)
var lightTheme = new ColorConfig
{
    LabelColor = new DumpColor("#6200EE"),
    TypeNameColor = new DumpColor("#018786"),
    PropertyNameColor = new DumpColor("#B00020"),
    PropertyValueColor = new DumpColor("#000000"),
    NullValueColor = new DumpColor("#999999")
};

data.Dump(config => config.SetColorConfig(darkTheme));
```

## Table Styling

### Header Control

```csharp
// Hide headers for cleaner output
data.Dump(config => config.UseTableConfig(table => table
    .ShowTableHeaders(false)));

// Show row separators
data.Dump(config => config.UseTableConfig(table => table
    .ShowMemberTypes(true)));
```

### Table Expansion

```csharp
// Force tables to expand to full width
data.Dump(config => config.UseTableConfig(table => table
    .SetExpandTables(true)));

// Compact tables
data.Dump(config => config.UseTableConfig(table => table
    .SetExpandTables(false)));
```

### Custom Separators

```csharp
data.Dump(config => config.UseTableConfig(table => table
    .SetColumnSeparator(" :: ")
    .SetRowSeparator("-")));
```

## Member Filtering

### Excluding Specific Properties

```csharp
data.Dump(config => config.UseMembersConfig(members => members
    .IncludeFields(false)
    .IncludePublicMembers(true)
    .IncludeNonPublicMembers(false)));
```

### Using Filters

```csharp
// Exclude properties by name
data.Dump(config => config.UseMembersConfig(members => members
    .SetMemberFilter(member => !member.Name.StartsWith("_"))
    .SetMemberFilter(member => member.Name != "Password")));

// Only include specific properties
var allowedProps = new[] { "Id", "Name", "Email" };
data.Dump(config => config.UseMembersConfig(members => members
    .SetMemberFilter(member => allowedProps.Contains(member.Name))));
```

### Filtering by Attribute

```csharp
// Exclude properties with [JsonIgnore]
data.Dump(config => config.UseMembersConfig(members => members
    .SetMemberFilter(member => 
        !member.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())));
```

## Custom Type Handlers

### Basic Type Handler

```csharp
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(SecureString)] = 
    (obj, type, config) => "****";

var secure = new SecureString();
// Adds characters...
secure.Dump(); // Outputs: ****
```

### Handler with Formatting

```csharp
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(TimeSpan)] = 
    (obj, type, config) =>
    {
        var ts = (TimeSpan)obj;
        if (ts.TotalDays >= 1)
            return $"{ts.Days}d {ts.Hours}h {ts.Minutes}m";
        if (ts.TotalHours >= 1)
            return $"{ts.Hours}h {ts.Minutes}m {ts.Seconds}s";
        return $"{ts.Minutes}m {ts.Seconds}s";
    };

TimeSpan.FromHours(25.5).Dump(); // Outputs: 1d 1h 30m
```

### Handler for Domain Types

```csharp
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(Money)] = 
    (obj, type, config) =>
    {
        var money = (Money)obj;
        return $"{money.Currency} {money.Amount:N2}";
    };

new Money { Currency = "USD", Amount = 1234.56m }.Dump(); // Outputs: USD 1,234.56
```

### Generic Type Handler

```csharp
// Handle all types implementing IEntity
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(IEntity)] = 
    (obj, type, config) =>
    {
        var entity = (IEntity)obj;
        return $"{type.Name}#{entity.Id}";
    };
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
deepNestedObject.Dump(config => config.SetMaxDepth(2));

// Deep inspection
deepNestedObject.Dump(config => config.SetMaxDepth(10));
```

### Combining with Filters

```csharp
// Show shallow with only specific members
complexObject.Dump(config => config
    .SetMaxDepth(1)
    .UseMembersConfig(m => m
        .SetMemberFilter(member => 
            member.Name == "Id" || 
            member.Name == "Name" || 
            member.Name == "Status")));
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

### Output Configuration

```csharp
data.Dump(config => config.UseOutputConfig(output => output
    // Configure output behavior
));
```

## Combining Configurations

### Complete Configuration Example

```csharp
var fullConfig = new DumpConfig
{
    Label = "Full Config Example",
    MaxDepth = 3,
    
    ColorConfig = new ColorConfig
    {
        LabelColor = new DumpColor(Color.Gold),
        TypeNameColor = new DumpColor("#00CED1"),
        PropertyNameColor = new DumpColor(Color.LightGreen),
        PropertyValueColor = new DumpColor("#FFFFFF"),
        NullValueColor = new DumpColor(Color.Gray),
        MetadataInfoColor = new DumpColor("#666666")
    },
    
    TableConfig = new TableConfig
    {
        ShowHeaders = true,
        ExpandTables = true,
        ShowMemberTypes = false
    },
    
    MembersConfig = new MembersConfig
    {
        IncludeFields = false,
        IncludePublicMembers = true,
        IncludeNonPublicMembers = false
    },
    
    TypeNamingConfig = new TypeNamingConfig
    {
        ShowTypeNames = true,
        UseAliases = true,
        UseFullTypeNames = false
    }
};

data.Dump(fullConfig);
```

### Layered Configuration

```csharp
// Base configuration
var baseConfig = new DumpConfig
{
    MaxDepth = 5,
    ColorConfig = new ColorConfig
    {
        PropertyValueColor = new DumpColor(Color.White)
    }
};

// Use base config with inline overrides
data.Dump(config => config
    .SetMaxDepth(baseConfig.MaxDepth)
    .UseColorConfig(color => color
        .SetPropertyValueColor(baseConfig.ColorConfig.PropertyValueColor)
        .SetLabelColor(Color.Yellow))); // Override just the label color
```

## See Also

- [Basic Usage](basic-usage.md) - Fundamental examples
- [Real-World Scenarios](real-world-scenarios.md) - Practical applications
- [Configuration Reference](../configuration/index.md) - All configuration options
- [Custom Type Handlers](../features/custom-type-handlers.md) - Detailed handler guide
