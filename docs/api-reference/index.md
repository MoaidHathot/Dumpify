---
layout: default
title: API Reference
nav_order: 4
has_children: true
description: "Detailed API documentation for Dumpify"
permalink: /api-reference/
---

# API Reference

This section provides detailed documentation for all public APIs in Dumpify.

## Quick Links

| Reference | Description |
|-----------|-------------|
| [Extension Methods](./dump-extensions.md) | `Dump()`, `DumpConsole()`, `DumpDebug()`, `DumpTrace()`, `DumpText()` |
| [DumpConfig](./dump-config.md) | Global configuration singleton |
| [DumpColor](./dump-color.md) | Color representation for styling |
| [DumpOutput](./dump-output.md) | Default output implementation |

## Enums

| Reference | Description |
|-----------|-------------|
| [TableBorderStyle](./table-border-style.md) | Table border styles (Rounded, Ascii, etc.) |
| [TruncationMode](./truncation-mode.md) | Collection truncation modes (Head, Tail, HeadAndTail) |

## Interfaces

| Reference | Description |
|-----------|-------------|
| [IDumpOutput](./idump-output.md) | Interface for custom output targets |
| [IRenderer](./irenderer.md) | Interface for custom renderers |

## Structs

| Reference | Description |
|-----------|-------------|
| [MemberFilterContext](./member-filter-context.md) | Context for member filtering callbacks |

## Extension Methods Overview

Dumpify provides five main extension methods that can be called on any object:

```csharp
// Primary method - outputs to configured default (Console by default)
object.Dump();

// Output-specific methods
object.DumpConsole();  // Always outputs to Console
object.DumpDebug();    // Always outputs to Debug
object.DumpTrace();    // Always outputs to Trace

// Returns string instead of outputting
string text = object.DumpText();
```

All extension methods share the same parameter signature. See [Extension Methods](./dump-extensions.md) for complete details.

## Configuration Classes

Configuration is handled through several specialized classes:

| Class | Purpose | Documentation |
|-------|---------|---------------|
| `DumpConfig` | Global singleton configuration | [DumpConfig Reference](./dump-config.md) |
| `ColorConfig` | Color scheme settings | [Color Configuration](../configuration/color-config.md) |
| `TableConfig` | Table rendering options | [Table Configuration](../configuration/table-config.md) |
| `MembersConfig` | Member filtering rules | [Members Configuration](../configuration/members-config.md) |
| `TypeNamingConfig` | Type name display options | [Type Naming Configuration](../configuration/type-naming-config.md) |
| `TypeRenderingConfig` | Custom type rendering | [Type Rendering Configuration](../configuration/type-rendering-config.md) |
| `OutputConfig` | Output dimensions | [Output Configuration](../configuration/output-config.md) |

## Utility Classes

| Class | Purpose | Documentation |
|-------|---------|---------------|
| `DumpColor` | Color representation | [DumpColor Reference](./dump-color.md) |
| `DumpOutput` | Default output implementation | [DumpOutput Reference](./dump-output.md) |

## Extensibility Interfaces

Dumpify exposes several interfaces for extensibility. See the detailed documentation for each:

- [IDumpOutput](./idump-output.md) - For creating custom output targets
- [IRenderer](./irenderer.md) - For creating custom renderers

```csharp
// Output targets
public interface IDumpOutput
{
    TextWriter TextWriter { get; }
    RendererConfig AdjustConfig(in RendererConfig config);
}

// Renderers
public interface IRenderer
{
    IRenderedObject Render(object? obj, IDescriptor? descriptor, RendererConfig config);
}

// Rendered output
public interface IRenderedObject
{
    void Output(IDumpOutput output, OutputConfig config);
}
```

## Static Accessors

### Outputs

Access built-in output targets:

```csharp
Dumpify.Outputs.Console  // Console output (default)
Dumpify.Outputs.Debug    // System.Diagnostics.Debug output
Dumpify.Outputs.Trace    // System.Diagnostics.Trace output
```

### Renderers

Access built-in renderers:

```csharp
Dumpify.Renderers.Table  // Table renderer (default)
```

## Method Chaining

All `Dump` methods (except `DumpText`) return the original object, enabling method chaining:

```csharp
var result = GetData()
    .Dump("Raw data")
    .Where(x => x.IsActive)
    .Dump("Filtered")
    .OrderBy(x => x.Name)
    .Dump("Sorted")
    .ToList();
```

## Null Safety

All extension methods handle null values gracefully:

```csharp
string? nullString = null;
nullString.Dump();  // Outputs: null
```

Methods use `[NotNullIfNotNull]` attribute to preserve nullability in return types.

---

## See Also

- [Getting Started](../getting-started.md)
- [Configuration Overview](../configuration/index.md)
- [Features](../features/index.md)
