namespace Dumpify;

/// <summary>
/// Helper class for merging configuration values.
/// </summary>
internal static class ConfigMergeHelper
{
    /// <summary>
    /// Merges a base value with an override value.
    /// If the override value differs from the default value, use the override; otherwise use the base.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="baseValue">The base configuration value.</param>
    /// <param name="overrideValue">The override configuration value.</param>
    /// <param name="defaultValue">The default value for comparison.</param>
    /// <returns>The merged value.</returns>
    public static TValue Merge<TValue>(TValue baseValue, TValue overrideValue, TValue defaultValue)
    {
        return Equals(overrideValue, defaultValue) ? baseValue : overrideValue;
    }
}
