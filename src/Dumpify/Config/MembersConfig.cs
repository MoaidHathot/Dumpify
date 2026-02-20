namespace Dumpify;

public class MembersConfig : ConfigBase<MembersConfig>
{
    public TrackableProperty<bool> IncludePublicMembers { get; set; } = new(true);
    public TrackableProperty<bool> IncludeNonPublicMembers { get; set; } = new(false);
    public TrackableProperty<bool> IncludeVirtualMembers { get; set; } = new(true);
    public TrackableProperty<bool> IncludeProperties { get; set; } = new(true);
    public TrackableProperty<bool> IncludeFields { get; set; } = new(false);
    public Func<MemberFilterContext, bool>? MemberFilter { get; set; }

    /// <inheritdoc />
    protected override MembersConfig MergeOverride(MembersConfig overrideConfig)
    {
        return new MembersConfig
        {
            IncludePublicMembers = Merge(IncludePublicMembers, overrideConfig.IncludePublicMembers),
            IncludeNonPublicMembers = Merge(IncludeNonPublicMembers, overrideConfig.IncludeNonPublicMembers),
            IncludeVirtualMembers = Merge(IncludeVirtualMembers, overrideConfig.IncludeVirtualMembers),
            IncludeProperties = Merge(IncludeProperties, overrideConfig.IncludeProperties),
            IncludeFields = Merge(IncludeFields, overrideConfig.IncludeFields),
            // MemberFilter is a delegate - if override provides one, always use it; otherwise use base
            MemberFilter = overrideConfig.MemberFilter ?? MemberFilter,
        };
    }
}
