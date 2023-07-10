namespace Dumpify.Extensions;

internal static class CollectionExtensions
{
    internal static bool None<T>(this IEnumerable<T> source)
        => !source.Any();

#if NETSTANDARD2_0
    public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        => dictionary.GetValueOrDefault(key, default);

    public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue)
    {
        if (dictionary is null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        return dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
    }
#endif
}