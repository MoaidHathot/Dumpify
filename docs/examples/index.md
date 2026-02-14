---
layout: default
title: Examples
nav_order: 6
has_children: true
description: "Practical examples demonstrating Dumpify usage"
permalink: /examples/
---

# Examples

This section provides practical examples demonstrating how to use Dumpify in various scenarios.

![Dumpify Output](https://raw.githubusercontent.com/MoaidHathot/Dumpify/main/assets/Dumpify-output.png)

## Example Categories

### [Basic Usage](basic-usage.md)

Get started with simple examples covering the fundamentals:

- Dumping primitive types
- Dumping objects and classes
- Dumping collections and arrays
- Using labels for identification
- Basic formatting options

### [Advanced Usage](advanced-usage.md)

Explore more sophisticated scenarios:

- Custom configuration per-dump
- Color customization
- Table styling and formatting
- Member filtering and selection
- Custom type handlers
- Handling circular references
- Nested object structures

### [Real-World Scenarios](real-world-scenarios.md)

See how Dumpify fits into actual development workflows:

- Debugging API responses
- Inspecting Entity Framework entities
- Logging and diagnostics
- Unit test debugging
- Console application development

## Quick Examples

### Simple Object Dump

```csharp
var person = new Person 
{ 
    Name = "John", 
    Age = 30 
};

person.Dump();
```

### Collection with Label

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
numbers.Dump("My Numbers");
```

### Configured Dump

```csharp
var data = GetComplexData();

data.Dump(
    maxDepth: 3,
    tableConfig: new TableConfig { ShowTableHeaders = false },
    colors: new ColorConfig { PropertyValueColor = "#00FFFF" }
);
```

### Multiple Output Targets

```csharp
// Console output
result.DumpConsole();

// Debug output (Visual Studio Output window)
result.DumpDebug();

// Get as string
var text = result.DumpText();
```

## Running the Examples

All examples assume you have:

1. Installed the Dumpify NuGet package:
   ```bash
   dotnet add package Dumpify
   ```

2. Added the using directive:
   ```csharp
   using Dumpify;
   ```

## See Also

- [Getting Started](../getting-started.md) - Initial setup guide
- [Configuration](../configuration/index.md) - All configuration options
- [API Reference](../api-reference/index.md) - Complete method documentation
- [Features](../features/index.md) - Feature deep-dives
