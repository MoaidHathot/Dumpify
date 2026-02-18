using Dumpify.Tests.DTO;
using VerifyXunit;
using static VerifyXunit.Verifier;

namespace Dumpify.Tests.Renderers.Spectre.Console;

/// <summary>
/// Basic snapshot tests for Spectre.Console table renderer.
/// </summary>
public class BasicTests
{
    [Fact]
    public Task HandleNullProperties()
    {
        var person = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathot" };
        return Verify(person.DumpText());
    }
}
