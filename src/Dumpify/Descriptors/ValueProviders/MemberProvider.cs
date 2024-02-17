using System.Reflection;

namespace Dumpify.Descriptors.ValueProviders;

internal sealed record MemberProvider : IMemberProvider
{
    private readonly bool _includeProperties;
    private readonly bool _includeFields;
    private readonly bool _includePublicMembers;
    private readonly bool _includeNonePublicMembers;

    public MemberProvider()
        : this(true, false, true, false)
    {

    }

    public MemberProvider(bool includeProperties, bool includeFields, bool includePublicMembers, bool includeNonePublicMembers)
    {
        _includeProperties = includeProperties;
        _includeFields = includeFields;
        _includePublicMembers = includePublicMembers;
        _includeNonePublicMembers = includeNonePublicMembers;
    }

    public IEnumerable<IValueProvider> GetMembers(Type type)
    {
        var flags = BindingFlags.Instance;

        flags |= _includePublicMembers ? BindingFlags.Public : BindingFlags.Default;
        flags |= _includeNonePublicMembers ? BindingFlags.NonPublic : BindingFlags.Default;

        var members = Enumerable.Empty<IValueProvider>();

        if (_includeProperties)
        {
            var properties = type.GetProperties(flags)
                .Where(p => p.GetIndexParameters().Length == 0)
                .Where(p => p.GetMethod is not null)
                .Select(p => new PropertyValueProvider(p));

            members = members.Concat(properties);
        }

        if (_includeFields)
        {
            var fields = type.GetFields(flags)
                .Select(f => new FieldValueProvider(f));

            members = members.Concat(fields);
        }

        return members;
    }

    public bool Equals(IMemberProvider? provider)
    {
        if (provider is not MemberProvider other)
        {
            return false;
        }

        return _includeProperties == other._includeProperties && _includeFields == other._includeFields && _includePublicMembers == other._includePublicMembers && _includeNonePublicMembers == other._includeNonePublicMembers;
    }

    public override int GetHashCode()
        => (_includeProperties, _includeFields, _includePublicMembers, _includeNonePublicMembers).GetHashCode();
}