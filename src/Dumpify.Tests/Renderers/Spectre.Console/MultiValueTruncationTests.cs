using System.Collections;
using System.Data;
using Xunit.Abstractions;

namespace Dumpify.Tests.Renderers.Spectre.Console;
public class MultiValueTruncationTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public MultiValueTruncationTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [ClassData(typeof(TruncateCollectionsData))]
    public void TruncateCollections(string name, object collection, int maxCount, string[] shouldContainItems, string[] shouldNotContainItems)
    {
        //Arrange
        _testOutputHelper.WriteLine($"Test: {name}");

        //Act
        var result = collection.DumpText(tableConfig: new TableConfig() { MaxCollectionCount = maxCount });

        _testOutputHelper.WriteLine(result);

        //Assert
        foreach(var shouldContain in shouldContainItems)
        {
            result.Should().Contain(shouldContain);
        }

        foreach (var shouldNotContain in shouldNotContainItems)
        {
            result.Should().NotContain(shouldNotContain);
        }
    }

    public class TruncateCollectionsData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                "Don't truncate if the number of items is equal to the max count",
                Enumerable.Range(1, 10).ToArray(),
                10,
                new [] { "9 │ 10" },
                new [] { "... truncated" }
            };

            yield return new object[]
            {
                "Don't truncate if the number of items is smaller than the max count",
                Enumerable.Range(1, 10).ToArray(),
                20,
                new [] { "9 │ 10" },
                new [] { " ... truncated" }
            };

            yield return new object[]
            {
                "Truncate if the number of items is greater than the max count",
                Enumerable.Range(1, 10).ToArray(),
                5,
                new [] { "4 │ 5", "  │ ... truncated 5 items" },
                new [] { "5 │ 6" }
            };

            yield return new object[]
            {
                "Truncate for lists",
                Enumerable.Range(1, 10).ToList(),
                5,
                new [] { "│ 5", "│ ... truncated 5 items" },
                new [] { "│ 6" }
            };

            yield return new object[]
            {
                "Truncate for data tables",
                CreateDataTable(10),
                5,
                new []
                    {
                        "│ 4      │ 8      │ 12     │ 16     │ 20     │        │",
                        "│ Value1 │ Value2 │ Value3 │ Value4 │ Value5 │ ... +5 │",
                        "│ ... +5 │        │        │"
                    },
                new [] { "│ 5      │ 10    │ 15     │ 20     │ 25     │        │" }
            };

            yield return new object[]
            {
                "Truncate for datasets",
                CreateDataSet(4),
                2,
                new [] { "Table 2", "... truncated 2 more tables" },
                new [] { "Table 3" }
            };

            yield return new object[]
            {
                "Truncate for dictionaries",
                Enumerable.Range(1, 10).ToDictionary(x => x, x => $"value for {x}"),
                5,
                new []
                    {
                        "│ 5   │ \"value for 5\"",
                        "│     │ ... truncated 5 items │"
                    },
                new [] { "│ 6   │ \"value for 6\"" }
            };

            yield return new object[]
            {
                "Truncate for two-dimensional arrays",
                new int[6, 6]
                {
                    { 1, 2, 3, 4, 5, 6 },
                    { 1, 2, 3, 4, 5, 6 },
                    { 1, 2, 3, 4, 5, 6 },
                    { 1, 2, 3, 4, 5, 6 },
                    { 1, 2, 3, 4, 5, 6 },
                    { 1, 2, 3, 4, 5, 6 }
                },
                3,
                new []
                    {
                        "│ 2      │ 1 │ 2 │ 3 │        │",
                        "│ #      │ 0 │ 1 │ 2 │ ... +3 │",
                        "│ ... +3 │   │   │   │        │"
                    },
                new []
                    {
                        "│ 3      │ 1 │ 2 │ 3 │        │"
                    }
            };

            yield return new object[]
            {
                "Truncate for three-dimensional arrays (they get flattened to a single list)",
                new int[4, 4, 4]
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
                },
                10,
                new []
                    {
                        "│ 10",
                        "│ ... truncated 54 items"
                    },
                new []
                    {
                        "│ 11"
                    }
            };
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

            for(int i = 0; i < rows; i++)
            {
                dataTable.Rows.Add(i * 1, i * 2, i * 3, i * 4, i * 5, i * 6, i * 7, i * 8, i * 9, i * 10);
            }

            return dataTable;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
