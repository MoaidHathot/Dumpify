---
layout: default
title: Labels
parent: Features
nav_order: 4
---

# Labels

Labels add descriptive headers to your dump output, making it easier to identify what data you're viewing, especially when debugging multiple values.

## Basic Usage

Add a label by passing a string as the first parameter:

```csharp
myObject.Dump("Customer Data");
```

Output:
```
┌───────────────┐
│ Customer Data │
├───────┬───────┤
│ Name  │ John  │
│ Age   │ 30    │
└───────┴───────┘
```

---

## Label Parameter

All dump methods accept an optional `label` parameter:

```csharp
// Dump
myObject.Dump(label: "My Label");
myObject.Dump("My Label");  // Shorthand

// DumpConsole
myObject.DumpConsole("Console Output");

// DumpDebug
myObject.DumpDebug("Debug Data");

// DumpTrace
myObject.DumpTrace("Trace Info");

// DumpText
string text = myObject.DumpText("Text Output");
```

---

## Auto Labels

Enable automatic labels based on the calling expression:

```csharp
DumpConfig.Default.UseAutoLabels = true;

// Now the variable name becomes the label
customerData.Dump();        // Label: "customerData"
GetUser().Dump();           // Label: "GetUser()"
users.Where(x => x.Active).Dump();  // Label: "users.Where(x => x.Active)"
```

### How It Works

Auto labels use the `[CallerArgumentExpression]` attribute (C# 10+) to capture the expression being dumped.

### Disabling for Specific Calls

Even with auto labels enabled, you can override with an explicit label:

```csharp
DumpConfig.Default.UseAutoLabels = true;

// Auto label: "complexExpression"
complexExpression.Dump();

// Explicit label overrides
complexExpression.Dump("My Custom Label");

// Use null to suppress any label
complexExpression.Dump(label: null);
```

---

## Use Cases

### Debugging LINQ Pipelines

```csharp
var result = users
    .Dump("All Users")
    .Where(u => u.IsActive)
    .Dump("Active Users")
    .OrderBy(u => u.Name)
    .Dump("Sorted Users")
    .Take(10)
    .Dump("Top 10")
    .ToList();
```

### Comparing Before/After

```csharp
data.Dump("Before Processing");

ProcessData(data);

data.Dump("After Processing");
```

### Tracking Variables

```csharp
for (int i = 0; i < 3; i++)
{
    var item = GetItem(i);
    item.Dump($"Item {i}");
}
```

### Method Inputs/Outputs

```csharp
public Result ProcessOrder(Order order)
{
    order.Dump("Input Order");
    
    var result = DoProcessing(order);
    
    result.Dump("Processing Result");
    return result;
}
```

---

## Dynamic Labels

Labels can be computed at runtime:

```csharp
// Include timestamp
myObject.Dump($"Data at {DateTime.Now:HH:mm:ss}");

// Include context
myObject.Dump($"User {userId} Profile");

// Include count
users.Dump($"Users ({users.Count} total)");
```

---

## Labels with String Interpolation

```csharp
var customerId = 123;
var customer = GetCustomer(customerId);
customer.Dump($"Customer #{customerId}");
```

---

## No Label

To dump without any label:

```csharp
// No label parameter
myObject.Dump();

// Explicit null (useful when auto labels are enabled)
myObject.Dump(label: null);
```

---

## Multi-line Labels

While not common, labels can contain newlines:

```csharp
myObject.Dump("Customer Data\nRetrieved from Database");
```

---

## Labels in DumpText

Labels are included in the text output:

```csharp
string output = myObject.DumpText("API Response");
Console.WriteLine(output);

// Or write to file
File.WriteAllText("dump.txt", myObject.DumpText("Debug Output"));
```

---

## Best Practices

### Be Descriptive

```csharp
// Good: Describes what the data represents
orders.Dump("Pending Orders from Last 24 Hours");

// Less helpful
orders.Dump("Data");
```

### Include Context

```csharp
// Include relevant IDs or identifiers
order.Dump($"Order {order.Id}");
user.Dump($"User: {user.Email}");
```

### Use Auto Labels During Development

```csharp
// Enable at the start of your debug session
DumpConfig.Default.UseAutoLabels = true;

// Now you don't need to type labels manually
suspiciousVariable.Dump();  // Auto-labeled!
```

### Remove or Reduce Labels in Production

If using Dump for logging in production, consider whether labels add value or just noise.

---

## See Also

- [Extension Methods](../api-reference/dump-extensions.md) - Label parameter details
- [DumpConfig Reference](../api-reference/dump-config.md) - UseAutoLabels setting
- [Basic Usage Examples](../examples/basic-usage.md)
- [Features Overview](./index.md)
