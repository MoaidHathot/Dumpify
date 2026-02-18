using System.Globalization;
using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

namespace Dumpify.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        // Set invariant culture for consistent test results across platforms
        // This ensures DateTime, numbers, etc. render the same on Windows, Linux, macOS
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        // Store verified files in a Snapshots folder relative to each test file
        Verifier.UseProjectRelativeDirectory("Snapshots");

        // Use strict ordering for consistent snapshots
        VerifierSettings.SortPropertiesAlphabetically();
    }
}
