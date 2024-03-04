
namespace Dumpify.Tests.Renderers.Spectre.Console.ExpectationTests;

public class BaseExpectationTests : IAsyncLifetime
{
    public BaseExpectationTests()
    {
    }

    public void Verify(object obj)
    {
        using var writer = new StringWriter();

        obj.Dump(output: new DumpOutput(writer));
        Verifier.Verify(writer.ToString());
    }

    public virtual Task InitializeAsync()
        => Task.CompletedTask;

    public Task DisposeAsync()
        => Task.CompletedTask;
}