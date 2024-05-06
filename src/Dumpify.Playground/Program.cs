using Dumpify;
using System.Buffers;
using System.Collections;
using System.Data;
using System.Text;

int[] arr = [1, 2, 3, 4];
arr.Dump(tableConfig: new () { MaxCollectionCount = 2 });

//DumpConfig.Default.Renderer = Renderers.Text;
//DumpConfig.Default.ColorConfig = ColorConfig.NoColors;

//DumpConfig.Default.Output = Outputs.Debug;

// DumpConfig.Default.TableConfig.ShowRowSeparators = true;
// DumpConfig.Default.TableConfig.ShowMemberTypes = true;
Console.WriteLine("---------------------");
TestSpecific();
// TestSingle();
// ShowEverything();

//todo: improve labels, make them work with simple objects as strings (not wrapped in other object) and consider changing colors

#pragma warning disable CS8321
#pragma warning disable CS0168

void TestSpecific()
{
    var moaid = new Person
    {
        FirstName = "Moaid",
        LastName = "Hathot",
        Profession = Profession.Software
    };

    var moaid2 = new Person
    {
        FirstName = "Moaid",
        LastName = "Hathot",
        Profession = Profession.Software
    };

    Person[] arr = [moaid, moaid];
    // arr.Dump();

    // var value = SearchValues.Create("lskdjflskdfj").Dump();
    new TestVirtual().Dump();
    new TestVirtual().Dump("explcit include", members: new MembersConfig { IncludeVirtualMembers = true });
    new TestVirtual().Dump("explcit exclude", members: new MembersConfig { IncludeVirtualMembers = false });

    moaid2.Dump(members: new MembersConfig { MemberFilter = member => member.Name != nameof(Person.FirstName) });
    moaid2.Dump(members: new MembersConfig { MemberFilter = member => member.Name != nameof(Person.LastName) });
    //value.Dump();
    // ((nuint)5).Dump();
    // ((nint)5).Dump();
    //'a'.Dump(typeNames: new TypeNamingConfig { });
    // Enumerable.Range(0, 10).Select(i => (char)(i + 'a')).Dump();
    // Enumerable.Range(0, 10).Select(i => (char)(i + 'a')).ToArray().Dump();
    // "this is a string".Dump();
    //new TestVirtual().Dump();
    // DumpConfig.Default.TypeRenderingConfig.StringQuotationChar = '`';
    //
    // var direct = new TestDirect();
    // var explici = new TestExplicit();
    //
    // direct.Add(new KeyValuePair<string, int>("1", 1));
    // direct.Add(new KeyValuePair<string, int>("2", 2));
    // direct.Add(new KeyValuePair<string, int>("3", 3));
    // direct.Add(new KeyValuePair<string, int>("4", 4));
    //
    //
    // explici.Add(new KeyValuePair<string, int>("1", 1));
    // explici.Add(new KeyValuePair<string, int>("2", 2));
    // explici.Add(new KeyValuePair<string, int>("3", 3));
    // explici.Add(new KeyValuePair<string, int>("4", 4));
    //

    // Regex.Matches("abc", "[a-z]").Dump();
    //direct.Dump("Direct");
    // explici.Dump("Explicit");


    //DumpConfig.Default.TypeRenderingConfig.StringQuotationChar = '\'';
    //"Hello".Dump();
    //"Hello".Dump("Default");
    //1.Dump();
    //new { Name = "Moaid", LastName = "Hathot", Age = 35 }.Dump("Default");
    //DumpConfig.Default.TypeRenderingConfig.QuoteStringValues = false;
    //"Hello".Dump("Global");
    //"Hello".Dump();
    //1.Dump();
    //new { Name = "Moaid", LastName = "Hathot", Age = 35  }.Dump("Global");
    //DumpConfig.Default.TypeRenderingConfig.QuoteStringValues = true;
    //"Hello".Dump("per dump", typeRenderingConfig: new TypeRenderingConfig { QuoteStringValues = false});
    //"Hello".Dump(typeRenderingConfig: new TypeRenderingConfig { QuoteStringValues = false});
    //1.Dump(typeRenderingConfig: new TypeRenderingConfig { QuoteStringValues = false});
    //new { Name = "Moaid", LastName = "Hathot", Age = 35  }.Dump("per dump", typeRenderingConfig: new TypeRenderingConfig { QuoteStringValues = false });
    //new Dictionary<string, int>() { ["1"] = 2, ["2"] = 2, ["3"] = 3 }.Dump();
    //Regex.Match("abc", "[a-z]").Dump();
    //try
    //{
    //    throw new Exception("Bla bla", new ArgumentNullException("paramName", "inner bla fla"));
    //}
    //catch (Exception e)
    //{
    //    e.Dump(maxDepth: 1, label: "Test Ex", colors: new ColorConfig { LabelValueColor = Color.DarkOrange });
    //}
    //

    // new { Description = "You can manually specify labels to objects" }.Dump("Manual label");

    //Set auto-label globally for all dumps if a custom label wasn't provided
    // DumpConfig.Default.UseAutoLabels = true;
    // new { Description = "Or set labels automatically with auto-labels" }.Dump();

    // new { fa = "Hello", bla = "Word!" }.Dump("without separators");
    // new { fa = "Hello", bla = "Word!" }.Dump("with separators", tableConfig: new TableConfig { ShowRowSeparators = true });
    // DumpConfig.Default.UseAutoLabels = true;
    // DumpConfig.Default.TableConfig.ShowMemberTypes = true;
    // DumpConfig.Default.TableConfig.ShowRowSeparators = true;
    // var str2 = new { fa = "Hello", bla = "World!" }.Dump();


    // new { Name = "Dumpify", Description = "Dump any object to Console" }.Dump(tableConfig: new TableConfig { ShowRowSeparators = true, ShowMemberTypes = true });



    // Enumerable.Range(1, 3).Dump("This is Enumerable", colors: new ColorConfig { LabelValueColor = Color.Orange });
    //Enumerable.Range(1, 3).ToArray().Dump("This is Array", colors: new ColorConfig { LabelValueColor = Color.Orange });
    //Enumerable.Range(1, 3).ToList().Dump("This is List", colors: new ColorConfig { LabelValueColor = Color.Orange });
    //1.Dump("This is one", colors: new ColorConfig { LabelValueColor = Color.Fuchsia });
    //"Moaid".Dump();
    //Guid.NewGuid().Dump("This is Guid", colors: new ColorConfig { LabelValueColor = Color.SlateBlue });
    //Guid.NewGuid().Dump();
    //new
    //{
    //    Property = typeof(Person).GetProperty("FirstName"),
    //    Ctor = typeof(Person).GetConstructors().First(),
    //    Type = typeof(Person),
    //    Field = typeof(Person).GetFields().First(),
    //    Method = typeof(Person).GetMethods().First(m => m.Name.Contains("FooMethod")),
    //    Ctor2 = typeof(Person2).GetConstructors().First(),
    //}.Dump("This is a test");

    //typeof(Person).Dump();
    // new
    // {
    //     Properties = typeof(Person).GetProperties(),
    //     Methods = typeof(Person).GetMethods(),
    //     Fields = typeof(Person).GetFields(),
    //     Ctors = typeof(Person).GetConstructors(),
    //     //Members = typeof(Person).GetMembers(),
    //     FooGuid = Guid.NewGuid(),
    //     Enum = Profession.Health,
    //     TimeSpan = TimeSpan.MinValue,
    //     DateTime = DateTime.Now,
    //     DateTimeOffset = DateTimeOffset.Now,
    //     DateOnly = DateOnly.FromDateTime(DateTime.Now),
    //     TimeOnly = TimeOnly.FromDateTime(DateTime.Now),
    //     Lambda1 = (object)(() => 10),
    //
    // }.Dump("Person");
    //
    // DateTime.Now.Dump("DT");
    // Guid.NewGuid().Dump("Guid");
    // Guid.NewGuid().Dump();
}

