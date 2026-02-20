namespace Dumpify;

/// <summary>
/// Interface for configuration classes that support merging with override configurations.
/// </summary>
/// <typeparam name="T">The configuration type.</typeparam>
public interface IConfigMergeable<T> where T : class
{
    /// <summary>
    /// Creates a new configuration instance by merging this (base) configuration with an override configuration.
    /// Properties in the override configuration that differ from their default values will take precedence.
    /// </summary>
    /// <param name="overrideConfig">The override configuration. If null, returns this configuration.</param>
    /// <returns>A new merged configuration instance.</returns>
    T MergeWith(T? overrideConfig);
}
