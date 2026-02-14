---
layout: default
title: Configuration
nav_order: 3
has_children: true
description: "Configure Dumpify's behavior and appearance"
permalink: /configuration/
---

# Configuration Overview

Dumpify is highly configurable. You can customize nearly every aspect of how objects are rendered, from colors to member filtering to output dimensions.

---

## Table of Contents

- [Configuration Hierarchy](#configuration-hierarchy)
- [Per-Dump vs Global Configuration](#per-dump-vs-global-configuration)
- [Configuration Classes](#configuration-classes)
- [Quick Examples](#quick-examples)

---

## Configuration Hierarchy

Dumpify uses a layered configuration system:

1. **Per-Dump Configuration** - Options passed directly to `.Dump()` methods
2. **Global Configuration** - Defaults set via `DumpConfig.Default`

Per-dump configuration always takes precedence over global configuration.

---

## Per-Dump vs Global Configuration

### Per-Dump Configuration

Pass configuration options directly to the `.Dump()` method:

```csharp
obj.Dump(
    label: "My Object",
    maxDepth: 3,
    colors: new ColorConfig { PropertyValueColor = new DumpColor("#FF5733") },
    tableConfig: new TableConfig { ShowRowSeparators = true }
);
```

### Global Configuration

Set defaults that apply to all dumps:

```csharp
// Set global defaults
DumpConfig.Default.MaxDepth = 5;
DumpConfig.Default.ColorConfig.TypeNameColor = new DumpColor("#FFD700");
DumpConfig.Default.TableConfig.ShowRowSeparators = true;

// Now all dumps use these settings
obj.Dump(); // Uses global settings
```

### Combining Both

```csharp
// Set global defaults
DumpConfig.Default.MaxDepth = 5;

// Override for a specific dump
obj.Dump(maxDepth: 2); // Uses depth 2, not 5
```

---

## Configuration Classes

Dumpify provides several configuration classes, each controlling different aspects:

| Class | Purpose | Documentation |
|-------|---------|---------------|
| [DumpConfig](global-configuration.md) | Main configuration container | [Global Configuration](global-configuration.md) |
| [ColorConfig](color-config.md) | Color customization | [ColorConfig](color-config.md) |
| [TableConfig](table-config.md) | Table display options | [TableConfig](table-config.md) |
| [MembersConfig](members-config.md) | Member filtering | [MembersConfig](members-config.md) |
| [TypeNamingConfig](type-naming-config.md) | Type name display | [TypeNamingConfig](type-naming-config.md) |
| [TypeRenderingConfig](type-rendering-config.md) | Value rendering | [TypeRenderingConfig](type-rendering-config.md) |
| [OutputConfig](output-config.md) | Output dimensions | [OutputConfig](output-config.md) |

---

## Quick Examples

### Disable Colors

```csharp
// Per-dump
obj.Dump(colors: ColorConfig.NoColors);

// Note: ColorConfig cannot be reassigned globally since it's a read-only property.
// To disable colors globally, set individual color properties to null or use per-dump configuration.
```

### Show Row Separators and Member Types

```csharp
obj.Dump(tableConfig: new TableConfig 
{ 
    ShowRowSeparators = true, 
    ShowMemberTypes = true 
});
```

![Row separators](https://raw.githubusercontent.com/MoaidHathot/Dumpify/main/assets/screenshots/row-separator.png)

### Include Private Members and Fields

```csharp
obj.Dump(members: new MembersConfig 
{ 
    IncludeFields = true, 
    IncludeNonPublicMembers = true 
});
```

![Private members](https://user-images.githubusercontent.com/8770486/232252840-c5b0ea4c-eae9-4dc2-bd6c-d42ee58505eb.png)

### Hide Type Names

```csharp
obj.Dump(typeNames: new TypeNamingConfig { ShowTypeNames = false });
```

### Limit Collection Size

```csharp
largeArray.Dump(tableConfig: new TableConfig { MaxCollectionCount = 10 });
```

---

## Next Steps

- [Global Configuration](global-configuration.md) - Learn about `DumpConfig.Default`
- [ColorConfig](color-config.md) - Customize colors
- [TableConfig](table-config.md) - Configure table display
- [MembersConfig](members-config.md) - Filter members
