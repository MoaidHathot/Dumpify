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
| `MemberFilter` | `Func<IValueProvider, bool>?` | `null` | Custom filter function |

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
DumpConfig.Default.MembersConfig.MemberFilter = member =>
{
    // Exclude if it's a navigation property (another entity type)
    return !typeof(IEnumerable).IsAssignableFrom(member.MemberType) 
        || member.MemberType == typeof(string);
};
```

---

## Custom Member Filters

Use `MemberFilter` for fine-grained control. The filter receives an `IValueProvider` which has `Name`, `Info` (the `MemberInfo`), and `MemberType` properties:

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = member =>
{
    // Exclude by name
    if (member.Name == "Password") return false;
    if (member.Name == "Secret") return false;
    
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
DumpConfig.Default.MembersConfig.MemberFilter = member =>
    !member.Info.GetCustomAttributes(typeof(NoDumpAttribute), true).Any();
```

### Filter by Type

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = member =>
{
    // Exclude Stream properties
    if (typeof(Stream).IsAssignableFrom(member.MemberType))
        return false;
    
    // Exclude Task properties
    if (typeof(Task).IsAssignableFrom(member.MemberType))
        return false;
    
    return true;
};
```

### Filter by Name Pattern

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = member =>
{
    // Exclude properties ending with "Internal"
    if (member.Name.EndsWith("Internal")) return false;
    
    // Exclude properties starting with underscore
    if (member.Name.StartsWith("_")) return false;
    
    return true;
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
    MemberFilter = member => 
        member.Name == "Id" || 
        member.Name == "Name" ||
        member.Name == "Status"
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
    MemberFilter = member =>        // ...but only if they pass this filter
    {
        // Still exclude sensitive data
        return !member.Name.Contains("Password") &&
               !member.Name.Contains("Secret");
    }
};
```

---

## Examples

### Hide Sensitive Data

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = member =>
{
    var sensitiveNames = new[] { "Password", "Token", "ApiKey", "Secret", "Credential" };
    return !sensitiveNames.Any(s => member.Name.Contains(s, StringComparison.OrdinalIgnoreCase));
};
```

### Show Only Primitive Properties

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = member =>
{
    var type = member.MemberType;
    return type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
};
```

### Debugging Entity Framework

```csharp
// Show everything except navigation properties
DumpConfig.Default.MembersConfig.MemberFilter = member =>
{
    // Keep primitives, strings, DateTimes, etc.
    var type = member.MemberType;
    if (type.IsPrimitive || type == typeof(string) || 
        type == typeof(DateTime) || type == typeof(Guid))
        return true;
    
    // Exclude complex types and collections (likely navigation properties)
    return false;
};
```

---

## See Also

- [Members Configuration](../configuration/members-config.md)
- [Custom Type Handlers](./custom-type-handlers.md)
- [Features Overview](./index.md)
