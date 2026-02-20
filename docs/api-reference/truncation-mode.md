---
layout: default
title: TruncationMode
parent: API Reference
nav_order: 5
---

# TruncationMode Enum

Specifies how collections should be truncated when they exceed `MaxCollectionCount`.

## Definition

```csharp
namespace Dumpify;

public enum TruncationMode
{
    Head,
    Tail,
    HeadAndTail
}
```

---

## Values

| Value | Description |
|-------|-------------|
| `Head` | Show the first N elements, truncate the rest at the end. |
| `Tail` | Show the last N elements, truncate the beginning. |
| `HeadAndTail` | Show first N/2 elements and last N/2 elements, with a truncation marker in the middle. |

---

## Usage

### Setting Global Default

```csharp
DumpConfig.Default.TruncationConfig.Mode = TruncationMode.HeadAndTail;
DumpConfig.Default.TruncationConfig.MaxCollectionCount = 10;
```

### Per-Call Override

```csharp
var truncationConfig = new TruncationConfig
{
    Mode = TruncationMode.Tail,
    MaxCollectionCount = 5
};

myLargeList.Dump(truncationConfig: truncationConfig);
```

---

## Visual Examples

Given a collection with 100 items and `MaxCollectionCount = 6`:

### Head Mode

Shows the first 6 items:

```
[0] Item 0
[1] Item 1
[2] Item 2
[3] Item 3
[4] Item 4
[5] Item 5
... and 94 more
```

### Tail Mode

Shows the last 6 items:

```
94 more items ...
[94] Item 94
[95] Item 95
[96] Item 96
[97] Item 97
[98] Item 98
[99] Item 99
```

### HeadAndTail Mode

Shows the first 3 and last 3 items:

```
[0] Item 0
[1] Item 1
[2] Item 2
... 94 more items ...
[97] Item 97
[98] Item 98
[99] Item 99
```

---

## Use Cases

| Mode | Best For |
|------|----------|
| `Head` | Seeing the beginning of ordered data, logs, or time-series |
| `Tail` | Seeing the most recent items in logs or queues |
| `HeadAndTail` | Getting a sense of both the start and end of a collection |

---

## Related Configuration

`TruncationMode` is used in conjunction with other `TruncationConfig` properties:

```csharp
var config = new TruncationConfig
{
    MaxCollectionCount = 10,        // Maximum items to show
    Mode = TruncationMode.HeadAndTail,
    PerDimension = true             // Apply truncation per dimension in multi-dimensional arrays
};
```

---

## See Also

- [Truncation Configuration](../configuration/truncation-config.md)
- [DumpConfig Reference](./dump-config.md)
