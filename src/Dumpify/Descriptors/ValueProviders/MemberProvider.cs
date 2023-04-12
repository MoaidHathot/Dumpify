using Dumpify.Config;
using Dumpify.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;
using System.Reflection;

namespace Dumpify.Descriptors.ValueProviders;

internal class MemberProvider : IMemberProvider
{
    private readonly bool _includeProperties;
    private readonly bool _includeFields;
    private readonly bool _includePublicMembers;
    private readonly bool _includeNonPublicMembers;

    public MemberProvider()
        : this(true, false, true, false)
    {
        
    }
    public MemberProvider(bool includeProperties, bool includeFields, bool includePublicMembers, bool includeNonPublicMembers)
    {
        _includeProperties = includeProperties;
        _includeFields = includeFields;
        _includePublicMembers = includePublicMembers;
        _includeNonPublicMembers = includeNonPublicMembers;
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
}
