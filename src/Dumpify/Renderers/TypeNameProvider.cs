namespace Dumpify.Renderers;

internal class TypeNameProvider : ITypeNameProvider
{
    private static readonly Dictionary<RuntimeTypeHandle, string> _aliases = new()
    {
        [typeof(string).TypeHandle] = "string",
        [typeof(int).TypeHandle] = "int",
        [typeof(uint).TypeHandle] = "uint",
        [typeof(byte).TypeHandle] = "byte",
        [typeof(sbyte).TypeHandle] = "sbyte",
        [typeof(short).TypeHandle] = "short",
        [typeof(ushort).TypeHandle] = "ushort",
        [typeof(long).TypeHandle] = "long",
        [typeof(ulong).TypeHandle] = "ulong",
        [typeof(float).TypeHandle] = "float",
        [typeof(double).TypeHandle] = "double",
        [typeof(decimal).TypeHandle] = "decimal",
        [typeof(object).TypeHandle] = "object",
        [typeof(bool).TypeHandle] = "bool",
        [typeof(char).TypeHandle] = "char",
    };

    private readonly bool _useAliases;
    private readonly bool _useFullTypeNames;

    public TypeNameProvider(bool useAliases, bool useFullTypeNames)
    {
        _useAliases = useAliases;
        _useFullTypeNames = useFullTypeNames;
    }

    public string GetTypeName(Type type)
    {
        var name = type.IsGenericType switch
        {
            true => GetGenericNameWithTypes(type, type.Name),
            false => _useAliases ? GetNameOrAlias(type) : type.Name
        };

        name = _useFullTypeNames switch
        {
            true => $"{(GetNamespace(type) is { Length: > 0 } str ? str : "")}.{name}",
            false => name,
        };

        return name;
    }

    public (string typeName, int rank) GetJaggedArrayNameWithRank(Type arrayType)
    {
        if(arrayType.IsArray is not true)
        {
            throw new ArgumentException($"The type {arrayType.FullName} is not an array");
        }

        int rank = 0;
        string name = arrayType.GetElementType() is { } elementType ? GetTypeName(elementType) : GetTypeName(arrayType);

        for(var type = arrayType; type.IsArray; type = type.GetElementType()!)
        {
            ++rank;
            name = GetTypeName(type.GetElementType()!);
        }

        return (name, rank);
    }

    private string GetNamespace(Type type)
        => type.Namespace ?? "";

    private string GetGenericNameWithTypes(Type type, string name)
        => $"{RemoveGenericAnnotations(name)}<{string.Join(", ", type.GenericTypeArguments.Select(GetTypeName))}>";

    private string RemoveGenericAnnotations(string name)
        => name[..name.LastIndexOf("`", StringComparison.InvariantCulture)];

    private string GetNameOrAlias(Type type)
        => _aliases.GetValueOrDefault(type.TypeHandle) ?? type.Name;
}
