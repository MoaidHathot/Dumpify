namespace Dumpify.Tests.Generators;

public class MultiValueDescriptorTests
{
    [Fact]
    public void ZArrayHasMultiValueDescriptor()
    {
        var arr = new[] { 1, 2, 3, };
        CompositeDescriptorGenerator generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());

        var descriptor = generator.Generate(arr.GetType(), null, new MemberProvider());

        descriptor.Should().NotBeNull();
        descriptor.Should().BeOfType<MultiValueDescriptor>();
        descriptor!.Type.Should().Be(typeof(int[]));

        var mvd = (MultiValueDescriptor)descriptor;
        mvd.ElementsType.Should().Be((typeof(int)));
    }
}
