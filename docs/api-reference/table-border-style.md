---
layout: default
title: TableBorderStyle
parent: API Reference
nav_order: 4
---

# TableBorderStyle Enum

Specifies the border style for tables rendered by Dumpify.

## Definition

```csharp
namespace Dumpify;

public enum TableBorderStyle
{
    Rounded,
    Square,
    Ascii,
    None,
    Heavy,
    Double,
    Minimal,
    Markdown
}
```

---

## Values

| Value | Description |
|-------|-------------|
| `Rounded` | Rounded corners using Unicode box-drawing characters (e.g., `+-+`). Best appearance but requires font/terminal support. |
| `Square` | Square corners using Unicode box-drawing characters (e.g., `+-+`). More widely supported than Rounded. |
| `Ascii` | ASCII-only characters (`+-+\|+-+`). Maximum compatibility with all terminals. |
| `None` | No border at all. |
| `Heavy` | Heavy/bold Unicode box-drawing characters. |
| `Double` | Double-line Unicode box-drawing characters. |
| `Minimal` | Minimal border style with horizontal lines only. |
| `Markdown` | Markdown-compatible table format (`\|---\|`). |

---

## Usage

### Setting Global Default

```csharp
DumpConfig.Default.TableConfig.BorderStyle = TableBorderStyle.Rounded;
```

### Per-Call Override

```csharp
var tableConfig = new TableConfig
{
    BorderStyle = TableBorderStyle.Ascii
};

myObject.Dump(tableConfig: tableConfig);
```

---

## Visual Examples

### Rounded (Default)

```
+-----------+---------+
| Name      | Value   |
+-----------+---------+
| FirstName | John    |
| LastName  | Doe     |
+-----------+---------+
```

### Ascii

```
+-----------+---------+
| Name      | Value   |
+-----------+---------+
| FirstName | John    |
| LastName  | Doe     |
+-----------+---------+
```

### None

```
Name        Value
FirstName   John
LastName    Doe
```

### Markdown

```
| Name      | Value   |
|-----------|---------|
| FirstName | John    |
| LastName  | Doe     |
```

---

## Terminal Compatibility

| Style | Unicode Required | Recommended For |
|-------|------------------|-----------------|
| `Rounded` | Yes | Modern terminals with Unicode support |
| `Square` | Yes | Terminals with basic Unicode support |
| `Ascii` | No | Legacy terminals, CI/CD logs, maximum compatibility |
| `None` | No | Minimal output, embedding in other text |
| `Heavy` | Yes | High-visibility output |
| `Double` | Yes | Formal/document-style output |
| `Minimal` | Yes | Clean, minimal appearance |
| `Markdown` | No | Documentation, GitHub issues, markdown files |

---

## See Also

- [Table Configuration](../configuration/table-config.md)
- [DumpConfig Reference](./dump-config.md)
- [Configuration Overview](../configuration/index.md)