// void TestObjectWithLargeWidth()
// {
var moaid = new Person
{
    FirstName = "Moaid",
    LastName = "Hathot",
    Profession = Profession.Software
};
var haneeni = new Person
{
    FirstName = "Haneeni",
    LastName = "Shibli",
    Profession = Profession.Health
};
moaid.Spouse = haneeni;
haneeni.Spouse = moaid;

// moaid.Dump("sdf");
// DumpConfig.Default.TableConfig.ShowTableHeaders = false;
// moaid.Dump("1112");
// moaid.Dump(tableConfig: new TableConfig { ShowTableHeaders = true, ShowRowSeparators = true, ShowMemberTypes = true }, typeNames: new TypeNamingConfig { ShowTypeNames = false });
//moaid.Dump();
//     var family = new Family
//     {
//         Parent1 = moaid,
//         Parent2 = haneeni,
//         FamilyId = 42,
//         ChildrenArray = new[] { new Person { FirstName = "Child1", LastName = "Hathot" }, new Person { FirstName = "Child2", LastName = "Hathot", Spouse = new Person { FirstName = "Child22", LastName = "Hathot", Spouse = new Person { FirstName = "Child222", LastName = "Hathot", Spouse = new Person { FirstName = "Child2222", LastName = "Hathot", Spouse = new Person
//         {
//             FirstName = "Child22222", LastName = "Hathot#@!%"
//         }}} } } },
//         ChildrenList = new List<Person> { new Person { FirstName = "Child1", LastName = "Hathot" }, new Person { FirstName = "Child2", LastName = "Hathot" } },
//         ChildrenArrayList = new ArrayList { new Person { FirstName = "Child1", LastName = "Hathot" }, new Person { FirstName = "Child2", LastName = "Hathot" } },
//         FamilyType = typeof(Family),
//         FamilyNameBuilder = new StringBuilder("This is the built Family Name"),
//     }.Dump().DumpDebug().DumpTrace().DumpText();
//     //File.WriteAllText(@"S:\Programming\Github\Dumpify\textDump.txt", family);
// }

