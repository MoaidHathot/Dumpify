# MembersConfig

`MembersConfig` controls which members (properties and fields) are included when dumping objects.

---

## Table of Contents

- [Properties](#properties)
- [Examples](#examples)
- [Custom Member Filtering](#custom-member-filtering)

---

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IncludePublicMembers` | `bool` | `true` | Include public members |
| `IncludeNonPublicMembers` | `bool` | `false` | Include private, protected, internal members |
| `IncludeVirtualMembers` | `bool` | `true` | Include virtual members |
| `IncludeProperties` | `bool` | `true` | Include properties |
| `IncludeFields` | `bool` | `false` | Include fields |
| `MemberFilter` | `Func<IValueProvider, bool>?` | `null` | Custom filter function |

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
    MemberFilter = member => !member.Info.CustomAttributes
        .Any(a => a.AttributeType == typeof(JsonIgnoreAttribute)) 
});
```

### Filter by Name

Exclude members with specific names:

```csharp
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = member => !member.Info.Name.StartsWith("_")
});
```

### Filter by Type

Only include string properties:

```csharp
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = member => member.Type == typeof(string)
});
```

### Combine Multiple Filters

```csharp
obj.Dump(members: new MembersConfig 
{ 
    MemberFilter = member => 
        !member.Info.Name.StartsWith("_") &&
        !member.Info.CustomAttributes.Any(a => 
            a.AttributeType == typeof(JsonIgnoreAttribute))
});
```

### Global Custom Filter

```csharp
// Never show properties marked with [JsonIgnore]
DumpConfig.Default.MembersConfig.MemberFilter = member => 
    !member.Info.CustomAttributes
        .Any(a => a.AttributeType == typeof(JsonIgnoreAttribute));
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
    MemberFilter = member => !member.Info.Name.Contains("Password")
};

sensitiveObject.Dump(members: membersConfig);
```

---

## See Also

- [Member Filtering Feature](../features/member-filtering.md)
- [Configuration Overview](index.md)
- [Global Configuration](global-configuration.md)
