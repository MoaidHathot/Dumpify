namespace Dumpify;

public class TypeNamingConfig
{
    public bool UseAliases { get; set; } = true;
    public bool UseFullName { get; set; }
    public bool ShowTypeNames { get; set; } = true;
    public bool SimplifyAnonymousObjectNames { get; set; } = true;
    public bool SeparateTypesWithSpace { get; set; } = true;
}
