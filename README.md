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
    
# How to Install
Either run `dotnet add package Dumpify` or `Install-Package Dumpify`
    
 
# Examples:
```csharp
new { Name = "Dumpify", Description = "Dump any object to Console" }.Dump();

```
![image](https://user-images.githubusercontent.com/8770486/230250399-b7778879-c24f-493e-9e77-e81f1f43e6db.png)


Support nesting as well
```csharp
var moaid = new Person { FirstName = "Moaid", LastName = "Hathot" };
var haneeni = new Person { FirstName = "Haneeni", LastName = "Shibli" };

moaid.Spouse = haneeni;
haneeni.Spouse = moaid;

moaid.Dump();
```
![image](https://user-images.githubusercontent.com/8770486/230250311-715af695-8f73-4fea-935d-03c9293bb478.png)

Support for special types like arrays and Dictionaries
```csharp
var arr = new[] { 1, 2, 3, 4 }.Dump();
```
![image](https://user-images.githubusercontent.com/8770486/230250695-0d5bbef2-a1b5-43e9-a24f-9d28168bca72.png)

```csharp
var arr2d = new int[,] { {1, 2}, {3, 4} }.Dump();
```
![image](https://user-images.githubusercontent.com/8770486/230250735-66703e54-ce02-41c0-91b7-fcbee5f80ac3.png)

```csharp
new Dictionary<string, string>
{
   ["Moaid"] = "Hathot",
   ["Haneeni"] = "Shibli",
   ["Eren"] = "Yeager",
   ["Mikasa"] = "Ackerman",
}.Dump();
```
![image](https://user-images.githubusercontent.com/8770486/230250919-838357bf-b6c2-4a91-8702-b639050ebe1d.png)


# Features for the future 0.5.0 release
* Consider making array's type name into a Table Title
* JavaScript-style renderer
* Consider disabling wrapping of Table titles
* re-introduce labels

# Features for the future 0.6.0 release
* Better styling of Custom values
	* Typeof(T) for example, Generic types, etc.
* Better rendering of Delegates

# To do
* Custom Outputs
* Live outputs
* Improve Cache By decoupling the PropertyInfo from the Descriptors.
* Rethink Generators caching keys
* Consider using Max Depth for Descriptors
* Refactor Renderers and make it better extendable
* Add more renderers
    * re-introduce Json Renderers
    * JavaScript's `console.log` style
    * CSharp Renderer
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
* Consider changing the style/view of ObjectDescriptors without properties (currently empty table)

# Disclaimer
This project is inspired by LinqPad's Dump features, authored by Joseph Albahari.
