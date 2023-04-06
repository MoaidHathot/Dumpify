# Dumpify
**This library is under development and is not recommended in Production payloads**

Improve productivity and debuggability by adding `.Dump()` extension methods to **Console Applications**, similar to LinqPad's.

One of my favorites [LinqPad](https://www.linqpad.net/) feature is its `.Dump()` extension methods and how customizable they are. It was always hard to return to Visual Studio or any Console Application and find out this feature doesn't exist there.

# Features
* Dump any object to Console
* Support for Arrays and IEnumerables
* Support for Custom descriptors
* Support for max nesting levels
* Support for circular dependencies and references
* Support for styling and customizations
* Configurable
* Spectre.Console renderers
* Special Handling for types:
    * Arrays, IEnumerables, Collections.
    * StringBuilder
    * System.Reflection types
    
# Examples:
```csharp
new { Name = "Dumpify", Description = "Dump any object to Console" }.Dump();

```
![image](https://user-images.githubusercontent.com/8770486/229388747-897790be-45ee-4db2-a21a-137c564774af.png)

Support nesting as well
```csharp
var moaid = new Person { FirstName = "Moaid", LastName = "Hathot" };
var haneeni = new Person { FirstName = "Haneeni", LastName = "Shibli" };

moaid.Spouse = haneeni;
haneeni.Spouse = moaid;

moaid.Dump();
```
![image](https://user-images.githubusercontent.com/8770486/229388818-1ef54ad1-4779-4043-bf04-8ff1f0c7d605.png)

# How to use
Either use `dotnet add package Dumpify` or `Install-Package Dumpify`

# To do
* Custom Outputs
* Live outputs
* Improve Cache By decoupling the PropertyInfo from the Descriptors.
* Rethink Generators caching keys.
* Consider using Max Depth for Descriptors
* Refactor Renderers and make it better extendable
* Better styling
* Custom styling
* Add more renderers
    * re-introduce Json Renderers
    * JavaScript's `console.log` style
* Decouple from Spectre.Console    
* Tests
    * More tests
    * Visual (Render) Tests - consider acceptance tests
    * Tests for Nesting
* Add support for include fields
* Add support for include private members
* Cache Spectre.Console styles and colors
* More sync between Custom Descriptors and Custom Renderers
	* Think how we can mark type's descriptor as needing special rendering.
	* The current CustomDescriptorGenerator must generate a value
	* Consider ValueTuple
* Refactor SpectureTableRenderer to share customization code
* Better handling of ObjectDescriptors without properties (currently empty table)

# Disclaimer
This project is inspired by LinqPad's Dump features, authored by Joseph Albahari.
