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

    private class ClassWithVirtualProperty
    {
        public string Foo { get; set; } = "Oleg";
        public virtual string Bar { get; set; } = "Hello";
    }
}