using Dumpify.Tests.DTO;
using VerifyXunit;
using static VerifyXunit.Verifier;

namespace Dumpify.Tests.Renderers.Spectre.Console;

/// <summary>
/// Snapshot tests for circular dependency handling in Spectre.Console renderer.
/// </summary>
public class RenderCircularDependencies
{
    [Fact]
    public Task CircularDependency_TwoPersonsMutuallyReferencing()
    {
        var moaid = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathot" };
        var haneeni = new PersonWithSignificantOther { FirstName = "Haneeni", LastName = "Hathot" };

        moaid.SignificantOther = haneeni;
        haneeni.SignificantOther = moaid;

        return Verify(moaid.DumpText());
    }

    [Fact]
    public Task CircularDependency_SelfReferencing()
    {
        var person = new PersonWithSignificantOther { FirstName = "Self", LastName = "Reference" };
        person.SignificantOther = person;

        return Verify(person.DumpText());
    }

    [Fact]
    public Task CircularDependency_ThreeWayChain()
    {
        var a = new PersonWithSignificantOther { FirstName = "Person", LastName = "A" };
        var b = new PersonWithSignificantOther { FirstName = "Person", LastName = "B" };
        var c = new PersonWithSignificantOther { FirstName = "Person", LastName = "C" };

        a.SignificantOther = b;
        b.SignificantOther = c;
        c.SignificantOther = a; // Creates cycle A -> B -> C -> A

        return Verify(a.DumpText());
    }
}
