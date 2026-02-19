---
layout: default
title: MembersConfig
parent: Configuration
nav_order: 4
---

# MembersConfig

`MembersConfig` controls which members (properties and fields) are included when dumping objects.

---

## Table of Contents

- [Properties](#properties)
- [MemberFilterContext](#memberfiltercontext)
- [Examples](#examples)
- [Custom Member Filtering](#custom-member-filtering)
- [Value-Based Filtering](#value-based-filtering)
- [Depth-Based Filtering](#depth-based-filtering)

---

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IncludePublicMembers` | `bool` | `true` | Include public members |
| `IncludeNonPublicMembers` | `bool` | `false` | Include private, protected, internal members |
| `IncludeVirtualMembers` | `bool` | `true` | Include virtual members |
| `IncludeProperties` | `bool` | `true` | Include properties |
| `IncludeFields` | `bool` | `false` | Include fields |
| `MemberFilter` | `Func<MemberFilterContext, bool>?` | `null` | Custom filter function |

---

## MemberFilterContext

The `MemberFilterContext` struct is passed to your filter function and provides rich context for filtering decisions:

| Property | Type | Description |
|----------|------|-------------|
| `Member` | `IValueProvider` | The member metadata (Name, Info, MemberType) |
| `Value` | `object?` | The actual value of the member (lazy-evaluated) |
| `Source` | `object` | The parent object containing this member |
| `Depth` | `int` | Current rendering depth (0 = root level) |

This enables powerful filtering scenarios including value-based filtering, depth-aware filtering, and conditional display based on parent object state.

---

## Examples

### Include Private Members

```csharp
obj.Dump(members: new MembersConfig { IncludeNonPublicMembers = true });
```

### Include Fields

By default, only properties are shown. To include fields:

```csharp
public class MyClass
{
    private readonly int _value;
    public int Property { get; set; }
    
    public MyClass(int value)
    {
        _value = value;
    }
}

var obj = new MyClass(42);
obj.Dump(members: new MembersConfig { IncludeFields = true });
```

### Include Fields and Private Members

```csharp
public class AdditionValue
{
    private readonly int _a;
    private readonly int _b;

    public AdditionValue(int a, int b)
    {
        _a = a;
        _b = b;
    }

    private int Value => _a + _b;
}

new AdditionValue(1, 2).Dump(members: new MembersConfig 
{ 
    IncludeFields = true, 
    IncludeNonPublicMembers = true 
});
```

![Private members and fields](https://user-images.githubusercontent.com/8770486/232252840-c5b0ea4c-eae9-4dc2-bd6c-d42ee58505eb.png)

### Exclude Virtual Members

```csharp
obj.Dump(members: new MembersConfig { IncludeVirtualMembers = false });
```

### Only Show Fields (No Properties)

```csharp
obj.Dump(members: new MembersConfig 
{ 
    IncludeProperties = false, 
    IncludeFields = true 
});
```

### Global Member Configuration

```csharp
// Set globally
DumpConfig.Default.MembersConfig.IncludeFields = true;
DumpConfig.Default.MembersConfig.IncludeNonPublicMembers = true;

// All dumps now include fields and private members
myObject.Dump();
```

---

## Custom Member Filtering

The `MemberFilter` property allows you to provide a custom function to determine which members to include.

### Filter by Attribute

Exclude members with a specific attribute (e.g., `[JsonIgnore]`):

```csharp
using System.Text.Json.Serialization;

public class Person
{
    public string Name { get; set; }
    
    [JsonIgnore]
    public string SensitiveData { get; set; }
}

var person = new Person 
{ 
    Name = "John", 
    SensitiveData = "Secret" 
};

person.Dump(members: new MembersConfig 
{ 
    MemberFilter = ctx => !ctx.Member.Info.CustomAttributes
        .Any(a => a.AttributeType == typeof(JsonIgnoreAttribute)) 
});
```

### Filter by Name

Exclude members with specific names:

```csharp
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = ctx => !ctx.Member.Info.Name.StartsWith("_")
});
```

### Filter by Type

Only include string properties:

```csharp
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = ctx => ctx.Member.MemberType == typeof(string)
});
```

### Combine Multiple Filters

```csharp
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = ctx => 
        !ctx.Member.Info.Name.StartsWith("_") &&
        !ctx.Member.Info.CustomAttributes.Any(a => 
            a.AttributeType == typeof(JsonIgnoreAttribute))
});
```

### Global Custom Filter

```csharp
// Never show properties marked with [JsonIgnore]
DumpConfig.Default.MembersConfig.MemberFilter = ctx => 
    !ctx.Member.Info.CustomAttributes
        .Any(a => a.AttributeType == typeof(JsonIgnoreAttribute));
```

---

## Value-Based Filtering

Filter members based on their actual runtime values:

### Hide Null Values

```csharp
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = ctx => ctx.Value is not null
});
```

### Hide Empty Collections

```csharp
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = ctx => ctx.Value is not ICollection { Count: 0 }
});
```

### Hide Default Values

```csharp
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = ctx =>
    {
        var value = ctx.Value;
        if (value is null) return false;
        if (value is 0 or 0L or 0.0 or 0.0f) return false;
        if (value is string s && string.IsNullOrEmpty(s)) return false;
        return true;
    }
});
```

### Conditional Display Based on Sibling Values

```csharp
public class Order
{
    public string Status { get; set; }
    public string InternalNotes { get; set; }
}

// Only show InternalNotes when Status is "Review"
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = ctx =>
    {
        if (ctx.Member.Name == "InternalNotes" && ctx.Source is Order order)
            return order.Status == "Review";
        return true;
    }
});
```

---

## Depth-Based Filtering

Control member visibility based on nesting depth:

### Show Less Detail at Nested Levels

```csharp
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = ctx =>
    {
        // At root (depth 0), show everything
        if (ctx.Depth == 0) return true;
        
        // At nested levels, only show key identifiers
        return ctx.Member.Name is "Id" or "Name";
    }
});
```

### Hide Internal Properties When Nested

```csharp
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = ctx =>
    {
        // Hide "Internal" suffix properties when not at root
        if (ctx.Depth > 0 && ctx.Member.Name.EndsWith("Internal"))
            return false;
        return true;
    }
});
```

---

## Complete Example

```csharp
var membersConfig = new MembersConfig
{
    IncludePublicMembers = true,
    IncludeNonPublicMembers = true,
    IncludeVirtualMembers = true,
    IncludeProperties = true,
    IncludeFields = true,
    MemberFilter = ctx => 
    {
        // Exclude password fields
        if (ctx.Member.Info.Name.Contains("Password")) return false;
        
        // Exclude null values
        if (ctx.Value is null) return false;
        
        // At nested levels, only show essential properties
        if (ctx.Depth > 1)
            return ctx.Member.Name is "Id" or "Name" or "Title";
        
        return true;
    }
};

sensitiveObject.Dump(members: membersConfig);
```

---

## See Also

- [Member Filtering Feature](../features/member-filtering.md)
- [Configuration Overview](index.md)
- [Global Configuration](global-configuration.md)
