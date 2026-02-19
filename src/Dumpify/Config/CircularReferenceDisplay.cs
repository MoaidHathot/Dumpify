namespace Dumpify;

/// <summary>
/// Flags to control what information is displayed for circular references.
/// Combine flags using bitwise OR: TypeName | Id | Path
/// </summary>
[Flags]
public enum CircularReferenceDisplay
{
    /// <summary>
    /// Display the type name (e.g., #Person)
    /// </summary>
    TypeName = 1,

    /// <summary>
    /// Display the object ID (e.g., :1)
    /// </summary>
    Id = 2,

    /// <summary>
    /// Display the path where the object was first seen (e.g., → Manager.Reports[0])
    /// </summary>
    Path = 4,
}
