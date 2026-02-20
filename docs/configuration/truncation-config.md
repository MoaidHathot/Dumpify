---
layout: default
title: TruncationConfig
parent: Configuration
nav_order: 8
---

# TruncationConfig

`TruncationConfig` controls how large collections are truncated when rendering, allowing you to limit output size and choose which elements to display.

---

## Table of Contents

- [Properties](#properties)
- [TruncationMode](#truncationmode)
- [Examples](#examples)
- [Use Cases](#use-cases)

---

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MaxCollectionCount` | `int?` | `null` | Maximum number of collection items to display. `null` means no limit. |
| `Mode` | `TruncationMode` | `Head` | How to truncate: Head, Tail, or HeadAndTail |
| `PerDimension` | `bool` | `true` | Apply truncation per dimension for multi-dimensional arrays |

---

## TruncationMode

The `Mode` property controls which elements are shown when truncation occurs:

| Mode | Description | Example (MaxCollectionCount = 4) |
|------|-------------|----------------------------------|
| `Head` | Show first N elements | `1, 2, 3, 4, [... 6 more]` |
| `Tail` | Show last N elements | `[... 6 more], 7, 8, 9, 10` |
| `HeadAndTail` | Show first N/2 and last N/2 | `1, 2, [... 6 more], 9, 10` |

---

## Examples

### Basic Truncation

Limit the number of items displayed from large collections:

```csharp
var largeArray = Enumerable.Range(1, 100).ToArray();

// Only show first 10 items
largeArray.Dump(truncationConfig: new TruncationConfig { MaxCollectionCount = 10 });
// Output: 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, [... 90 more]
```

### Tail Truncation

Show the last N elements instead of the first:

```csharp
var numbers = Enumerable.Range(1, 20).ToArray();

numbers.Dump(truncationConfig: new TruncationConfig 
{ 
    MaxCollectionCount = 5, 
    Mode = TruncationMode.Tail 
});
// Output: [... 15 more], 16, 17, 18, 19, 20
```

### Head and Tail Truncation

Show elements from both ends with truncation in the middle:

```csharp
var numbers = Enumerable.Range(1, 20).ToArray();

numbers.Dump(truncationConfig: new TruncationConfig 
{ 
    MaxCollectionCount = 6, 
    Mode = TruncationMode.HeadAndTail 
});
// Output: 1, 2, 3, [... 14 more], 19, 20
```

### Multi-Dimensional Arrays

By default, truncation is applied per dimension:

```csharp
var matrix = new int[10, 10];
// Fill matrix...

// Truncate both rows and columns
matrix.Dump(truncationConfig: new TruncationConfig { MaxCollectionCount = 3 });
// Shows 3x3 grid with truncation markers for remaining rows/columns
```

To truncate only the total count (flattened):

```csharp
matrix.Dump(truncationConfig: new TruncationConfig 
{ 
    MaxCollectionCount = 10, 
    PerDimension = false 
});
```

### Global Configuration

Set truncation globally for all dumps:

```csharp
// Limit all collections to 50 items by default
DumpConfig.Default.TruncationConfig.MaxCollectionCount = 50;

// Use HeadAndTail mode globally
DumpConfig.Default.TruncationConfig.Mode = TruncationMode.HeadAndTail;

// All dumps now use these settings
myLargeCollection.Dump();
```

### Complete Truncation Configuration

```csharp
var truncationConfig = new TruncationConfig
{
    MaxCollectionCount = 20,
    Mode = TruncationMode.HeadAndTail,
    PerDimension = true
};

largeCollection.Dump(truncationConfig: truncationConfig);
```

---

## Use Cases

### Debugging Large Data Sets

When working with large collections, truncation prevents overwhelming output:

```csharp
// Database query results
var users = database.GetAllUsers(); // Returns 10,000 records

// Show just enough to verify the query
users.Dump(truncationConfig: new TruncationConfig { MaxCollectionCount = 10 });
```

### Log Files and CI Output

For log output where space is limited:

```csharp
// Compact truncation for logs
DumpConfig.Default.TruncationConfig.MaxCollectionCount = 5;
DumpConfig.Default.TruncationConfig.Mode = TruncationMode.HeadAndTail;
```

### API Response Inspection

When debugging API responses with large arrays:

```csharp
var response = await httpClient.GetFromJsonAsync<ApiResponse>(url);

// See start and end of data
response.Items.Dump(truncationConfig: new TruncationConfig 
{ 
    MaxCollectionCount = 10, 
    Mode = TruncationMode.HeadAndTail 
});
```

### Memory-Efficient Debugging

Truncation is memory-efficient - elements beyond the limit are not stored or processed:

```csharp
// Safe to dump large IEnumerables
GetMillionsOfItems()
    .Dump(truncationConfig: new TruncationConfig { MaxCollectionCount = 100 });
// Only first 100 items are enumerated
```

---

## Truncation Message Format

When truncation occurs, a marker is displayed showing how many items were truncated:

- **Default format**: `[... N more]` - e.g., `[... 990 more]`
- **Compact format**: `[... +N]` - Used in table headers for multi-dimensional arrays

---

## See Also

- [Configuration Overview](index.md)
- [Global Configuration](global-configuration.md)
- [TableConfig](table-config.md)
- [Collections Feature](../features/collections.md)
