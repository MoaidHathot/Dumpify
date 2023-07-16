using Newtonsoft.Json.Linq;
using Spectre.Console.Rendering;
using System.Collections;
using System.Text.Json.Nodes;

namespace Dumpify.Tests.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;

[TestClass]
public class DictionaryTypeRendererTests
{
    [TestMethod]
    [DynamicData(nameof(GetDataFor_DictionaryTypeRenderer_ShouldHandleInput_WhenInputHasIsIenumerableWithGenericKeyValuePair), DynamicDataSourceType.Method)]
    public void DictionaryTypeRenderer_ShouldHandleInput_WhenInputHasIsIenumerableWithGenericKeyValuePair(object input, bool expectedItem1, List<(object?, object?)> expectedItem2)
    {
        //Arrange
        IRendererHandler<IRenderable, SpectreRendererState> handler = default!;
        var renderer = new DictionaryTypeRenderer(handler);
        var generator = new MultiValueGenerator();
        var descriptor = generator.Generate(input.GetType(), null, default!)!;

        //Act
        var result = renderer.ShouldHandle(descriptor, input);

        //Assert
        Assert.AreEqual(expectedItem1, result.Item1);
        CollectionAssert.AreEquivalent(expectedItem2, (ICollection)result.Item2!);
    }

    private static IEnumerable<object[]> GetDataFor_DictionaryTypeRenderer_ShouldHandleInput_WhenInputHasIsIenumerableWithGenericKeyValuePair()
    {
        var resultList = new List<(object, object)>()
        {
            new ("key1", "value1"),
            new ("key2", "value2"),
        };
        const string jsonstring
        = """
        {
            "key1": "value1",
            "key2": "value2"
        }
        """;
        JsonObject systemTextJsonObject = JsonNode.Parse(jsonstring)!.AsObject();
        JObject newtonsoftJsonObject = JObject.Parse(jsonstring);
        yield return new object[]
        {
            new Dictionary<string, string>()
            {
                {"key1", "value1"},
                {"key2", "value2"},
            },
            true,
            resultList
        };
        yield return new object[]
        {
            new TestEnumerable(new[]
            {
                new KeyValuePair<string, string>("key1", "value1"),
                new KeyValuePair<string, string>("key2", "value2")
            }),
            true,
            resultList
        };
        yield return new object[]
        {
            newtonsoftJsonObject,
            true,
            new List<(object, object)>()
            {
                new ("key1", new JValue("value1")),
                new ("key2", new JValue("value2")),
            }
        };
        yield return new object[]
        {
            systemTextJsonObject,
            true,
            new List<(object, object)>()
            {
                new ("key1", systemTextJsonObject.AsEnumerable().FirstOrDefault(x => x.Value!.ToString() == "value1").Value!.AsValue()),
                new ("key2", systemTextJsonObject.AsEnumerable().FirstOrDefault(x => x.Value!.ToString() == "value2").Value!.AsValue()),
            }
        };
    }

    private class TestEnumerable : IEnumerable<KeyValuePair<string, string>>
    {
        private IEnumerable<KeyValuePair<string, string>> _internalIEnumerable;

        public TestEnumerable(KeyValuePair<string, string>[] inputArray)
        {
            _internalIEnumerable = inputArray;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => new TestEnumerator(_internalIEnumerable);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class TestEnumerator : IEnumerator<KeyValuePair<string, string>>
        {
            private IEnumerator<KeyValuePair<string, string>> _internalEnumerator;

            public TestEnumerator(IEnumerable<KeyValuePair<string, string>> inputEnumerable)
            {
                _internalEnumerator = inputEnumerable.GetEnumerator();
            }

            public KeyValuePair<string, string> Current => _internalEnumerator.Current;

            object IEnumerator.Current => _internalEnumerator.Current;

            public void Dispose() => _internalEnumerator.Dispose();
            public bool MoveNext() => _internalEnumerator.MoveNext();
            public void Reset() => _internalEnumerator.Reset();
        }
    }
}
