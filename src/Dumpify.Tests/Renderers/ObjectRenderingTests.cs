using VerifyXunit;
using static VerifyXunit.Verifier;

namespace Dumpify.Tests.Renderers;

/// <summary>
/// Snapshot tests for complex object rendering.
/// Tests classes, records, anonymous types, and nested objects.
/// </summary>
public class ObjectRenderingTests
{
    #region Simple Classes

    [Fact]
    public Task SimpleClass_Renders()
    {
        var obj = new SimpleClass { Id = 1, Name = "Test" };
        return Verify(obj.DumpText());
    }

    [Fact]
    public Task ClassWithAllPropertyTypes_Renders()
    {
        var obj = new ClassWithAllPropertyTypes
        {
            IntProperty = 42,
            StringProperty = "Hello",
            BoolProperty = true,
            DoubleProperty = 3.14,
            DateTimeProperty = new DateTime(2024, 1, 15, 10, 30, 0),
            GuidProperty = new Guid("12345678-1234-1234-1234-123456789abc")
        };
        return Verify(obj.DumpText());
    }

    [Fact]
    public Task ClassWithNullProperty_Renders()
    {
        var obj = new SimpleClass { Id = 1, Name = null };
        return Verify(obj.DumpText());
    }

    #endregion

    #region Records

    [Fact]
    public Task Record_Renders()
    {
        var record = new PersonRecord("John", "Doe", 30);
        return Verify(record.DumpText());
    }

    [Fact]
    public Task RecordStruct_Renders()
    {
        var record = new PointRecordStruct(10, 20);
        return Verify(record.DumpText());
    }

    #endregion

    #region Anonymous Types

    [Fact]
    public Task AnonymousType_Renders()
    {
        var anon = new { Name = "Test", Value = 42, Active = true };
        return Verify(anon.DumpText());
    }

    [Fact]
    public Task AnonymousTypeNested_Renders()
    {
        var anon = new 
        { 
            Person = new { Name = "John", Age = 30 },
            Address = new { City = "Seattle", Country = "USA" }
        };
        return Verify(anon.DumpText());
    }

    #endregion

    #region Nested Objects

    [Fact]
    public Task NestedClass_Renders()
    {
        var obj = new Order
        {
            OrderId = 1001,
            Customer = new Customer { CustomerId = 1, Name = "John Doe" },
            Items = new List<OrderItem>
            {
                new OrderItem { ProductName = "Widget", Quantity = 2, Price = 9.99m },
                new OrderItem { ProductName = "Gadget", Quantity = 1, Price = 19.99m }
            }
        };
        return Verify(obj.DumpText());
    }

    [Fact]
    public Task DeeplyNestedObject_Renders()
    {
        var obj = new Level1
        {
            Name = "L1",
            Child = new Level2
            {
                Name = "L2",
                Child = new Level3
                {
                    Name = "L3",
                    Value = 42
                }
            }
        };
        return Verify(obj.DumpText());
    }

    #endregion

    #region Structs

    [Fact]
    public Task Struct_Renders()
    {
        var point = new Point { X = 10, Y = 20 };
        return Verify(point.DumpText());
    }

    [Fact]
    public Task ReadOnlyStruct_Renders()
    {
        var point = new ReadOnlyPoint(5, 15);
        return Verify(point.DumpText());
    }

    #endregion

    #region Inheritance

    [Fact]
    public Task InheritedClass_Renders()
    {
        var employee = new Employee 
        { 
            Id = 1, 
            Name = "Jane Doe", 
            Department = "Engineering",
            EmployeeId = "E001"
        };
        return Verify(employee.DumpText());
    }

    #endregion

    #region Collections as Properties

    [Fact]
    public Task ClassWithListProperty_Renders()
    {
        var obj = new ClassWithCollection
        {
            Name = "Test",
            Items = new List<string> { "apple", "banana", "cherry" }
        };
        return Verify(obj.DumpText());
    }

    [Fact]
    public Task ClassWithDictionaryProperty_Renders()
    {
        var obj = new ClassWithDictionary
        {
            Name = "Config",
            Settings = new Dictionary<string, int>
            {
                ["timeout"] = 30,
                ["retries"] = 3
            }
        };
        return Verify(obj.DumpText());
    }

    #endregion

    #region Special Cases

    [Fact]
    public Task ClassWithReadOnlyProperty_Renders()
    {
        var obj = new ClassWithReadOnlyProperty("Initial Value");
        return Verify(obj.DumpText());
    }

    [Fact]
    public Task ClassWithPrivateSetters_Renders()
    {
        var obj = ClassWithPrivateSetters.Create(42, "Secret");
        return Verify(obj.DumpText());
    }

    [Fact]
    public Task ObjectWithToString_Renders()
    {
        var obj = new ClassWithToString { Id = 1, Name = "Test" };
        return Verify(obj.DumpText());
    }

    #endregion

    #region Test Helper Classes

    private class SimpleClass
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private class ClassWithAllPropertyTypes
    {
        public int IntProperty { get; set; }
        public string? StringProperty { get; set; }
        public bool BoolProperty { get; set; }
        public double DoubleProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public Guid GuidProperty { get; set; }
    }

    private record PersonRecord(string FirstName, string LastName, int Age);

    private record struct PointRecordStruct(int X, int Y);

    private class Order
    {
        public int OrderId { get; set; }
        public Customer? Customer { get; set; }
        public List<OrderItem>? Items { get; set; }
    }

    private class Customer
    {
        public int CustomerId { get; set; }
        public string? Name { get; set; }
    }

    private class OrderItem
    {
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    private class Level1
    {
        public string? Name { get; set; }
        public Level2? Child { get; set; }
    }

    private class Level2
    {
        public string? Name { get; set; }
        public Level3? Child { get; set; }
    }

    private class Level3
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }

    private struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    private readonly struct ReadOnlyPoint
    {
        public int X { get; }
        public int Y { get; }

        public ReadOnlyPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    private class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private class Employee : Person
    {
        public string? Department { get; set; }
        public string? EmployeeId { get; set; }
    }

    private class ClassWithCollection
    {
        public string? Name { get; set; }
        public List<string>? Items { get; set; }
    }

    private class ClassWithDictionary
    {
        public string? Name { get; set; }
        public Dictionary<string, int>? Settings { get; set; }
    }

    private class ClassWithReadOnlyProperty
    {
        public string ReadOnlyValue { get; }

        public ClassWithReadOnlyProperty(string value)
        {
            ReadOnlyValue = value;
        }
    }

    private class ClassWithPrivateSetters
    {
        public int Id { get; private set; }
        public string? Secret { get; private set; }

        public static ClassWithPrivateSetters Create(int id, string secret)
        {
            return new ClassWithPrivateSetters { Id = id, Secret = secret };
        }
    }

    private class ClassWithToString
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public override string ToString() => $"[{Id}] {Name}";
    }

    #endregion
}
