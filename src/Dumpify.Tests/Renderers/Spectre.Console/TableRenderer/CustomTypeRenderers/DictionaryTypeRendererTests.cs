using Newtonsoft.Json.Linq;
using Spectre.Console.Rendering;
using System.Collections;
using System.Text.Json.Nodes;

namespace Dumpify.Tests.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;

public class DictionaryTypeRendererTests
{
    [Theory]
    [MemberData(nameof(GetDataFor_DictionaryTypeRenderer_ShouldHandleInput_WhenInputHasIsIEnumerableWithGenericKeyValuePair))]
    public void DictionaryTypeRenderer_ShouldHandleInput_WhenInputHasIsIEnumerableWithGenericKeyValuePair(object input, bool expectedShouldHandle, List<(object?, object?)> expectedItem2)
    {
        //Arrange
        IRendererHandler<IRenderable, SpectreRendererState> handler = default!;
        var renderer = new DictionaryTypeRenderer(handler);
        var generator = new MultiValueGenerator();
        var descriptor = generator.Generate(input.GetType(), null, default!)!;

        //Act
        var result = renderer.ShouldHandle(descriptor, input);

        //Assert
        expectedShouldHandle.Should().Be(result.Item1);
        expectedItem2.Should().BeEquivalentTo((IEnumerable<(object, object)>)result.Item2!);
    }

    public static IEnumerable<object[]> GetDataFor_DictionaryTypeRenderer_ShouldHandleInput_WhenInputHasIsIEnumerableWithGenericKeyValuePair()
    {
        var resultList = new List<(object, object)>()
        {
            new ("key1", "value1"),
            new ("key2", "value2"),
        };
        const string jsonString
        = """
        {
            "key1": "value1",
            "key2": "value2"
        }
        """;
        const bool shouldHandle = true;
        const bool shouldNotHandle = false;
        JsonObject systemTextJsonObject = JsonNode.Parse(jsonString)!.AsObject();
        JObject newtonsoftJsonObject = JObject.Parse(jsonString);
        yield return new object[]
        {
            new Dictionary<string, string>()
            {
                {"key1", "value1"},
                {"key2", "value2"},
            },
            shouldHandle,
            resultList
        };
        yield return new object[]
        {
            new TestEnumerable(new[]
            {
                new KeyValuePair<string, string>("key1", "value1"),
                new KeyValuePair<string, string>("key2", "value2")
            }),
            shouldHandle,
            resultList
        };
        yield return new object[]
        {
            newtonsoftJsonObject,
            shouldHandle,
            new List<(object, object)>()
            {
                new ("key1", new JValue("value1")),
                new ("key2", new JValue("value2")),
            }
        };
        yield return new object[]
        {
            systemTextJsonObject,
            shouldHandle,
            new List<(object, object)>()
            {
                new ("key1", systemTextJsonObject.AsEnumerable().First(x => x.Value!.ToString() == "value1").Value!.AsValue()),
                new ("key2", systemTextJsonObject.AsEnumerable().First(x => x.Value!.ToString() == "value2").Value!.AsValue()),
            }
        };
        yield return new object[]
        {
            new string[] {"value"},
            shouldNotHandle,
            default!
        };
    }

    private class TestEnumerable : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _internalIEnumerable;

        public TestEnumerable(KeyValuePair<string, string>[] inputArray)
        {
            _internalIEnumerable = inputArray;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => new TestEnumerator(_internalIEnumerable);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class TestEnumerator : IEnumerator<KeyValuePair<string, string>>
        {
            private readonly IEnumerator<KeyValuePair<string, string>> _internalEnumerator;

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
