using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Tests.Generators;

[TestClass]
public class MultiValueDescriptorTests
{
    [TestMethod]
    public void ZArrayHasMultiValueDescriptor()
    {
        var arr = new[] { 1, 2, 3, };
        var generator = new CompositeDescriptorGenerator(new Dictionary<RuntimeTypeHandle, Func<object, Type, System.Reflection.PropertyInfo?, object>>());

        var descriptor = generator.Generate(arr.GetType(), null);

        descriptor.Should().NotBeNull();
        descriptor.Should().BeOfType<MultiValueDescriptor>();
        descriptor!.Type.Should().Be(typeof(int[]));

        var mvd = (MultiValueDescriptor)descriptor;
        mvd.ElementsType.Should().Be((typeof(int)));
    }
}
