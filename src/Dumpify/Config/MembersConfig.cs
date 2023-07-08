namespace Dumpify;

public class MembersConfig
{
    public bool IncludePublicMembers { get; init; } = true;
    public bool IncludeNonPublicMembers { get; init; } = false;
    public bool IncludeProperties { get; init; } = true;
    public bool IncludeFields { get; init; } = false;
}
