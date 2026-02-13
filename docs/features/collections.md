# Collections

Dumpify provides intelligent rendering for all common .NET collection types, displaying them as organized, easy-to-read tables.

## Supported Collection Types

- Arrays (single and multi-dimensional)
- `List<T>`, `IList<T>`
- `Dictionary<TKey, TValue>`, `IDictionary<TKey, TValue>`
- `IEnumerable<T>`, `ICollection<T>`
- `HashSet<T>`, `Queue<T>`, `Stack<T>`
- `ImmutableArray<T>`, `ImmutableList<T>`
- And more...

---

## Arrays

### Simple Arrays

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
numbers.Dump();
```

### Object Arrays

```csharp
var people = new[]
{
    new { Name = "John", Age = 30 },
    new { Name = "Jane", Age = 25 }
};
people.Dump();
```

![Array Of Objects](https://raw.githubusercontent.com/MoaidHathworker/Dumpify/main/assets/2-arrayOfObjects.png)

### Multi-Dimensional Arrays

```csharp
var matrix = new int[,]
{
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 }
};
matrix.Dump("3x3 Matrix");
```

---

## Lists

```csharp
var list = new List<string> { "Apple", "Banana", "Cherry" };
list.Dump();

// With complex objects
var users = new List<User>
{
    new User { Id = 1, Name = "Alice" },
    new User { Id = 2, Name = "Bob" }
};
users.Dump("Users");
```

---

## Dictionaries

Dictionaries are rendered with Key and Value columns:

```csharp
var dict = new Dictionary<string, int>
{
    ["one"] = 1,
    ["two"] = 2,
    ["three"] = 3
};
dict.Dump("Number Dictionary");
```

### Complex Values

```csharp
var userMap = new Dictionary<int, User>
{
    [1] = new User { Name = "Alice", Email = "alice@example.com" },
    [2] = new User { Name = "Bob", Email = "bob@example.com" }
};
userMap.Dump();
```

### Nested Dictionaries

```csharp
var nested = new Dictionary<string, Dictionary<string, int>>
{
    ["Group1"] = new Dictionary<string, int> { ["A"] = 1, ["B"] = 2 },
    ["Group2"] = new Dictionary<string, int> { ["C"] = 3, ["D"] = 4 }
};
nested.Dump();
```

---

## Nested Collections

Collections within objects are rendered inline:

```csharp
var company = new
{
    Name = "Acme Corp",
    Departments = new[] { "Engineering", "Sales", "HR" },
    Employees = new List<Employee>
    {
        new Employee { Name = "John", Skills = new[] { "C#", "SQL" } },
        new Employee { Name = "Jane", Skills = new[] { "Python", "ML" } }
    }
};
company.Dump();
```

![Nested Objects](https://raw.githubusercontent.com/MoaidHathworker/Dumpify/main/assets/3-nestedObjects.png)

---

## Empty Collections

Empty collections are clearly indicated:

```csharp
var emptyList = new List<int>();
emptyList.Dump();  // Shows empty collection

var emptyDict = new Dictionary<string, string>();
emptyDict.Dump();  // Shows empty dictionary
```

---

## Null Collections

Null collections display the null indicator:

```csharp
List<int>? nullList = null;
nullList.Dump();  // Shows null
```

---

## Collection Properties

Objects with collection properties are fully supported:

```csharp
public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; }
    public Dictionary<string, decimal> Discounts { get; set; }
}

var order = new Order
{
    Id = 1001,
    Items = new List<OrderItem>
    {
        new OrderItem { Product = "Widget", Quantity = 2 },
        new OrderItem { Product = "Gadget", Quantity = 1 }
    },
    Discounts = new Dictionary<string, decimal>
    {
        ["SUMMER10"] = 0.10m,
        ["LOYALTY"] = 0.05m
    }
};
order.Dump();
```

---

## LINQ Results

Dump integrates seamlessly with LINQ:

```csharp
var result = users
    .Where(u => u.Age > 18)
    .Dump("Adults")
    .OrderBy(u => u.Name)
    .Dump("Sorted Adults")
    .ToList();
```

### IEnumerable Handling

LINQ queries return `IEnumerable<T>` which is lazily evaluated. Dumpify will enumerate the collection for display:

```csharp
// This enumerates the collection
users.Where(u => u.IsActive).Dump();

// Be careful with infinite sequences!
// GetInfiniteSequence().Dump();  // Don't do this!
```

---

## Tuples in Collections

```csharp
var pairs = new List<(string Name, int Value)>
{
    ("Alpha", 1),
    ("Beta", 2),
    ("Gamma", 3)
};
pairs.Dump();
```

---

## Collection Depth

For deeply nested collections, use `maxDepth`:

```csharp
var deepStructure = new List<List<List<int>>>
{
    new List<List<int>>
    {
        new List<int> { 1, 2, 3 }
    }
};

// Limit depth to prevent overwhelming output
deepStructure.Dump(maxDepth: 2);
```

---

## Performance Considerations

### Large Collections

For very large collections, consider limiting output:

```csharp
// Only dump first 100 items
largeList.Take(100).Dump("First 100 items");

// Or check count first
largeList.Count.Dump("Total count");
largeList.Take(10).Dump("Sample");
```

### Lazy Evaluation

Be aware that dumping an `IEnumerable` will enumerate it:

```csharp
IEnumerable<int> GetNumbers()
{
    Console.WriteLine("Generating...");
    yield return 1;
    yield return 2;
    yield return 3;
}

// This will trigger enumeration and print "Generating..."
GetNumbers().Dump();
```

---

## See Also

- [Basic Usage Examples](../examples/basic-usage.md)
- [Member Filtering](./member-filtering.md)
- [Features Overview](./index.md)
