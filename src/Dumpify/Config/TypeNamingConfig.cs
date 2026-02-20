namespace Dumpify;

public class TypeNamingConfig : IConfigMergeable<TypeNamingConfig>
{
    private static readonly TypeNamingConfig Defaults = new();

    public bool UseAliases { get; set; } = true;
    public bool UseFullName { get; set; }
    public bool ShowTypeNames { get; set; } = true;
    public bool SimplifyAnonymousObjectNames { get; set; } = true;
    public bool SeparateTypesWithSpace { get; set; } = true;

    /// <inheritdoc />
    public TypeNamingConfig MergeWith(TypeNamingConfig? overrideConfig)
    {
        if (overrideConfig is null)
        {
            return this;
        }

        return new TypeNamingConfig
        {
            UseAliases = ConfigMergeHelper.Merge(UseAliases, overrideConfig.UseAliases, Defaults.UseAliases),
            UseFullName = ConfigMergeHelper.Merge(UseFullName, overrideConfig.UseFullName, Defaults.UseFullName),
            ShowTypeNames = ConfigMergeHelper.Merge(ShowTypeNames, overrideConfig.ShowTypeNames, Defaults.ShowTypeNames),
            SimplifyAnonymousObjectNames = ConfigMergeHelper.Merge(SimplifyAnonymousObjectNames, overrideConfig.SimplifyAnonymousObjectNames, Defaults.SimplifyAnonymousObjectNames),
            SeparateTypesWithSpace = ConfigMergeHelper.Merge(SeparateTypesWithSpace, overrideConfig.SeparateTypesWithSpace, Defaults.SeparateTypesWithSpace),
        };
    }
}
