﻿using System.Reflection;

namespace Dumpify.Descriptors.ValueProviders;

internal sealed record MemberProvider : IMemberProvider
{
    private readonly bool _includeProperties;
    private readonly bool _includeFields;
    private readonly bool _includePublicMembers;
    private readonly bool _includeNonPublicMembers;
    private readonly bool _includeVirtualMembers;
    private readonly Func<IValueProvider, bool>? _memberFilter;

    public MemberProvider()
        : this(true, false, true, false, true, null) { }

    public MemberProvider(
        bool includeProperties,
        bool includeFields,
        bool includePublicMembers,
        bool includeNonPublicMembers,
        bool includeVirtualMembers,
        Func<IValueProvider, bool>? memberFilter)
    {
        _includeProperties = includeProperties;
        _includeFields = includeFields;
        _includePublicMembers = includePublicMembers;
        _includeNonPublicMembers = includeNonPublicMembers;
        _includeVirtualMembers = includeVirtualMembers;
        _memberFilter = memberFilter;
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
                .Where(p => p.GetMethod is not null);

            if (!_includeVirtualMembers)
                properties = properties.Where(p => !IsVirtualProperty(p));

            var providers = properties.Select(p => new PropertyValueProvider(p));

            members = members.Concat(providers);
        }

        if (_includeFields)
        {
            var fields = type.GetFields(flags).Select(f => new FieldValueProvider(f));

            members = members.Concat(fields);
        }

        if(_memberFilter != null)
        {
            members = members.Where(member => _memberFilter(member));
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
               && _includeVirtualMembers == other._includeVirtualMembers
               && Equals(_memberFilter, other._memberFilter);
    }

    public override int GetHashCode() =>
        (
            _includeProperties,
            _includeFields,
            _includePublicMembers,
            _includeNonPublicMembers,
            _includeVirtualMembers,
            _memberFilter
        ).GetHashCode();
    
    private bool IsVirtualProperty(PropertyInfo propertyInfo) =>
        propertyInfo.GetAccessors().Any(accessor => accessor.IsVirtual);
}
