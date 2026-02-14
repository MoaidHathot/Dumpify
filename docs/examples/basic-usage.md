---
layout: default
title: Basic Usage
parent: Examples
nav_order: 1
---

# Basic Usage Examples

This guide covers fundamental Dumpify usage patterns to get you productive quickly.

## Table of Contents

- [Dumping Primitive Types](#dumping-primitive-types)
- [Dumping Objects](#dumping-objects)
- [Dumping Collections](#dumping-collections)
- [Using Labels](#using-labels)
- [Chaining Dumps](#chaining-dumps)
- [Anonymous Types](#anonymous-types)
- [Nullable Values](#nullable-values)

## Dumping Primitive Types

Dumpify handles all primitive types seamlessly:

```csharp
using Dumpify;

// Strings
"Hello, World!".Dump();

// Numbers
42.Dump();
3.14159.Dump();
decimal.MaxValue.Dump();

// Booleans
true.Dump();

// Characters
'A'.Dump();

// DateTime
DateTime.Now.Dump();
DateOnly.FromDateTime(DateTime.Now).Dump();
TimeOnly.FromDateTime(DateTime.Now).Dump();

// Guid
Guid.NewGuid().Dump();
```

### With Labels

```csharp
"Hello, World!".Dump("Greeting");
42.Dump("The Answer");
DateTime.Now.Dump("Current Time");
```

## Dumping Objects

### Simple Classes

```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
}

var person = new Person
{
    Name = "Alice",
    Age = 28,
    Email = "alice@example.com"
};

person.Dump();
```

Output displays as a formatted table showing each property name and value.

### Nested Objects

```csharp
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}

public class Employee
{
    public string Name { get; set; }
    public Address HomeAddress { get; set; }
    public Address WorkAddress { get; set; }
}

var employee = new Employee
{
    Name = "Bob",
    HomeAddress = new Address 
    { 
        Street = "123 Home St", 
        City = "Hometown", 
        Country = "USA" 
    },
    WorkAddress = new Address 
    { 
        Street = "456 Office Ave", 
        City = "Workville", 
        Country = "USA" 
    }
};

employee.Dump("Employee Details");
```

### Records

```csharp
public record Product(string Name, decimal Price, int Stock);

var product = new Product("Widget", 19.99m, 100);
product.Dump();
```

### Structs

```csharp
public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

var point = new Point { X = 10, Y = 20 };
point.Dump("Coordinates");
```

## Dumping Collections

### Arrays

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
numbers.Dump("Numbers");

var names = new[] { "Alice", "Bob", "Charlie" };
names.Dump("Names");

// Multidimensional arrays
var matrix = new[,] 
{ 
    { 1, 2, 3 }, 
    { 4, 5, 6 }, 
    { 7, 8, 9 } 
};
matrix.Dump("3x3 Matrix");
```

### Lists

```csharp
var list = new List<string> { "Apple", "Banana", "Cherry" };
list.Dump("Fruits");

var personList = new List<Person>
{
    new Person { Name = "Alice", Age = 28, Email = "alice@example.com" },
    new Person { Name = "Bob", Age = 35, Email = "bob@example.com" }
};
personList.Dump("People");
```

### Dictionaries

```csharp
var dict = new Dictionary<string, int>
{
    ["one"] = 1,
    ["two"] = 2,
    ["three"] = 3
};
dict.Dump("Number Words");

var complexDict = new Dictionary<string, Person>
{
    ["manager"] = new Person { Name = "Alice", Age = 40 },
    ["developer"] = new Person { Name = "Bob", Age = 30 }
};
complexDict.Dump("Team");
```

### HashSets

```csharp
var set = new HashSet<int> { 1, 2, 3, 4, 5 };
set.Dump("Unique Numbers");
```

### Queues and Stacks

```csharp
var queue = new Queue<string>();
queue.Enqueue("First");
queue.Enqueue("Second");
queue.Enqueue("Third");
queue.Dump("Queue");

var stack = new Stack<int>();
stack.Push(1);
stack.Push(2);
stack.Push(3);
stack.Dump("Stack");
```

## Using Labels

Labels help identify output when dumping multiple values:

```csharp
var user = GetCurrentUser();
var permissions = GetUserPermissions(user.Id);
var settings = GetUserSettings(user.Id);

user.Dump("Current User");
permissions.Dump("User Permissions");
settings.Dump("User Settings");
```

### Dynamic Labels

```csharp
foreach (var item in items)
{
    item.Dump($"Item #{item.Id}");
}
```

### Labels with Method Chains

```csharp
GetData()
    .Dump("Raw Data")
    .Where(x => x.IsActive)
    .Dump("Active Items")
    .OrderBy(x => x.Name)
    .Dump("Sorted")
    .ToList();
```

## Chaining Dumps

Since `Dump()` returns the original value, you can chain it in expressions:

```csharp
// Dump intermediate results
var result = GetItems()
    .Dump("All Items")
    .Where(x => x.Price > 10)
    .Dump("Expensive Items")
    .Select(x => x.Name)
    .Dump("Names Only")
    .ToList();

// Use in calculations
var total = orders
    .Dump("Orders")
    .Sum(o => o.Amount)
    .Dump("Total");

// Use in conditionals
if (GetUser().Dump("User Check") is not null)
{
    // Process user
}
```

## Anonymous Types

```csharp
var anonymous = new 
{ 
    Name = "Test", 
    Value = 42, 
    Nested = new { Inner = "Data" } 
};
anonymous.Dump("Anonymous Object");

// From LINQ queries
var projection = users
    .Select(u => new { u.Name, u.Email, NameLength = u.Name.Length })
    .Dump("User Projection");
```

## Nullable Values

```csharp
int? nullableInt = 42;
nullableInt.Dump("Has Value");

int? nullInt = null;
nullInt.Dump("Null Value");

string? nullableString = null;
nullableString.Dump("Nullable String");

Person? nullPerson = null;
nullPerson.Dump("Null Person");
```

## Tuples

```csharp
// Value tuples
var tuple = (Name: "Alice", Age: 30);
tuple.Dump("Named Tuple");

var simpleTuple = (1, "two", 3.0);
simpleTuple.Dump("Simple Tuple");

// From methods
(string, int) GetNameAndAge() => ("Bob", 25);
GetNameAndAge().Dump("Method Result");
```

## Enums

```csharp
public enum Status
{
    Pending,
    Active,
    Completed,
    Cancelled
}

Status.Active.Dump("Current Status");

// Flags enum
[Flags]
public enum Permissions
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4
}

var perms = Permissions.Read | Permissions.Write;
perms.Dump("User Permissions");
```

## See Also

- [Advanced Usage](advanced-usage.md) - More complex scenarios
- [Configuration](../configuration/index.md) - Customizing output
- [Features](../features/index.md) - Deep-dive into features
- [API Reference](../api-reference/dump-extensions.md) - Method documentation