void TestSingle()
{
    new { Name = "Dumpify", Description = "Dump any object to Console" }.Dump();

    DateTime.Now.Dump();
    DateTime.UtcNow.Dump();
    DateTimeOffset.Now.Dump();
    DateTimeOffset.UtcNow.Dump();
    TimeSpan.FromSeconds(10).Dump();

    var moaid = new Person
    {
        FirstName = "Moaid",
        LastName = "Hathot",
        Profession = Profession.Software
    };
    var haneeni = new Person
    {
        FirstName = "Haneeni",
        LastName = "Shibli",
        Profession = Profession.Health
    };
    moaid.Spouse = haneeni;
    haneeni.Spouse = moaid;

    ("ItemA", "ItemB").Dump();
    Tuple.Create("ItemAA", "ItemBB").Dump();

    // moaid.Dump(typeNames: new TypeNamingConfig { ShowTypeNames = false }, tableConfig: new TableConfig { ShowTableHeaders = false });

    // var s = Enumerable.Range(0, 10).Select(i => $"#{i}").Dump();
    // string.Join(", ", s).Dump();

    //Test().Dump();

    //IPAddress.IPv6Any.Dump();

    var map = new Dictionary<string, string>();
    map.Add("One", "1");
    map.Add("Two", "2");
    map.Add("Three", "3");
    map.Add("Four", "4");
    map.Add("Five", "5");
    map.Dump();

    var map2 = new Dictionary<string, Person>();
    map2.Add("Moaid", new Person { FirstName = "Moaid", LastName = "Hathot" });
    map2.Add("Haneeni", new Person { FirstName = "Haneeni", LastName = "Shibli" });
    map2.Dump("Test Label");

    //
    // map.Dump(map.GetType().Name);
    //
    //
    // map.Add("Five", "5");
    // map.Add("Six", "6");
    //
    // map.Add("Seven", "7");
    // // test.Sum(a => a.);
    //
    // var map2 = new ConcurrentDictionary<string, string>();
    // map2.TryAdd("One", "1");
    // map2.TryAdd("Two", "2");
    // map2.TryAdd("Three", "3");
    // map2.TryAdd("Four", "4");
    // map2.Dump(map2.GetType().Name);
    //
    //
    //
    // var test = new Test();
    // test.Add(new KeyValuePair<string, int>("One", 1));
    // test.Add(new KeyValuePair<string, int>("Two", 2));
    // test.Add(new KeyValuePair<string, int>("Three", 3));
    // test.Add(new KeyValuePair<string, int>("Four", 4));
    // test.Add(new KeyValuePair<string, int>("Five", 5));
    // test.Dump();
    //
    //
    // test.Dump(test.GetType().Name);
    //
    // async IAsyncEnumerable<int> Test()
    // {
    //     await Task.Yield();
    //     yield return 1;
    //     yield return 2;
    //     yield return 3;
    //     yield return 4;
    // }

    // new[] { new { Foo = moaid }, new { Foo = moaid }, new { Foo = moaid } }.Dump(label: "bla");

    var dataTable = new DataTable("Moaid Table");
    dataTable.Columns.Add("A");
    dataTable.Columns.Add("B");
    dataTable.Columns.Add("C");

    dataTable.Rows.Add("a", "b", "c");
    dataTable.Rows.Add("A", "B", "C");
    dataTable.Rows.Add("1", "2", "3");

    dataTable.Dump("Test Label 2");

    var set = new DataSet();

    set.Tables.Add(dataTable);
    set.Dump("Test Label 3");

    var arr = new[] { 1, 2, 3, 4 }.Dump();
    var arr2d = new int[,]
    {
        { 1, 2 },
        { 3, 4 }
    }.Dump();
    var arr3d = new int[,,]
    {
        {
            { 1, 2 },
            { 3, 4 }
        },
        {
            { 3, 4 },
            { 5, 6 }
        },
        {
            { 6, 7 },
            { 8, 9 }
        }
    }.Dump();
}

