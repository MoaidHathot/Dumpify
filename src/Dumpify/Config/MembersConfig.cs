using Dumpify.Descriptors.ValueProviders;

namespace Dumpify;

public class MembersConfig
{
    public bool IncludePublicMembers { get; set; } = true;
    public bool IncludeNonPublicMembers { get; set; } = false;
    public bool IncludeVirtualMembers { get; set; } = true;
    public bool IncludeProperties { get; set; } = true;
    public bool IncludeFields { get; set; } = false;
    public Func<IValueProvider, bool>? MemberFilter { get; set; }
}