namespace Dumpify;

/// <summary>
/// Structured representation of a reference label (circular or shared).
/// The renderer uses this to format the final output string.
/// </summary>
/// <param name="TypeName">The type name (e.g., "Person") or null if not included</param>
/// <param name="Id">The object ID (e.g., 1) or null if not included</param>
/// <param name="Path">The path where first seen (e.g., "Manager.Reports[0]") or null if not included</param>
public record ReferenceLabel(string? TypeName, int? Id, string? Path);