void ShowEverything()
{
    var moaid = new Person
    {
        FirstName = "Moaid",
        LastName = "Hathot",
        Profession = Profession.Software
    };
    var haneeni = new Person
    {
        FirstName = "Haneeni",
        LastName = "Shibli",
        Profession = Profession.Health
    };
    moaid.Spouse = haneeni;
    haneeni.Spouse = moaid;

    //DumpConfig.Default.Output = Outputs.Debug; //Outputs.Trace, Outputs.Console
    //moaid.Dump(output: Outputs.Trace);
    //moaid.DumpDebug();
    //moaid.DumpTrace();

    //moaid.Dump(output: myCustomOutput);


    DumpConfig.Default.TypeNamingConfig.UseAliases = true;
    DumpConfig.Default.TypeNamingConfig.UseFullName = true;

    moaid.Dump(typeNames: new TypeNamingConfig { UseAliases = true, ShowTypeNames = false });

    moaid.Dump();

    var family = new Family
    {
        Parent1 = moaid,
        Parent2 = haneeni,
        FamilyId = 42,
        ChildrenArray = new[]
        {
            new Person { FirstName = "Child1", LastName = "Hathot" },
            new Person
            {
                FirstName = "Child2",
                LastName = "Hathot",
                Spouse = new Person
                {
                    FirstName = "Child22",
                    LastName = "Hathot",
                    Spouse = new Person { FirstName = "Child222", LastName = "Hathot" }
                }
            }
        },
        ChildrenList = new List<Person>
        {
            new Person { FirstName = "Child1", LastName = "Hathot" },
            new Person { FirstName = "Child2", LastName = "Hathot" }
        },
        ChildrenArrayList = new ArrayList
        {
            new Person { FirstName = "Child1", LastName = "Hathot" },
            new Person { FirstName = "Child2", LastName = "Hathot" }
        },
        FamilyType = typeof(Family),
        FamilyNameBuilder = new StringBuilder("This is the built Family Name"),
    };

    System.Tuple.Create(10, 55, "hello").Dump();
    (10, "hello").Dump();

    var f = () => 10;
    // f.Dump();

    family.Dump(label: "This is my family label");

    new HashSet<string> { "A", "B", "C", "A" }.Dump();

    var stack = new Stack<int>();
    stack.Push(1);
    stack.Push(2);
    stack.Push(3);
    stack.Push(4);
    stack.Push(5);

    stack.Dump();

    moaid.Dump(
        tableConfig: new TableConfig { ShowTableHeaders = false },
        typeNames: new TypeNamingConfig { ShowTypeNames = false }
    );
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


    new Dictionary<Person, string>
    {
        [new Person { FirstName = "Moaid", LastName = "Hathot" }] = "Moaid Hathot",
        [new Person { FirstName = "Haneeni", LastName = "Shibli" }] = "Haneeni Shibli",
        [new Person { FirstName = "Waseem", LastName = "Hathot" }] = "Waseem Hathot",
    }.Dump();

    new Dictionary<string, Person>
    {
        ["Moaid"] = new Person { FirstName = "Moaid", LastName = "Hathot" },
        ["Haneeni"] = new Person { FirstName = "Haneeni", LastName = "Shibli" },
        ["Waseem"] = new Person { FirstName = "Waseem", LastName = "Hathot" },
    }.Dump(colors: ColorConfig.NoColors);

    new Dictionary<string, string>
    {
        ["Moaid"] = "Hathot",
        ["Haneeni"] = "Shibli",
        ["Eren"] = "Yeager",
        ["Mikasa"] = "Ackerman",
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
    var arr2d = new int[,]
    {
        { 1, 2 },
        { 3, 4 }
    }.Dump();

    DumpConfig.Default.TableConfig.ShowArrayIndices = false;

    arr.Dump();
    arr2d.Dump();

    DumpConfig.Default.TableConfig.ShowArrayIndices = true;

    moaid.Dump();

    //new[] { "Hello", "World", "This", "Is", "Dumpy" }.Dump(renderer: Renderers.Text);

    new Exception(
        "This is an exception",
        new ArgumentNullException("blaParam", "This is inner exception")
    ).Dump();

    new AdditionValue(1, 10).Dump(
        members: new()
        {
            IncludeFields = true,
            IncludeNonPublicMembers = true,
            IncludeProperties = false
        }
    );
    new AdditionValue(1, 10).Dump(
        members: new() { IncludeFields = true, IncludeNonPublicMembers = true }
    );

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
    //


    typeof(Person).Dump();
    new
    {
        Properties = typeof(Person).GetProperties(),
        Methods = typeof(Person).GetMethods(),
        Fields = typeof(Person).GetFields(),
        Ctors = typeof(Person).GetConstructors(),
        //Members = typeof(Person).GetMembers(),
        FooGuid = Guid.NewGuid(),
        Enum = Profession.Health,
        TimeSpan = TimeSpan.MinValue,
        DateTime = DateTime.Now,
        DateTimeOffset = DateTimeOffset.Now,
        DateOnly = DateOnly.FromDateTime(DateTime.Now),
        TimeOnly = TimeOnly.FromDateTime(DateTime.Now),
    }.Dump("Person");

    DateTime.Now.Dump("DT");
    Guid.NewGuid().Dump("Guid");
    Guid.NewGuid().Dump();
}
#pragma warning restore CS8321

public enum Profession
{
    Software,
    Health
};

public record class Person
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public Person? Spouse { get; set; }

    public Profession Profession { get; set; }
    public string? _fooField;

    public string? FooMethod(int a) => "";
}

