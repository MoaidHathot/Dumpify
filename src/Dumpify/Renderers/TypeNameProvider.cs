using System.Reflection;
using System.Runtime.CompilerServices;

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
    private readonly bool _simplifyAnonymousObjectNames;

    public TypeNameProvider(bool useAliases, bool useFullTypeNames, bool simplifyAnonymousObjectNames)
    {
        _useAliases = useAliases;
        _useFullTypeNames = useFullTypeNames;
        _simplifyAnonymousObjectNames = simplifyAnonymousObjectNames;
    }

    public string GetTypeName(Type type)
    {
        var name = type.IsGenericType switch
        {
            true => GetGenericNameWithTypes(type),
            false => _useAliases ? GetNameOrAlias(type) : type.Name
        };

        name = (_useFullTypeNames, type.Namespace) switch
        {
            (_, null) => name,
            (false, _) => name,
            (true, { Length: 0 }) => name,
            (true, { Length: > 0 } prefix) => $"{prefix}.{name}",
        };

        return name;
    }

    public (string typeName, int rank) GetJaggedArrayNameWithRank(Type arrayType)
    {
        if (arrayType.IsArray is not true)
        {
            throw new ArgumentException($"The type {arrayType.FullName} is not an array");
        }

        int rank = 0;
        string name = arrayType.GetElementType() is { } elementType ? GetTypeName(elementType) : GetTypeName(arrayType);

        for (var type = arrayType; type.IsArray; type = type.GetElementType()!)
        {
            ++rank;
            name = GetTypeName(type.GetElementType()!);
        }

        return (name, rank);
    }

    private (bool simplified, string name) GetNameOrSimplifiedAnonymousObjectName(Type type)
    {
        if (_simplifyAnonymousObjectNames is not true)
        {
            return (false, type.Name);
        }

        return IsAnonymousType(type) switch
        {
            false => (false, type.Name),
            true => (true, "Anonymous Type"),
        };
    }

    private string GetGenericNameWithTypes(Type type)
    {
        var (simplified, name) = GetNameOrSimplifiedAnonymousObjectName(type);

        if (simplified)
        {
            return name;
        }

        var prefix = RemoveGenericAnnotations(name);
        var genericArguments = string.Join(", ", type.GenericTypeArguments.Select(GetTypeName));

        return $"{prefix}<{genericArguments}>";
    }

    private string RemoveGenericAnnotations(string name)
        => name[..name.LastIndexOf("`", StringComparison.InvariantCulture)];

    private string GetNameOrAlias(Type type)
        => _aliases.GetValueOrDefault(type.TypeHandle) ?? type.Name;

    private bool IsAnonymousType(Type type)
    {
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType
                && ((type.Name.StartsWith("<>") || type.Name.StartsWith("VB$")))
                && type.Name.Contains("AnonymousType")
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
    }
}