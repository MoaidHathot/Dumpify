namespace Dumpify;

/// <summary>
/// Abstract base class for configuration classes that support merging with override configurations.
/// </summary>
/// <typeparam name="T">The configuration type (must be the derived class itself).</typeparam>
public abstract class ConfigBase<T> : IConfigMergeable<T> where T : ConfigBase<T>, new()
{
    /// <summary>
    /// Creates a new configuration instance by merging this (base) configuration with an override configuration.
    /// Properties in the override configuration that have been explicitly set will take precedence.
    /// </summary>
    /// <param name="overrideConfig">The override configuration. If null, returns this configuration.</param>
    /// <returns>A new merged configuration instance, or this instance if overrideConfig is null.</returns>
    public T MergeWith(T? overrideConfig)
    {
        if (overrideConfig is null)
        {
            return (T)this;
        }

        return MergeOverride(overrideConfig);
    }

    /// <summary>
    /// Creates a new configuration instance by merging this (base) configuration with a non-null override configuration.
    /// Implementations should create a new instance and merge each property.
    /// </summary>
    /// <param name="overrideConfig">The override configuration (guaranteed to be non-null).</param>
    /// <returns>A new merged configuration instance.</returns>
    protected abstract T MergeOverride(T overrideConfig);

    /// <summary>
    /// Merges a base TrackableProperty value with an override TrackableProperty value.
    /// If the override value has been explicitly set, use the override; otherwise use the base.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    /// <param name="baseValue">The base configuration value.</param>
    /// <param name="overrideValue">The override configuration value.</param>
    /// <returns>The merged value.</returns>
    protected static TValue Merge<TValue>(TrackableProperty<TValue> baseValue, TrackableProperty<TValue> overrideValue)
    {
        return overrideValue.IsSet ? overrideValue.Value : baseValue.Value;
    }

    /// <summary>
    /// Merges a base value with an override value using default-value comparison.
    /// If the override value differs from the default value, use the override; otherwise use the base.
    /// Use this for nullable or reference types that don't use TrackableProperty.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="baseValue">The base configuration value.</param>
    /// <param name="overrideValue">The override configuration value.</param>
    /// <param name="defaultValue">The default value for comparison.</param>
    /// <returns>The merged value.</returns>
    protected static TValue Merge<TValue>(TValue baseValue, TValue overrideValue, TValue defaultValue)
    {
        return Equals(overrideValue, defaultValue) ? baseValue : overrideValue;
    }
}
