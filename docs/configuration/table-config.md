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
| `BorderStyle` | `TableBorderStyle` | `Rounded` | Border style for tables (see below) |

### Border Styles

The `BorderStyle` property controls the characters used for table borders. This is useful for terminals that don't properly render Unicode box-drawing characters.

| Style | Description | Example Characters |
|-------|-------------|-------------------|
| `Rounded` | Rounded corners (default) | `╭─╮│╰─╯` |
| `Square` | Square corners | `┌─┐│└─┘` |
| `Ascii` | ASCII only (maximum compatibility) | `+-+\|+-+` |
| `None` | No borders | |
| `Heavy` | Heavy/bold lines | `┏━┓┃┗━┛` |
| `Double` | Double lines | `╔═╗║╚═╝` |
| `Minimal` | Minimal with horizontal lines | `─` |
| `Markdown` | Markdown-compatible format | `\|---\|` |

---

## Examples

### Change Border Style

Fix terminal rendering issues by changing the border style:

```csharp
// For terminals with Unicode issues (VS Code, some Windows Terminal fonts)
// Use ASCII for maximum compatibility
obj.Dump(tableConfig: new TableConfig { BorderStyle = TableBorderStyle.Ascii });

// Use Square instead of Rounded if only corners are broken
obj.Dump(tableConfig: new TableConfig { BorderStyle = TableBorderStyle.Square });
```

Set globally to fix all dumps:

```csharp
// Fix for entire application
DumpConfig.Default.TableConfig.BorderStyle = TableBorderStyle.Ascii;

// All dumps now use ASCII borders
myObject.Dump();
```

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
    NoColumnWrapping = false,
    BorderStyle = TableBorderStyle.Rounded
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

## Troubleshooting Terminal Display Issues

If table borders appear as garbled characters or question marks, your terminal or font may not support the Unicode box-drawing characters used by the default `Rounded` border style.

### VS Code Terminal

VS Code's integrated terminal may not render rounded corners correctly with some fonts:

```csharp
// Option 1: Use ASCII borders (maximum compatibility)
DumpConfig.Default.TableConfig.BorderStyle = TableBorderStyle.Ascii;

// Option 2: Use Square borders (works with most fonts)
DumpConfig.Default.TableConfig.BorderStyle = TableBorderStyle.Square;
```

### Windows Terminal

If corners display incorrectly but other borders are fine:

```csharp
// Square borders use simpler Unicode characters
DumpConfig.Default.TableConfig.BorderStyle = TableBorderStyle.Square;
```

### CI/CD Logs

For build logs and CI environments where rendering may vary:

```csharp
DumpConfig.Default.TableConfig.BorderStyle = TableBorderStyle.Ascii;
```

---

## See Also

- [Configuration Overview](index.md)
- [Global Configuration](global-configuration.md)
- [Collections Feature](../features/collections.md)
