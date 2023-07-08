namespace Dumpify;

public interface ITypeNameProvider
{
    string GetTypeName(Type type);
    (string typeName, int rank) GetJaggedArrayNameWithRank(Type arrayType);
}