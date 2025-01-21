using System.Collections;
using System.Runtime.CompilerServices;

namespace Dumpify;

internal class ReferenceEqualityComparer : IEqualityComparer<object?>, IEqualityComparer
{
    private ReferenceEqualityComparer() { }

    /// <summary>
    /// Gets the singleton <see cref="ReferenceEqualityComparer"/> instance.
    /// </summary>
    public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

    public new bool Equals(object? x, object? y)
        => ReferenceEquals(x, y);

    public int GetHashCode(object? obj)
        => RuntimeHelpers.GetHashCode(obj!);
}