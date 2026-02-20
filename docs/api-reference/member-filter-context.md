---
layout: default
title: MemberFilterContext
parent: API Reference
nav_order: 10
---

# MemberFilterContext Struct

Provides context for member filtering during rendering.

## Definition

```csharp
namespace Dumpify;

public readonly struct MemberFilterContext
{
    public IValueProvider Member { get; }
    public object? Value { get; }
    public object Source { get; }
    public int Depth { get; }
}
```

---

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Member` | `IValueProvider` | The member (property/field) being filtered. Provides access to member metadata. |
| `Value` | `object?` | The actual value of the member. Lazily evaluated when accessed. |
| `Source` | `object` | The parent object that contains this member. |
| `Depth` | `int` | The current rendering depth (0 = root object). |

---

## Usage

`MemberFilterContext` is passed to member filter functions configured via `MembersConfig.MemberFilter` or the `memberFilter` parameter on `Dump` methods.

### Basic Filtering

```csharp
// Exclude null values
DumpConfig.Default.MembersConfig.MemberFilter = ctx => ctx.Value is not null;

// Exclude specific property names
DumpConfig.Default.MembersConfig.MemberFilter = ctx => 
    ctx.Member.Name != "Password" && 
    ctx.Member.Name != "Secret";
```

### Filtering by Depth

```csharp
// Only show top-level properties
DumpConfig.Default.MembersConfig.MemberFilter = ctx => ctx.Depth == 0;

// Show details only for first 2 levels
DumpConfig.Default.MembersConfig.MemberFilter = ctx => ctx.Depth < 2;
```

### Filtering by Type

```csharp
// Exclude byte arrays (e.g., large binary data)
DumpConfig.Default.MembersConfig.MemberFilter = ctx => 
    ctx.Member.MemberType != typeof(byte[]);

// Only show string and numeric properties
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    var type = ctx.Member.MemberType;
    return type == typeof(string) || 
           type.IsPrimitive || 
           type == typeof(decimal);
};
```

### Per-Call Filtering

```csharp
myObject.Dump(memberFilter: ctx => ctx.Value is not null);
```

---

## IValueProvider Interface

The `Member` property exposes an `IValueProvider` with useful metadata:

```csharp
public interface IValueProvider
{
    string Name { get; }           // Property or field name
    MemberInfo Info { get; }       // Reflection metadata
    Type MemberType { get; }       // The type of the member
    object? GetValue(object source);
}
```

### Accessing Member Metadata

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // Check for custom attributes
    var hasIgnoreAttr = ctx.Member.Info
        .GetCustomAttribute<JsonIgnoreAttribute>() != null;
    
    return !hasIgnoreAttr;
};
```

---

## Performance Considerations

The `Value` property is lazily evaluated - it only calls `GetValue()` when accessed. This means:

```csharp
// Good: Only evaluates Value when needed
ctx => ctx.Member.Name != "ExpensiveProperty" && ctx.Value != null

// Better for performance if filtering by name is common
ctx => ctx.Member.Name == "ExpensiveProperty" || ctx.Value != null
```

If your filter doesn't need the actual value, avoid accessing `ctx.Value` for better performance.

---

## Examples

### Hide Sensitive Data

```csharp
var sensitiveNames = new HashSet<string> 
{ 
    "Password", "Secret", "ApiKey", "Token", "ConnectionString" 
};

DumpConfig.Default.MembersConfig.MemberFilter = ctx => 
    !sensitiveNames.Contains(ctx.Member.Name);
```

### Show Only Changed Values

```csharp
// Assuming you have a way to determine default values
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    var value = ctx.Value;
    if (value is null) return false;
    if (value is string s && string.IsNullOrEmpty(s)) return false;
    if (value is 0 or 0L or 0.0 or 0.0f) return false;
    return true;
};
```

### Depth-Based Detail Levels

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // Always show top level
    if (ctx.Depth == 0) return true;
    
    // At deeper levels, hide collections and complex objects
    if (ctx.Depth > 1 && ctx.Value is IEnumerable && ctx.Value is not string)
        return false;
    
    return true;
};
```

---

## See Also

- [Members Configuration](../configuration/members-config.md)
- [DumpConfig Reference](./dump-config.md)
- [Extension Methods](./dump-extensions.md)
