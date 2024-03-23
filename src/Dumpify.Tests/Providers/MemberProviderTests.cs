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
    public void ApplyMemberFilter(Func<IValueProvider, bool>? filter, bool mustIncludeFoo)
    {
        var membersConfig = new MembersConfig { MemberFilter = filter };
        var testClass = new ClassWithFilterableProperty();

        var output = testClass.DumpText(members: membersConfig);
        var isContainsFoo = output.Contains(testClass.Foo);

        isContainsFoo.Should().Be(mustIncludeFoo);
    }

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

    private class MemberFilterData : IEnumerable<object?[]>
    {
        public IEnumerator<object?[]> GetEnumerator()
        {
            // Properties that don't have the Foo name will be included
            yield return new object?[] { new Func<IValueProvider, bool>(member => member.Info.Name != nameof(ClassWithFilterableProperty.Foo)), false };
            
            // Only properties that have the Foo name will be included
            yield return new object?[] { new Func<IValueProvider, bool>(member => member.Info.Name == nameof(ClassWithFilterableProperty.Foo)), true };
            
            // If the JsonIgnore attribute is applied, don't include the property
            yield return new object?[] { new Func<IValueProvider, bool>(member => !member.Info.CustomAttributes.Any(a => a.AttributeType == typeof(JsonIgnoreAttribute))), false };
            
            // If there is no filter provided, filtering will not be applied
            yield return new object?[] { null, true };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}