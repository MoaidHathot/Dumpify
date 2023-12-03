using System.Reflection;
using System.Text;

namespace Dumpify.Tests.Generators;

public class CustomDescriptorTests
{
    [Theory]
    [InlineData(typeof(StringBuilder))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(DateOnly))]
    [InlineData(typeof(TimeOnly))]
    [InlineData(typeof(TimeSpan))]
    [InlineData(typeof(Type))]
    [InlineData(typeof(PropertyInfo))]
    public void ShouldBeCustomValueDescriptor(Type type)
    {
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(type, null, new MemberProvider());

        descriptor.Should().BeOfType<CustomDescriptor>($"{type.FullName} is a custom value", descriptor);
    }

    [Theory]
    [InlineData(typeof(BookStatus))]
    public void EnumsShouldBeCustomValueDescriptor(Type type)
    {

        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(type, null, new MemberProvider());

        descriptor.Should().BeOfType<CustomDescriptor>($"{type.FullName} is a custom value", descriptor);
    }
}
