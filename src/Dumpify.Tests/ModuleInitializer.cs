using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

namespace Dumpify.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        // Store verified files in a Snapshots folder relative to each test file
        Verifier.UseProjectRelativeDirectory("Snapshots");

        // Use strict ordering for consistent snapshots
        VerifierSettings.SortPropertiesAlphabetically();
    }
}
