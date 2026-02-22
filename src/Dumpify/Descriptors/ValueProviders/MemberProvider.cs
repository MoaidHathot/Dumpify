using System.Reflection;

namespace Dumpify.Descriptors.ValueProviders;

internal sealed record MemberProvider : IMemberProvider
{
    private readonly bool _includeProperties;
    private readonly bool _includeFields;
    private readonly bool _includePublicMembers;
    private readonly bool _includeNonPublicMembers;
    private readonly bool _includeVirtualMembers;

    public MemberProvider()
        : this(includeProperties: true, includeFields: false, includePublicMembers: true, includeNonPublicMembers: false, includeVirtualMembers: true) { }

    public MemberProvider(
        bool includeProperties,
        bool includeFields,
        bool includePublicMembers,
        bool includeNonPublicMembers,
        bool includeVirtualMembers)
    {
        _includeProperties = includeProperties;
        _includeFields = includeFields;
        _includePublicMembers = includePublicMembers;
        _includeNonPublicMembers = includeNonPublicMembers;
        _includeVirtualMembers = includeVirtualMembers;
    }

    public IEnumerable<IValueProvider> GetMembers(Type type)
    {
        var flags = BindingFlags.Instance;

        flags |= _includePublicMembers ? BindingFlags.Public : BindingFlags.Default;
        flags |= _includeNonPublicMembers ? BindingFlags.NonPublic : BindingFlags.Default;

        var members = Enumerable.Empty<IValueProvider>();

        if (_includeProperties)
        {
            var properties = type.GetProperties(flags)
                .Where(p => p.GetIndexParameters().Length == 0)
                .Where(p => p.GetMethod is not null)
                .Where(ShouldIncludeProperty)
                .Select(p => new PropertyValueProvider(p));

            members = members.Concat(properties);
        }

        if (_includeFields)
        {
            var fields = type.GetFields(flags).Select(f => new FieldValueProvider(f));

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

        return _includeProperties == other._includeProperties
               && _includeFields == other._includeFields
               && _includePublicMembers == other._includePublicMembers
               && _includeNonPublicMembers == other._includeNonPublicMembers
               && _includeVirtualMembers == other._includeVirtualMembers;
    }

    public override int GetHashCode() =>
        (
            _includeProperties,
            _includeFields,
            _includePublicMembers,
            _includeNonPublicMembers,
            _includeVirtualMembers
        ).GetHashCode();

    private bool IsVirtualProperty(PropertyInfo property)
        => property.GetMethod?.IsVirtual is true;

    private bool ShouldIncludeProperty(PropertyInfo propertyInfo)
        => _includeVirtualMembers || !IsVirtualProperty(propertyInfo);
}
