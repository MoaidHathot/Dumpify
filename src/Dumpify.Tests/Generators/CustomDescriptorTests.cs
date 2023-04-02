using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Tests.Generators;

[TestClass]
public class CustomDescriptorTests
{
    [TestMethod]
    [DataRow(typeof(StringBuilder))]
    [DataRow(typeof(DateTime))]
    [DataRow(typeof(DateTimeOffset))]
    [DataRow(typeof(DateOnly))]
    [DataRow(typeof(TimeOnly))]
    [DataRow(typeof(TimeSpan))]
    [DataRow(typeof(Type))]
    [DataRow(typeof(PropertyInfo))]
    public void ShouldBeCustomValueDescriptor(Type type)
    {
        var generator = new CompositeDescriptorGenerator(new Dictionary<RuntimeTypeHandle, Func<object, Type, System.Reflection.PropertyInfo?, object>>());
        var descriptor = generator.Generate(type, null);

        descriptor.Should().BeOfType<CustomDescriptor>($"{type.FullName} is a custom value", descriptor);
    }
}
