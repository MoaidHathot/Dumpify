---
layout: default
title: IRenderer
parent: API Reference
nav_order: 8
---

# IRenderer Interface

Defines a renderer that transforms objects into renderable output.

## Definition

```csharp
namespace Dumpify;

public interface IRenderer
{
    IRenderedObject Render(object? obj, IDescriptor? descriptor, RendererConfig config);
}
```

---

## Members

| Member | Type | Description |
|--------|------|-------------|
| `Render` | Method | Transforms an object into an `IRenderedObject` that can be output. |

### Render Method Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `obj` | `object?` | The object to render. Can be null. |
| `descriptor` | `IDescriptor?` | Type descriptor providing metadata about the object. |
| `config` | `RendererConfig` | Configuration controlling rendering behavior. |

### Returns

`IRenderedObject` - An object that can be output to an `IDumpOutput`.

---

## Built-in Renderers

Dumpify provides a built-in table renderer via the `Renderers` static class:

```csharp
// Table renderer (default) - uses Spectre.Console
Dumpify.Renderers.Table
```

---

## IRenderedObject Interface

The `Render` method returns an `IRenderedObject`:

```csharp
public interface IRenderedObject
{
    void Output(IDumpOutput output, OutputConfig config);
}
```

This allows the rendered representation to be output to different targets.

---

## Implementing Custom Renderers

You can create custom renderers by implementing `IRenderer`:

```csharp
public class JsonRenderer : IRenderer
{
    public IRenderedObject Render(object? obj, IDescriptor? descriptor, RendererConfig config)
    {
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        return new StringRenderedObject(json);
    }
}

// Simple IRenderedObject implementation
public class StringRenderedObject : IRenderedObject
{
    private readonly string _content;
    
    public StringRenderedObject(string content)
    {
        _content = content;
    }
    
    public void Output(IDumpOutput output, OutputConfig config)
    {
        output.TextWriter.WriteLine(_content);
    }
}
```

### Using Custom Renderer

```csharp
// Set as global default
DumpConfig.Default.Renderer = new JsonRenderer();

// Or use per-call
myObject.Dump(renderer: new JsonRenderer());
```

---

## RendererConfig

The `RendererConfig` record contains all configuration needed for rendering:

```csharp
public record RendererConfig
{
    public string? Label { get; init; }
    public int MaxDepth { get; init; }
    public ColorConfig? ColorConfig { get; init; }
    public TableConfig? TableConfig { get; init; }
    public TypeNamingConfig? TypeNamingConfig { get; init; }
    public TypeRenderingConfig? TypeRenderingConfig { get; init; }
    public TruncationConfig? TruncationConfig { get; init; }
    public IMemberProvider? MemberProvider { get; init; }
    public ITypeNameProvider? TypeNameProvider { get; init; }
    public Func<MemberFilterContext, bool>? MemberFilter { get; init; }
}
```

---

## Advanced: Working with Descriptors

The `IDescriptor` parameter provides type metadata:

```csharp
public interface IDescriptor
{
    Type Type { get; }
    IValueProvider? ValueProvider { get; }
    string? Name { get; }
}
```

This can be used to:
- Determine the runtime type of the object
- Access property/field metadata
- Get the member name if rendering a property value

---

## See Also

- [IDumpOutput](./idump-output.md) - Interface for output targets
- [DumpConfig Reference](./dump-config.md) - Global configuration
- [MemberFilterContext](./member-filter-context.md) - Context for filtering members
