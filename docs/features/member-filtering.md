---
layout: default
title: Member Filtering
parent: Features
nav_order: 5
---

# Member Filtering

Dumpify allows you to control which members (properties and fields) are displayed when dumping objects.

## Table of Contents

- [MembersConfig Properties](#membersconfig-properties)
- [Including Properties](#including-properties)
- [Including Fields](#including-fields)
- [Non-Public Members](#non-public-members)
- [Virtual Members](#virtual-members)
- [Custom Member Filters](#custom-member-filters)
- [Value-Based Filtering](#value-based-filtering)
- [Depth-Based Filtering](#depth-based-filtering)
- [Per-Call Configuration](#per-call-configuration)

---

## MembersConfig Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IncludeProperties` | `bool` | `true` | Include public properties |
| `IncludeFields` | `bool` | `false` | Include public fields |
| `IncludePublicMembers` | `bool` | `true` | Include public members |
| `IncludeNonPublicMembers` | `bool` | `false` | Include private/protected members |
| `IncludeVirtualMembers` | `bool` | `true` | Include virtual properties |
| `MemberFilter` | `Func<MemberFilterContext, bool>?` | `null` | Custom filter function |

---

## MemberFilterContext

The `MemberFilterContext` struct provides rich context for filtering decisions:

| Property | Type | Description |
|----------|------|-------------|
| `Member` | `IValueProvider` | The member metadata (Name, Info, MemberType) |
| `Value` | `object?` | The actual value of the member (lazy-evaluated) |
| `Source` | `object` | The parent object containing this member |
| `Depth` | `int` | Current rendering depth (0 = root level) |

---

## Including Properties

By default, only public properties are included:

```csharp
public class Person
{
    public string Name { get; set; }      // Included
    public int Age { get; set; }          // Included
    private string Secret { get; set; }   // Excluded
}

new Person { Name = "John", Age = 30 }.Dump();
```

---

## Including Fields

Fields are excluded by default. Enable them globally or per-call:

```csharp
public class Data
{
    public string Name;                  // Field, excluded by default
    public int Value { get; set; }       // Property, included
}

// Include fields globally
DumpConfig.Default.MembersConfig.IncludeFields = true;

// Or per-call
var membersConfig = new MembersConfig { IncludeFields = true };
data.Dump(members: membersConfig);
```

---

## Non-Public Members

Access private and protected members:

```csharp
public class SecureData
{
    public string PublicInfo { get; set; }
    private string PrivateInfo { get; set; } = "secret";
    protected int ProtectedValue { get; set; } = 42;
}

// Include non-public members
DumpConfig.Default.MembersConfig.IncludeNonPublicMembers = true;

var data = new SecureData { PublicInfo = "visible" };
data.Dump();  // Shows all three properties
```

### Common Use Cases

- **Debugging** - Inspect private state during development
- **Testing** - Verify internal object state
- **Troubleshooting** - Understand third-party library objects

---

## Virtual Members

Virtual properties are included by default. You can exclude them:

```csharp
public class BaseEntity
{
    public int Id { get; set; }
    public virtual string Type => GetType().Name;  // Virtual property
}

// Exclude virtual members
DumpConfig.Default.MembersConfig.IncludeVirtualMembers = false;
```

### Entity Framework Considerations

EF navigation properties are typically virtual. To exclude them:

```csharp
DumpConfig.Default.MembersConfig.IncludeVirtualMembers = false;

// Or use a custom filter for more control
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // Exclude if it's a navigation property (another entity type)
    return !typeof(IEnumerable).IsAssignableFrom(ctx.Member.MemberType) 
        || ctx.Member.MemberType == typeof(string);
};
```

---

## Custom Member Filters

Use `MemberFilter` for fine-grained control. The filter receives a `MemberFilterContext` which provides access to `Member` (with `Name`, `Info`, and `MemberType` properties), `Value`, `Source`, and `Depth`:

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // Exclude by name
    if (ctx.Member.Name == "Password")
    {
        return false;
    }
    
    if (ctx.Member.Name == "Secret")
    {
        return false;
    }
    
    return true;
};
```

### Filter by Attribute

```csharp
// Custom attribute
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class NoDumpAttribute : Attribute { }

public class User
{
    public string Username { get; set; }
    
    [NoDump]
    public string Password { get; set; }
}

// Filter out properties with [NoDump] attribute
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
    !ctx.Member.Info.GetCustomAttributes(typeof(NoDumpAttribute), true).Any();
```

### Filter by Type

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // Exclude Stream properties
    if (typeof(Stream).IsAssignableFrom(ctx.Member.MemberType))
    {
        return false;
    }
    
    // Exclude Task properties
    if (typeof(Task).IsAssignableFrom(ctx.Member.MemberType))
    {
        return false;
    }
    
    return true;
};
```

### Filter by Name Pattern

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // Exclude properties ending with "Internal"
    if (ctx.Member.Name.EndsWith("Internal"))
    {
        return false;
    }
    
    // Exclude properties starting with underscore
    if (ctx.Member.Name.StartsWith("_"))
    {
        return false;
    }
    
    return true;
};
```

---

## Value-Based Filtering

One of the most powerful features of `MemberFilterContext` is the ability to filter based on actual member values at render time:

### Hide Null Values

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // Exclude members with null values
    return ctx.Value is not null;
};
```

### Hide Default Values

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    var value = ctx.Value;
    
    // Exclude null
    if (value is null)
    {
        return false;
    }
    
    // Exclude default numeric values
    if (value is 0 or 0L or 0.0 or 0.0f or 0m)
    {
        return false;
    }
    
    // Exclude empty strings
    if (value is string s && string.IsNullOrEmpty(s))
    {
        return false;
    }
    
    // Exclude empty collections
    if (value is ICollection { Count: 0 })
    {
        return false;
    }
    
    return true;
};
```

### Hide Sensitive Values

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // Hide any property containing "password" in its value
    if (ctx.Value is string s && s.Contains("password", StringComparison.OrdinalIgnoreCase))
    {
        return false;
    }
    
    return true;
};
```

### Conditional Display Based on Value

```csharp
public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
    public string InternalNotes { get; set; }
}

// Only show InternalNotes if Status is "Review"
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    if (ctx.Member.Name == "InternalNotes")
    {
        // Access the parent object to check Status
        if (ctx.Source is Order order)
            return order.Status == "Review";
    }
    return true;
};
```

---

## Depth-Based Filtering

Control member visibility based on nesting depth:

### Limit Nested Object Details

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // At depth 0 (root), show everything
    if (ctx.Depth == 0)
    {
        return true;
    }
    
    // At deeper levels, only show key properties
    return ctx.Member.Name is "Id" or "Name" or "Title";
};
```

### Hide Internal Properties at Nested Levels

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // Hide "Internal" suffixed properties when nested
    if (ctx.Depth > 0 && ctx.Member.Name.EndsWith("Internal"))
    {
        return false;
    }
    
    return true;
};
```

### Progressive Detail Reduction

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // Depth 0: Show all
    // Depth 1: Hide metadata properties
    // Depth 2+: Only show essential properties
    
    return ctx.Depth switch
    {
        0 => true,
        1 => !ctx.Member.Name.StartsWith("Meta"),
        _ => ctx.Member.Name is "Id" or "Name"
    };
};
```

---

## Per-Call Configuration

Override member filtering for specific calls:

```csharp
// Show everything for debugging
var debugConfig = new MembersConfig
{
    IncludeFields = true,
    IncludeNonPublicMembers = true,
    IncludeVirtualMembers = true
};

problematicObject.Dump(members: debugConfig);
```

### Temporary Filter

```csharp
// Only show certain properties for this call
var limitedConfig = new MembersConfig
{
    MemberFilter = ctx => 
        ctx.Member.Name == "Id" || 
        ctx.Member.Name == "Name" ||
        ctx.Member.Name == "Status"
};

detailedObject.Dump(members: limitedConfig);
```

---

## Combining Filters

Filters work together with the boolean settings:

```csharp
var config = new MembersConfig
{
    IncludeFields = true,           // Include fields...
    IncludeNonPublicMembers = true, // Include private members...
    MemberFilter = ctx =>           // ...but only if they pass this filter
    {
        // Still exclude sensitive data
        return !ctx.Member.Name.Contains("Password") &&
               !ctx.Member.Name.Contains("Secret");
    }
};
```

---

## Examples

### Hide Sensitive Data

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    var sensitiveNames = new[] { "Password", "Token", "ApiKey", "Secret", "Credential" };
    return !sensitiveNames.Any(s => ctx.Member.Name.Contains(s, StringComparison.OrdinalIgnoreCase));
};
```

### Show Only Primitive Properties

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    var type = ctx.Member.MemberType;
    return type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
};
```

### Hide Null and Empty Values

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    var value = ctx.Value;
    
    if (value is null)
    {
        return false;
    }
    
    if (value is string s && string.IsNullOrWhiteSpace(s))
    {
        return false;
    }
    
    if (value is ICollection { Count: 0 })
    {
        return false;
    }
    
    return true;
};
```

### Debugging Entity Framework

```csharp
// Show everything except navigation properties
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // Keep primitives, strings, DateTimes, etc.
    var type = ctx.Member.MemberType;
    
    if (type.IsPrimitive || type == typeof(string) || 
        type == typeof(DateTime) || type == typeof(Guid))
    {
        return true;
    }
    
    // Exclude complex types and collections (likely navigation properties)
    return false;
};
```

### Depth-Aware Sensitive Data Hiding

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = ctx =>
{
    // At root level, hide passwords
    // At nested levels, also hide tokens and keys
    var sensitiveAtRoot = new[] { "Password" };
    var sensitiveNested = new[] { "Password", "Token", "ApiKey", "Secret" };
    
    var sensitiveNames = ctx.Depth == 0 ? sensitiveAtRoot : sensitiveNested;
    
    return !sensitiveNames.Any(s => 
        ctx.Member.Name.Contains(s, StringComparison.OrdinalIgnoreCase));
};
```

---

## See Also

- [Members Configuration](../configuration/members-config.md)
- [Custom Type Handlers](./custom-type-handlers.md)
- [Features Overview](./index.md)
