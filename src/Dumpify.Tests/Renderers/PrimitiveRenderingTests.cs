using VerifyXunit;
using static VerifyXunit.Verifier;

namespace Dumpify.Tests.Renderers;

/// <summary>
/// Snapshot tests for rendering primitive types.
/// These tests ensure that primitive values are rendered consistently.
/// </summary>
public class PrimitiveRenderingTests
{
    #region Numeric Types

    [Fact]
    public Task Integer_Renders()
        => Verify(42.DumpText());

    [Fact]
    public Task NegativeInteger_Renders()
        => Verify((-42).DumpText());

    [Fact]
    public Task Long_Renders()
        => Verify(9223372036854775807L.DumpText());

    [Fact]
    public Task Double_Renders()
        => Verify(3.14159265359.DumpText());

    [Fact]
    public Task Decimal_Renders()
        => Verify(123.456789m.DumpText());

    [Fact]
    public Task Float_Renders()
        => Verify(3.14f.DumpText());

    #endregion

    #region String and Char Types

    [Fact]
    public Task String_RendersWithQuotes()
        => Verify("hello world".DumpText());

    [Fact]
    public Task EmptyString_Renders()
        => Verify(string.Empty.DumpText());

    [Fact]
    public Task StringWithSpecialChars_Renders()
        => Verify("hello\nworld\ttab".DumpText());

    [Fact]
    public Task Char_RendersWithQuotes()
        => Verify('A'.DumpText());

    #endregion

    #region Boolean Type

    [Fact]
    public Task BooleanTrue_Renders()
        => Verify(true.DumpText());

    [Fact]
    public Task BooleanFalse_Renders()
        => Verify(false.DumpText());

    #endregion

    #region Enum Types

    [Fact]
    public Task Enum_RendersWithTypeName()
        => Verify(DayOfWeek.Monday.DumpText());

    [Fact]
    public Task FlagsEnum_Renders()
        => Verify((FileAttributes.ReadOnly | FileAttributes.Hidden).DumpText());

    #endregion

    #region Date and Time Types

    [Fact]
    public Task DateTime_Renders()
        => Verify(new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc).DumpText());

    [Fact]
    public Task DateOnly_Renders()
        => Verify(new DateOnly(2024, 1, 15).DumpText());

    [Fact]
    public Task TimeOnly_Renders()
        => Verify(new TimeOnly(10, 30, 45).DumpText());

    [Fact]
    public Task TimeSpan_Renders()
        => Verify(TimeSpan.FromHours(2.5).DumpText());

    [Fact]
    public Task DateTimeOffset_Renders()
        => Verify(new DateTimeOffset(2024, 1, 15, 10, 30, 45, TimeSpan.FromHours(2)).DumpText());

    #endregion

    #region Other Primitive Types

    [Fact]
    public Task Guid_Renders()
        => Verify(new Guid("12345678-1234-1234-1234-123456789abc").DumpText());

    [Fact]
    public Task Byte_Renders()
        => Verify(((byte)255).DumpText());

    [Fact]
    public Task Short_Renders()
        => Verify(((short)32767).DumpText());

    #endregion

    #region Nullable Types

    [Fact]
    public Task NullableInt_WithValue_Renders()
        => Verify(((int?)42).DumpText());

    [Fact]
    public Task NullableInt_WithNull_Renders()
        => Verify(((int?)null).DumpText());

    [Fact]
    public Task NullableBool_WithValue_Renders()
        => Verify(((bool?)true).DumpText());

    [Fact]
    public Task NullableGuid_WithNull_Renders()
        => Verify(((Guid?)null).DumpText());

    #endregion

    #region Null Values

    [Fact]
    public Task NullString_Renders()
        => Verify(((string?)null).DumpText());

    [Fact]
    public Task NullObject_Renders()
        => Verify(((object?)null).DumpText());

    #endregion
}
