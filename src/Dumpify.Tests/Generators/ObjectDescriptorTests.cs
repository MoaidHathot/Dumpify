namespace Dumpify.Tests.Generators;

public class ObjectDescriptorTests
{
    [Fact]
    public void PersonHasObjectDescriptor()
    {
        var person = new Person { FirstName = "Moaid", LastName = "Hathot" };
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());

        var descriptor = generator.Generate(person.GetType(), null, new MemberProvider());

        descriptor.Should().BeOfType<ObjectDescriptor>();
        descriptor!.ValueProvider.Should().BeNull();

        var objDescriptor = (ObjectDescriptor)descriptor!;

        objDescriptor.Properties.Should().NotBeNull();
        objDescriptor.Properties.Count().Should().Be(2);

        var first = objDescriptor.Properties.First();
        var last = objDescriptor.Properties.Last();

        first.Should().BeOfType<SingleValueDescriptor>();
        last.Should().BeOfType<SingleValueDescriptor>();

        first.ValueProvider!.Info.Should().BeSameAs(typeof(Person).GetProperty("FirstName"));
        last.ValueProvider!.Info.Should().BeSameAs(typeof(Person).GetProperty("LastName"));
    }

    [Fact]
    public void CircularDependencyIsHandled()
    {
        var moaid = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathot" };
        var haneeni = new PersonWithSignificantOther { FirstName = "Haneeni", LastName = "Shibli" };

        moaid.SignificantOther = haneeni;
        haneeni.SignificantOther = moaid;

        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());

        var memberProvider = new MemberProvider();
        var descriptor = generator.Generate(moaid.GetType(), null, memberProvider);

        descriptor.Should().BeOfType<ObjectDescriptor>();
        descriptor!.ValueProvider.Should().BeNull();

        var objDescriptor = (ObjectDescriptor)descriptor;

        objDescriptor.Properties.Should().NotBeNull();
        objDescriptor.Properties.Count().Should().Be(3);

        var first = objDescriptor.Properties.First();
        var last = objDescriptor.Properties.Skip(1).First();
        var other = objDescriptor.Properties.Last();

        first.Should().BeOfType<SingleValueDescriptor>();
        last.Should().BeOfType<SingleValueDescriptor>();
        other.Should().BeOfType<ObjectDescriptor>();
        other.Type.Should().Be(typeof(PersonWithSignificantOther));
    }
}