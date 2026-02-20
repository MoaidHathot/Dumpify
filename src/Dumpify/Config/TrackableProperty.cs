namespace Dumpify;

public readonly struct TrackableProperty<T>
{
    private readonly T _value;

    public T Value => _value;
    public bool IsSet { get; }

    /// <summary>
    /// Constructor for factory defaults (IsSet = false).
    /// </summary>
    public TrackableProperty(T defaultValue)
    {
        _value = defaultValue;
        IsSet = false;
    }

    /// <summary>
    /// Private constructor for creating "set" instances.
    /// </summary>
    private TrackableProperty(T value, bool isSet)
    {
        _value = value;
        IsSet = isSet;
    }

    /// <summary>
    /// User assignment: config.Prop = value -> IsSet = true
    /// </summary>
    public static implicit operator TrackableProperty<T>(T value)
        => new(value, isSet: true);

    /// <summary>
    /// Reading: T x = config.Prop -> returns Value
    /// </summary>
    public static implicit operator T(TrackableProperty<T> trackable) => trackable._value;

    public override string ToString() => _value?.ToString() ?? string.Empty;
}
