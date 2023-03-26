using Dumpify;
using System.Collections;
using System.Text;
using System.Text.Json;

var moaid = new Person { FirstName = "Moaid", LastName = "Hathot" };
var haneeni = new Person { FirstName = "Haneeni", LastName = "Shibli" };

moaid.Other = haneeni;
haneeni.Other = moaid;

var family = new Family
{
    Parent1 = moaid,
    Parent2 = haneeni,
    FamilyId = 42,
    ChildrenArray = new[] { new Person { FirstName = "Child1", LastName = "Hathot" }, new Person { FirstName = "Child2", LastName = "Hathot", Other = new Person { FirstName = "Child22", LastName = "Hathot", Other = new Person { FirstName = "Child222", LastName = "Hathot" } } } },
    ChildrenList = new List<Person> { new Person { FirstName = "Child1", LastName = "Hathot" }, new Person { FirstName = "Child2", LastName = "Hathot" } },
    ChildrenArrayList = new ArrayList { new Person { FirstName = "Child1", LastName = "Hathot" }, new Person { FirstName = "Child2", LastName = "Hathot" } },
    FamilyType = typeof(Family),
};

var arr = new[] { 1, 2, 3, 4 };

moaid.Dump(maxNestingLevel: 1);
family.Dump(maxNestingLevel: 2);
arr.Dump();
//((object)null).Dump();

//var result = DumpConfig.Default.Generator.Generate(family.GetType(), null);
;
//JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });

public class Person
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public Person? Other { get; set; }
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
}