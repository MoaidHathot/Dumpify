# Features

Comprehensive guide to Dumpify's features and capabilities.

## Table of Contents

- [Circular Reference Handling](#circular-reference-handling)
- [Collections Support](#collections-support)
- [Member Filtering](#member-filtering)
- [Custom Type Handlers](#custom-type-handlers)
- [Labels](#labels)
- [Output Targets](#output-targets)
- [Depth Control](#depth-control)
- [Type Name Display](#type-name-display)

## Circular Reference Handling

Dumpify automatically detects and safely handles circular references, preventing infinite loops and stack overflows.

### How It Works

When Dumpify encounters an object it has already seen in the current dump tree, it displays a reference marker instead of recursively dumping the same object again.

### Example

```csharp
public class Node
{
    public string Name { get; set; }
    public Node? Next { get; set; }
}

var node1 = new Node { Name = "First" };
var node2 = new Node { Name = "Second" };
node1.Next = node2;
node2.Next = node1;  // Circular reference

node1.Dump();  // Safely displays without infinite loop
```

### Parent-Child Relationships

Common in Entity Framework and tree structures:

```csharp
public class TreeNode
{
    public string Value { get; set; }
    public TreeNode? Parent { get; set; }
    public List<TreeNode> Children { get; set; } = new();
}

var root = new TreeNode { Value = "Root" };
var child = new TreeNode { Value = "Child", Parent = root };
root.Children.Add(child);

root.Dump();  // Shows "Circular reference" for Parent property
```

## Collections Support

Dumpify supports all common .NET collection types with optimized display.

### Supported Types

| Type | Display |
|------|---------|
| Arrays | Indexed table |
| `List<T>` | Indexed table |
| `Dictionary<K,V>` | Key-value table |
| `HashSet<T>` | Value list |
| `Queue<T>` | Value list |
| `Stack<T>` | Value list |
| `IEnumerable<T>` | Indexed table |
| Multidimensional arrays | Nested tables |

### Examples

```csharp
// Arrays
new[] { 1, 2, 3 }.Dump();

// Lists of objects
var people = new List<Person> { person1, person2 };
people.Dump();

// Dictionaries
new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }.Dump();

// Nested collections
var nested = new Dictionary<string, List<int>>
{
    ["odds"] = new List<int> { 1, 3, 5 },
    ["evens"] = new List<int> { 2, 4, 6 }
};
nested.Dump();
```

### Collection of Objects

Collections of complex objects display as tables with columns for each property:

```csharp
var users = new[]
{
    new User { Id = 1, Name = "Alice", Email = "alice@example.com" },
    new User { Id = 2, Name = "Bob", Email = "bob@example.com" }
};
users.Dump();

// Displays as:
// | Id | Name  | Email             |
// | 1  | Alice | alice@example.com |
// | 2  | Bob   | bob@example.com   |
```

## Member Filtering

Control which members appear in dump output.

### Include/Exclude by Type

```csharp
data.Dump(config => config.UseMembersConfig(m => m
    .IncludeFields(false)           // Exclude fields
    .IncludePublicMembers(true)     // Include public properties
    .IncludeNonPublicMembers(true)  // Include private properties
));
```

### Custom Filter Function

```csharp
// Exclude sensitive properties
data.Dump(config => config.UseMembersConfig(m => m
    .SetMemberFilter(member => 
        !new[] { "Password", "Token", "Secret" }.Contains(member.Name))));

// Include only specific properties
data.Dump(config => config.UseMembersConfig(m => m
    .SetMemberFilter(member => 
        new[] { "Id", "Name", "Email" }.Contains(member.Name))));

// Filter by attribute
data.Dump(config => config.UseMembersConfig(m => m
    .SetMemberFilter(member => 
        !member.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())));

// Filter by naming pattern
data.Dump(config => config.UseMembersConfig(m => m
    .SetMemberFilter(member => !member.Name.StartsWith("_"))));
```

## Custom Type Handlers

Register custom rendering logic for specific types.

### Basic Handler

```csharp
// Register globally
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(SecureString)] = 
    (obj, type, config) => "****";
```

### Formatting Handler

```csharp
// Custom DateTime format
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(DateTime)] = 
    (obj, type, config) =>
    {
        var dt = (DateTime)obj;
        return dt.ToString("yyyy-MM-dd HH:mm:ss");
    };

// Custom TimeSpan format
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(TimeSpan)] = 
    (obj, type, config) =>
    {
        var ts = (TimeSpan)obj;
        if (ts.TotalDays >= 1) return $"{ts.Days}d {ts.Hours}h";
        if (ts.TotalHours >= 1) return $"{ts.Hours}h {ts.Minutes}m";
        return $"{ts.Minutes}m {ts.Seconds}s";
    };
```

### Domain Type Handler

```csharp
public record Money(string Currency, decimal Amount);

DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(Money)] = 
    (obj, type, config) =>
    {
        var money = (Money)obj;
        return $"{money.Currency} {money.Amount:N2}";
    };

new Money("USD", 1234.56m).Dump();  // Output: USD 1,234.56
```

### Handler Signature

```csharp
Func<object?, Type, DumpConfig, string?>
```

- `object?` - The value being rendered
- `Type` - The type of the value  
- `DumpConfig` - Current configuration
- Returns `string?` - Custom representation, or null to use default

## Labels

Labels identify dump output and appear above the rendered content.

### Basic Labels

```csharp
data.Dump("My Data");
result.Dump("Query Result");
```

### Dynamic Labels

```csharp
foreach (var item in items)
{
    item.Dump($"Item #{item.Id}");
}
```

### With Configuration

```csharp
data.Dump("Styled Label", config => config
    .UseColorConfig(c => c.SetLabelColor(Color.Yellow)));
```

## Output Targets

Send dump output to different destinations.

### Console (Default)

```csharp
data.Dump();        // Implicit console
data.DumpConsole(); // Explicit console
```

### Debug Output

Sends to Visual Studio Output window (Debug category):

```csharp
data.DumpDebug();
```

### Trace Output

Sends to trace listeners:

```csharp
data.DumpTrace();
```

### String Output

Returns the dump as a string for logging or custom handling:

```csharp
var text = data.DumpText();
logger.LogInformation("Data: {Data}", text);
File.WriteAllText("dump.txt", text);
```

### Conditional Output

```csharp
#if DEBUG
data.Dump("Debug only");
#endif

if (config.VerboseLogging)
{
    data.DumpDebug("Verbose");
}
```

## Depth Control

Control how deep nested objects are rendered.

### Setting Max Depth

```csharp
// Per-dump
deepObject.Dump(config => config.SetMaxDepth(2));

// Global default
DumpConfig.Default.MaxDepth = 3;
```

### Behavior

- Depth 1: Only immediate properties
- Depth 2: Properties and their nested objects
- Depth N: N levels of nesting

```csharp
// Level 1: person.Name, person.Address (shown as type)
// Level 2: person.Name, person.Address.Street, person.Address.City
// Level 3+: Continues deeper
```

## Type Name Display

Control how type names appear in output.

### Configuration Options

```csharp
data.Dump(config => config.UseTypeNamingConfig(t => t
    .SetShowTypeNames(true)      // Show type names at all
    .SetUseAliases(true)         // Use 'int' instead of 'Int32'
    .SetUseFullTypeNames(false)  // Use 'Person' instead of 'MyApp.Models.Person'
));
```

### Examples

| Setting | `int` displays as | `MyApp.Person` displays as |
|---------|-------------------|---------------------------|
| `UseAliases: true` | `int` | `Person` |
| `UseAliases: false` | `Int32` | `Person` |
| `UseFullTypeNames: true` | `System.Int32` | `MyApp.Person` |

## See Also

- [Configuration](configuration.md) - All settings
- [API Reference](api-reference.md) - Method documentation
- [Examples](examples.md) - Usage examples
