using System.Text;

namespace Dumpify.Tests.Generators;

public class SingleValueDescriptorTests
{
    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(byte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(long))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(Half))]
    public void ShouldBeSingleValueDescriptor(Type type)
    {
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(type, null, new MemberProvider());

        descriptor.Should().BeOfType<SingleValueDescriptor>($"{type.FullName} is a single value", descriptor);
    }

    [Theory]
    [InlineData(typeof(StringBuilder))]
    [InlineData(typeof(Person))]
    [InlineData(typeof(List<int>))]
    [InlineData(typeof(string[]))]
    [InlineData(typeof(int[]))]
    public void ShouldNotBeSingleValueDescriptor(Type type)
    {
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(type, null, new MemberProvider());

        descriptor.Should().NotBeOfType<SingleValueDescriptor>($"{type.FullName} is not a single value", descriptor);
    }
}