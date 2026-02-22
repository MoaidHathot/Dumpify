using System.Data;
using VerifyXunit;
using static VerifyXunit.Verifier;

namespace Dumpify.Tests.Renderers.Spectre.Console;

/// <summary>
/// Snapshot tests for collection truncation behavior in Spectre.Console renderer.
/// </summary>
public class MultiValueTruncationTests
{
    #region Helper Methods

    private static DataTable CreateDataTable(int rows)
    {
        var dataTable = new DataTable
        {
            Columns =
            {
                new DataColumn("Value1", typeof(int)),
                new DataColumn("Value2", typeof(int)),
                new DataColumn("Value3", typeof(int)),
                new DataColumn("Value4", typeof(int)),
                new DataColumn("Value5", typeof(int)),
                new DataColumn("Value6", typeof(int)),
                new DataColumn("Value7", typeof(int)),
                new DataColumn("Value8", typeof(int)),
                new DataColumn("Value9", typeof(int)),
                new DataColumn("Value10", typeof(int))
            }
        };

        for (int i = 0; i < rows; i++)
        {
            dataTable.Rows.Add(i * 1, i * 2, i * 3, i * 4, i * 5, i * 6, i * 7, i * 8, i * 9, i * 10);
        }

        return dataTable;
    }

    private static DataSet CreateDataSet(int tables)
    {
        DataSet dataSet = new DataSet();
        for (int i = 0; i < tables; i++)
        {
            var dataTable = CreateDataTable(10);
            dataTable.TableName = $"Table {i + 1}";
            dataSet.Tables.Add(dataTable);
        }

        return dataSet;
    }

    private static string DumpWithMaxCount(object collection, int maxCount)
    {
        return collection.DumpText(truncationConfig: new TruncationConfig { MaxCollectionCount = maxCount });
    }

    #endregion

    #region Array Truncation Tests

    [Fact]
    public Task Array_NoTruncation_WhenCountEqualsMax()
    {
        var array = Enumerable.Range(1, 10).ToArray();
        return Verify(DumpWithMaxCount(array, 10));
    }

    [Fact]
    public Task Array_NoTruncation_WhenCountLessThanMax()
    {
        var array = Enumerable.Range(1, 10).ToArray();
        return Verify(DumpWithMaxCount(array, 20));
    }

    [Fact]
    public Task Array_Truncates_WhenCountGreaterThanMax()
    {
        var array = Enumerable.Range(1, 10).ToArray();
        return Verify(DumpWithMaxCount(array, 5));
    }

    #endregion

    #region List Truncation Tests

    [Fact]
    public Task List_Truncates_WhenCountGreaterThanMax()
    {
        var list = Enumerable.Range(1, 10).ToList();
        return Verify(DumpWithMaxCount(list, 5));
    }

    #endregion

    #region DataTable Truncation Tests

    [Fact]
    public Task DataTable_Truncates_RowsAndColumns()
    {
        var dataTable = CreateDataTable(10);
        return Verify(DumpWithMaxCount(dataTable, 5));
    }

    #endregion

    #region DataSet Truncation Tests

    [Fact]
    public Task DataSet_Truncates_Tables()
    {
        var dataSet = CreateDataSet(4);
        return Verify(DumpWithMaxCount(dataSet, 2));
    }

    #endregion

    #region Dictionary Truncation Tests

    [Fact]
    public Task Dictionary_Truncates_WhenCountGreaterThanMax()
    {
        // Use SortedDictionary to ensure deterministic ordering across platforms
        var dictionary = new SortedDictionary<int, string>(
            Enumerable.Range(1, 10).ToDictionary(x => x, x => $"value for {x}"));
        return Verify(DumpWithMaxCount(dictionary, 5));
    }

    #endregion

    #region Multi-Dimensional Array Truncation Tests

    [Fact]
    public Task TwoDimensionalArray_Truncates_RowsAndColumns()
    {
        var array = new int[6, 6]
        {
            { 1, 2, 3, 4, 5, 6 },
            { 1, 2, 3, 4, 5, 6 },
            { 1, 2, 3, 4, 5, 6 },
            { 1, 2, 3, 4, 5, 6 },
            { 1, 2, 3, 4, 5, 6 },
            { 1, 2, 3, 4, 5, 6 }
        };
        return Verify(DumpWithMaxCount(array, 3));
    }

    [Fact]
    public Task ThreeDimensionalArray_Truncates_FlattenedList()
    {
        var array = new int[4, 4, 4]
        {
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 },
                { 13, 14, 15, 16 }
            },
            {
                { 17, 18, 19, 20 },
                { 21, 22, 23, 24 },
                { 25, 26, 27, 28 },
                { 29, 30, 31, 32 }
            },
            {
                { 33, 34, 35, 36 },
                { 37, 38, 39, 40 },
                { 41, 42, 43, 44 },
                { 45, 46, 47, 48 }
            },
            {
                { 49, 50, 51, 52 },
                { 53, 54, 55, 56 },
                { 57, 58, 59, 60 },
                { 61, 62, 63, 64 }
            }
        };
        return Verify(DumpWithMaxCount(array, 10));
    }

    #endregion
}
