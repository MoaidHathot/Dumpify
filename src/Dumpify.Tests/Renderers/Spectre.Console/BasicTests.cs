using Dumpify.Descriptors.ValueProviders;
using Dumpify.Renderers;
using Dumpify.Renderers.Spectre.Console;
using Dumpify.Renderers.Spectre.Console.TableRenderer;
using Dumpify.Tests.DTO;
using System.Collections.Concurrent;

namespace Dumpify.Tests.Renderers.Spectre.Console;

[TestClass]
public class BasicTests
{
    [TestMethod]
    public void HandleNullProperties()
    {
        var moaid = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathot" };

        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(moaid.GetType(), null, new MemberProvider());

        var renderer = new SpectreConsoleTableRenderer();

        renderer.Render(moaid, descriptor, new RendererConfig(){ MemberProvider = new MemberProvider(), TypeNameProvider = new TypeNameProvider(true, false) });
    }
}
