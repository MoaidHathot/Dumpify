namespace Dumpify.Tests.Generators;

public class LazyDescriptorTests
{
    [Fact]
    public void LazyOfInt_ShouldBeCustomValueDescriptor()
    {
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(typeof(Lazy<int>), null, new MemberProvider());

        descriptor.Should().BeOfType<CustomDescriptor>($"Lazy<int> should be a custom value descriptor");
    }

    [Fact]
    public void LazyOfString_ShouldBeCustomValueDescriptor()
    {
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(typeof(Lazy<string>), null, new MemberProvider());

        descriptor.Should().BeOfType<CustomDescriptor>($"Lazy<string> should be a custom value descriptor");
    }

    [Fact]
    public void LazyOfComplexType_ShouldBeCustomValueDescriptor()
    {
        var generator = new CompositeDescriptorGenerator(new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>());
        var descriptor = generator.Generate(typeof(Lazy<Person>), null, new MemberProvider());

        descriptor.Should().BeOfType<CustomDescriptor>($"Lazy<Person> should be a custom value descriptor");
    }

    [Fact]
    public void Lazy_NotEvaluated_ShouldNotForceEvaluation()
    {
        // Arrange
        var wasEvaluated = false;
        var lazy = new Lazy<int>(() =>
        {
            wasEvaluated = true;
            return 42;
        });

        // Act - Dump should NOT force evaluation
        lazy.Dump();

        // Assert
        wasEvaluated.Should().BeFalse("Dumping a Lazy<T> should not force evaluation");
        lazy.IsValueCreated.Should().BeFalse("Lazy value should not have been created");
    }

    [Fact]
    public void Lazy_AlreadyEvaluated_ShouldShowValue()
    {
        // Arrange
        var lazy = new Lazy<int>(() => 42);
        _ = lazy.Value; // Force evaluation

        // Act & Assert - should not throw, value is already evaluated
        lazy.IsValueCreated.Should().BeTrue("Lazy value should have been created");
        var act = () => lazy.Dump();
        act.Should().NotThrow();
    }

    [Fact]
    public void Lazy_NotEvaluated_IsValueCreatedShouldRemainFalse()
    {
        // Arrange
        var lazy = new Lazy<string>(() => "Hello World");

        // Verify initial state
        lazy.IsValueCreated.Should().BeFalse();

        // Act
        lazy.Dump();

        // Assert - IsValueCreated should still be false
        lazy.IsValueCreated.Should().BeFalse("Dumping should not change IsValueCreated state");
    }
}
