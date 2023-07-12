# Dumpify
[![Github version](https://badge.fury.io/nu/Dumpify.svg)](https://badge.fury.io/nu/Dumpify)

Improve productivity and debuggability by adding `.Dump()` extension methods to **Console Applications**.
`Dump` any object in a structured and colorful way into the Console, Trace, Debug events or your own custom output.

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
* Support for differnt otuput targets: Console, Trace, Debug, Text, Custom
* Fast!
    
# Examples:
## Anonymous types
```csharp
new { Name = "Dumpify", Description = "Dump any object to Console" }.Dump();
```
![image](https://user-images.githubusercontent.com/8770486/232251633-5830bd48-0e45-4c89-9b26-3c678230a90a.png)


### Support nesting and circular references
```csharp
var moaid = new Person { FirstName = "Moaid", LastName = "Hathot", Profession = Profession.Software };
var haneeni = new Person { FirstName = "Haneeni", LastName = "Shibli", Profession = Profession.Health };

moaid.Spouse = haneeni;
haneeni.Spouse = moaid;

moaid.Dump();
//You can define max depth as well, e.g `moaid.Dump(maxDepth: 2)`
```
![image](https://user-images.githubusercontent.com/8770486/232280616-c6127820-7e2b-448b-81ca-1aded2894cdc.png)

### Support for Arrays, Dictionaries and Collections
```csharp
var arr = new[] { 1, 2, 3, 4 }.Dump();
```
![image](https://user-images.githubusercontent.com/8770486/232251833-ef2650fe-64a3-476d-b676-4a0f73339560.png)

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
![image](https://user-images.githubusercontent.com/8770486/232251913-add4a0d8-3355-44f6-ba94-5dfbf8d8e2ac.png)


### You can turn on or off fields and private members
```csharp
public class AdditionValue
{
    private readonly int _a;
    private readonly int _b;

    public AdditionValue(int a, int b)
    {
        _a = a;
        _b = b;
    }

    private int Value => _a + _b;
}


new AdditionValue(1, 2).Dump(members: new MembersConfig { IncludeFields = true, IncludeNonPublicMembers = true });
```
![image](https://user-images.githubusercontent.com/8770486/232252840-c5b0ea4c-eae9-4dc2-bd6c-d42ee58505eb.png)



### You can customize colors
```csharp
var package = new { Name = "Dumpify", Description = "Dump any object to Console" };
package.Dump(colors: ColorConfig.NoColors);
package.Dump(colors: new ColorConfig { PropertyValueColor = new DumpColor(Color.RoyalBlue)});
```
![image](https://user-images.githubusercontent.com/8770486/232252235-18d43c3a-0b54-475a-befc-0f957777f150.png)

### You can turn on or off type names, headers, lables and much more
```csharp
var moaid = new Person { FirstName = "Moaid", LastName = "Hathot", Profession = Profession.Software };
var haneeni = new Person { FirstName = "Haneeni", LastName = "Shibli", Profession = Profession.Health };
moaid.Spouse = haneeni;
haneeni.Spouse = moaid;

moaid.Dump(typeNames: new TypeNamingConfig { ShowTypeNames = false }, tableConfig: new TableConfig { ShowTableHeaders = false });
```
![image](https://user-images.githubusercontent.com/8770486/232252319-58a98036-5a0e-4514-8d08-df6fdff5a8a7.png)


### There are multiple output options (Console, Trace, Debug, Text) or provide your own
```csharp
var package = new { Name = "Dumpify", Description = "Dump any object to Console" };
package.Dump(); //Similar to `package.DumpConsole()` and `package.Dump(output: Outputs.Console))`
package.DumpDebug(); //Dump to Visual Studio's Debug source
package.DumpTrace(); //Dump to Trace 
var text = package.DumpText(); //The table in a text format

using var writer = new StringWriter();
package.Dump(output: new DumpOutput(writer)); //Custom output
```


### Every configuration can be defined per-Dump or globally for all Dumps, e.g:
```csharp
DumpConfig.Default.TypeNamingConfig.UseAliases = true;
DumpConfig.Default.TypeNamingConfig.ShowTypeNames = false;
DumpConfig.Default.ColorConfig.TypeNameColor = Color.Gold;
DumpConfig.Default.MaxDepth = 3;
//Much more...
```



# Features for the future 0.6.0 release
* Add configuration for formatting Anonymous Objects type names
* Cache Spectre.Console styles and colors
* Text renderer
* Consider disabling wrapping of Table titles
* re-introduce labels
* Better styling of Custom values
	* Typeof(T) for example, Generic types, etc.
* Better rendering of Delegates
* Unifying namespaces of library types
* Handling all Reflection types
* Documentation
* Support .NET Standard 2.0 instad of .NET Standard 2.1
* Better support for `Nullable<T>`
* Better support for Reflection types.


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
