# Custom Type Handlers

Dumpify allows you to register custom handlers that control how specific types are rendered.

## Table of Contents

- [Overview](#overview)
- [Adding Custom Handlers](#adding-custom-handlers)
- [Handler Signature](#handler-signature)
- [Removing Handlers](#removing-handlers)
- [Common Use Cases](#common-use-cases)
- [Examples](#examples)

---

## Overview

Custom type handlers let you:

- Format types in a specific way
- Hide sensitive information
- Simplify complex types
- Add custom string representations

---

## Adding Custom Handlers

Use `DumpConfig.Default.AddCustomTypeHandler()`:

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(DateTime),
    (obj, type, valueProvider, memberProvider) =>
    {
        var dt = (DateTime)obj;
        return dt.ToString("yyyy-MM-dd HH:mm:ss");
    }
);

// Now DateTime objects display in custom format
DateTime.Now.Dump();  // "2024-01-15 14:30:45"
```

---

## Handler Signature

```csharp
Func<object, Type, IValueProvider?, IMemberProvider, object?>
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `obj` | `object` | The value being rendered |
| `type` | `Type` | The runtime type of the object |
| `valueProvider` | `IValueProvider?` | Information about the property/field |
| `memberProvider` | `IMemberProvider` | Member discovery service |

**Returns:** The value to display (typically a string, but can be any object)

---

## Removing Handlers

```csharp
// Remove a specific handler
DumpConfig.Default.RemoveCustomTypeHandler(typeof(DateTime));
```

---

## Common Use Cases

### Format DateTime

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(DateTime),
    (obj, type, vp, mp) => ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss")
);
```

### Format TimeSpan

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(TimeSpan),
    (obj, type, vp, mp) =>
    {
        var ts = (TimeSpan)obj;
        if (ts.TotalDays >= 1) return $"{ts.TotalDays:F1} days";
        if (ts.TotalHours >= 1) return $"{ts.TotalHours:F1} hours";
        if (ts.TotalMinutes >= 1) return $"{ts.TotalMinutes:F1} minutes";
        return $"{ts.TotalSeconds:F1} seconds";
    }
);
```

### Hide Sensitive Data

```csharp
// Custom type for sensitive data
public class SensitiveString
{
    public string Value { get; set; }
}

DumpConfig.Default.AddCustomTypeHandler(
    typeof(SensitiveString),
    (obj, type, vp, mp) => "********"
);
```

### Simplify Complex Types

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(HttpClient),
    (obj, type, vp, mp) => "[HttpClient Instance]"
);

DumpConfig.Default.AddCustomTypeHandler(
    typeof(Stream),
    (obj, type, vp, mp) =>
    {
        var stream = (Stream)obj;
        return $"[Stream: {stream.Length} bytes, Position: {stream.Position}]";
    }
);
```

### Format GUIDs

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(Guid),
    (obj, type, vp, mp) => ((Guid)obj).ToString("N").Substring(0, 8) + "..."
);
```

### Format Byte Arrays

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(byte[]),
    (obj, type, vp, mp) =>
    {
        var bytes = (byte[])obj;
        if (bytes.Length <= 16)
            return BitConverter.ToString(bytes).Replace("-", " ");
        return $"[{bytes.Length} bytes]";
    }
);
```

---

## Examples

### Full DateTime Handler

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(DateTime),
    (obj, type, valueProvider, memberProvider) =>
    {
        var dt = (DateTime)obj;
        
        // Different format based on property name
        var propName = valueProvider?.Name;
        
        if (propName?.Contains("Date", StringComparison.OrdinalIgnoreCase) == true &&
            !propName.Contains("Time", StringComparison.OrdinalIgnoreCase))
        {
            return dt.ToString("yyyy-MM-dd");  // Date only
        }
        
        if (dt.TimeOfDay == TimeSpan.Zero)
        {
            return dt.ToString("yyyy-MM-dd");  // Midnight = date only
        }
        
        return dt.ToString("yyyy-MM-dd HH:mm:ss");
    }
);
```

### Money/Currency Handler

```csharp
public record Money(decimal Amount, string Currency);

DumpConfig.Default.AddCustomTypeHandler(
    typeof(Money),
    (obj, type, vp, mp) =>
    {
        var money = (Money)obj;
        return money.Currency switch
        {
            "USD" => $"${money.Amount:N2}",
            "EUR" => $"{money.Amount:N2}",
            "GBP" => $"{money.Amount:N2}",
            _ => $"{money.Amount:N2} {money.Currency}"
        };
    }
);
```

### Exception Handler

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(Exception),
    (obj, type, vp, mp) =>
    {
        var ex = (Exception)obj;
        return $"{ex.GetType().Name}: {ex.Message}";
    }
);
```

### Uri Handler

```csharp
DumpConfig.Default.AddCustomTypeHandler(
    typeof(Uri),
    (obj, type, vp, mp) =>
    {
        var uri = (Uri)obj;
        if (uri.IsAbsoluteUri)
            return $"{uri.Scheme}://{uri.Host}{uri.PathAndQuery}";
        return uri.ToString();
    }
);
```

### Lazy<T> Handler

```csharp
// Handle any Lazy<T> to avoid triggering evaluation
DumpConfig.Default.AddCustomTypeHandler(
    typeof(Lazy<>),
    (obj, type, vp, mp) =>
    {
        var isValueCreated = (bool)type.GetProperty("IsValueCreated")!.GetValue(obj)!;
        var innerType = type.GetGenericArguments()[0];
        
        if (isValueCreated)
        {
            var value = type.GetProperty("Value")!.GetValue(obj);
            return $"Lazy<{innerType.Name}>: {value}";
        }
        
        return $"Lazy<{innerType.Name}>: (not evaluated)";
    }
);
```

---

## TypeRenderingConfig Alternative

For simpler cases, you can also use `TypeRenderingConfig`:

```csharp
// Using TypeRenderingConfig for custom rendering
DumpConfig.Default.TypeRenderingConfig.CustomTypeRenderers[typeof(Guid)] = 
    guid => ((Guid)guid).ToString("N");
```

See [Type Rendering Configuration](../configuration/type-rendering-config.md) for details.

---

## Best Practices

1. **Return Strings** - Return string values for simple formatting
2. **Handle Nulls** - Check for null before casting
3. **Keep It Simple** - Don't do heavy computation in handlers
4. **Be Consistent** - Apply handlers early in application startup

```csharp
// Good practice: null-safe handler
DumpConfig.Default.AddCustomTypeHandler(
    typeof(DateTime?),
    (obj, type, vp, mp) =>
    {
        if (obj == null) return null;
        return ((DateTime)obj).ToString("yyyy-MM-dd");
    }
);
```

---

## See Also

- [Type Rendering Configuration](../configuration/type-rendering-config.md)
- [DumpConfig Reference](../api-reference/dump-config.md)
- [Member Filtering](./member-filtering.md)
- [Features Overview](./index.md)
