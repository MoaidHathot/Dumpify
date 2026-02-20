---
layout: home
title: Home
nav_order: 1
description: "Dumpify - Add .Dump() extension methods to any .NET Console application"
permalink: /
---

# Dumpify Documentation

<img src="https://raw.githubusercontent.com/MoaidHathot/Dumpify/main/assets/Dumpify-logo-styled.png" alt="Dumpify Logo" width="200" />

[![NuGet version](https://badge.fury.io/nu/Dumpify.svg)](https://badge.fury.io/nu/Dumpify)
![Build](https://github.com/MoaidHathot/Dumpify/actions/workflows/build-dumpify.yml/badge.svg)
![NuGet Downloads](https://img.shields.io/nuget/dt/Dumpify)
![GitHub License](https://img.shields.io/github/license/MoaidHathot/Dumpify)

**Dumpify** is a .NET library that improves productivity and debuggability by adding `.Dump()` extension methods to console applications. Dump any object in a structured, colorful way to Console, Debug, Trace, or your own custom output.

---

## Table of Contents

- [Installation](#installation)
- [Quick Example](#quick-example)
- [Features](#features)
- [Documentation](#documentation)
- [Target Frameworks](#target-frameworks)

---

## Installation

Install Dumpify via NuGet:

```bash
# .NET CLI
dotnet add package Dumpify

# Package Manager Console
Install-Package Dumpify
```

Or use Visual Studio's [NuGet Package Manager](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio).

---

## Quick Example

```csharp
using Dumpify;

// Dump any object to the console
new { Name = "Dumpify", Description = "Dump any object to Console" }.Dump();
```

![Anonymous type dump](https://user-images.githubusercontent.com/8770486/232251633-5830bd48-0e45-4c89-9b26-3c678230a90a.png)

---

## Features

- **Universal Object Dumping** - Dump any .NET object including anonymous types, records, classes, structs
- **Structured Output** - Objects are displayed in formatted, easy-to-read tables
- **Colorful Console** - Syntax highlighting with customizable colors
- **Circular Reference Handling** - Automatically detects and handles circular references
- **Collection Support** - Arrays, Lists, Dictionaries, DataTables, and more
- **Member Filtering** - Include/exclude properties, fields, public/private members
- **Multiple Outputs** - Console, Debug, Trace, Text, or custom outputs
- **Highly Configurable** - Extensive configuration options for all aspects of rendering
- **Fast** - Optimized with descriptor caching for performance

---

## Documentation

### Getting Started

- [Getting Started Guide](getting-started.md) - Quick introduction to Dumpify

### Configuration

- [Configuration Overview](configuration/index.md) - How configuration works in Dumpify
- [Global Configuration](configuration/global-configuration.md) - Setting up global defaults
- [ColorConfig](configuration/color-config.md) - Customizing colors
- [TableConfig](configuration/table-config.md) - Table display options
- [MembersConfig](configuration/members-config.md) - Member filtering options
- [TypeNamingConfig](configuration/type-naming-config.md) - Type name display options
- [TypeRenderingConfig](configuration/type-rendering-config.md) - Value rendering options
- [OutputConfig](configuration/output-config.md) - Output dimension options

### API Reference

- [API Overview](api-reference/index.md) - Public APIs at a glance
- [Dump Extensions](api-reference/dump-extensions.md) - All `.Dump()` extension methods
- [DumpConfig](api-reference/dump-config.md) - The DumpConfig class
- [DumpColor](api-reference/dump-color.md) - The DumpColor class

### Features

- [Features Overview](features/index.md) - All features explained
- [Circular References](features/circular-references.md) - Handling circular dependencies
- [Collections](features/collections.md) - Arrays, lists, dictionaries support
- [Member Filtering](features/member-filtering.md) - Filtering which members to display
- [Custom Type Handlers](features/custom-type-handlers.md) - Adding custom type rendering
- [Labels](features/labels.md) - Adding labels to dumps
- [Output Targets](features/output-targets.md) - Different output destinations

### Examples

- [Examples Overview](examples/index.md) - All examples
- [Basic Usage](examples/basic-usage.md) - Common usage patterns
- [Advanced Usage](examples/advanced-usage.md) - Complex configurations
- [Real-World Scenarios](examples/real-world-scenarios.md) - Practical use cases

---

## Target Frameworks

Dumpify is designed to support almost all versions of .NET, providing broad compatibility across the entire .NET ecosystem:

- **.NET 10.0** - Latest .NET version with full feature support
- **.NET 6.0** - Long-Term Support (LTS) version
- **.NET Standard 2.1** - Compatible with .NET Core 3.0+, .NET 5+
- **.NET Standard 2.0** - Maximum compatibility including .NET Framework 4.6.1+, .NET Core 2.0+, Mono, Xamarin, and Unity

By targeting .NET Standard 2.0, Dumpify works with a wide range of platforms including .NET Framework, .NET Core, .NET 5+, Mono, Xamarin, and Unity. This means you can use Dumpify in virtually any .NET project, from legacy .NET Framework applications to the latest .NET 10 projects.

---

## License

Dumpify is licensed under the [MIT License](https://github.com/MoaidHathot/Dumpify/blob/main/LICENSE).

---

## Links

- [GitHub Repository](https://github.com/MoaidHathot/Dumpify)
- [NuGet Package](https://www.nuget.org/packages/Dumpify)
- [Overview Video](https://www.youtube.com/watch?v=ERWAMSgz-vc)
