---
layout: default
title: Circular References
parent: Features
nav_order: 1
---

# Circular References

Dumpify safely handles objects with circular references, preventing infinite loops and stack overflows during rendering.

## The Problem

Consider a linked list or parent-child relationship where objects reference each other:

```csharp
public class Node
{
    public int Value { get; set; }
    public Node? Next { get; set; }
}

var node1 = new Node { Value = 1 };
var node2 = new Node { Value = 2 };
node1.Next = node2;
node2.Next = node1;  // Circular reference!
```

Without circular reference handling, attempting to serialize or display this object would cause an infinite loop.

## How Dumpify Handles It

Dumpify tracks visited objects during rendering. When it encounters an object it has already seen, it displays a reference indicator instead of recursing infinitely.

```csharp
var node = new Node { Value = 1 };
node.Next = node;  // Self-reference

node.Dump();
```

Output shows the circular reference is detected and safely handled.

## Common Circular Reference Scenarios

### Self-Reference

```csharp
public class Category
{
    public string Name { get; set; }
    public Category? Parent { get; set; }
}

var root = new Category { Name = "Root" };
root.Parent = root;  // Points to itself

root.Dump();  // Safe!
```

### Parent-Child Relationships

```csharp
public class TreeNode
{
    public string Name { get; set; }
    public TreeNode? Parent { get; set; }
    public List<TreeNode> Children { get; set; } = new();
}

var parent = new TreeNode { Name = "Parent" };
var child = new TreeNode { Name = "Child", Parent = parent };
parent.Children.Add(child);

parent.Dump();  // Safe!
```

### Linked Lists

```csharp
public class LinkedNode<T>
{
    public T Value { get; set; }
    public LinkedNode<T>? Next { get; set; }
    public LinkedNode<T>? Previous { get; set; }
}

var first = new LinkedNode<int> { Value = 1 };
var second = new LinkedNode<int> { Value = 2 };
first.Next = second;
second.Previous = first;

first.Dump();  // Safe!
```

### Graph Structures

```csharp
public class GraphNode
{
    public string Id { get; set; }
    public List<GraphNode> Connections { get; set; } = new();
}

var nodeA = new GraphNode { Id = "A" };
var nodeB = new GraphNode { Id = "B" };
var nodeC = new GraphNode { Id = "C" };

nodeA.Connections.Add(nodeB);
nodeB.Connections.Add(nodeC);
nodeC.Connections.Add(nodeA);  // Creates cycle

nodeA.Dump();  // Safe!
```

## Controlling Depth

While circular references are handled, you may want to limit depth to keep output manageable:

```csharp
// Limit to 3 levels
complexGraph.Dump(maxDepth: 3);

// Or set globally
DumpConfig.Default.MaxDepth = 3;
```

## Entity Framework Relationships

Entity Framework navigation properties often create circular references:

```csharp
public class Order
{
    public int Id { get; set; }
    public Customer Customer { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Order> Orders { get; set; }
}

// Customer.Orders[0].Customer points back to Customer
customer.Dump();  // Safe!
```

## Best Practices

### Use MaxDepth for Large Graphs

```csharp
// Limit depth for complex object graphs
DumpConfig.Default.MaxDepth = 5;
```

### Filter Navigation Properties

For EF entities, you might want to exclude navigation properties:

```csharp
DumpConfig.Default.MembersConfig.MemberFilter = member =>
{
    // Skip navigation properties
    if (member.Name == "Customer" || member.Name == "Orders")
        return false;
    return true;
};
```

### Use Labels to Identify Objects

```csharp
node1.Dump("First Node");
node2.Dump("Second Node");
```

## Technical Details

Dumpify uses object reference tracking (not value comparison) to detect cycles. This means:

- Two objects with identical values are NOT considered circular
- The same object instance referenced multiple times IS detected
- Value types cannot be circular (they're copied)

```csharp
// Not circular - different instances with same values
var a = new Point { X = 1, Y = 2 };
var b = new Point { X = 1, Y = 2 };
new[] { a, b }.Dump();  // Both rendered fully

// Circular - same instance
var c = new Node { Value = 1 };
new[] { c, c }.Dump();  // Second reference detected
```

---

## See Also

- [Member Filtering](./member-filtering.md) - Filter out problematic properties
- [DumpConfig Reference](../api-reference/dump-config.md) - MaxDepth setting
- [Features Overview](./index.md)
