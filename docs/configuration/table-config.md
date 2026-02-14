---
layout: default
title: TableConfig
parent: Configuration
nav_order: 3
---

# TableConfig

`TableConfig` controls how tables are displayed when rendering objects, including headers, separators, indices, and collection truncation.

---

## Table of Contents

- [Properties](#properties)
- [Examples](#examples)

---

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowArrayIndices` | `bool` | `true` | Show index numbers for array elements |
| `ShowTableHeaders` | `bool` | `true` | Show column headers in tables |
| `NoColumnWrapping` | `bool` | `false` | Prevent text wrapping in columns |
| `ExpandTables` | `bool` | `false` | Expand tables to full width |
| `ShowMemberTypes` | `bool` | `false` | Show a column with member types |
| `ShowRowSeparators` | `bool` | `false` | Show separator lines between rows |
| `MaxCollectionCount` | `int` | `int.MaxValue` | Maximum number of collection items to display |

---

## Examples

### Show Row Separators

Add horizontal lines between rows for better readability:

```csharp
obj.Dump(tableConfig: new TableConfig { ShowRowSeparators = true });
```

### Show Member Types

Display a column showing the type of each member:

```csharp
obj.Dump(tableConfig: new TableConfig { ShowMemberTypes = true });
```

### Combined: Row Separators and Member Types

```csharp
obj.Dump(tableConfig: new TableConfig 
{ 
    ShowRowSeparators = true, 
    ShowMemberTypes = true 
});
```

![Row separators and member types](https://raw.githubusercontent.com/MoaidHathot/Dumpify/main/assets/screenshots/row-separator.png)

### Hide Table Headers

```csharp
obj.Dump(tableConfig: new TableConfig { ShowTableHeaders = false });
```

### Hide Array Indices

```csharp
var arr = new[] { "a", "b", "c" };
arr.Dump(tableConfig: new TableConfig { ShowArrayIndices = false });
```

### Truncate Large Collections

Limit the number of items displayed from large collections:

```csharp
var largeArray = Enumerable.Range(1, 1000).ToArray();

// Only show first 10 items
largeArray.Dump(tableConfig: new TableConfig { MaxCollectionCount = 10 });
// Output includes: "... truncated 990 items"
```

### Expand Tables

Make tables expand to fill available width:

```csharp
obj.Dump(tableConfig: new TableConfig { ExpandTables = true });
```

### Prevent Column Wrapping

Keep text on single lines (may cause horizontal scrolling):

```csharp
obj.Dump(tableConfig: new TableConfig { NoColumnWrapping = true });
```

### Global Table Configuration

```csharp
// Set globally
DumpConfig.Default.TableConfig.ShowRowSeparators = true;
DumpConfig.Default.TableConfig.ShowMemberTypes = true;
DumpConfig.Default.TableConfig.MaxCollectionCount = 100;

// All dumps now use these settings
myObject.Dump();
```

### Complete Table Configuration

```csharp
var tableConfig = new TableConfig
{
    ShowArrayIndices = true,
    ShowTableHeaders = true,
    ShowRowSeparators = true,
    ShowMemberTypes = true,
    MaxCollectionCount = 50,
    ExpandTables = false,
    NoColumnWrapping = false
};

obj.Dump(tableConfig: tableConfig);
```

---

## Use Cases

### Debugging Large Collections

When debugging large collections, use `MaxCollectionCount` to avoid overwhelming output:

```csharp
// Only show first 20 items from a large dataset
database.GetAllUsers()
    .Dump(tableConfig: new TableConfig { MaxCollectionCount = 20 });
```

### Detailed Object Inspection

For detailed inspection, enable all display options:

```csharp
obj.Dump(tableConfig: new TableConfig 
{ 
    ShowRowSeparators = true,
    ShowMemberTypes = true,
    ShowTableHeaders = true,
    ShowArrayIndices = true
});
```

### Compact Output

For minimal output, disable extra information:

```csharp
obj.Dump(tableConfig: new TableConfig 
{ 
    ShowTableHeaders = false,
    ShowArrayIndices = false,
    ShowMemberTypes = false
});
```

---

## See Also

- [Configuration Overview](index.md)
- [Global Configuration](global-configuration.md)
- [Collections Feature](../features/collections.md)
