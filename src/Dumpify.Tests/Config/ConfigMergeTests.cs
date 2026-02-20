using System.Drawing;

namespace Dumpify.Tests.Config;

public class ConfigMergeTests
{
    #region TableConfig Tests

    [Fact]
    public void TableConfig_MergeWith_Null_ReturnsSameInstance()
    {
        var baseConfig = new TableConfig { ShowMemberTypes = true };

        var result = baseConfig.MergeWith(null);

        Assert.Same(baseConfig, result);
    }

    [Fact]
    public void TableConfig_MergeWith_PreservesBaseValues_WhenOverrideHasDefaults()
    {
        var baseConfig = new TableConfig
        {
            ShowArrayIndices = false,
            ShowTableHeaders = false,
            ShowMemberTypes = true,
            ShowRowSeparators = true,
            BorderStyle = TableBorderStyle.Heavy
        };

        // Override with default values - should NOT override base
        var overrideConfig = new TableConfig();

        var result = baseConfig.MergeWith(overrideConfig);

        Assert.False(result.ShowArrayIndices);
        Assert.False(result.ShowTableHeaders);
        Assert.True(result.ShowMemberTypes);
        Assert.True(result.ShowRowSeparators);
        Assert.Equal(TableBorderStyle.Heavy, (TableBorderStyle)result.BorderStyle);
    }

    [Fact]
    public void TableConfig_MergeWith_OverridesOnlySpecifiedValues()
    {
        var baseConfig = new TableConfig
        {
            ShowMemberTypes = true,
            ShowRowSeparators = true,
            BorderStyle = TableBorderStyle.Heavy
        };

        // Only override ShowArrayIndices
        var overrideConfig = new TableConfig { ShowArrayIndices = false };

        var result = baseConfig.MergeWith(overrideConfig);

        // Base values should be preserved
        Assert.True(result.ShowMemberTypes);
        Assert.True(result.ShowRowSeparators);
        Assert.Equal(TableBorderStyle.Heavy, (TableBorderStyle)result.BorderStyle);

        // Override value should be applied
        Assert.False(result.ShowArrayIndices);
    }

    [Fact]
    public void TableConfig_MergeWith_CreatesNewInstance()
    {
        var baseConfig = new TableConfig { ShowMemberTypes = true };
        var overrideConfig = new TableConfig { ShowArrayIndices = false };

        var result = baseConfig.MergeWith(overrideConfig);

        Assert.NotSame(baseConfig, result);
        Assert.NotSame(overrideConfig, result);
    }

    [Fact]
    public void TableConfig_MergeWith_ExplicitlySettingDefaultValue_OverridesBaseValue()
    {
        // Base config has non-default values
        var baseConfig = new TableConfig
        {
            ShowArrayIndices = false,  // default is true
            ShowTableHeaders = false,  // default is true
        };

        // Override explicitly sets properties to their default values
        // With TrackableProperty, this should now override the base
        var overrideConfig = new TableConfig
        {
            ShowArrayIndices = true,   // explicitly set to default
            ShowTableHeaders = true,   // explicitly set to default
        };

        var result = baseConfig.MergeWith(overrideConfig);

        // The override's explicit values should be used, even though they equal defaults
        Assert.True(result.ShowArrayIndices);
        Assert.True(result.ShowTableHeaders);
    }

    [Fact]
    public void TrackableProperty_IsSet_FalseForConstructorDefault()
    {
        var config = new TableConfig();

        Assert.False(config.ShowArrayIndices.IsSet);
        Assert.False(config.ShowTableHeaders.IsSet);
        Assert.False(config.ShowMemberTypes.IsSet);
        Assert.False(config.BorderStyle.IsSet);
    }

    [Fact]
    public void TrackableProperty_IsSet_TrueAfterAssignment()
    {
        var config = new TableConfig
        {
            ShowArrayIndices = true,  // same as default, but explicitly set
        };

        Assert.True(config.ShowArrayIndices.IsSet);
        Assert.False(config.ShowTableHeaders.IsSet);  // not set
    }

    #endregion

    #region ColorConfig Tests

    [Fact]
    public void ColorConfig_MergeWith_Null_ReturnsSameInstance()
    {
        var baseConfig = new ColorConfig { TypeNameColor = new DumpColor(Color.Red) };

        var result = baseConfig.MergeWith(null);

        Assert.Same(baseConfig, result);
    }

