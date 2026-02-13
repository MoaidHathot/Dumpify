# Features

Dumpify provides a rich set of features for debugging and visualizing data in .NET console applications.

## Feature Overview

| Feature | Description |
|---------|-------------|
| [Circular References](./circular-references.md) | Safe handling of circular object references |
| [Collections](./collections.md) | Smart rendering of arrays, lists, dictionaries |
| [Member Filtering](./member-filtering.md) | Control which properties and fields are displayed |
| [Custom Type Handlers](./custom-type-handlers.md) | Define custom rendering for specific types |
| [Labels](./labels.md) | Add descriptive labels to output |
| [Output Targets](./output-targets.md) | Multiple output destinations |

---

## Quick Feature Highlights

### Beautiful Table Rendering

Dumpify renders objects as formatted tables with color-coded values:

```csharp
new Person { Name = "John", Age = 30 }.Dump();
```

![Single Object](https://raw.githubusercontent.com/MoaidHathworker/Dumpify/main/assets/1-singleObject.png)

### Collection Support

Arrays, lists, and dictionaries are rendered as organized tables:

```csharp
var people = new[] 
{
    new Person { Name = "John", Age = 30 },
    new Person { Name = "Jane", Age = 25 }
};
people.Dump();
```

![Array Of Objects](https://raw.githubusercontent.com/MoaidHathworker/Dumpify/main/assets/2-arrayOfObjects.png)

### Nested Objects

Complex object hierarchies are displayed with proper indentation:

```csharp
new 
{
    Company = "Acme",
    Employees = new[] 
    { 
        new { Name = "John", Department = "Engineering" }
    }
}.Dump();
```

![Nested Objects](https://raw.githubusercontent.com/MoaidHathworker/Dumpify/main/assets/3-nestedObjects.png)

### Circular Reference Protection

Objects with circular references are safely handled:

```csharp
var node = new Node { Value = 1 };
node.Next = node;  // Circular reference
node.Dump();       // Won't crash!
```

### Method Chaining

Dump methods return the original object for seamless LINQ integration:

```csharp
var result = GetUsers()
    .Dump("Raw")
    .Where(u => u.IsActive)
    .Dump("Active")
    .OrderBy(u => u.Name)
    .ToList();
```

### Multiple Output Targets

Send output to different destinations:

```csharp
obj.Dump();        // Default output
obj.DumpConsole(); // Console
obj.DumpDebug();   // Debug window
obj.DumpTrace();   // Trace listeners

string text = obj.DumpText();  // Return as string
```

### Colorful Output

Values are color-coded by type for easy reading:

- Strings in green
- Numbers in yellow
- Booleans in blue/red
- Null values in gray

### Customizable

Control every aspect of the output:

```csharp
DumpConfig.Default.MaxDepth = 3;
DumpConfig.Default.ColorConfig.NullValueColor = "#FF0000";
DumpConfig.Default.MembersConfig.IncludeFields = true;
```

---

## Feature Categories

### Display Features

- **[Labels](./labels.md)** - Add context with descriptive headers
- **Table Formatting** - Configure borders, headers, alignment
- **Color Themes** - Customize colors for all value types
- **Type Names** - Control how type names are displayed

### Data Control

- **[Member Filtering](./member-filtering.md)** - Include/exclude properties, fields, private members
- **[Circular References](./circular-references.md)** - Safe handling of recursive structures
- **[Collections](./collections.md)** - Special handling for collections and dictionaries
- **Depth Limiting** - Prevent excessive nesting

### Extensibility

- **[Custom Type Handlers](./custom-type-handlers.md)** - Define how specific types are rendered
- **[Output Targets](./output-targets.md)** - Console, Debug, Trace, or custom outputs
- **Custom Renderers** - Implement your own rendering engine

---

## See Also

- [Getting Started](../getting-started.md)
- [Configuration Overview](../configuration/index.md)
- [API Reference](../api-reference/index.md)
- [Examples](../examples/index.md)
