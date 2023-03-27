using Dumpify.Renderers;
using Dumpify.Renderers.Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Tests.Renderers.Spectre.Console;

[TestClass]
public class BasicTests
{
    [TestMethod]
    public void HandleNullProperties()
    {
        var moaid = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathot" };

        var generator = new CompositeDescriptorGenerator(new Dictionary<RuntimeTypeHandle, Func<object, Type, System.Reflection.PropertyInfo?, object>>());
        var descriptor = generator.Generate(moaid.GetType(), null);

        var renderer = new SpectreConsoleTableRenderer();

        renderer.Render(moaid, descriptor, new RendererConfig() );
    }
}
