namespace Dumpify;

public class MembersConfig : IConfigMergeable<MembersConfig>
{
    private static readonly MembersConfig Defaults = new();

    public bool IncludePublicMembers { get; set; } = true;
    public bool IncludeNonPublicMembers { get; set; } = false;
    public bool IncludeVirtualMembers { get; set; } = true;
    public bool IncludeProperties { get; set; } = true;
    public bool IncludeFields { get; set; } = false;
    public Func<MemberFilterContext, bool>? MemberFilter { get; set; }

    /// <inheritdoc />
    public MembersConfig MergeWith(MembersConfig? overrideConfig)
    {
        if (overrideConfig is null)
        {
            return this;
        }

        return new MembersConfig
        {
            IncludePublicMembers = ConfigMergeHelper.Merge(IncludePublicMembers, overrideConfig.IncludePublicMembers, Defaults.IncludePublicMembers),
            IncludeNonPublicMembers = ConfigMergeHelper.Merge(IncludeNonPublicMembers, overrideConfig.IncludeNonPublicMembers, Defaults.IncludeNonPublicMembers),
            IncludeVirtualMembers = ConfigMergeHelper.Merge(IncludeVirtualMembers, overrideConfig.IncludeVirtualMembers, Defaults.IncludeVirtualMembers),
            IncludeProperties = ConfigMergeHelper.Merge(IncludeProperties, overrideConfig.IncludeProperties, Defaults.IncludeProperties),
            IncludeFields = ConfigMergeHelper.Merge(IncludeFields, overrideConfig.IncludeFields, Defaults.IncludeFields),
            // MemberFilter is a delegate - if override provides one, always use it; otherwise use base
            MemberFilter = overrideConfig.MemberFilter ?? MemberFilter,
        };
    }
}