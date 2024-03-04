namespace Dumpify.Tests.Renderers.Spectre.Console.ExpectationTests;

public class IntegralTypesTests : BaseExpectationTests
{
    [Fact]
    public void TestInt()
    {
        Verify(2);
    }
}