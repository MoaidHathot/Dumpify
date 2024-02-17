namespace Dumpify;

public class MembersConfig
{
    public bool IncludePublicMembers { get; set; } = true;
    public bool IncludeNonePublicMembers { get; set; } = false;
    public bool IncludeProperties { get; set; } = true;
    public bool IncludeFields { get; set; } = false;
}
