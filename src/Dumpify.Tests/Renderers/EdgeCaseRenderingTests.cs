using Dumpify.Tests.DTO;
using static VerifyXunit.Verifier;

namespace Dumpify.Tests.Renderers;

/// <summary>
/// Snapshot tests for edge cases in rendering including circular references,
/// max depth limits, null handling, truncation, and special object scenarios.
/// </summary>
public class EdgeCaseRenderingTests
{
    #region Test Models

    private class SelfReferencing
    {
        public string Name { get; set; } = "Self";
        public SelfReferencing? Self { get; set; }
    }

    private class Node
    {
        public string Value { get; set; } = "";
        public Node? Left { get; set; }
        public Node? Right { get; set; }
    }

    private class Address
    {
        public string City { get; set; } = "";
        public string Street { get; set; } = "";
    }

    private class PersonWithTwoAddresses
    {
        public string Name { get; set; } = "";
        public Address? HomeAddress { get; set; }
        public Address? WorkAddress { get; set; }
    }

    private class DeepNesting
    {
        public string Level { get; set; } = "";
        public DeepNesting? Child { get; set; }
    }

    private class AllNullProperties
    {
        public string? Name { get; set; }
        public int? Age { get; set; }
        public object? Data { get; set; }
        public List<string>? Items { get; set; }
    }

    private class MixedNullProperties
    {
        public string Name { get; set; } = "Test";
        public string? NullableName { get; set; }
        public int Age { get; set; } = 25;
        public int? NullableAge { get; set; }
    }

    private class EmptyCollectionProperties
    {
        public List<int> EmptyList { get; set; } = new();
        public int[] EmptyArray { get; set; } = Array.Empty<int>();
        public Dictionary<string, int> EmptyDictionary { get; set; } = new();
        public HashSet<string> EmptyHashSet { get; set; } = new();
    }

    private class SpecialStringProperties
    {
        public string Empty { get; set; } = "";
        public string Whitespace { get; set; } = "   ";
        public string WithNewlines { get; set; } = "Line1\nLine2\nLine3";
        public string WithTabs { get; set; } = "Col1\tCol2\tCol3";
        public string WithQuotes { get; set; } = "He said \"Hello\"";
        public string Unicode { get; set; } = "Hello 世界 🌍";
    }

    private class LargeObject
    {
        public string Prop1 { get; set; } = "Value1";
        public string Prop2 { get; set; } = "Value2";
        public string Prop3 { get; set; } = "Value3";
        public string Prop4 { get; set; } = "Value4";
        public string Prop5 { get; set; } = "Value5";
        public string Prop6 { get; set; } = "Value6";
        public string Prop7 { get; set; } = "Value7";
        public string Prop8 { get; set; } = "Value8";
        public string Prop9 { get; set; } = "Value9";
        public string Prop10 { get; set; } = "Value10";
    }