    [Fact]
    public void ColorConfig_MergeWith_PreservesBaseValues_WhenOverrideHasDefaults()
    {
        var baseConfig = new ColorConfig
        {
            TypeNameColor = new DumpColor(Color.Red),
            PropertyValueColor = new DumpColor(Color.Blue)
        };

        // Override with default values
        var overrideConfig = new ColorConfig();

        var result = baseConfig.MergeWith(overrideConfig);

        // Compare the actual color values since DumpColor doesn't override Equals
        Assert.Equal(Color.Red, result.TypeNameColor?.Color);
        Assert.Equal(Color.Blue, result.PropertyValueColor?.Color);
    }

    [Fact]
    public void ColorConfig_MergeWith_OverridesOnlySpecifiedValues()
    {
        var baseConfig = new ColorConfig
        {
            TypeNameColor = new DumpColor(Color.Red),
            PropertyValueColor = new DumpColor(Color.Blue)
        };

        // Only override TypeNameColor with a non-default value
        var overrideConfig = new ColorConfig { TypeNameColor = new DumpColor(Color.Green) };

        var result = baseConfig.MergeWith(overrideConfig);

        // Compare the actual color values since DumpColor doesn't override Equals
        Assert.Equal(Color.Green, result.TypeNameColor?.Color);
        Assert.Equal(Color.Blue, result.PropertyValueColor?.Color);
    }

    [Fact]
    public void ColorConfig_MergeWith_NoColors_OverridesAll()
    {
        var baseConfig = new ColorConfig
        {
            TypeNameColor = new DumpColor(Color.Red),
            PropertyValueColor = new DumpColor(Color.Blue)
        };

        // Use NoColors - sets all to null
        var overrideConfig = ColorConfig.NoColors;

        var result = baseConfig.MergeWith(overrideConfig);

        // NoColors sets everything to null, which is different from defaults
        Assert.Null(result.TypeNameColor);
        Assert.Null(result.PropertyValueColor);
    }

    #endregion

    #region MembersConfig Tests

    [Fact]
    public void MembersConfig_MergeWith_Null_ReturnsSameInstance()
    {
        var baseConfig = new MembersConfig { IncludeFields = true };

        var result = baseConfig.MergeWith(null);

        Assert.Same(baseConfig, result);
    }

    [Fact]
    public void MembersConfig_MergeWith_PreservesBaseValues_WhenOverrideHasDefaults()
    {
        var baseConfig = new MembersConfig
        {
            IncludeFields = true,
            IncludeNonPublicMembers = true,
            IncludePublicMembers = false
        };

        var overrideConfig = new MembersConfig();

        var result = baseConfig.MergeWith(overrideConfig);

        Assert.True(result.IncludeFields);
        Assert.True(result.IncludeNonPublicMembers);
        Assert.False(result.IncludePublicMembers);
    }

    [Fact]
    public void MembersConfig_MergeWith_MemberFilter_UsesOverrideWhenProvided()
    {
        Func<MemberFilterContext, bool> baseFilter = _ => true;
        Func<MemberFilterContext, bool> overrideFilter = _ => false;

        var baseConfig = new MembersConfig { MemberFilter = baseFilter };
        var overrideConfig = new MembersConfig { MemberFilter = overrideFilter };

        var result = baseConfig.MergeWith(overrideConfig);

        Assert.Same(overrideFilter, result.MemberFilter);
    }

    [Fact]
    public void MembersConfig_MergeWith_MemberFilter_PreservesBaseWhenOverrideIsNull()
    {
        Func<MemberFilterContext, bool> baseFilter = _ => true;

        var baseConfig = new MembersConfig { MemberFilter = baseFilter };
        var overrideConfig = new MembersConfig { MemberFilter = null };

        var result = baseConfig.MergeWith(overrideConfig);

        Assert.Same(baseFilter, result.MemberFilter);
    }

    #endregion

    #region TypeNamingConfig Tests

    [Fact]
    public void TypeNamingConfig_MergeWith_Null_ReturnsSameInstance()
    {
        var baseConfig = new TypeNamingConfig { UseFullName = true };

        var result = baseConfig.MergeWith(null);

        Assert.Same(baseConfig, result);
    }

