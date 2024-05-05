# Dumpify
Improve productivity and debuggability by adding `.Dump()` extension methods to **Console Applications**.
`Dump` any object in a structured and colorful way into the Console, Trace, Debug events or your own custom output.

# Features
* Dump any object in a structured, colorful way to Console, Debug, Trace or any other custom output
* Support Properties, Fields and non-public members
* Support max nesting levels
* Support circular dependencies and references
* Support styling and customizations
* Highly Configurable
* Support for different output targets: Console, Trace, Debug, Text, Custom
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

### You can provide a custom filter to determine if members should be included or not
```csharp
public class Person
{
    public string Name { get; set; }

    [JsonIgnore]
    public string SensitiveData { get; set; }
}

new Person()
{
    Name = "Moaid",
    SensitiveData = "We don't want this to show up"
}.Dump(members: new MembersConfig { MemberFilter = member => !member.Info.CustomAttributes.Any(a => a.AttributeType == typeof(JsonIgnoreAttribute)) });
```

### You can turn on or off row separators and a type column
```csharp
//globally
DumpConfig.Default.TableConfig.ShowMemberTypes = true;
DumpConfig.Default.TableConfig.ShowRowSeparators = true;

new { Name = "Dumpify", Description = "Dump any object to Console" }.Dump();

//or Per dump
new { Name = "Dumpify", Description = "Dump any object to Console" }.Dump(tableConfig: new TableConfig { ShowRowSeparators = true, ShowMemberTypes = true });
```
![image](https://raw.githubusercontent.com/MoaidHathot/Dumpify/main/assets/screenshots/row-separator.png)

### You can set custom labels or auto-labels
```csharp
new { Description = "You can manually specify labels to objects" }.Dump("Manual label");

//Set auto-label globally for all dumps if a custom label wasn't provider
DumpConfig.Default.UseAutoLabels = true;
new { Description = "Or set labels automatically with auto-labels" }.Dump();
```
![image](https://raw.githubusercontent.com/MoaidHathot/Dumpify/main/assets/screenshots/custom-label-and-auto-labels.png)

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
