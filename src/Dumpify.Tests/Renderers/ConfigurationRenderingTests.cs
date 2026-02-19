using VerifyXunit;
using static VerifyXunit.Verifier;

namespace Dumpify.Tests.Renderers;

public class ConfigurationRenderingTests
{
    #region Test Data
    
    private record Person(string Name, int Age, string? Email = null);
    private record Address(string Street, string City, int ZipCode);
    private record PersonWithAddress(string Name, Address HomeAddress);
    private record PersonWithNullables(string Name, int Age, string? Email);
    
    private class PersonWithDefaults
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public double Score { get; set; }
    }
    
    private class PersonWithStrings
    {
        public string Name { get; set; } = "";
        public string Nickname { get; set; } = "";
        public string Title { get; set; } = "";
    }
    
    private class PersonWithSensitive
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public string Password { get; set; } = "";
        public string Token { get; set; } = "";
    }
    
    private class ClassWithPrivateMembers
    {
        public string PublicProperty { get; set; } = "Public";
        private string PrivateProperty { get; set; } = "Private";
        protected string ProtectedProperty { get; set; } = "Protected";
        internal string InternalProperty { get; set; } = "Internal";
        public string PublicField = "PublicField";
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private string PrivateField = "PrivateField";
#pragma warning restore CS0414
    }
    
    private class ClassWithVirtualMembers
    {
        public virtual string VirtualProperty { get; set; } = "Virtual";
        public string NonVirtualProperty { get; set; } = "NonVirtual";
    }
    
    #endregion

    #region TableConfig Tests
    
    [Fact]
    public Task TableConfig_ShowArrayIndices_True()
    {
        var data = new[] { "first", "second", "third" };
        var output = data.DumpText(tableConfig: new TableConfig { ShowArrayIndices = true });
        return Verify(output);
    }
    
    [Fact]
    public Task TableConfig_ShowArrayIndices_False()
    {
        var data = new[] { "first", "second", "third" };
        var output = data.DumpText(tableConfig: new TableConfig { ShowArrayIndices = false });
        return Verify(output);
    }
    
    [Fact]
    public Task TableConfig_ShowTableHeaders_True()
    {
        var data = new Person("Alice", 30);
        var output = data.DumpText(tableConfig: new TableConfig { ShowTableHeaders = true });
        return Verify(output);
    }
    
    [Fact]
    public Task TableConfig_ShowTableHeaders_False()
    {
        var data = new Person("Alice", 30);
        var output = data.DumpText(tableConfig: new TableConfig { ShowTableHeaders = false });
        return Verify(output);
    }
    
    [Fact]
    public Task TableConfig_ShowMemberTypes_True()
    {
        var data = new Person("Alice", 30, "alice@example.com");
        var output = data.DumpText(tableConfig: new TableConfig { ShowMemberTypes = true });
        return Verify(output);
    }
    
    [Fact]
    public Task TableConfig_ShowMemberTypes_False()
    {
        var data = new Person("Alice", 30, "alice@example.com");
        var output = data.DumpText(tableConfig: new TableConfig { ShowMemberTypes = false });
        return Verify(output);
    }
    
    [Fact]
    public Task TableConfig_ShowRowSeparators_True()
    {
        var data = new[] { new Person("Alice", 30), new Person("Bob", 25) };
        var output = data.DumpText(tableConfig: new TableConfig { ShowRowSeparators = true });
        return Verify(output);
    }
    
    [Fact]
    public Task TableConfig_ShowRowSeparators_False()
    {
        var data = new[] { new Person("Alice", 30), new Person("Bob", 25) };
        var output = data.DumpText(tableConfig: new TableConfig { ShowRowSeparators = false });
        return Verify(output);
    }
    
    [Fact]
    public Task TableConfig_MaxCollectionCount_LimitedTo2()
    {
        var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var output = data.DumpText(tableConfig: new TableConfig { MaxCollectionCount = 2 });
        return Verify(output);
    }
    
    [Fact]
    public Task TableConfig_MaxCollectionCount_LimitedTo5()
    {
        var data = Enumerable.Range(1, 20).ToArray();
        var output = data.DumpText(tableConfig: new TableConfig { MaxCollectionCount = 5 });
        return Verify(output);
    }
    
    [Fact]
    public Task TableConfig_MaxCollectionCount_Unlimited()
    {
        var data = new[] { 1, 2, 3, 4, 5 };
        var output = data.DumpText(tableConfig: new TableConfig { MaxCollectionCount = int.MaxValue });
        return Verify(output);
    }
    
    [Fact]
    public Task TableConfig_CombinedOptions()
    {
        var data = new[] { new Person("Alice", 30), new Person("Bob", 25), new Person("Charlie", 35) };
        var config = new TableConfig
        {
            ShowArrayIndices = true,
            ShowTableHeaders = true,
            ShowMemberTypes = true,
            ShowRowSeparators = true,
            MaxCollectionCount = 2
        };
        var output = data.DumpText(tableConfig: config);
        return Verify(output);
    }
    
    #endregion
    
    #region MembersConfig Tests
    
    [Fact]
    public Task MembersConfig_IncludePublicMembers_Only()
    {
        var data = new ClassWithPrivateMembers();
        var output = data.DumpText(members: new MembersConfig 
        { 
            IncludePublicMembers = true,
            IncludeNonPublicMembers = false,
            IncludeFields = false
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_IncludeNonPublicMembers()
    {
        var data = new ClassWithPrivateMembers();
        var output = data.DumpText(members: new MembersConfig 
        { 
            IncludePublicMembers = true,
            IncludeNonPublicMembers = true,
            IncludeFields = false
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_IncludeFields()
    {
        var data = new ClassWithPrivateMembers();
        var output = data.DumpText(members: new MembersConfig 
        { 
            IncludePublicMembers = true,
            IncludeNonPublicMembers = false,
            IncludeFields = true
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_IncludeAllMembersAndFields()
    {
        var data = new ClassWithPrivateMembers();
        var output = data.DumpText(members: new MembersConfig 
        { 
            IncludePublicMembers = true,
            IncludeNonPublicMembers = true,
            IncludeFields = true
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_ExcludeVirtualMembers()
    {
        var data = new ClassWithVirtualMembers();
        var output = data.DumpText(members: new MembersConfig 
        { 
            IncludeVirtualMembers = false
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_IncludeVirtualMembers()
    {
        var data = new ClassWithVirtualMembers();
        var output = data.DumpText(members: new MembersConfig 
        { 
            IncludeVirtualMembers = true
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_ExcludeProperties()
    {
        var data = new ClassWithPrivateMembers();
        var output = data.DumpText(members: new MembersConfig 
        { 
            IncludeProperties = false,
            IncludeFields = true
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_MemberFilter_ByName()
    {
        var data = new Person("Alice", 30, "alice@example.com");
        var output = data.DumpText(members: new MembersConfig 
        { 
            MemberFilter = ctx => ctx.Member.Name != "Email"
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_MemberFilter_OnlyStrings()
    {
        var data = new Person("Alice", 30, "alice@example.com");
        var output = data.DumpText(members: new MembersConfig 
        { 
            MemberFilter = ctx => ctx.Member.MemberType == typeof(string)
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_MemberFilter_ByValue_ExcludeNulls()
    {
        var data = new PersonWithNullables("Alice", 30, null);
        var output = data.DumpText(members: new MembersConfig 
        { 
            MemberFilter = ctx => ctx.Value is not null
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_MemberFilter_ByValue_ExcludeDefaults()
    {
        var data = new PersonWithDefaults { Name = "Bob", Age = 0, Score = 0.0 };
        var output = data.DumpText(members: new MembersConfig 
        { 
            MemberFilter = ctx =>
            {
                var value = ctx.Value;
                if (value is int i && i == 0) return false;
                if (value is double d && d == 0.0) return false;
                return true;
            }
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_MemberFilter_ByValue_OnlyNonEmptyStrings()
    {
        var data = new PersonWithStrings { Name = "Charlie", Nickname = "", Title = "Mr." };
        var output = data.DumpText(members: new MembersConfig 
        { 
            MemberFilter = ctx => ctx.Value is not string str || !string.IsNullOrEmpty(str)
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_MemberFilter_ByDepth_OnlyTopLevel()
    {
        var data = new PersonWithAddress("Dave", new Address("123 Main St", "Springfield", 12345));
        var output = data.DumpText(members: new MembersConfig 
        { 
            MemberFilter = ctx => ctx.Depth == 0
        });
        return Verify(output);
    }
    
    [Fact]
    public Task MembersConfig_MemberFilter_Combined_NameAndValue()
    {
        var data = new PersonWithSensitive { Name = "Eve", Age = 25, Password = "secret123", Token = "" };
        var output = data.DumpText(members: new MembersConfig 
        { 
            MemberFilter = ctx =>
            {
                // Exclude sensitive fields by name
                if (ctx.Member.Name is "Password" or "Token") return false;
                // Exclude empty strings
                if (ctx.Value is string str && string.IsNullOrEmpty(str)) return false;
                return true;
            }
        });
        return Verify(output);
    }
    
    #endregion
    
    #region TypeRenderingConfig Tests
    
    [Fact]
    public Task TypeRenderingConfig_QuoteStringValues_True()
    {
        var data = "Hello World";
        var output = data.DumpText(typeRenderingConfig: new TypeRenderingConfig { QuoteStringValues = true });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeRenderingConfig_QuoteStringValues_False()
    {
        var data = "Hello World";
        var output = data.DumpText(typeRenderingConfig: new TypeRenderingConfig { QuoteStringValues = false });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeRenderingConfig_CustomStringQuotationChar()
    {
        var data = "Hello World";
        var output = data.DumpText(typeRenderingConfig: new TypeRenderingConfig 
        { 
            QuoteStringValues = true,
            StringQuotationChar = '\''
        });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeRenderingConfig_QuoteCharValues_True()
    {
        var data = 'A';
        var output = data.DumpText(typeRenderingConfig: new TypeRenderingConfig { QuoteCharValues = true });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeRenderingConfig_QuoteCharValues_False()
    {
        var data = 'A';
        var output = data.DumpText(typeRenderingConfig: new TypeRenderingConfig { QuoteCharValues = false });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeRenderingConfig_CustomCharQuotationChar()
    {
        var data = 'A';
        var output = data.DumpText(typeRenderingConfig: new TypeRenderingConfig 
        { 
            QuoteCharValues = true,
            CharQuotationChar = '`'
        });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeRenderingConfig_StringInObject_QuotesEnabled()
    {
        var data = new Person("Alice", 30);
        var output = data.DumpText(typeRenderingConfig: new TypeRenderingConfig { QuoteStringValues = true });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeRenderingConfig_StringInObject_QuotesDisabled()
    {
        var data = new Person("Alice", 30);
        var output = data.DumpText(typeRenderingConfig: new TypeRenderingConfig { QuoteStringValues = false });
        return Verify(output);
    }
    
    #endregion
    
    #region TypeNamingConfig Tests
    
    [Fact]
    public Task TypeNamingConfig_UseAliases_True()
    {
        var data = new Dictionary<string, int> { { "one", 1 } };
        var output = data.DumpText(typeNames: new TypeNamingConfig { UseAliases = true });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeNamingConfig_UseAliases_False()
    {
        var data = new Dictionary<string, int> { { "one", 1 } };
        var output = data.DumpText(typeNames: new TypeNamingConfig { UseAliases = false });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeNamingConfig_UseFullName_True()
    {
        var data = new Person("Alice", 30);
        var output = data.DumpText(typeNames: new TypeNamingConfig { UseFullName = true });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeNamingConfig_UseFullName_False()
    {
        var data = new Person("Alice", 30);
        var output = data.DumpText(typeNames: new TypeNamingConfig { UseFullName = false });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeNamingConfig_ShowTypeNames_True()
    {
        var data = new Person("Alice", 30);
        var output = data.DumpText(typeNames: new TypeNamingConfig { ShowTypeNames = true });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeNamingConfig_ShowTypeNames_False()
    {
        var data = new Person("Alice", 30);
        var output = data.DumpText(typeNames: new TypeNamingConfig { ShowTypeNames = false });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeNamingConfig_SimplifyAnonymousObjectNames_True()
    {
        var data = new { Name = "Anonymous", Value = 42 };
        var output = data.DumpText(typeNames: new TypeNamingConfig { SimplifyAnonymousObjectNames = true });
        return Verify(output);
    }
    
    [Fact]
    public Task TypeNamingConfig_SimplifyAnonymousObjectNames_False()
    {
        var data = new { Name = "Anonymous", Value = 42 };
        var output = data.DumpText(typeNames: new TypeNamingConfig { SimplifyAnonymousObjectNames = false });
        return Verify(output);
    }
    
    #endregion
    
    #region MaxDepth Tests
    
    [Fact]
    public Task MaxDepth_1_Level()
    {
        var data = new PersonWithAddress("Alice", new Address("123 Main St", "Springfield", 12345));
        var output = data.DumpText(maxDepth: 1);
        return Verify(output);
    }
    
    [Fact]
    public Task MaxDepth_2_Levels()
    {
        var data = new PersonWithAddress("Alice", new Address("123 Main St", "Springfield", 12345));
        var output = data.DumpText(maxDepth: 2);
        return Verify(output);
    }
    
    [Fact]
    public Task MaxDepth_DeepNesting()
    {
        // Create deeply nested structure
        var level4 = new { Level = 4, Value = "Deepest" };
        var level3 = new { Level = 3, Nested = level4 };
        var level2 = new { Level = 2, Nested = level3 };
        var level1 = new { Level = 1, Nested = level2 };
        var root = new { Level = 0, Nested = level1 };
        
        var output = root.DumpText(maxDepth: 3);
        return Verify(output);
    }
    
    [Fact]
    public Task MaxDepth_CollectionsWithObjects()
    {
        var data = new[]
        {
            new PersonWithAddress("Alice", new Address("123 Main St", "Springfield", 12345)),
            new PersonWithAddress("Bob", new Address("456 Oak Ave", "Shelbyville", 67890))
        };
        var output = data.DumpText(maxDepth: 2);
        return Verify(output);
    }
    
    #endregion
    
    #region Label Tests
    
    [Fact]
    public Task Label_SimpleString()
    {
        var data = "test value";
        var output = data.DumpText(label: "My Label");
        return Verify(output);
    }
    
    [Fact]
    public Task Label_WithObject()
    {
        var data = new Person("Alice", 30);
        var output = data.DumpText(label: "Person Data");
        return Verify(output);
    }
    
    [Fact]
    public Task Label_WithCollection()
    {
        var data = new[] { 1, 2, 3 };
        var output = data.DumpText(label: "Numbers Array");
        return Verify(output);
    }
    
    #endregion
    
    #region Combined Configuration Tests
    
    [Fact]
    public Task Combined_TableAndTypeNaming()
    {
        var data = new Person("Alice", 30, "alice@example.com");
        var output = data.DumpText(
            tableConfig: new TableConfig 
            { 
                ShowMemberTypes = true,
                ShowTableHeaders = true 
            },
            typeNames: new TypeNamingConfig 
            { 
                UseAliases = true,
                ShowTypeNames = true 
            }
        );
        return Verify(output);
    }
    
    [Fact]
    public Task Combined_MembersAndTypeRendering()
    {
        var data = new ClassWithPrivateMembers();
        var output = data.DumpText(
            members: new MembersConfig 
            { 
                IncludeNonPublicMembers = true,
                IncludeFields = true 
            },
            typeRenderingConfig: new TypeRenderingConfig 
            { 
                QuoteStringValues = false 
            }
        );
        return Verify(output);
    }
    
    [Fact]
    public Task Combined_AllConfigurations()
    {
        var data = new[] 
        { 
            new Person("Alice", 30, "alice@example.com"),
            new Person("Bob", 25, null)
        };
        
        var output = data.DumpText(
            label: "People List",
            maxDepth: 5,
            tableConfig: new TableConfig 
            { 
                ShowArrayIndices = true,
                ShowMemberTypes = true,
                ShowRowSeparators = true,
                MaxCollectionCount = 10
            },
            typeNames: new TypeNamingConfig 
            { 
                UseAliases = true,
                ShowTypeNames = true 
            },
            typeRenderingConfig: new TypeRenderingConfig 
            { 
                QuoteStringValues = true 
            }
        );
        return Verify(output);
    }
    
    #endregion
}
