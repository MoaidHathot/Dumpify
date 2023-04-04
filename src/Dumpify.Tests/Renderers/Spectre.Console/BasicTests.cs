using Dumpify.Renderers;
using Dumpify.Renderers.Spectre.Console;
using System.Collections.Concurrent;

namespace Dumpify.Tests.Renderers.Spectre.Console;

[TestClass]
public class BasicTests
{
    [TestMethod]
    public void HandleNullProperties()
    {
        var moaid = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathot" };

        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, System.Reflection.PropertyInfo?, object>>());
        var descriptor = generator.Generate(moaid.GetType(), null);

        var renderer = new SpectreConsoleTableRenderer();

        renderer.Render(moaid, descriptor, new RendererConfig() );
    }
}
