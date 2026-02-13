# Examples

Practical examples demonstrating Dumpify usage patterns.

## Table of Contents

- [Basic Usage](#basic-usage)
- [Dumping Different Types](#dumping-different-types)
- [Collections](#collections)
- [Configuration Examples](#configuration-examples)
- [Output Targets](#output-targets)
- [Real-World Scenarios](#real-world-scenarios)

## Basic Usage

### Simple Dump

```csharp
using Dumpify;

// Dump any object
var person = new Person { Name = "John", Age = 30 };
person.Dump();

// With a label
person.Dump("Current User");
```

### Method Chaining

Since `Dump()` returns the original value, chain it in expressions:

```csharp
var result = GetData()
    .Dump("Raw Data")
    .Where(x => x.IsActive)
    .Dump("Active Items")
    .OrderBy(x => x.Name)
    .Dump("Sorted")
    .ToList();
```

### Inline Debugging

```csharp
// Dump intermediate values
var total = orders.Dump("Orders").Sum(o => o.Amount).Dump("Total");

// Dump in conditionals
if (GetUser().Dump("User Check") is not null)
{
    // Process
}
```

## Dumping Different Types

### Primitives

```csharp
"Hello, World!".Dump("String");
42.Dump("Integer");
3.14159.Dump("Double");
true.Dump("Boolean");
DateTime.Now.Dump("DateTime");
Guid.NewGuid().Dump("Guid");
```

### Objects

```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Address? Address { get; set; }
}

new Person 
{ 
    Name = "Alice", 
    Age = 28,
    Address = new Address { City = "Seattle" }
}.Dump();
```

### Anonymous Types

```csharp
new { Name = "Test", Value = 42, Nested = new { Inner = "Data" } }.Dump();

// From LINQ
users.Select(u => new { u.Name, u.Email }).Dump("User Projection");
```

### Records and Structs

```csharp
public record Product(string Name, decimal Price);
new Product("Widget", 19.99m).Dump();

public struct Point { public int X; public int Y; }
new Point { X = 10, Y = 20 }.Dump();
```

### Enums

```csharp
public enum Status { Pending, Active, Completed }
Status.Active.Dump();

[Flags]
public enum Permissions { Read = 1, Write = 2, Execute = 4 }
(Permissions.Read | Permissions.Write).Dump();
```

### Tuples

```csharp
(1, "two", 3.0).Dump("Simple Tuple");
(Name: "Alice", Age: 30).Dump("Named Tuple");
```

### Nullable Values

```csharp
int? value = 42;
value.Dump("Has Value");

int? nullValue = null;
nullValue.Dump("Null Value");
```

## Collections

### Arrays and Lists

```csharp
new[] { 1, 2, 3, 4, 5 }.Dump("Array");
new List<string> { "A", "B", "C" }.Dump("List");

// List of objects
new List<Person>
{
    new() { Name = "Alice", Age = 28 },
    new() { Name = "Bob", Age = 35 }
}.Dump("People");
```

### Dictionaries

```csharp
new Dictionary<string, int>
{
    ["one"] = 1,
    ["two"] = 2,
    ["three"] = 3
}.Dump("Number Words");

new Dictionary<string, Person>
{
    ["manager"] = new Person { Name = "Alice" },
    ["developer"] = new Person { Name = "Bob" }
}.Dump("Team");
```

### Other Collections

```csharp
new HashSet<int> { 1, 2, 3 }.Dump("HashSet");

var queue = new Queue<string>();
queue.Enqueue("First");
queue.Enqueue("Second");
queue.Dump("Queue");

var stack = new Stack<int>();
stack.Push(1);
stack.Push(2);
stack.Dump("Stack");
```

## Configuration Examples

### Color Customization

```csharp
// Using System.Drawing.Color
data.Dump(config => config.UseColorConfig(c => c
    .SetLabelColor(Color.Yellow)
    .SetPropertyValueColor(Color.Cyan)));

// Using hex strings
data.Dump(config => config.UseColorConfig(c => c
    .SetLabelColor("#FFD700")
    .SetPropertyValueColor("#00CED1")));
```

### Table Configuration

```csharp
// Hide headers
data.Dump(config => config.UseTableConfig(t => t
    .ShowTableHeaders(false)));

// Expand tables
data.Dump(config => config.UseTableConfig(t => t
    .SetExpandTables(true)));

// Custom separators
data.Dump(config => config.UseTableConfig(t => t
    .SetColumnSeparator(" | ")));
```

### Member Filtering

```csharp
// Exclude fields
data.Dump(config => config.UseMembersConfig(m => m
    .IncludeFields(false)));

// Include private members
data.Dump(config => config.UseMembersConfig(m => m
    .IncludeNonPublicMembers(true)));

// Custom filter
data.Dump(config => config.UseMembersConfig(m => m
    .SetMemberFilter(member => member.Name != "Password")));
```

### Depth Control

```csharp
// Shallow dump
deepObject.Dump(config => config.SetMaxDepth(1));

// Deep inspection
deepObject.Dump(config => config.SetMaxDepth(10));
```

### Combined Configuration

```csharp
data.Dump("Configured Dump", config => config
    .SetMaxDepth(3)
    .UseColorConfig(c => c
        .SetLabelColor(Color.Gold)
        .SetPropertyValueColor(Color.White))
    .UseTableConfig(t => t
        .ShowTableHeaders(true)
        .SetExpandTables(false))
    .UseMembersConfig(m => m
        .IncludeFields(false)
        .SetMemberFilter(member => !member.Name.StartsWith("_"))));
```

### Reusable Configuration

```csharp
var debugConfig = new DumpConfig
{
    MaxDepth = 5,
    ColorConfig = new ColorConfig
    {
        LabelColor = new DumpColor("#FFD700"),
        PropertyValueColor = new DumpColor(Color.White)
    }
};

item1.Dump(debugConfig);
item2.Dump(debugConfig);
item3.Dump(debugConfig);
```

### Global Configuration

```csharp
// Set defaults for all dumps
DumpConfig.Default.MaxDepth = 4;
DumpConfig.Default.ColorConfig.PropertyValueColor = new DumpColor(Color.Aqua);

// Register custom type handler
DumpConfig.Default.TypeRenderingConfig.CustomTypeHandlers[typeof(SecureString)] = 
    (obj, type, config) => "****";
```

## Output Targets

### Multiple Targets

```csharp
data.Dump();           // Console (default)
data.DumpConsole();    // Console (explicit)
data.DumpDebug();      // Visual Studio Output
data.DumpTrace();      // Trace listeners
var text = data.DumpText();  // Get as string
```

### Logging Integration

```csharp
var text = data.DumpText();
_logger.LogInformation("Data: {Data}", text);
```

### Conditional Output

```csharp
#if DEBUG
data.Dump("Debug Mode Only");
#endif

if (verbose)
{
    details.DumpDebug("Verbose Info");
}
```

## Real-World Scenarios

### API Response Debugging

```csharp
public async Task<User> GetUserAsync(int id)
{
    var response = await _client.GetAsync($"/api/users/{id}");
    var user = await response.Content.ReadFromJsonAsync<User>();
    
    user.Dump($"User {id}");
    
    return user!;
}
```

### Entity Framework Debugging

```csharp
var orders = await _context.Orders
    .Where(o => o.Status == OrderStatus.Pending)
    .Include(o => o.Items)
    .ToListAsync();

orders.Dump("Pending Orders");
```

### LINQ Pipeline Debugging

```csharp
var result = products
    .Dump("All Products")
    .Where(p => p.InStock)
    .Dump("In Stock")
    .OrderBy(p => p.Price)
    .Dump("Sorted by Price")
    .Take(10)
    .Dump("Top 10")
    .ToList();
```

### Error Context Capture

```csharp
try
{
    return await ProcessAsync(request);
}
catch (Exception ex)
{
    new { Request = request, Error = ex.Message, Timestamp = DateTime.UtcNow }
        .Dump("Error Context");
    throw;
}
```

### Test Debugging

```csharp
[Fact]
public void Should_Calculate_Total()
{
    var order = CreateTestOrder();
    order.Dump("Test Input");
    
    var result = _calculator.Calculate(order);
    result.Dump("Result");
    
    Assert.Equal(100m, result.Total);
}
```

### Configuration Inspection

```csharp
// Startup.cs
#if DEBUG
configuration.AsEnumerable()
    .Where(kvp => !kvp.Key.Contains("Password"))
    .ToDictionary(k => k.Key, v => v.Value)
    .Dump("App Configuration");
#endif
```

## See Also

- [Configuration](configuration.md) - All configuration options
- [API Reference](api-reference.md) - Method documentation
- [Features](features.md) - Feature details
