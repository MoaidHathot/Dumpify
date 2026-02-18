using VerifyXunit;
using static VerifyXunit.Verifier;

namespace Dumpify.Tests.Renderers;

/// <summary>
/// Snapshot tests for collection type rendering.
/// Tests arrays, lists, dictionaries, sets, and other collection types.
/// </summary>
public class CollectionRenderingTests
{
    #region Arrays

    [Fact]
    public Task IntArray_Renders()
    {
        var array = new[] { 1, 2, 3, 4, 5 };
        return Verify(array.DumpText());
    }

    [Fact]
    public Task StringArray_Renders()
    {
        var array = new[] { "apple", "banana", "cherry" };
        return Verify(array.DumpText());
    }

    [Fact]
    public Task EmptyArray_Renders()
    {
        var array = Array.Empty<int>();
        return Verify(array.DumpText());
    }

    [Fact]
    public Task ArrayWithNulls_Renders()
    {
        var array = new string?[] { "hello", null, "world" };
        return Verify(array.DumpText());
    }

    [Fact]
    public Task MultiDimensionalArray_Renders()
    {
        var array = new int[,] { { 1, 2, 3 }, { 4, 5, 6 } };
        return Verify(array.DumpText());
    }

    [Fact]
    public Task JaggedArray_Renders()
    {
        var array = new int[][] 
        { 
            new[] { 1, 2 }, 
            new[] { 3, 4, 5 }, 
            new[] { 6 } 
        };
        return Verify(array.DumpText());
    }

    #endregion

    #region Lists

    [Fact]
    public Task List_Renders()
    {
        var list = new List<int> { 10, 20, 30 };
        return Verify(list.DumpText());
    }

    [Fact]
    public Task EmptyList_Renders()
    {
        var list = new List<string>();
        return Verify(list.DumpText());
    }

    [Fact]
    public Task ListOfObjects_Renders()
    {
        var list = new List<object> { 1, "two", 3.0, true };
        return Verify(list.DumpText());
    }

    [Fact]
    public Task LinkedList_Renders()
    {
        var list = new LinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);
        return Verify(list.DumpText());
    }

    #endregion

    #region Dictionaries

    [Fact]
    public Task Dictionary_StringToInt_Renders()
    {
        var dict = new Dictionary<string, int>
        {
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3
        };
        return Verify(dict.DumpText());
    }

    [Fact]
    public Task Dictionary_IntToString_Renders()
    {
        var dict = new Dictionary<int, string>
        {
            [1] = "one",
            [2] = "two",
            [3] = "three"
        };
        return Verify(dict.DumpText());
    }

    [Fact]
    public Task EmptyDictionary_Renders()
    {
        var dict = new Dictionary<string, string>();
        return Verify(dict.DumpText());
    }

    [Fact]
    public Task DictionaryWithNullValue_Renders()
    {
        var dict = new Dictionary<string, string?>
        {
            ["key1"] = "value1",
            ["key2"] = null,
            ["key3"] = "value3"
        };
        return Verify(dict.DumpText());
    }

    [Fact]
    public Task SortedDictionary_Renders()
    {
        var dict = new SortedDictionary<string, int>
        {
            ["zebra"] = 3,
            ["apple"] = 1,
            ["banana"] = 2
        };
        return Verify(dict.DumpText());
    }

    #endregion

    #region Sets

    [Fact]
    public Task HashSet_Renders()
    {
        var set = new HashSet<int> { 3, 1, 4, 1, 5, 9 }; // duplicates removed
        return Verify(set.DumpText());
    }

    [Fact]
    public Task SortedSet_Renders()
    {
        var set = new SortedSet<string> { "cherry", "apple", "banana" };
        return Verify(set.DumpText());
    }

    [Fact]
    public Task EmptyHashSet_Renders()
    {
        var set = new HashSet<int>();
        return Verify(set.DumpText());
    }

    #endregion

    #region Queues and Stacks

    [Fact]
    public Task Queue_Renders()
    {
        var queue = new Queue<int>();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);
        return Verify(queue.DumpText());
    }

    [Fact]
    public Task Stack_Renders()
    {
        var stack = new Stack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        return Verify(stack.DumpText());
    }

    [Fact]
    public Task PriorityQueue_Renders()
    {
        var pq = new PriorityQueue<string, int>();
        pq.Enqueue("low", 3);
        pq.Enqueue("high", 1);
        pq.Enqueue("medium", 2);
        return Verify(pq.DumpText());
    }

    #endregion

    #region Tuples

    [Fact]
    public Task ValueTuple_TwoElements_Renders()
    {
        var tuple = (1, "hello");
        return Verify(tuple.DumpText());
    }

    [Fact]
    public Task ValueTuple_NamedElements_Renders()
    {
        var tuple = (Id: 42, Name: "Test", IsActive: true);
        return Verify(tuple.DumpText());
    }

    [Fact]
    public Task ValueTuple_ManyElements_Renders()
    {
        var tuple = (1, 2, 3, 4, 5, 6, 7, 8);
        return Verify(tuple.DumpText());
    }

    [Fact]
    public Task ClassicTuple_Renders()
    {
        var tuple = Tuple.Create(1, "hello", true);
        return Verify(tuple.DumpText());
    }

    #endregion

    #region Nested Collections

    [Fact]
    public Task ListOfLists_Renders()
    {
        var list = new List<List<int>>
        {
            new List<int> { 1, 2, 3 },
            new List<int> { 4, 5 },
            new List<int> { 6, 7, 8, 9 }
        };
        return Verify(list.DumpText());
    }

    [Fact]
    public Task DictionaryOfLists_Renders()
    {
        var dict = new Dictionary<string, List<int>>
        {
            ["evens"] = new List<int> { 2, 4, 6 },
            ["odds"] = new List<int> { 1, 3, 5 }
        };
        return Verify(dict.DumpText());
    }

    [Fact]
    public Task ArrayOfDictionaries_Renders()
    {
        var array = new Dictionary<string, int>[]
        {
            new Dictionary<string, int> { ["a"] = 1 },
            new Dictionary<string, int> { ["b"] = 2, ["c"] = 3 }
        };
        return Verify(array.DumpText());
    }

    #endregion

    #region Special Collections

    [Fact]
    public Task KeyValuePair_Renders()
    {
        var kvp = new KeyValuePair<string, int>("key", 42);
        return Verify(kvp.DumpText());
    }

    [Fact]
    public Task IEnumerable_FromRange_Renders()
    {
        var enumerable = Enumerable.Range(1, 5);
        return Verify(enumerable.DumpText());
    }

    [Fact]
    public Task ArraySegment_Renders()
    {
        var array = new[] { 1, 2, 3, 4, 5 };
        var segment = new ArraySegment<int>(array, 1, 3);
        return Verify(segment.DumpText());
    }

    [Fact]
    public Task ReadOnlyCollection_Renders()
    {
        var list = new List<int> { 1, 2, 3 };
        var readOnly = list.AsReadOnly();
        return Verify(readOnly.DumpText());
    }

    #endregion
}
