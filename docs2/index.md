# Dumpify

**Add superpowers to your Console Applications** - Dump any object to your console with colorful, structured output.

![Dumpify Output](https://raw.githubusercontent.com/MoaidHathworkerNumber/Dumpify/main/assets/Dumpify-output.png)

Dumpify is a .NET library that adds `.Dump()` extension methods to any object, outputting beautifully formatted, colored tables to the console. Perfect for debugging, prototyping, and understanding your data.

## Installation

```bash
dotnet add package Dumpify
```

Or via Package Manager Console:
```powershell
Install-Package Dumpify
```

## Quick Start

Add the using directive:

```csharp
using Dumpify;
```

Then dump any object:

```csharp
var person = new Person { Name = "John", Age = 30 };
person.Dump();
```

That's it! You'll see a colorful, formatted table in your console.

## Core Features

### Dump Anything

```csharp
// Primitives
"Hello, World!".Dump();
42.Dump();

// Objects
new { Name = "Alice", Age = 25 }.Dump();

// Collections
new[] { 1, 2, 3 }.Dump();
new List<string> { "A", "B", "C" }.Dump();

// Dictionaries
new Dictionary<string, int> { ["one"] = 1, ["two"] = 2 }.Dump();

// Nested objects
complexObject.Dump();
```

### Add Labels

```csharp
data.Dump("My Data");
result.Dump($"Result for {id}");
```

### Chain in Expressions

Since `Dump()` returns the original value, chain it anywhere:

```csharp
var result = GetData()
    .Dump("Raw")
    .Where(x => x.Active)
    .Dump("Filtered")
    .OrderBy(x => x.Name)
    .ToList();
```

### Multiple Output Targets

```csharp
data.Dump();           // Console (default)
data.DumpConsole();    // Console (explicit)
data.DumpDebug();      // Debug output (Visual Studio Output window)
data.DumpTrace();      // Trace output

var text = data.DumpText();  // Get as string
```

## Documentation

- **[Configuration](configuration.md)** - All configuration options
- **[API Reference](api-reference.md)** - Extension methods and classes
- **[Features](features.md)** - Detailed feature documentation
- **[Examples](examples.md)** - Usage examples

## Supported Platforms

- .NET 9.0
- .NET Standard 2.1
- .NET Standard 2.0

## Dependencies

- [Spectre.Console](https://spectreconsole.net/) - For rendering colored tables

## Links

- [GitHub Repository](https://github.com/MoaidHathworkerNumber/Dumpify)
- [NuGet Package](https://www.nuget.org/packages/Dumpify)
