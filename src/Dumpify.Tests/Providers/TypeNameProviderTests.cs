using Dumpify.Renderers;
using System.Text;

namespace Dumpify.Tests.Providers
{
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
        [DataRow(typeof(TestType), "TestType", false, false)]
        [DataRow(typeof(TestType), "TestType", true, false)]
        [DataRow(typeof(TestType), "TestType", false, true)]
        [DataRow(typeof(TestType), "TestType", true, true)]
        public void GetTypeNameReturnCorrectResult(Type type, string expectedTypeName, bool useAliases, bool useFullTypeName)
        {
            var provider = new TypeNameProvider(useAliases, useFullTypeName, false);

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
            var provider = new TypeNameProvider(useAliases, useFullTypeName, false);

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
            var provider = new TypeNameProvider(useAliases, useFullTypeName, false);

            var func = () => provider.GetJaggedArrayNameWithRank(type);
            func.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        [DataRow(true, true, true)]
        [DataRow(true, true, false)]
        [DataRow(true, false, true)]
        [DataRow(true, false, false)]
        [DataRow(false, true, true)]
        [DataRow(false, true, false)]
        [DataRow(false, false, false)]
        [DataRow(false, false, true)]
        public void AnonymousTypeNameAreSimplified(bool useAliases, bool useFullTypeNames, bool simplifyAnonymousObjectNames)
        {
            var provider = new TypeNameProvider(useAliases, useFullTypeNames, simplifyAnonymousObjectNames);

            var obj = new { Foo = "Foo1", Bar = 2 };

            var result = provider.GetTypeName(obj.GetType());

            if (simplifyAnonymousObjectNames)
            {
                result.Should().Be("Anonymous Type");
            }
            else
            {
                var genericArgs = (useAliases, useFullTypeNames) switch
                {
                    (true, true) => "<System.string, System.int>",
                    (true, false) => "<string, int>",
                    (false, true) => "<System.String, System.Int32>",
                    (false, false) => "<String, Int32>",
                };

            var anonymousTypeName = obj.GetType().Name;
            var prefix = anonymousTypeName[..anonymousTypeName.LastIndexOf('`')];

                var expected = $"{prefix}{genericArgs}";
                result.Should().Be(expected);
            }
        }

        [TestMethod]
        [DataRow(true, true, true)]
        [DataRow(true, true, false)]
        [DataRow(true, false, true)]
        [DataRow(true, false, false)]
        [DataRow(false, true, true)]
        [DataRow(false, true, false)]
        [DataRow(false, false, false)]
        [DataRow(false, false, true)]
        public void GetArrayNameAndRankReturnCorrectSimplifiedAnonymousTypeName(bool useAliases, bool useFullTypeName, bool simplifyAnonymousObjectNames)
        {
            var provider = new TypeNameProvider(useAliases, useFullTypeName, simplifyAnonymousObjectNames);

            var array1 = Enumerable.Range(0, 3).Select(i => new { Index = i }).ToArray();
            var array2 = Enumerable.Range(1, 3).Select(_ => array1).ToArray();
            var array3 = Enumerable.Range(1, 3).Select(_ => array2).ToArray();

            var (name1, rank1) = provider.GetJaggedArrayNameWithRank(array1.GetType());
            var (name2, rank2) = provider.GetJaggedArrayNameWithRank(array2.GetType());
            var (name3, rank3) = provider.GetJaggedArrayNameWithRank(array3.GetType());

            var anonymousTypeName = array1[0].GetType().Name;
            var prefix = anonymousTypeName[..anonymousTypeName.LastIndexOf('`')];

            var expectedElementName = (simplifyAnonymousObjectNames, useAliases, useFullTypeName) switch
            {
                (true, _, _) => "Anonymous Type",
                (false, true, true) => $"{prefix}<System.int>",
                (false,true, false) => $"{prefix}<int>",
                (false,false, true) => $"{prefix}<System.Int32>",
                (false,false, false) => $"{prefix}<Int32>",
            };

            rank1.Should().Be(1);
            name1.Should().Be(expectedElementName);

            rank2.Should().Be(2);
            name2.Should().Be(expectedElementName);

            rank3.Should().Be(3);
            name3.Should().Be(expectedElementName);
        }


        [TestMethod]
        public void NamespaceOfTestTypeIsNull()
        {
            //Utility test for making sure that the Namespace of `TestType` stays null during future refactoring
            //This is an assumption in other tests, and mainly in GetTypeNameReturnCorrectResult
            typeof(TestType).Namespace.Should().BeNull();
        }
    }
}


//This type should be outside of namespace so that it shall be null
public record TestType(string Prop1, string Prop2);