    private struct LargeStruct
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public string Name { get; set; }
    }

    #endregion

    #region Circular Reference Tests

    [Fact]
    public Task CircularReference_SelfReferencing()
    {
        var obj = new SelfReferencing { Name = "Root" };
        obj.Self = obj;

        var result = obj.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task CircularReference_MutualReference()
    {
        var person1 = new PersonWithSignificantOther { FirstName = "Alice", LastName = "Smith" };
        var person2 = new PersonWithSignificantOther { FirstName = "Bob", LastName = "Jones" };
        person1.SignificantOther = person2;
        person2.SignificantOther = person1;

        var result = person1.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task CircularReference_TreeWithBackReference()
    {
        var root = new Node { Value = "Root" };
        var left = new Node { Value = "Left" };
        var right = new Node { Value = "Right" };
        
        root.Left = left;
        root.Right = right;
        left.Right = root; // Back reference to root

        var result = root.DumpText();
        return Verify(result);
    }

    #endregion

    #region Shared Reference Tests (Not Cycles)

    [Fact]
    public Task SharedReference_SameObjectInTwoProperties_ShouldNotBeCycle()
    {
        var sharedAddress = new Address { City = "NYC", Street = "Broadway" };
        var person = new PersonWithTwoAddresses
        {
            Name = "John",
            HomeAddress = sharedAddress,
            WorkAddress = sharedAddress  // Same instance, NOT a cycle
        };

        var result = person.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task SharedReference_InCollection_ShouldNotBeCycle()
    {
        var sharedNode = new Node { Value = "Shared" };
        var items = new[] { sharedNode, sharedNode, sharedNode };

        var result = items.DumpText();
        return Verify(result);
    }

    #endregion

    #region Max Depth Tests

    [Fact]
    public Task MaxDepth_LimitedTo1()
    {
        var obj = CreateDeepNesting(5);
        var result = obj.DumpText(maxDepth: 1);
        return Verify(result);
    }

    [Fact]
    public Task MaxDepth_LimitedTo2()
    {
        var obj = CreateDeepNesting(5);
        var result = obj.DumpText(maxDepth: 2);
        return Verify(result);
    }

    [Fact]
    public Task MaxDepth_LimitedTo3()
    {
        var obj = CreateDeepNesting(5);
        var result = obj.DumpText(maxDepth: 3);
        return Verify(result);
    }

    [Fact]
    public Task MaxDepth_NestedCollections()
    {
        var data = new
        {
            Level1 = new
            {
                Level2 = new
                {
                    Level3 = new
                    {
                        Items = new[] { 1, 2, 3 }
                    }
                }
            }
        };

        var result = data.DumpText(maxDepth: 2);
        return Verify(result);
    }

    private static DeepNesting CreateDeepNesting(int depth)
    {
        var root = new DeepNesting { Level = "Level 1" };
        var current = root;
        
        for (int i = 2; i <= depth; i++)
        {
            current.Child = new DeepNesting { Level = $"Level {i}" };
            current = current.Child;
        }
        
        return root;
    }

    #endregion

    #region Null Handling Tests

    [Fact]
    public Task Null_DirectNull()
    {
        object? obj = null;
        var result = obj.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Null_AllNullProperties()
    {
        var obj = new AllNullProperties();
        var result = obj.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Null_MixedNullAndNonNullProperties()
    {
        var obj = new MixedNullProperties
        {
            Name = "Test",
            NullableName = null,
            Age = 30,
            NullableAge = null
        };
        var result = obj.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Null_NullInArray()
    {
        var arr = new string?[] { "first", null, "third", null, "fifth" };
        var result = arr.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Null_NullInList()
    {
        var list = new List<object?> { 1, null, "text", null, 3.14 };
        var result = list.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Null_NullInDictionary()
    {
        var dict = new Dictionary<string, string?>
        {
            ["key1"] = "value1",
            ["key2"] = null,
            ["key3"] = "value3"
        };
        var result = dict.DumpText();
        return Verify(result);
    }

    #endregion

    #region Collection Truncation Tests

    [Fact]
    public Task Truncation_ArrayTruncatedAt5()
    {
        var arr = Enumerable.Range(1, 20).ToArray();
        var result = arr.DumpText(truncationConfig: new TruncationConfig { MaxCollectionCount = 5 });
        return Verify(result);
    }

    [Fact]
    public Task Truncation_ListTruncatedAt3()
    {
        var list = Enumerable.Range(1, 10).ToList();
        var result = list.DumpText(truncationConfig: new TruncationConfig { MaxCollectionCount = 3 });
        return Verify(result);
    }

    [Fact]
    public Task Truncation_DictionaryTruncatedAt2()
    {
        // Use SortedDictionary to ensure deterministic ordering across platforms
        var dict = new SortedDictionary<string, int>(
            Enumerable.Range(1, 8).ToDictionary(x => $"Key{x}", x => x * 10));
        var result = dict.DumpText(truncationConfig: new TruncationConfig { MaxCollectionCount = 2 });
        return Verify(result);
    }

    [Fact]
    public Task Truncation_NoTruncationWhenUnderLimit()
    {
        var arr = new[] { 1, 2, 3 };
        var result = arr.DumpText(truncationConfig: new TruncationConfig { MaxCollectionCount = 10 });
        return Verify(result);
    }

    [Fact]
    public Task Truncation_ExactlyAtLimit()
    {
        var arr = new[] { 1, 2, 3, 4, 5 };
        var result = arr.DumpText(truncationConfig: new TruncationConfig { MaxCollectionCount = 5 });
        return Verify(result);
    }

    #endregion

    #region Empty Collection Tests

    [Fact]
    public Task Empty_EmptyArray()
    {
        var arr = Array.Empty<int>();
        var result = arr.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Empty_EmptyList()
    {
        var list = new List<string>();
        var result = list.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Empty_EmptyDictionary()
    {
        var dict = new Dictionary<string, int>();
        var result = dict.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Empty_ObjectWithEmptyCollections()
    {
        var obj = new EmptyCollectionProperties();
        var result = obj.DumpText();
        return Verify(result);
    }

    #endregion

    #region Special String Tests

    [Fact]
    public Task SpecialStrings_EmptyAndWhitespace()
    {
        var obj = new SpecialStringProperties();
        var result = obj.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task SpecialStrings_VeryLongString()
    {
        var longString = new string('x', 500);
        var result = longString.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task SpecialStrings_MultilineString()
    {
        var multiline = @"Line 1
Line 2
Line 3
    Indented Line 4";
        var result = multiline.DumpText();
        return Verify(result);
    }

    #endregion

    #region Numeric Edge Cases

    [Fact]
    public Task Numeric_ExtremeValues()
    {
        var extremes = new
        {
            MaxInt = int.MaxValue,
            MinInt = int.MinValue,
            MaxLong = long.MaxValue,
            MinLong = long.MinValue,
            MaxDouble = double.MaxValue,
            MinDouble = double.MinValue,
            Epsilon = double.Epsilon,
            PositiveInfinity = double.PositiveInfinity,
            NegativeInfinity = double.NegativeInfinity,
            NaN = double.NaN
        };
        var result = extremes.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Numeric_DecimalPrecision()
    {
        var decimals = new
        {
            SmallDecimal = 0.000000001m,
            LargeDecimal = 99999999999999.99m,
            NegativeDecimal = -123456.789m,
            WholeDecimal = 100m
        };
        var result = decimals.DumpText();
        return Verify(result);
    }

    #endregion

    #region Struct and Value Type Tests

    [Fact]
    public Task Struct_SimpleStruct()
    {
        var point = new LargeStruct { X = 10, Y = 20, Z = 30, Name = "Origin" };
        var result = point.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Struct_NullableStructWithValue()
    {
        int? nullable = 42;
        var result = nullable.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Struct_NullableStructWithoutValue()
    {
        int? nullable = null;
        var result = nullable.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Struct_DateTimeValues()
    {
        var dates = new
        {
            MinValue = DateTime.MinValue,
            MaxValue = DateTime.MaxValue,
            SpecificDate = new DateTime(2024, 6, 15, 14, 30, 45)
        };
        var result = dates.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task Struct_GuidValues()
    {
        var guids = new
        {
            Empty = Guid.Empty,
            Specific = new Guid("12345678-1234-1234-1234-123456789012")
        };
        var result = guids.DumpText();
        return Verify(result);
    }

    #endregion

    #region Large Object Tests

    [Fact]
    public Task LargeObject_ManyProperties()
    {
        var obj = new LargeObject();
        var result = obj.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task LargeObject_DeeplyNestedAnonymous()
    {
        var obj = new
        {
            A = new
            {
                B = new
                {
                    C = new
                    {
                        D = new
                        {
                            Value = "Deep"
                        }
                    }
                }
            }
        };
        var result = obj.DumpText();
        return Verify(result);
    }

    #endregion

    #region Mixed Type Tests

    [Fact]
    public Task MixedTypes_ObjectArray()
    {
        var arr = new object[] { 1, "two", 3.0, true, null!, new { X = 1 } };
        var result = arr.DumpText();
        return Verify(result);
    }

    [Fact]
    public Task MixedTypes_NestedCollections()
    {
        var nested = new Dictionary<string, object>
        {
            ["numbers"] = new[] { 1, 2, 3 },
            ["strings"] = new List<string> { "a", "b", "c" },
            ["mixed"] = new object[] { 1, "two", 3.0 }
        };
        var result = nested.DumpText();
        return Verify(result);
    }

    #endregion
}