    [Fact]
    public void TypeNamingConfig_MergeWith_PreservesBaseValues_WhenOverrideHasDefaults()
    {
        var baseConfig = new TypeNamingConfig
        {
            UseFullName = true,
            UseAliases = false,
            ShowTypeNames = false
        };

        var overrideConfig = new TypeNamingConfig();

        var result = baseConfig.MergeWith(overrideConfig);

        Assert.True(result.UseFullName);
        Assert.False(result.UseAliases);
        Assert.False(result.ShowTypeNames);
    }

    [Fact]
    public void TypeNamingConfig_MergeWith_OverridesOnlySpecifiedValues()
    {
        var baseConfig = new TypeNamingConfig
        {
            UseFullName = true,
            UseAliases = false
        };

        // Only override UseFullName (set to false, which is the default, so won't override)
        // and ShowTypeNames (set to false, which is NOT the default)
        var overrideConfig = new TypeNamingConfig { ShowTypeNames = false };

        var result = baseConfig.MergeWith(overrideConfig);

        Assert.True(result.UseFullName);
        Assert.False(result.UseAliases);
        Assert.False(result.ShowTypeNames);
    }

    #endregion

    #region TypeRenderingConfig Tests

    [Fact]
    public void TypeRenderingConfig_MergeWith_Null_ReturnsSameInstance()
    {
        var baseConfig = new TypeRenderingConfig { QuoteStringValues = false };

        var result = baseConfig.MergeWith(null);

        Assert.Same(baseConfig, result);
    }

    [Fact]
    public void TypeRenderingConfig_MergeWith_PreservesBaseValues_WhenOverrideHasDefaults()
    {
        var baseConfig = new TypeRenderingConfig
        {
            QuoteStringValues = false,
            StringQuotationChar = '\'',
            QuoteCharValues = false,
            CharQuotationChar = '`'
        };

        var overrideConfig = new TypeRenderingConfig();

        var result = baseConfig.MergeWith(overrideConfig);

        Assert.False(result.QuoteStringValues);
        Assert.Equal('\'', (char)result.StringQuotationChar);
        Assert.False(result.QuoteCharValues);
        Assert.Equal('`', (char)result.CharQuotationChar);
    }

    #endregion

    #region OutputConfig Tests

    [Fact]
    public void OutputConfig_MergeWith_Null_ReturnsSameInstance()
    {
        var baseConfig = new OutputConfig { WidthOverride = 100 };

        var result = baseConfig.MergeWith(null);

        Assert.Same(baseConfig, result);
    }

    [Fact]
    public void OutputConfig_MergeWith_PreservesBaseValues_WhenOverrideHasDefaults()
    {
        var baseConfig = new OutputConfig
        {
            WidthOverride = 100,
            HeightOverride = 200
        };

        var overrideConfig = new OutputConfig();

        var result = baseConfig.MergeWith(overrideConfig);

        Assert.Equal(100, result.WidthOverride);
        Assert.Equal(200, result.HeightOverride);
    }

    [Fact]
    public void OutputConfig_MergeWith_OverridesOnlySpecifiedValues()
    {
        var baseConfig = new OutputConfig
        {
            WidthOverride = 100,
            HeightOverride = 200
        };

        var overrideConfig = new OutputConfig { WidthOverride = 500 };

        var result = baseConfig.MergeWith(overrideConfig);

        Assert.Equal(500, result.WidthOverride);
        Assert.Equal(200, result.HeightOverride);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void GlobalConfig_MergesWithPerDumpConfig_InDumpExtensions()
    {
        // Instead of modifying global config (which can cause test pollution),
        // we verify that per-dump configs merge properly by checking that 
        // setting one property in per-dump config doesn't reset other properties to defaults
        
        // When we provide a TruncationConfig with MaxCollectionCount set,
        // the default values for other properties should come from global config
        // (which uses default values). So we verify the truncation works.
        var data = new[] { 1, 2, 3, 4, 5 };
        var output = data.DumpText(truncationConfig: new TruncationConfig { MaxCollectionCount = 2 });

        // Verify truncation is happening (MaxCollectionCount = 2 is being respected)
        Assert.Contains("1", output);
        Assert.Contains("2", output);
        Assert.Contains("... 3 more", output);
        
        // Also verify that the default border style (Rounded) is preserved
        // Rounded border uses these characters: ╭ ╮ ╰ ╯
        Assert.Contains("╭", output);
        Assert.Contains("╯", output);
    }

    #endregion
}
