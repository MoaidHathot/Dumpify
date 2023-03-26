using Dumpify.Renderers;
using Dumpify.Renderers.Spectre.Console;
using System;
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
        var moaid = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathto" };
        var haneeni = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathot" };

        moaid.SignificantOther = haneeni;
        haneeni.SignificantOther = moaid;

        var generator = new CompositeDescriptorGenerator();

        var descriptor = generator.Generate(moaid.GetType(), null);

        var renderer = new SpectreTableRenderer();

        renderer.Render(moaid, descriptor, new RendererConfig());
    }
}
