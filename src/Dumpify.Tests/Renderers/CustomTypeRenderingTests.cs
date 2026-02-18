using System.Data;
using VerifyXunit;
using static VerifyXunit.Verifier;

namespace Dumpify.Tests.Renderers;

/// <summary>
/// Snapshot tests for custom type renderers (Array, Enum, Guid, Time types, System.Reflection, DataTable, DataSet, Lazy, Task, Tuple, Dictionary).
/// </summary>
public class CustomTypeRenderingTests
{
    #region Array Type Renderer Tests

    [Fact]
    public Task ArrayTypeRenderer_SingleDimensionalArray()
    {
        var array = new[] { 1, 2, 3, 4, 5 };
        return Verify(array.DumpText());
    }

    [Fact]
    public Task ArrayTypeRenderer_TwoDimensionalArray()
    {
        var array = new int[3, 3]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };
        return Verify(array.DumpText());
    }

    [Fact]
    public Task ArrayTypeRenderer_JaggedArray()
    {
        var array = new int[][]
        {
            new[] { 1, 2 },
            new[] { 3, 4, 5 },
            new[] { 6 }
        };
        return Verify(array.DumpText());
    }

    [Fact]
    public Task ArrayTypeRenderer_EmptyArray()
    {
        var array = Array.Empty<int>();
        return Verify(array.DumpText());
    }

    [Fact]
    public Task ArrayTypeRenderer_ArrayOfStrings()
    {
        var array = new[] { "hello", "world", "test" };
        return Verify(array.DumpText());
    }

    #endregion

    #region Enum Type Renderer Tests

    [Fact]
    public Task EnumTypeRenderer_SimpleEnum()
    {
        var value = DayOfWeek.Wednesday;
        return Verify(value.DumpText());
    }

    [Fact]
    public Task EnumTypeRenderer_FlagsEnum()
    {
        var value = FileAttributes.ReadOnly | FileAttributes.Hidden;
        return Verify(value.DumpText());
    }

    [Fact]
    public Task EnumTypeRenderer_EnumInObject()
    {
        var obj = new { Day = DayOfWeek.Friday, Status = FileAttributes.Normal };
        return Verify(obj.DumpText());
    }

    #endregion

    #region Guid Type Renderer Tests

    [Fact]
    public Task GuidTypeRenderer_SimpleGuid()
    {
        var guid = new Guid("12345678-1234-1234-1234-123456789abc");
        return Verify(guid.DumpText());
    }

    [Fact]
    public Task GuidTypeRenderer_EmptyGuid()
    {
        var guid = Guid.Empty;
        return Verify(guid.DumpText());
    }

    [Fact]
    public Task GuidTypeRenderer_GuidInObject()
    {
        var obj = new { Id = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), Name = "Test" };
        return Verify(obj.DumpText());
    }

    #endregion

    #region Time Types Renderer Tests

    [Fact]
    public Task TimeTypesRenderer_DateTime()
    {
        var dt = new DateTime(2025, 6, 15, 14, 30, 45, DateTimeKind.Utc);
        return Verify(dt.DumpText());
    }

    [Fact]
    public Task TimeTypesRenderer_DateTimeOffset()
    {
        var dto = new DateTimeOffset(2025, 6, 15, 14, 30, 45, TimeSpan.FromHours(-5));
        return Verify(dto.DumpText());
    }

    [Fact]
    public Task TimeTypesRenderer_TimeSpan()
    {
        var ts = new TimeSpan(2, 3, 45, 30, 500);
        return Verify(ts.DumpText());
    }

    [Fact]
    public Task TimeTypesRenderer_DateOnly()
    {
        var date = new DateOnly(2025, 6, 15);
        return Verify(date.DumpText());
    }

    [Fact]
    public Task TimeTypesRenderer_TimeOnly()
    {
        var time = new TimeOnly(14, 30, 45);
        return Verify(time.DumpText());
    }

    #endregion

    #region System.Reflection Type Renderer Tests

    [Fact]
    public Task SystemReflectionTypeRenderer_TypeObject()
    {
        var type = typeof(string);
        return Verify(type.DumpText());
    }

    [Fact]
    public Task SystemReflectionTypeRenderer_GenericType()
    {
        var type = typeof(List<int>);
        return Verify(type.DumpText());
    }

    [Fact]
    public Task SystemReflectionTypeRenderer_MethodInfo()
    {
        var method = typeof(string).GetMethod("ToUpper", Type.EmptyTypes);
        return Verify(method.DumpText());
    }

    #endregion

    #region DataTable Type Renderer Tests

    [Fact]
    public Task DataTableTypeRenderer_SimpleTable()
    {
        var dt = new DataTable("TestTable");
        dt.Columns.Add("Id", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Value", typeof(decimal));

        dt.Rows.Add(1, "First", 100.50m);
        dt.Rows.Add(2, "Second", 200.75m);
        dt.Rows.Add(3, "Third", 300.25m);

        return Verify(dt.DumpText());
    }

    [Fact]
    public Task DataTableTypeRenderer_EmptyTable()
    {
        var dt = new DataTable("EmptyTable");
        dt.Columns.Add("Column1", typeof(string));
        dt.Columns.Add("Column2", typeof(int));

        return Verify(dt.DumpText());
    }

    [Fact]
    public Task DataTableTypeRenderer_TableWithNulls()
    {
        var dt = new DataTable("NullTable");
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Value", typeof(int));
        dt.Columns["Value"]!.AllowDBNull = true;

        dt.Rows.Add("WithValue", 42);
        dt.Rows.Add("WithNull", DBNull.Value);

        return Verify(dt.DumpText());
    }

    #endregion

    #region DataSet Type Renderer Tests

    [Fact]
    public Task DataSetTypeRenderer_MultipleTablesDataSet()
    {
        var ds = new DataSet("TestDataSet");

        var table1 = new DataTable("Users");
        table1.Columns.Add("Id", typeof(int));
        table1.Columns.Add("Name", typeof(string));
        table1.Rows.Add(1, "Alice");
        table1.Rows.Add(2, "Bob");
        ds.Tables.Add(table1);

        var table2 = new DataTable("Orders");
        table2.Columns.Add("OrderId", typeof(int));
        table2.Columns.Add("UserId", typeof(int));
        table2.Rows.Add(100, 1);
        table2.Rows.Add(101, 2);
        ds.Tables.Add(table2);

        return Verify(ds.DumpText());
    }

    [Fact]
    public Task DataSetTypeRenderer_EmptyDataSet()
    {
        var ds = new DataSet("EmptyDataSet");
        return Verify(ds.DumpText());
    }

    #endregion

    #region Lazy Type Renderer Tests

    [Fact]
    public Task LazyTypeRenderer_LazyValue()
    {
        var lazy = new Lazy<int>(() => 42);
        _ = lazy.Value; // Force evaluation
        return Verify(lazy.DumpText());
    }

    [Fact]
    public Task LazyTypeRenderer_LazyObject()
    {
        var lazy = new Lazy<object>(() => new { Name = "Test", Value = 123 });
        _ = lazy.Value;
        return Verify(lazy.DumpText());
    }

    [Fact]
    public Task LazyTypeRenderer_UnevaluatedLazy()
    {
        var lazy = new Lazy<int>(() => 42);
        // Don't evaluate - show unevaluated state
        return Verify(lazy.DumpText());
    }

    #endregion

    #region Task Type Renderer Tests

    [Fact]
    public Task TaskTypeRenderer_CompletedTask()
    {
        var task = Task.FromResult(42);
        return Verify(task.DumpText()).ScrubLinesWithReplace(line => 
            System.Text.RegularExpressions.Regex.Replace(line, @"│ Id\s+│ \d+\s+│", "│ Id              │ [TaskId]                   │"));
    }

    [Fact]
    public Task TaskTypeRenderer_CompletedTaskWithObject()
    {
        var task = Task.FromResult(new { Name = "Result", Count = 5 });
        return Verify(task.DumpText()).ScrubLinesWithReplace(line => 
            System.Text.RegularExpressions.Regex.Replace(line, @"│ Id\s+│ \d+\s+│", "│ Id              │ [TaskId]                   │"));
    }

    [Fact]
    public Task TaskTypeRenderer_FaultedTask()
    {
        var task = Task.FromException<int>(new InvalidOperationException("Test error"));
        return Verify(task.DumpText()).ScrubLinesWithReplace(line => 
            System.Text.RegularExpressions.Regex.Replace(line, @"│ Id\s+│ \d+\s+│", "│ Id              │ [TaskId]                   │"));
    }

    [Fact]
    public Task TaskTypeRenderer_CanceledTask()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var task = Task.FromCanceled<int>(cts.Token);
        return Verify(task.DumpText()).ScrubLinesWithReplace(line => 
            System.Text.RegularExpressions.Regex.Replace(line, @"│ Id\s+│ \d+\s+│", "│ Id              │ [TaskId]                   │"));
    }

    #endregion

    #region Tuple Type Renderer Tests

    [Fact]
    public Task TupleTypeRenderer_ValueTuple()
    {
        var tuple = (Name: "Test", Value: 42, Active: true);
        return Verify(tuple.DumpText());
    }

    [Fact]
    public Task TupleTypeRenderer_UnnamedValueTuple()
    {
        var tuple = (1, "hello", 3.14);
        return Verify(tuple.DumpText());
    }

    [Fact]
    public Task TupleTypeRenderer_ClassTuple()
    {
        var tuple = Tuple.Create("First", 2, true);
        return Verify(tuple.DumpText());
    }

    [Fact]
    public Task TupleTypeRenderer_NestedTuple()
    {
        var tuple = (Outer: "Level1", Inner: (Name: "Level2", Value: 100));
        return Verify(tuple.DumpText());
    }

    #endregion

    #region Dictionary Type Renderer Tests

    [Fact]
    public Task DictionaryTypeRenderer_StringStringDictionary()
    {
        var dict = new Dictionary<string, string>
        {
            ["key1"] = "value1",
            ["key2"] = "value2",
            ["key3"] = "value3"
        };
        return Verify(dict.DumpText());
    }

    [Fact]
    public Task DictionaryTypeRenderer_IntObjectDictionary()
    {
        var dict = new Dictionary<int, object>
        {
            [1] = "string value",
            [2] = 42,
            [3] = new { Nested = true }
        };
        return Verify(dict.DumpText());
    }

    [Fact]
    public Task DictionaryTypeRenderer_EmptyDictionary()
    {
        var dict = new Dictionary<string, int>();
        return Verify(dict.DumpText());
    }

    [Fact]
    public Task DictionaryTypeRenderer_SortedDictionary()
    {
        var dict = new SortedDictionary<string, int>
        {
            ["charlie"] = 3,
            ["alpha"] = 1,
            ["bravo"] = 2
        };
        return Verify(dict.DumpText());
    }

    #endregion
}