public class Person2
{
    public Person2(int a, string b, double c) { }
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

    private int Value => _a + _b;
}

public class Device
{
    public bool isPowered { get; set; }
}

class TestDirect : ICollection<KeyValuePair<string, int>>
{
    private List<(string key, int value)> _list = new();

    public IEnumerator<KeyValuePair<string, int>> GetEnumerator() =>
        _list.Select(l => new KeyValuePair<string, int>(l.key, l.value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(KeyValuePair<string, int> item) => _list.Add((item.Key, item.Value));

    public void Clear() => _list.Clear();

    public bool Contains(KeyValuePair<string, int> item) => _list.Contains((item.Key, item.Value));

    public void CopyTo(KeyValuePair<string, int>[] array, int arrayIndex) =>
        throw new NotImplementedException();

    public bool Remove(KeyValuePair<string, int> item) => throw new NotImplementedException();

    public int Count => _list.Count;
    public bool IsReadOnly { get; } = false;
}

class TestExplicit : IEnumerable<(string, int)>, IEnumerable<KeyValuePair<string, int>>
{
    private List<(string key, int value)> _list = new();

    IEnumerator<(string, int)> IEnumerable<(string, int)>.GetEnumerator() => _list.GetEnumerator();

    //IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
    //    => new TestEnumerator(_list);

    public IEnumerator GetEnumerator() => new TestEnumerator(_list);

    IEnumerator<KeyValuePair<string, int>> IEnumerable<KeyValuePair<string, int>>.GetEnumerator() =>
        _list.Select(l => new KeyValuePair<string, int>(l.key, l.value)).GetEnumerator();

    IEnumerator<KeyValuePair<string, int>> GetExplicitEnumerator() =>
        _list.Select(l => new KeyValuePair<string, int>(l.key, l.value)).GetEnumerator();

    public void Add(KeyValuePair<string, int> item) => _list.Add((item.Key, item.Value));

    public void Clear() => _list.Clear();

    public bool Contains(KeyValuePair<string, int> item) => _list.Contains((item.Key, item.Value));

    public void CopyTo(KeyValuePair<string, int>[] array, int arrayIndex) =>
        throw new NotImplementedException();

    public bool Remove(KeyValuePair<string, int> item) => throw new NotImplementedException();

    public int Count => _list.Count;
    public bool IsReadOnly { get; } = false;

    private class TestEnumerator : IEnumerator<KeyValuePair<string, string>>
    {
        private readonly List<(string key, int value)> _list;
        private readonly IEnumerator<(string key, int value)> _enumerator;

        public TestEnumerator(List<(string key, int value)> list)
        {
            _list = list;
            _enumerator = _list.GetEnumerator();
        }

        public void Dispose() { }

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();

        public KeyValuePair<string, string> Current =>
            new KeyValuePair<string, string>(
                _enumerator.Current.key,
                _enumerator.Current.value.ToString()
            );

        object IEnumerator.Current => _enumerator.Current;
    }

    //private TestWrapper :
}

public class TestVirtual
{
    public string Foo { get; set; } = "Moaid";
    public virtual string Bar { get; set; } = "Hello";
    public string Baz
    {
        set { _ = value; }
    }
}