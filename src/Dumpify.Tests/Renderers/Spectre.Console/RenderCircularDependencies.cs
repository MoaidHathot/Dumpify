using Dumpify.Config;
using Dumpify.Renderers;
using Dumpify.Renderers.Spectre.Console;
using Dumpify.Renderers.Spectre.Console.TableRenderer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Tests.Renderers.Spectre.Console;

[TestClass]
public class RenderCircularDependencies
{
    [TestMethod]
    public void RenderCircularDependenciesWithoutCrashes()
    {
        var moaid = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathot" };
        var haneeni = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathot" };

        moaid.SignificantOther = haneeni;
        haneeni.SignificantOther = moaid;

        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, System.Reflection.PropertyInfo?, object>>());

        var descriptor = generator.Generate(moaid.GetType(), null);

        var renderer = new SpectreConsoleTableRenderer();

        renderer.Render(moaid, descriptor, new RendererConfig());
    }
}
