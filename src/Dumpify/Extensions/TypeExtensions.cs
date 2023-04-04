namespace Dumpify.Extensions;

internal static class TypeExtensions
{
    internal static string GetGenericTypeName(this Type type)
    {
        return type.IsGenericType switch
        {
            false => type.Name,
            true => $"{RemoveGenericTypeNameAnnotations(type)}<{string.Join(", ", type.GenericTypeArguments.Select(t => GetGenericTypeName(t)))}>"
        };
    }

    internal static string RemoveGenericTypeNameAnnotations(this Type type)
    {
        return type.IsGenericType switch
        {
            false => type.Name,
            true => type.Name[..type.Name.LastIndexOf("`", StringComparison.InvariantCulture)],
        };
    }
}
