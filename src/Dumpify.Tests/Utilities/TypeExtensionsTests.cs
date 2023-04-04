using Dumpify.Extensions;
using System.Text;

namespace Dumpify.Tests.Utilities;

[TestClass]
public class TypeExtensionsTests
{
    [TestMethod]
    [DataRow(typeof(int[]), 1, "Int32")]
    [DataRow(typeof(int[][]), 2, "Int32")]
    [DataRow(typeof(string[]), 1, "String")]
    [DataRow(typeof(string[][]), 2, "String")]
    [DataRow(typeof(List<int>[][][]), 3, "List`1")]
    [DataRow(typeof(List<int>), 0, "List`1")]
    [DataRow(typeof(string), 0, "String")]
    public void GetArrayNameAndRankReturnCorrectResults(Type type, int expectedRank, string expectedElementName)
    {
        var (name, rank) = type.GetJaggedArrayNameAndRank();

        rank.Should().Be(expectedRank);
        name.Should().Be(expectedElementName);
    }

    [TestMethod]
    [DataRow(typeof(List<int>), "List")]
    [DataRow(typeof(Stack<int>), "Stack")]
    [DataRow(typeof(Dictionary<string, int>), "Dictionary")]
    [DataRow(typeof(List<Stack<int>>), "List")]
    public void RemoveGenericTypeNameAnnotationsReturnCorrectResults(Type type, string expectedTypeName)
    {
        var result = type.GetTypeNameWithoutGenericTypeAnnotations();

        result.Should().Be(expectedTypeName);
    }

    [TestMethod]
    [DataRow(typeof(List<int>), "List<Int32>")]
    [DataRow(typeof(string), "String")]
    [DataRow(typeof(StringBuilder), "StringBuilder")]
    [DataRow(typeof(Dictionary<string, long>), "Dictionary<String, Int64>")]
    [DataRow(typeof(Stack<int>), "Stack<Int32>")]
    public void GetGenericTypeNameReturnCorrectResults(Type type, string expectedName)
    {
        var result = type.GetGenericTypeName();

        result.Should().Be(expectedName);
    }
}
