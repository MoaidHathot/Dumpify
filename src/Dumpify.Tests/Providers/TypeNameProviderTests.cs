using Dumpify.Renderers;
using Dumpify.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Tests.Providers;

[TestClass]
public class TypeNameProviderTests
{
    [TestMethod]
    [DataRow(typeof(List<int>), "List<Int32>", false, false)]
    [DataRow(typeof(List<int>), "List<int>", true, false)]
    [DataRow(typeof(List<int>), "System.Collections.Generic.List<System.Int32>", false, true)]
    [DataRow(typeof(List<int>), "System.Collections.Generic.List<System.int>", true, true)]
    [DataRow(typeof(string), "String", false, false)]
    [DataRow(typeof(string), "string", true, false)]
    [DataRow(typeof(string), "System.String", false, true)]
    [DataRow(typeof(string), "System.string", true, true)]
    [DataRow(typeof(StringBuilder), "StringBuilder", false, false)]
    [DataRow(typeof(StringBuilder), "StringBuilder", true, false)]
    [DataRow(typeof(StringBuilder), "System.Text.StringBuilder", false, true)]
    [DataRow(typeof(StringBuilder), "System.Text.StringBuilder", true, true)]
    [DataRow(typeof(Dictionary<string, long>), "Dictionary<String, Int64>", false, false)]
    [DataRow(typeof(Dictionary<string, long>), "Dictionary<string, long>", true, false)]
    [DataRow(typeof(Dictionary<string, long>), "System.Collections.Generic.Dictionary<System.String, System.Int64>", false, true)]
    [DataRow(typeof(Dictionary<string, long>), "System.Collections.Generic.Dictionary<System.string, System.long>", true, true)]
    [DataRow(typeof(Stack<int>), "Stack<Int32>", false, false)]
    [DataRow(typeof(Stack<int>), "Stack<int>", true, false)]
    [DataRow(typeof(Stack<int>), "System.Collections.Generic.Stack<System.Int32>", false, true)]
    [DataRow(typeof(Stack<int>), "System.Collections.Generic.Stack<System.int>", true, true)]
    [DataRow(typeof(Dictionary<string, Dictionary<string, int>>), "Dictionary<String, Dictionary<String, Int32>>", false, false)]
    [DataRow(typeof(Dictionary<string, Dictionary<string, int>>), "Dictionary<string, Dictionary<string, int>>", true, false)]
    [DataRow(typeof(Dictionary<string, Dictionary<string, int>>), "System.Collections.Generic.Dictionary<System.String, System.Collections.Generic.Dictionary<System.String, System.Int32>>", false, true)]
    [DataRow(typeof(Dictionary<string, Dictionary<string, int>>), "System.Collections.Generic.Dictionary<System.string, System.Collections.Generic.Dictionary<System.string, System.int>>", true, true)]
    [DataRow(typeof(Func<int>), "Func<Int32>", false, false)]
    [DataRow(typeof(Func<int>), "Func<int>", true, false)]
    [DataRow(typeof(Func<int>), "System.Func<System.Int32>", false, true)]
    [DataRow(typeof(Func<int>), "System.Func<System.int>", true, true)]
    public void GetGenericNameUseGenericAnnotations(Type type, string expectedTypeName, bool useAliases, bool useFullTypeName)
    {
        var provider = new TypeNameProvider(useAliases, useFullTypeName);

        var result = provider.GetTypeName(type);

        result.Should().Be(expectedTypeName);
    }

    [TestMethod]
    [DataRow(typeof(int[]), 1, "Int32", false, false)]
    [DataRow(typeof(int[]), 1, "System.Int32", false, true)]
    [DataRow(typeof(int[]), 1, "int", true, false)]
    [DataRow(typeof(int[]), 1, "System.int", true, true)]
    [DataRow(typeof(int[][]), 2, "Int32", false, false)]
    [DataRow(typeof(int[][]), 2, "System.Int32", false, true)]
    [DataRow(typeof(int[][]), 2, "int", true, false)]
    [DataRow(typeof(int[][]), 2, "System.int", true, true)]
    [DataRow(typeof(string[]), 1, "String", false, false)]
    [DataRow(typeof(string[]), 1, "System.String", false, true)]
    [DataRow(typeof(string[]), 1, "string", true, false)]
    [DataRow(typeof(string[]), 1, "System.string", true, true)]
    [DataRow(typeof(string[][]), 2, "String", false, false)]
    [DataRow(typeof(string[][]), 2, "System.String", false, true)]
    [DataRow(typeof(string[][]), 2, "string", true, false)]
    [DataRow(typeof(string[][]), 2, "System.string", true, true)]
    [DataRow(typeof(List<int>[][][]), 3, "List<Int32>", false, false)]
    [DataRow(typeof(List<int>[][][]), 3, "System.Collections.Generic.List<System.Int32>", false, true)]
    [DataRow(typeof(List<int>[][][]), 3, "List<int>", true, false)]
    [DataRow(typeof(List<int>[][][]), 3, "System.Collections.Generic.List<System.int>", true, true)]
    public void GetArrayNameAndRankReturnCorrectResults(Type type, int expectedRank, string expectedElementName, bool useAliases, bool useFullTypeName)
    {
        var provider = new TypeNameProvider(useAliases, useFullTypeName);

        var (name, rank) = provider.GetJaggedArrayNameWithRank(type);

        rank.Should().Be(expectedRank);
        name.Should().Be(expectedElementName);
    }

    [TestMethod]
    [DataRow(typeof(List<int>), false, false)]
    [DataRow(typeof(List<int>), false, true)]
    [DataRow(typeof(List<int>), true, false)]
    [DataRow(typeof(List<int>), true, true)]
    [DataRow(typeof(string), false, false)]
    [DataRow(typeof(string), false, true)]
    [DataRow(typeof(string), true, false)]
    [DataRow(typeof(string), true, true)]
    public void GetArrayNameAndRankThrowsException(Type type, bool useAliases, bool useFullTypeName)
    {
        var provider = new TypeNameProvider(useAliases, useFullTypeName);

        var func = () => provider.GetJaggedArrayNameWithRank(type);
        func.Should().Throw<ArgumentException>();
    }
}
