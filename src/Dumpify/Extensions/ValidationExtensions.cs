using System.Diagnostics.CodeAnalysis;

namespace Dumpify.Extensions;
internal static class ValidationExtensions
{
    public static int? MustBeGreaterThan(this int? value, int greater)
    {
        if(value is null)
        {
            return null;
        }

        return value.Value.MustBeGreaterThan(greater);
    }

    public static int MustBeGreaterThan(this int value, int greater)
    {
        if(value <= greater)
        {
            throw new ArgumentException($"{value} must be greater than {greater}");
        }

        return value;
    }

    [return: NotNull]
    public static T MustNotBeNull<T>(this T? value, string reason = "")
        => value ?? throw new ArgumentNullException(nameof(value), $"Value should not be null. {(string.IsNullOrWhiteSpace(reason) ? "" : $"Reason: {reason}")}");

    public static string MustNotBeNullOrWhiteSpace(this string? value)
        => !string.IsNullOrWhiteSpace(value) ? value! : throw new ArgumentException($"String '{value}' should not be null, empty or whitespace");

    public static string MustNotBeNullOrEmpty(this string? value)
        => !string.IsNullOrWhiteSpace(value) ? value! : throw new ArgumentException($"String '{value}' should not be null or empty");
}
