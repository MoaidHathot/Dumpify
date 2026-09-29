using VerifyXunit;
using static VerifyXunit.Verifier;

namespace Dumpify.Tests.Renderers;

/// <summary>
/// Snapshot tests for table layout strategies (Vertical vs Horizontal).
/// Tests the new TableLayout system that controls how objects and collections are rendered.
/// </summary>
public class TableLayoutTests
{
    #region Test Data

    private record Person(string Name, int Age, string? Email = null);
    private record Address(string Street, string City, int ZipCode);
    private record PersonWithAddress(string Name, Address HomeAddress);

    private class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public bool InStock { get; set; }
    }

    private class Order
    {
        public int OrderId { get; set; }
        public string Customer { get; set; } = "";
        public List<Product> Products { get; set; } = new();
    }

    #endregion

    #region Vertical Layout Tests (Default)

    [Fact]
    public Task VerticalLayout_SingleObject_IsDefault()
    {
        var person = new Person("Alice", 30, "alice@example.com");
        // Default layout should be Vertical
        return Verify(person.DumpText());
    }

    [Fact]
    public Task VerticalLayout_Collection_IsDefault()
    {
        var people = new[]
        {
            new Person("Alice", 30),
            new Person("Bob", 25),
            new Person("Charlie", 35)
        };
        // Default layout should be Vertical
        return Verify(people.DumpText());
    }

    [Fact]
    public Task VerticalLayout_ExplicitlySet_SingleObject()
    {
        var person = new Person("Alice", 30, "alice@example.com");
        var output = person.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Vertical });
        return Verify(output);
    }

    [Fact]
    public Task VerticalLayout_ExplicitlySet_Collection()
    {
        var people = new[]
        {
            new Person("Alice", 30),
            new Person("Bob", 25)
        };
        var output = people.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Vertical });
        return Verify(output);
    }

    #endregion

    #region Horizontal Layout Tests

    [Fact]
    public Task HorizontalLayout_SingleObject_PropertiesAsColumns()
    {
        var person = new Person("Alice", 30, "alice@example.com");
        var output = person.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_Collection_ItemsAsRows()
    {
        var people = new[]
        {
            new Person("Alice", 30, "alice@example.com"),
            new Person("Bob", 25, "bob@example.com"),
            new Person("Charlie", 35)
        };
        var output = people.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_Collection_ShowsRowIndicesByDefault()
    {
        var people = new[]
        {
            new Person("Alice", 30),
            new Person("Bob", 25)
        };
        // Horizontal layout should show row indices by default
        var output = people.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_Collection_WithTruncation()
    {
        var people = Enumerable.Range(1, 10)
            .Select(i => new Person($"Person{i}", 20 + i))
            .ToArray();

        var output = people.DumpText(
            tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal },
            truncationConfig: new TruncationConfig { MaxCollectionCount = 3 });
        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_PrimitiveCollection_FallsBackToVertical()
    {
        // Primitive collections should use vertical even when horizontal is requested
        var numbers = new[] { 1, 2, 3, 4, 5 };
        var output = numbers.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_StringCollection_FallsBackToVertical()
    {
        // String collections should use vertical even when horizontal is requested
        var strings = new[] { "apple", "banana", "cherry" };
        var output = strings.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_EmptyCollection_Renders()
    {
        var empty = Array.Empty<Person>();
        var output = empty.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_NestedObjects_RendersNestedTables()
    {
        var data = new PersonWithAddress("Alice", new Address("123 Main St", "Springfield", 12345));
        var output = data.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
        return Verify(output);
    }

    #endregion

    #region Layout Rules Tests

    [Fact]
    public Task LayoutRules_SetLayoutForType_MatchesExactType()
    {
        var tableConfig = new TableConfig()
            .SetLayoutForType<Person>(TableLayout.Horizontal);

        var person = new Person("Alice", 30);
        var output = person.DumpText(tableConfig: tableConfig);
        return Verify(output);
    }

    [Fact]
    public Task LayoutRules_SetLayoutForType_DoesNotMatchDifferentType()
    {
        var tableConfig = new TableConfig()
            .SetLayoutForType<Address>(TableLayout.Horizontal);

        // Person should still use default (Vertical) layout
        var person = new Person("Alice", 30);
        var output = person.DumpText(tableConfig: tableConfig);
        return Verify(output);
    }

    [Fact]
    public Task LayoutRules_SetLayoutWhen_PredicateMatches()
    {
        var tableConfig = new TableConfig()
            .SetLayoutWhen(ctx => ctx.Type.Name.StartsWith("Person"), TableLayout.Horizontal);

        var person = new Person("Alice", 30);
        var output = person.DumpText(tableConfig: tableConfig);
        return Verify(output);
    }

    [Fact]
    public Task LayoutRules_MultipleRules_FirstMatchWins()
    {
        var tableConfig = new TableConfig()
            .SetLayoutForType<Person>(TableLayout.Horizontal)
            .SetLayoutWhen(_ => true, TableLayout.Vertical); // This should NOT match Person because first rule wins

        var person = new Person("Alice", 30);
        var output = person.DumpText(tableConfig: tableConfig);
        return Verify(output);
    }

    #endregion

    #region Config Override Tests

    [Fact]
    public Task LayoutResult_OverrideShowRowIndices_True()
    {
        var tableConfig = new TableConfig()
            .SetLayoutForType<Person>(new TableLayoutResult
            {
                Layout = TableLayout.Horizontal,
                ShowRowIndices = true
            });

        var people = new[] { new Person("Alice", 30), new Person("Bob", 25) };
        var output = people.DumpText(tableConfig: tableConfig);
        return Verify(output);
    }

    [Fact]
    public Task LayoutResult_OverrideShowRowIndices_False()
    {
        var tableConfig = new TableConfig()
            .SetLayoutForType<Person>(new TableLayoutResult
            {
                Layout = TableLayout.Horizontal,
                ShowRowIndices = false
            });

        var people = new[] { new Person("Alice", 30), new Person("Bob", 25) };
        var output = people.DumpText(tableConfig: tableConfig);
        return Verify(output);
    }

    [Fact]
    public Task LayoutResult_OverrideShowMemberTypes_True()
    {
        var tableConfig = new TableConfig()
            .SetLayoutForType<Person>(new TableLayoutResult
            {
                Layout = TableLayout.Horizontal,
                ShowMemberTypes = true
            });

        var person = new Person("Alice", 30, "alice@example.com");
        var output = person.DumpText(tableConfig: tableConfig);
        return Verify(output);
    }

    [Fact]
    public Task LayoutResult_OverrideShowRowSeparators_True()
    {
        var tableConfig = new TableConfig()
            .SetLayoutForType<Person>(new TableLayoutResult
            {
                Layout = TableLayout.Horizontal,
                ShowRowSeparators = true
            });

        var people = new[] { new Person("Alice", 30), new Person("Bob", 25), new Person("Charlie", 35) };
        var output = people.DumpText(tableConfig: tableConfig);
        return Verify(output);
    }

    [Fact]
    public Task LayoutResult_OverrideShowTableHeaders_False()
    {
        var tableConfig = new TableConfig()
            .SetLayoutForType<Person>(new TableLayoutResult
            {
                Layout = TableLayout.Horizontal,
                ShowTableHeaders = false
            });

        var person = new Person("Alice", 30);
        var output = person.DumpText(tableConfig: tableConfig);
        return Verify(output);
    }

    #endregion

    #region Complex Scenarios

    [Fact]
    public Task HorizontalLayout_ComplexObject_WithProducts()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Widget",
            Price = 19.99m,
            InStock = true
        };

        var output = product.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_Collection_OfComplexObjects()
    {
        var products = new[]
        {
            new Product { Id = 1, Name = "Widget", Price = 19.99m, InStock = true },
            new Product { Id = 2, Name = "Gadget", Price = 29.99m, InStock = false },
            new Product { Id = 3, Name = "Gizmo", Price = 9.99m, InStock = true }
        };

        var output = products.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_Collection_WithNullItems()
    {
        var people = new Person?[]
        {
            new Person("Alice", 30),
            null,
            new Person("Charlie", 35)
        };

        var output = people.DumpText(tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });
        return Verify(output);
    }

    [Fact]
    public Task LayoutRules_DifferentLayoutsForDifferentTypes()
    {
        var tableConfig = new TableConfig()
            .SetLayoutForType<Person>(TableLayout.Horizontal)
            .SetLayoutForType<Address>(TableLayout.Vertical);

        // Test with a container that has both types
        var data = new
        {
            People = new[] { new Person("Alice", 30), new Person("Bob", 25) },
            Location = new Address("123 Main St", "Springfield", 12345)
        };

        var output = data.DumpText(tableConfig: tableConfig);
        return Verify(output);
    }

    #endregion

    #region MemberFilter with Horizontal Layout Tests

    [Fact]
    public Task HorizontalLayout_Collection_MemberFilter_ShouldFilterColumns()
    {
        // This test reproduces the bug: MemberFilter does not work with horizontal layout for collections
        // The user reported that filtering columns via MemberFilter has no effect on horizontal layout
        var products = new[]
        {
            new Product { Id = 1, Name = "Widget", Price = 19.99m, InStock = true },
            new Product { Id = 2, Name = "Gadget", Price = 29.99m, InStock = false },
            new Product { Id = 3, Name = "Gizmo", Price = 9.99m, InStock = true }
        };

        // Filter to only show Name and Price columns
        var limitColumns = new MembersConfig
        {
            MemberFilter = ctx => ctx.Member.Name == "Name" || ctx.Member.Name == "Price"
        };

        var output = products.DumpText(
            members: limitColumns,
            tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_SingleObject_MemberFilter_ShouldFilterColumns()
    {
        // MemberFilter should work for single objects in horizontal layout
        var product = new Product { Id = 1, Name = "Widget", Price = 19.99m, InStock = true };

        var limitColumns = new MembersConfig
        {
            MemberFilter = ctx => ctx.Member.Name == "Name" || ctx.Member.Name == "Price"
        };

        var output = product.DumpText(
            members: limitColumns,
            tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_Collection_MemberFilter_AllFiltered_ShowsNoMembers()
    {
        // When MemberFilter filters out ALL properties, should show "No members" message
        var products = new[]
        {
            new Product { Id = 1, Name = "Widget", Price = 19.99m, InStock = true },
            new Product { Id = 2, Name = "Gadget", Price = 29.99m, InStock = false }
        };

        // Filter that matches no properties
        var filterAll = new MembersConfig
        {
            MemberFilter = ctx => ctx.Member.Name == "NonExistentProperty"
        };

        var output = products.DumpText(
            members: filterAll,
            tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_Collection_MemberFilter_ByValue_UsesFirstItemAsPrototype()
    {
        // Value-based filters are evaluated against the first non-null item to decide the columns
        var people = new[]
        {
            new Person("Alice", 30, Email: null),
            new Person("Bob", 25, Email: "bob@example.com")
        };

        var hideNulls = new MembersConfig
        {
            MemberFilter = ctx => ctx.Value is not null
        };

        var output = people.DumpText(
            members: hideNulls,
            tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_EmptyCollection_MemberFilter_ByValue_KeepsAllColumns()
    {
        // With no items there is nothing to evaluate a value-based filter against,
        // so all columns are kept instead of failing to render
        var people = Array.Empty<Person>();

        var hideNulls = new MembersConfig
        {
            MemberFilter = ctx => ctx.Value is not null
        };

        var output = people.DumpText(
            members: hideNulls,
            tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

        return Verify(output);
    }

    [Fact]
    public Task HorizontalLayout_AllNullItems_MemberFilter_ByValue_KeepsAllColumns()
    {
        // Same as the empty case: null items cannot serve as a prototype for value-based filtering
        var people = new Person?[] { null, null };

        var hideNulls = new MembersConfig
        {
            MemberFilter = ctx => ctx.Value is not null
        };

        var output = people.DumpText(
            members: hideNulls,
            tableConfig: new TableConfig { TableLayout = TableLayout.Horizontal });

        return Verify(output);
    }

    #endregion
}
