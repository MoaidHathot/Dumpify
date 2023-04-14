# Dumpify
Improve productivity and debuggability by adding `.Dump()` extension methods to **Console Applications**.
`Dump` any object in a structured and colorful way into the Console, Trace, Debug events or your own custom output.

One of my favorites [LinqPad](https://www.linqpad.net/) feature is its `.Dump()` extension methods and how customizable they are. It was always hard to return to Visual Studio, VSCode, Rider, or any other IDE/Editor and find out this feature doesn't exist there.

# How to Install
The library is published as a [Nuget](https://www.nuget.org/packages/Dumpify)
Either run `dotnet add package Dumpify`, `Install-Package Dumpify` or use Visual Studio's [NuGet Package Manager](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio)

# Features
* Dump any object in a structured, colorful way to Console, Debug, Trace or any other custom output
* Support Properties, Fields and non-public members
* Support max nesting levels
* Support circular dependencies and references
* Support styling and customizations
* Highly Configurable
* Fast!
* Support for differnt otuput target:
    * Console
    * Trace
    * Debug
    * Text
    * Custom
    
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


# Features for the future 0.6.0 release
* Add configuration for formatting Anonymous Objects type names
* Cache Spectre.Console styles and colors
* Text renderer
* Consider disabling wrapping of Table titles
* re-introduce labels
* Better styling of Custom values
	* Typeof(T) for example, Generic types, etc.
* Better rendering of Delegates

# To do
* Live outputs
* Add custom rendering for more types:
    - DataTable & DataSets
    - Exceptions, AggregateExceptions, etc...
* Rethink Generators caching keys
* Consider using Max Depth for Descriptors
* Refactor Renderers and make it better extendable
* Add more renderers
    * Text Renderers
    * re-introduce Json
    * CSharp Renderer
* Consider Decoupling from Spectre.Console
* Tests
    * More tests
    * Visual (Render) Tests - consider acceptance tests
    * Tests for Nesting
* More sync between Custom Descriptors and Custom Renderers
	* Think how we can mark type's descriptor as needing special rendering.
	* The current CustomDescriptorGenerator must generate a value
	* Consider ValueTuple
* Refactor SpectureTableRenderer to share customization code
* Consider changing the style/view of ObjectDescriptors without properties (currently empty table)
