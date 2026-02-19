using Dumpify.Descriptors.ValueProviders;

namespace Dumpify;

/// <summary>
/// Provides context for member filtering during rendering.
/// </summary>
public readonly struct MemberFilterContext
{
    internal MemberFilterContext(IValueProvider member, object source, int depth)
    {
        Member = member;
        Source = source;
        Depth = depth;
    }

    /// <summary>
    /// The member (property/field) being filtered.
    /// </summary>
    public IValueProvider Member { get; } 

    /// <summary>
    /// The actual value of the member. Lazily evaluated when accessed.
    /// </summary>
    public object? Value => Member.GetValue(Source);

    /// <summary>
    /// The parent object that contains this member.
    /// </summary>
    public object Source { get; }

    /// <summary>
    /// The current rendering depth.
    /// </summary>
    public int Depth { get; }
}