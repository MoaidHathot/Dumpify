using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Tests.Generators;

[TestClass]
public class ObjectDescriptorTests
{
    [TestMethod]
    public void PersonHasObjectDescriptor()
    {
        var person = new Person { FirstName = "Moaid", LastName = "Hathot" };
        var generator = new CompositeDescriptorGenerator(new Dictionary<RuntimeTypeHandle, Func<object, Type, System.Reflection.PropertyInfo?, object>>());

        var descriptor = generator.Generate(person.GetType(), null);

        descriptor.Should().BeOfType<ObjectDescriptor>();
        descriptor!.PropertyInfo.Should().BeNull();

        var objDescriptor = (ObjectDescriptor)descriptor!;

        objDescriptor.Properties.Should().NotBeNull();
        objDescriptor.Properties.Count().Should().Be(2);

        var first = objDescriptor.Properties.First();
        var last = objDescriptor.Properties.Last();

        first.Should().BeOfType<SingleValueDescriptor>();
        last.Should().BeOfType<SingleValueDescriptor>();

        first.PropertyInfo.Should().BeSameAs(typeof(Person).GetProperty("FirstName"));
        last.PropertyInfo.Should().BeSameAs(typeof(Person).GetProperty("LastName"));
    }

    [TestMethod]
    public void CircularDependencyIsHandled()
    {
        var moaid = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathto" };
        var haneeni = new PersonWithSignificantOther { FirstName = "Moaid", LastName = "Hathot" };

        moaid.SignificantOther = haneeni;
        haneeni.SignificantOther = moaid;

        var generator = new CompositeDescriptorGenerator(new Dictionary<RuntimeTypeHandle, Func<object, Type, System.Reflection.PropertyInfo?, object>>());

        var descriptor = generator.Generate(moaid.GetType(), null);

        descriptor.Should().BeOfType<ObjectDescriptor>();
        descriptor!.PropertyInfo.Should().BeNull();

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
