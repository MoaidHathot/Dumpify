namespace Dumpify;

public class TypeNamingConfig : ConfigBase<TypeNamingConfig>
{
    public TrackableProperty<bool> UseAliases { get; set; } = new(true);
    public TrackableProperty<bool> UseFullName { get; set; } = new(false);
    public TrackableProperty<bool> ShowTypeNames { get; set; } = new(true);
    public TrackableProperty<bool> SimplifyAnonymousObjectNames { get; set; } = new(true);
    public TrackableProperty<bool> SeparateTypesWithSpace { get; set; } = new(true);

    /// <inheritdoc />
    protected override TypeNamingConfig MergeOverride(TypeNamingConfig overrideConfig)
    {
        return new TypeNamingConfig
        {
            UseAliases = Merge(UseAliases, overrideConfig.UseAliases),
            UseFullName = Merge(UseFullName, overrideConfig.UseFullName),
            ShowTypeNames = Merge(ShowTypeNames, overrideConfig.ShowTypeNames),
            SimplifyAnonymousObjectNames = Merge(SimplifyAnonymousObjectNames, overrideConfig.SimplifyAnonymousObjectNames),
            SeparateTypesWithSpace = Merge(SeparateTypesWithSpace, overrideConfig.SeparateTypesWithSpace),
        };
    }
}
