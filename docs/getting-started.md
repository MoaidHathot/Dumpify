---
layout: default
title: Getting Started
nav_order: 2
description: "Quick introduction to using Dumpify in your .NET projects"
---

# Getting Started with Dumpify

This guide will help you get up and running with Dumpify in just a few minutes.

---

## Table of Contents

- [Installation](#installation)
- [Basic Usage](#basic-usage)
- [Dumping Different Types](#dumping-different-types)
- [Adding Labels](#adding-labels)
- [Controlling Depth](#controlling-depth)
- [Different Output Targets](#different-output-targets)
- [Next Steps](#next-steps)

---

## Installation

Install Dumpify via NuGet:

```bash
# .NET CLI
dotnet add package Dumpify

# Package Manager Console
Install-Package Dumpify
```

After installation, add the `using` directive to your code:

```csharp
using Dumpify;
```

That's it! The `.Dump()` extension method is now available on all objects.

---

## Basic Usage

The simplest way to use Dumpify is to call `.Dump()` on any object:

```csharp
using Dumpify;

// Dump a simple object
var person = new { Name = "John", Age = 30 };
person.Dump();
```

![Anonymous type dump](https://user-images.githubusercontent.com/8770486/232251633-5830bd48-0e45-4c89-9b26-3c678230a90a.png)

The `.Dump()` method returns the original object, so you can chain it in your code:

```csharp
var result = GetData()
    .Dump()  // Inspect the data
    .Where(x => x.IsActive)
    .Dump()  // Inspect filtered data
    .ToList();
```

---

## Dumping Different Types

### Classes and Records

```csharp
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}

var person = new Person 
{ 
    FirstName = "John", 
    LastName = "Doe", 
    Age = 30 
};
person.Dump();
```

### Arrays

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
numbers.Dump();
```

![Array dump](https://user-images.githubusercontent.com/8770486/232251833-ef2650fe-64a3-476d-b676-4a0f73339560.png)

### Multi-dimensional Arrays

```csharp
var matrix = new int[,] { { 1, 2 }, { 3, 4 } };
matrix.Dump();
```

![2D Array dump](https://user-images.githubusercontent.com/8770486/230250735-66703e54-ce02-41c0-91b7-fcbee5f80ac3.png)

### Dictionaries

```csharp
var dict = new Dictionary<string, string>
{
    ["Key1"] = "Value1",
    ["Key2"] = "Value2"
};
dict.Dump();
```

![Dictionary dump](https://user-images.githubusercontent.com/8770486/232251913-add4a0d8-3355-44f6-ba94-5dfbf8d8e2ac.png)

---

## Adding Labels

You can add a label to identify your dumps:

```csharp
// Manual label
person.Dump("My Person Object");

// Auto-labels (use variable name as label)
DumpConfig.Default.UseAutoLabels = true;
person.Dump(); // Label will be "person"
```

![Labels](https://raw.githubusercontent.com/MoaidHathot/Dumpify/main/assets/screenshots/custom-label-and-auto-labels.png)

---

## Controlling Depth

For deeply nested objects, you can control the maximum depth:

```csharp
// Limit to 2 levels of nesting
complexObject.Dump(maxDepth: 2);

// Or set globally
DumpConfig.Default.MaxDepth = 3;
```

---

## Different Output Targets

Dumpify supports multiple output targets:

```csharp
// Console (default)
obj.Dump();
obj.DumpConsole();

// Visual Studio Debug output
obj.DumpDebug();

// Trace output
obj.DumpTrace();

// Get as plain text string
string text = obj.DumpText();
```

---

## Next Steps

Now that you know the basics, explore more:

- [Configuration](configuration/index.md) - Customize colors, table layout, and more
- [Features](features/index.md) - Learn about all Dumpify features
- [Examples](examples/index.md) - See more code examples
- [API Reference](api-reference/index.md) - Complete API documentation

---

## Quick Reference

| Method | Description |
|--------|-------------|
| `.Dump()` | Dump to configured output (Console by default) |
| `.Dump("label")` | Dump with a custom label |
| `.Dump(maxDepth: n)` | Dump with limited nesting depth |
| `.DumpConsole()` | Dump explicitly to Console |
| `.DumpDebug()` | Dump to Visual Studio Debug output |
| `.DumpTrace()` | Dump to Trace output |
| `.DumpText()` | Get dump as a plain text string |
