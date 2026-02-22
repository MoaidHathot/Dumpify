using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace Dumpify.Tests.Providers;

public class MemberProviderTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IncludeVirtualMembers(bool includeVirtualMembers)
    {
        var membersConfig = new MembersConfig { IncludeVirtualMembers = includeVirtualMembers };
        var testClass = new ClassWithVirtualProperty();

        var output = testClass.DumpText(members: membersConfig);
        var isContainsVirtualProperty = output.Contains(testClass.Bar);

        includeVirtualMembers.Should().Be(isContainsVirtualProperty);
    }

    [Theory]
    [ClassData(typeof(MemberFilterData))]
    public void ApplyMemberFilter(Func<MemberFilterContext, bool>? filter, bool mustIncludeFoo)
    {
        var membersConfig = new MembersConfig { MemberFilter = filter };
        var testClass = new ClassWithFilterableProperty();

        var output = testClass.DumpText(members: membersConfig);
        var isContainsFoo = output.Contains(testClass.Foo);

        isContainsFoo.Should().Be(mustIncludeFoo);
    }

    #region Value-Based Filtering Tests

    [Fact]
    public void MemberFilter_CanFilterByValue_ExcludeNullValues()
    {
        var testClass = new ClassWithNullableProperties
        {
            Name = "Test",
            Description = null,
            Count = 5
        };

        var membersConfig = new MembersConfig
        {
            MemberFilter = ctx => ctx.Value is not null
        };

        var output = testClass.DumpText(members: membersConfig);

        output.Should().Contain("Name");
        output.Should().Contain("Test");
        output.Should().Contain("Count");
        output.Should().NotContain("Description");
    }

    [Fact]
    public void MemberFilter_CanFilterByValue_ExcludeEmptyStrings()
    {
        var testClass = new ClassWithStringProperties
        {
            Name = "Test",
            Empty = "",
            Whitespace = "   "
        };

        var membersConfig = new MembersConfig
        {
            MemberFilter = ctx => ctx.Value is not string str || !string.IsNullOrWhiteSpace(str)
        };

        var output = testClass.DumpText(members: membersConfig);

        output.Should().Contain("Name");
        output.Should().Contain("Test");
        output.Should().NotContain("Empty");
        output.Should().NotContain("Whitespace");
    }

    [Fact]
    public void MemberFilter_CanFilterByValue_ExcludeDefaultValues()
    {
        var testClass = new ClassWithDefaultValues
        {
            Id = 0,
            Name = "Test",
            Count = 0,
            IsActive = false
        };

        var membersConfig = new MembersConfig
        {
            MemberFilter = ctx =>
            {
                var value = ctx.Value;
                if (value is null) return false;
                if (value is int i && i == 0) return false;
                if (value is bool b && b == false) return false;
                return true;
            }
        };

        var output = testClass.DumpText(members: membersConfig);

        output.Should().Contain("Name");
        output.Should().Contain("Test");
        output.Should().NotContain("Id");
        output.Should().NotContain("Count");
        output.Should().NotContain("IsActive");
    }

    [Fact]
    public void MemberFilter_CanFilterByValue_OnlyPositiveNumbers()
    {
        var testClass = new ClassWithNumbers
        {
            Positive = 10,
            Negative = -5,
            Zero = 0
        };

        var membersConfig = new MembersConfig
        {
            MemberFilter = ctx => ctx.Value is int num && num > 0
        };

        var output = testClass.DumpText(members: membersConfig);

        output.Should().Contain("Positive");
        output.Should().Contain("10");
        output.Should().NotContain("Negative");
        output.Should().NotContain("Zero");
    }

    #endregion

    #region Source Object Access Tests

    [Fact]
    public void MemberFilter_CanAccessSourceObject()
    {
        var testClass = new ClassWithFilterableProperty
        {
            Foo = "TestFoo",
            Bar = "TestBar"
        };

        object? capturedSource = null;
        var membersConfig = new MembersConfig
        {
            MemberFilter = ctx =>
            {
                capturedSource = ctx.Source;
                return true;
            }
        };

        testClass.DumpText(members: membersConfig);

        capturedSource.Should().BeSameAs(testClass);
    }

    #endregion

    #region Depth-Based Filtering Tests

    [Fact]
    public void MemberFilter_CanAccessDepth()
    {
        var testClass = new ClassWithNestedObject
        {
            Name = "Parent",
            Nested = new NestedClass { Value = "Child" }
        };

        var depthsObserved = new List<int>();
        var membersConfig = new MembersConfig
        {
            MemberFilter = ctx =>
            {
                depthsObserved.Add(ctx.Depth);
                return true;
            }
        };

        testClass.DumpText(members: membersConfig);

        // Should observe depth 0 for root object properties and depth 1 for nested object properties
        depthsObserved.Should().Contain(0);
        depthsObserved.Should().Contain(1);
    }

    [Fact]
    public void MemberFilter_CanFilterByDepth_OnlyTopLevel()
    {
        var testClass = new ClassWithNestedObject
        {
            Name = "Parent",
            Nested = new NestedClass { Value = "Child" }
        };

        var membersConfig = new MembersConfig
        {
            MemberFilter = ctx => ctx.Depth == 0 // Only show top-level properties
        };

        var output = testClass.DumpText(members: membersConfig);

        output.Should().Contain("Name");
        output.Should().Contain("Parent");
        // Nested object's Value property should not appear since it's at depth 1
        // But the Nested property itself should appear since it's at depth 0
        output.Should().Contain("Nested");
    }

    #endregion

    #region Combined Filter Tests

    [Fact]
    public void MemberFilter_CombineNameAndValueFiltering()
    {
        var testClass = new ClassWithMixedProperties
        {
            Id = 0,
            Name = "Test",
            Secret = "hidden",
            Count = 5
        };

        var membersConfig = new MembersConfig
        {
            MemberFilter = ctx =>
            {
                // Exclude properties named "Secret"
                if (ctx.Member.Name == "Secret") return false;
                // Exclude zero values
                if (ctx.Value is int i && i == 0) return false;
                return true;
            }
        };

        var output = testClass.DumpText(members: membersConfig);

        output.Should().Contain("Name");
        output.Should().Contain("Count");
        output.Should().NotContain("Secret");
        output.Should().NotContain("Id");
    }

    #endregion

    #region Test Classes

    private class ClassWithVirtualProperty
    {
        public string Foo { get; set; } = "Oleg";
        public virtual string Bar { get; set; } = "Hello";
    }

    private class ClassWithFilterableProperty
    {
        [JsonIgnore]
        public string Foo { get; set; } = "Oleg";
        public virtual string Bar { get; set; } = "Hello";
    }

    private class ClassWithNullableProperties
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Count { get; set; }
    }

    private class ClassWithStringProperties
    {
        public string Name { get; set; } = "";
        public string Empty { get; set; } = "";
        public string Whitespace { get; set; } = "";
    }

    private class ClassWithDefaultValues
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Count { get; set; }
        public bool IsActive { get; set; }
    }

    private class ClassWithNumbers
    {
        public int Positive { get; set; }
        public int Negative { get; set; }
        public int Zero { get; set; }
    }

    private class ClassWithNestedObject
    {
        public string Name { get; set; } = "";
        public NestedClass? Nested { get; set; }
    }

    private class NestedClass
    {
        public string Value { get; set; } = "";
    }

    private class ClassWithMixedProperties
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Secret { get; set; } = "";
        public int Count { get; set; }
    }

    #endregion

    #region Test Data

    private class MemberFilterData : IEnumerable<object?[]>
    {
        public IEnumerator<object?[]> GetEnumerator()
        {
            // Properties that don't have the Foo name will be included
            yield return new object?[] { new Func<MemberFilterContext, bool>(ctx => ctx.Member.Name != nameof(ClassWithFilterableProperty.Foo)), false };
            
            // Only properties that have the Foo name will be included
            yield return new object?[] { new Func<MemberFilterContext, bool>(ctx => ctx.Member.Name == nameof(ClassWithFilterableProperty.Foo)), true };
            
            // If the JsonIgnore attribute is applied, don't include the property
            yield return new object?[] { new Func<MemberFilterContext, bool>(ctx => !ctx.Member.Info.CustomAttributes.Any(a => a.AttributeType == typeof(JsonIgnoreAttribute))), false };
            
            // If there is no filter provided, filtering will not be applied
            yield return new object?[] { null, true };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    #endregion
}