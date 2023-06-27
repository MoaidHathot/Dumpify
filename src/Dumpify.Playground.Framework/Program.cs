using Dumpify.Config;

namespace Dumpify.Playground.Framework;

internal static class Program
{
    static void Main(string[] args)
    {
        DateTime.Now.Dump();
        DateTime.UtcNow.Dump();
        DateTimeOffset.Now.Dump();
        DateTimeOffset.UtcNow.Dump();
        TimeSpan.FromSeconds(10).Dump();

        var map = new Dictionary<string, string>();
        map.Add("One", "1");
        map.Add("Two", "2");
        map.Add("Three", "3");
        map.Add("Four", "4");
        map.Add("Five", "5");
        map.Dump();

        ("ItemA", "ItemB").Dump();
        Tuple.Create("ItemAA", "ItemBB").Dump();

        var moaid = new Person { FirstName = "Moaid", LastName = "Hathot", Profession = Profession.Software };
        var haneeni = new Person { FirstName = "Haneeni", LastName = "Shibli", Profession = Profession.Health };
        moaid.Spouse = haneeni;
        haneeni.Spouse = moaid;

        DumpConfig.Default.TypeNamingConfig.UseAliases = true;
        DumpConfig.Default.TypeNamingConfig.UseFullName = true;

        moaid.Dump(typeNames: new TypeNamingConfig { UseAliases = true, ShowTypeNames = false });

        moaid.Dump();
    }
}

public enum Profession { Software, Health };
public class Person
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public Person? Spouse { get; set; }

    public Profession Profession { get; set; }
}