using Dumpify;
using Dumpify.Config;
using Dumpify.Outputs;
using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

//DumpConfig.Default.Renderer = Renderers.Text;
//DumpConfig.Default.ColorConfig = ColorConfig.NoColors;

//DumpConfig.Default.Output = Outputs.Debug;

TestSingle();
//ShowEverything();

#pragma warning disable CS8321
void TestSingle()
{
    new Book(new[] { "a", "b", "c" }).Dump(tableConfig: new TableConfig { ShowTableHeaders = true});
    new Book(new[] { "a", "b", "c" }).Dump(tableConfig: new TableConfig { ShowTableHeaders = false});
    //new List<string> { "A", "B", "C" }.Dump(renderer: Renderers.Table, output: Outputs.Console);
    //new List<string> { "A", "B", "C" }.Dump(renderer: Renderers.Text);

    //var map = new Dictionary<string, int>() { ["One"] = 1, ["Two"] = 2, ["Three"] = 3 }.DumpConsole();

    new AdditionValue(1, 10).Dump(members: new MembersConfig { IncludeFields = true, IncludeNonPublicMembers = true });
}

void ShowEverything()
{
    var moaid = new Person { FirstName = "Moaid", LastName = "Hathot" };
    var haneeni = new Person { FirstName = "Haneeni", LastName = "Shibli" };

    moaid.Spouse = haneeni;
    haneeni.Spouse = moaid;

    moaid.Dump();

    var family = new Family
    {
        Parent1 = moaid,
        Parent2 = haneeni,
        FamilyId = 42,
        ChildrenArray = new[] { new Person { FirstName = "Child1", LastName = "Hathot" }, new Person { FirstName = "Child2", LastName = "Hathot", Spouse = new Person { FirstName = "Child22", LastName = "Hathot", Spouse = new Person { FirstName = "Child222", LastName = "Hathot" } } } },
        ChildrenList = new List<Person> { new Person { FirstName = "Child1", LastName = "Hathot" }, new Person { FirstName = "Child2", LastName = "Hathot" } },
        ChildrenArrayList = new ArrayList { new Person { FirstName = "Child1", LastName = "Hathot" }, new Person { FirstName = "Child2", LastName = "Hathot" } },
        FamilyType = typeof(Family),
        FamilyNameBuilder = new StringBuilder("This is the built Family Name"),
    };

    System.Tuple.Create(10, 55, "hello").Dump();
    (10, "hello").Dump();

    var f = () => 10;
    f.Dump();

    family.Dump();

    new HashSet<string> { "A", "B", "C", "A" }.Dump();

    var stack = new Stack<int>();
    stack.Push(1);
    stack.Push(2);
    stack.Push(3);
    stack.Push(4);
    stack.Push(5);

    stack.Dump();


    moaid.Dump(tableConfig: new TableConfig { ShowTableHeaders = false }, typeNames: new TypeNamingConfig { ShowTypeNames = false });
    moaid.Dump();

    new int[][] { new int[] { 1, 2, 3, 4 }, new int[] { 1, 2, 3, 4, 5 } }.Dump();

//moaid.Dump(label: "Test");
//moaid.Dump();

//new { Name = "MyBook", Author = new { FirstName = "Moaid", LastName = "Hathot", Address = new { Email = "moaid@test.com" } } }.Dump(maxDepth: 7, showTypeNames: true, showHeaders: true);
//moaid.Dump();

//DumpConfig.Default.ShowTypeNames = false;
//DumpConfig.Default.ShowHeaders = false;

//DumpConfig.Default.Generator.Generate(new { Name = "MyBook", Author = new { FirstName = "Moaid", LastName = "Hathot", Address = new { Email = "moaid@test.com" } } }.GetType(), null).Dump();
//moaid.Dump();

    new { Name = "Dumpify", Description = "Dump any object to Console" }.Dump();
//new HashSet<string> { "Moaid", "Hathot", "shibli" }.Dump();


    new Dictionary<Person, string> { [new Person { FirstName = "Moaid", LastName = "Hathot" }] = "Moaid Hathot", [new Person { FirstName = "Haneeni", LastName = "Shibli" }] = "Haneeni Shibli", [new Person { FirstName = "Waseem", LastName = "Hathot" }] = "Waseem Hathot", }.Dump();


    new Dictionary<string, Person> { ["Moaid"] = new Person { FirstName = "Moaid", LastName = "Hathot" }, ["Haneeni"] = new Person { FirstName = "Haneeni", LastName = "Shibli" }, ["Waseem"] = new Person { FirstName = "Waseem", LastName = "Hathot" }, }.Dump(colors: ColorConfig.NoColors);


    new Dictionary<string, string>
    {
        ["Moaid"] = "Hathot", ["Haneeni"] = "Shibli", ["Eren"] = "Yeager", ["Mikasa"] = "Ackerman",
    }.Dump();

//ItemOrder.First.Dump();
//var d = DumpConfig.Default.Generator.Generate(ItemOrder.First.GetType(), null);
//d.Dump();


//var ao = new
//{
//    DateTime = DateTime.Now,
//    DateTimeUtc = DateTime.UtcNow,
//    DateTimeOffset = DateTimeOffset.Now,
//    DateOnly = DateOnly.FromDateTime(DateTime.Now),
//    TimeOnly = TimeOnly.FromDateTime(DateTime.Now),
//    TimeSpan = TimeSpan.FromMicroseconds(30324),
//}.Dump();

//var d = DumpConfig.Default.Generator.Generate(ao.GetType(), null);
//d.Dump();

//DumpConfig.Default.Generator.Generate(typeof(Family), null).Dump();

//var arr = new[,,] { { { 1, 2, 4 } }, { { 3, 4, 6 } }, { {1, 2, 88 } } }.Dump();

    var arr = new[] { 1, 2, 3, 4 }.Dump();
    var arr2d = new int[,] { { 1, 2 }, { 3, 4 } }.Dump();


    DumpConfig.Default.TableConfig.ShowArrayIndices = false;

    arr.Dump();
    arr2d.Dump();

    DumpConfig.Default.TableConfig.ShowArrayIndices = true;

    moaid.Dump();

    //new[] { "Hello", "World", "This", "Is", "Dumpy" }.Dump(renderer: Renderers.Text);

    new Exception("This is an exception", new ArgumentNullException("blaParam", "This is inner exception")).Dump();

//arr.Dump();
//moaid.Dump();


//family.Dump(maxDepth: 2);

//Console.WriteLine(JsonSerializer.Serialize(moaid));


//moaid.Dump();
//arr2d.Dump();

//moaid.Dump(maxDepth: 2);
//family.Dump(maxDepth: 2);
//arr.Dump();
//arr2d.Dump();
//((object)null).Dump();

//var result = DumpConfig.Default.Generator.Generate(family.GetType(), null);

//JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
}
#pragma warning restore CS8321

public enum ItemOrder { First, Second, Last };
public record class Person
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public Person? Spouse { get; set; }

    public ItemOrder TestEnum { get; set; } = ItemOrder.Second;
}

public class Family
{
    public Person? Parent1 { get; set; }
    public Person? Parent2 { get; set; }

    public int FamilyId { get; set; }

    public Person[]? ChildrenArray { get; set; }
    public List<Person>? ChildrenList { get; set; }
    public ArrayList? ChildrenArrayList { get; set; }

    public Type? FamilyType { get; set; }

    public StringBuilder? FamilyNameBuilder { get; set; }
}

public record class Book(string[] Authors);

public class AdditionValue
{
    private readonly int _a;
    private readonly int _b;

    public AdditionValue(int a, int b)
    {
        _a = a;
        _b = b;
    }

    public int Value => _a + _b;
}
