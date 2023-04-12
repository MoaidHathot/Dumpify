using Dumpify.Descriptors.ValueProviders;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

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
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(type, null, new MemberProvider());

        descriptor.Should().BeOfType<CustomDescriptor>($"{type.FullName} is a custom value", descriptor);
    }

    [TestMethod]
    [DataRow(typeof(BookStatus))]
    public void EnumsShouldBeCustomValueDescriptor(Type type)
    {

        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(type, null, new MemberProvider());

        descriptor.Should().BeOfType<CustomDescriptor>($"{type.FullName} is a custom value", descriptor);
    }
}
