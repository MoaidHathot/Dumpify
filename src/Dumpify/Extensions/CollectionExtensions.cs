namespace Dumpify.Extensions;
internal static class CollectionExtensions
{
    internal static bool None<T>(this IEnumerable<T> source)
        => !source.Any();
}
