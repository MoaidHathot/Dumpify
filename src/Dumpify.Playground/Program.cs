using Dumpify;
using System.Collections;
using System.Data;
using System.Text;

// Use the new IsCollectionElement property to set horizontal layout for all collection elements
// DumpConfig.Default.TableConfig.SetLayoutWhen(context => context.IsCollectionElement, TableLayout.Horizontal);

// typeof(string).Dump("Single string");

var memberConfig = new MembersConfig
{
    MemberFilter = ctx => ctx.Member.Name == "Employees1",
};

var msft = new { Name = "Microsoft", Employees = new[]
{
    new Employee { Salary = 1, Name = "Alice", Department = "HR" },
    new Employee { Salary = 2, Name = "Bob", Department = "IT" },
    new Employee { Salary = 3, Name = "Charlie", Department = "Finance" }
}}.Dump("Company with Employees", members: memberConfig);

msft.Dump("Company with Employees - Show Types", members: memberConfig, tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

// employees.Dump();

// new []{ "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" }.Dump();
// DemoTableLayoutFeatures();

// =============================================================================
// Table Layout Demo - Showcasing rendering flexibility
// =============================================================================
#pragma warning disable CS8321 // Local function is declared but never used
void DemoTableLayoutFeatures()
{
    Console.WriteLine("=== Horizontal Layout Demo ===\n");

    // 1. Simple object - properties as columns
    Console.WriteLine("1. Single Object");
    var product = new { Name = "Laptop", Price = 999.99m, InStock = true };
    product.Dump("Vertical (default)");
    product.Dump("Horizontal", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

    // 2. Collection of objects - each item as a row, properties as columns
    Console.WriteLine("\n2. Collection of Objects");
    var products = new[]
    {
        new { Name = "Laptop", Price = 999.99m, InStock = true },
        new { Name = "Mouse", Price = 29.99m, InStock = true },
        new { Name = "Keyboard", Price = 79.99m, InStock = false },
    };
    products.Dump("Horizontal Products", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

    // 3. Nested objects - nested tables preserve their own structure
    Console.WriteLine("\n3. Nested Objects");
    var order = new
    {
        OrderId = 1001,
        Customer = "Alice",
        Items = new[]
        {
            new { Product = "Laptop", Qty = 1 },
            new { Product = "Mouse", Qty = 2 },
        }
    };
    order.Dump("Order with nested items", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

    // 4. Mixed types in collection - graceful handling
    Console.WriteLine("\n4. Row Separators");
    var employees = new[]
    {
        new Employee { Name = "Alice", Department = "Engineering", Salary = 95000 },
        new Employee { Name = "Bob", Department = "Marketing", Salary = 75000 },
        new Employee { Name = "Carol", Department = "Engineering", Salary = 105000 },
    };
    employees.Dump("Employees", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal, ShowRowSeparators = true });

    // 5. With row indices shown
    Console.WriteLine("\n5. Collection with Row Indices");
    var tasks = new[]
    {
        new { Task = "Review PR", Status = "Done", Priority = "High" },
        new { Task = "Write tests", Status = "In Progress", Priority = "Medium" },
        new { Task = "Update docs", Status = "Pending", Priority = "Low" },
    };
    tasks.Dump("Tasks (with indices)", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal, ShowRowIndices = true });

    // 6. Primitive collection - falls back to vertical (appropriate for simple types)
    Console.WriteLine("\n6. Primitive Collection (auto vertical fallback)");
    var numbers = new[] { 1, 2, 3, 4, 5 };
    numbers.Dump("Numbers - Horizontal requested", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

    // 7. Side-by-side comparison
    Console.WriteLine("\n7. Side-by-Side: Vertical vs Horizontal");
    var people = new[]
    {
        new Person { FirstName = "Moaid", LastName = "Hathot", Profession = Profession.Software },
        new Person { FirstName = "Haneeni", LastName = "Shibli", Profession = Profession.Health },
    };
    people.Dump("Vertical Layout");
    people.Dump("Horizontal Layout", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

    // 8. Dynamic/Hybrid Layout using SetLayoutWhen with depth-based rules
    Console.WriteLine("\n8. Hybrid Layout (Horizontal at depth 0, Vertical for nested)");
    var hybridConfig = new TableConfig();
    hybridConfig.SetLayoutWhen(
        ctx => ctx.CurrentDepth == 0,  // Only at the top level
        TableLayout.Horizontal
    );

    var ordersWithDetails = new[]
    {
        new
        {
            OrderId = 1001,
            Customer = "Alice",
            Items = new[]
            {
                new { Product = "Laptop", Qty = 1, Price = 999.99m },
                new { Product = "Mouse", Qty = 2, Price = 29.99m },
            }
        },
        new
        {
            OrderId = 1002,
            Customer = "Bob",
            Items = new[]
            {
                new { Product = "Keyboard", Qty = 1, Price = 79.99m },
            }
        },
    };

    Console.WriteLine("With hybrid config: top-level horizontal, nested stays vertical");
    ordersWithDetails.Dump("Orders (Hybrid)", tableConfig: hybridConfig);

    // 9. Type-specific layout rules
    Console.WriteLine("\n9. Type-Specific Layout Rules");
    var typeSpecificConfig = new TableConfig();
    typeSpecificConfig.SetLayoutForType<Employee>(TableLayout.Horizontal);
    // Person stays vertical (default)

    var team = new
    {
        Manager = new Person { FirstName = "Alice", LastName = "Smith", Profession = Profession.Software },
        Members = new[]
        {
            new Employee { Name = "Bob", Department = "Engineering", Salary = 85000 },
            new Employee { Name = "Carol", Department = "Engineering", Salary = 90000 },
        }
    };
    team.Dump("Team (Employee=Horizontal, Person=Vertical)", tableConfig: typeSpecificConfig);

    // 10. Complex rule with TableLayoutResult overrides
    Console.WriteLine("\n10. Dynamic Config with Result Overrides");
    var advancedConfig = new TableConfig();
    advancedConfig.SetLayoutWhen(ctx =>
        ctx.CurrentDepth == 0
            ? new TableLayoutResult { Layout = TableLayout.Horizontal, ShowRowSeparators = true }
            : TableLayoutResult.None  // Let other rules or default handle it
    );

    employees.Dump("Employees (Horizontal + Row Separators)", tableConfig: advancedConfig);

    Console.WriteLine("\n=== End Demo ===");
}

// =============================================================================
// Old playground code (preserved but not called)
// =============================================================================
void OldPlaygroundCode()
{
    //DumpConfig.Default.Renderer = Renderers.Text;
    //DumpConfig.Default.ColorConfig = ColorConfig.NoColors;

    //DumpConfig.Default.Output = Outputs.Debug;

    // DumpConfig.Default.TableConfig.ShowRowSeparators = true;
    // DumpConfig.Default.TableConfig.ShowMemberTypes = true;
    // new DirectoryInfo("C:\\Program Files").Dump();
    // (1, 2, 3, 4, ("1", "b"), 5, 6, 7, 8, 9, 10, 11, 12, 13, "14", "15", 16, 17, 18).Dump("ValueTuple", tableConfig: new TableConfig {  MaxCollectionCount = 4 });
    // (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, "14", "15", 16, 17, 18).Dump("ValueTuple 1");
    // (1, 2, 3, 4, ("1", "b"), 5, 6, 7, 8, 9, 10, 11, 12, 13, "14", "15", 16, 17, 18).Dump("ValueTuple");
    // Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9, 10, 11)).Dump("System.Tuple");
    // new[] { 1, 2, 3,  4, 5, 6, 7, 8, 9, 10 }.Dump(tableConfig: new TableConfig { MaxCollectionCount = 3 });
    // DumpConfig.Default.TableConfig.BorderStyle = TableBorderStyle.Ascii;

    new { Name = "Moaid", FamilyName = "Hathot", Age = 35, Birthday = new DateOnly(1988, 09, 30), Birthday2 = DateTime.Parse("1988.09.30") }.Dump("Simple object");
    new { Name = "Moaid", FamilyName = "Hathot", Age = 35, Birthday = new DateOnly(1988, 09, 30), Birthday2 = DateTime.Parse("1988.09.30") }.Dump("Simple object", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
    Console.WriteLine("---------------------");
    Console.WriteLine("=== Testing Truncation ===");

    // Test array with truncation
    var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    numbers.Dump("Array with truncation", truncationConfig: new TruncationConfig { MaxCollectionCount = 5 });

    Console.WriteLine("---------------------");
    Console.WriteLine("=== Testing Horizontal Layout ===");

    // Test simple object with horizontal layout
    var testPerson = new Person
    {
        FirstName = "Moaid",
        LastName = "Hathot",
        Profession = Profession.Software
    };

    Console.WriteLine("\n--- Single object with VERTICAL (default) layout ---");
    testPerson.Dump("Vertical Layout");

    Console.WriteLine("\n--- Single object with HORIZONTAL layout ---");
    testPerson.Dump("Horizontal Layout", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

    // Test collection with horizontal layout
    var people = new[]
    {
        new Person { FirstName = "Moaid", LastName = "Hathot", Profession = Profession.Software },
        new Person { FirstName = "Haneeni", LastName = "Shibli", Profession = Profession.Health }
    };

    Console.WriteLine("\n--- Collection with VERTICAL (default) layout ---");
    people.Dump("Vertical Collection");

    Console.WriteLine("\n--- Collection with HORIZONTAL layout ---");
    people.Dump("Horizontal Collection", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
    people.ToList().Dump("Horizontal Collection List", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
    people.ToList().Dump("Horizontal Collection List with rowIndices", tableConfig: new TableConfig { TableLayout = TableLayout.Vertical, ShowRowIndices = true });

    Console.WriteLine("\n=== End Horizontal Layout Test ===");
    Console.WriteLine("---------------------");

    var moaid1 = new Person
    {
        FirstName = "Moaid",
        LastName = "Hathot",
        Profession = Profession.Software
    };

    var haneeni1 = new Person
    {
        FirstName = "Haneeni",
        LastName = "Shibli",
        Profession = Profession.Health
    };
    var lily1 = new Person
    {
        FirstName = "Lily",
        LastName = "Hathot",
        Profession = Profession.Software
    };

    new[] { moaid1, haneeni1, lily1 }.Dump("Family Members", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
    new[] { moaid1, haneeni1, lily1 }.Dump("Family Members", tableConfig: new TableConfig { TableLayout = TableLayout.Vertical });


    // moaid1.Spouse = haneeni1;
    // haneeni1.Spouse = moaid1;

    moaid1.Dump("Moaid with Spouse");
    moaid1.Dump("Horizontal Layout", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

    new Dictionary<string, Person>
    {
        ["Moaid"] = moaid1,
        ["Haneeni"] = haneeni1
    }.Dump("Dictionary of People");


    new Dictionary<string, Person>
    {
        ["Moaid"] = moaid1,
        ["Haneeni"] = haneeni1
    }.Dump("Dictionary of People", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

    new[] { moaid1, haneeni1 }.Dump("Array of People");
    new[] { moaid1, haneeni1 }.Dump("Array of People", tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

    // moaid1.Dump("Moaid");
    // new [] { moaid1, haneieni1 }.Dump();
    // moaid1.Dump("Use global");
    // moaid1.Dump("Override per dump", tableConfig: new TableConfig { BorderStyle = TableBorderStyle.Minimal, ShowRowSeparators = true });
    // moaid1.Dump("Use globali 2", maxDepth: 1);

    // Enumerable.Range(0, 10).ToDictionary(i => i).Dump("Enumerable Range", truncationConfig: new TruncationConfig { MaxCollectionCount = 3, Mode = TruncationMode.Tail });
    // Enumerable.Range(0, 10).ToDictionary(i => i).Dump("Enumerable Range", truncationConfig: new TruncationConfig { MaxCollectionCount = 3, Mode = TruncationMode.HeadAndTail });
    // Enumerable.Range(0, 10).ToDictionary(i => i).Dump("Enumerable Range", truncationConfig: new TruncationConfig { MaxCollectionCount = 3, Mode = TruncationMode.HeadAndTail, PerDimension = true });

    //
    // var lazy = new Lazy<int>(()=> 10);
    // lazy.Dump();
    // lazy.Dump("With value");
    // _ = lazy.Value;
    // lazy.Dump();
    //
    // var lazy2 = new Lazy<string>(() => null!);
    // lazy2.Dump();
    // _ = lazy2.Value;
    // lazy2.Dump();
    // ((object)null!).Dump();
    //

    // var task = Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(_ => 10);
    var task = Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(_ => 10);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    // task.Dump();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed


    // moaid1.Dump();
    // TestSpecific();
    // TestSingle();
    // ShowEverything();

    //todo: improve labels, make them work with simple objects as strings (not wrapped in other object) and consider changing colors

#pragma warning disable CS8321
#pragma warning disable CS0168

    DumpConfig.Default.AddCustomTypeHandler(typeof(byte[]), (obj, t, valueProvider, memberProvider) =>
    {
        "sdfdsf".Dump();
        return ((Byte[])obj).Take(3).ToArray();
    });

    //var foo = new { Name = "Moaid", LastName = "Hathot", Age = 35, Content = Enumerable.Range(0, 10).Select(i => (char)(i + 'a')).ToArray() };
    //foo.Dump("Test");

    // var moaid = new Person
    // {
    //     FirstName = "Moaid",
    //     LastName = "Hathot",
    //     Profession = Profession.Software
    // };
    // var haneeni = new Person
    // {
    //     FirstName = "Haneeni",
    //     LastName = "Shibli",
    //     Profession = Profession.Health
    // };
    // moaid.Spouse = haneeni;
    // haneeni.Spouse = moaid;

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
}

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

    var haneen = new Person
    {
        FirstName = "Haneen",
        LastName = "Shibli",
        Profession = Profession.Health
    };

    Person[] arr = [moaid, moaid2, haneen];
    arr.Dump();

    if (moaid.FirstName.Equals("Moaid"))
    {
        return;
    }


    // var value = SearchValues.Create("lskdjflskdfj").Dump();
    new TestVirtual().Dump();
    new TestVirtual().Dump("explcit include", members: new MembersConfig { IncludeVirtualMembers = true });
    new TestVirtual().Dump("explcit exclude", members: new MembersConfig { IncludeVirtualMembers = false });

    moaid2.Dump("Filter property Name", members: new MembersConfig { MemberFilter = ctx => ctx.Member.Name != nameof(Person.FirstName) });
    moaid2.Dump("Filter property Value", members: new MembersConfig { MemberFilter = ctx => !object.ReferenceEquals(ctx.Value, "Moaid") });
}

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

    var arr = new[] { 1, 2, 3, 4 }.Dump();
    var arr2d = new int[,]
    {
        { 1, 2 },
        { 3, 4 }
    }.Dump();

    DumpConfig.Default.TableConfig.ShowRowIndices = false;

    arr.Dump();
    arr2d.Dump();

    DumpConfig.Default.TableConfig.ShowRowIndices = true;

    moaid.Dump();

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

    typeof(Person).Dump();
    new
    {
        Properties = typeof(Person).GetProperties(),
        Methods = typeof(Person).GetMethods(),
        Fields = typeof(Person).GetFields(),
        Ctors = typeof(Person).GetConstructors(),
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

#pragma warning disable CS8321
#pragma warning disable CS0168

public class Employee
{
    public required string Name { get; set; }
    public required string Department { get; set; }
    public decimal Salary { get; set; }
}

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