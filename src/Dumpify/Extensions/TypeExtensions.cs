namespace Dumpify.Extensions;

internal static class TypeExtensions
{
    internal static string GetGenericTypeName(this Type type)
    {
        return type.IsGenericType switch
        {
            false => type.Name,
            true => $"{GetTypeNameWithoutGenericTypeAnnotations(type)}<{string.Join(", ", type.GenericTypeArguments.Select(t => GetGenericTypeName(t)))}>"
        };
    }

    internal static string GetTypeNameWithoutGenericTypeAnnotations(this Type type)
    {
        return type.IsGenericType switch
        {
            false => type.Name,
            true => type.Name[..type.Name.LastIndexOf("`", StringComparison.InvariantCulture)],
        };
    }

    internal static (string typeName, int rank) GetJaggedArrayNameAndRank(this Type arrayType)
    {
        if(arrayType.IsArray is false)
        {
            return (arrayType.Name, 0);
        }

        int rank = 0;
        string name = arrayType.GetElementType()?.Name ?? arrayType.Name;
        
        for(var type = arrayType.GetElementType; arrayType.IsArray; arrayType = arrayType.GetElementType()!)
        {
            ++rank;
            name = arrayType.GetElementType()!.Name;
        }

        return (name, rank);
    }
}
