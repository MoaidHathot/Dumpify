namespace Dumpify;

/// <summary>
/// Factory to create IReferenceRenderingStrategy from configuration.
/// Determines what needs to be tracked based on the configured display options.
/// </summary>
public static class ReferenceRenderingStrategyFactory
{
    /// <summary>
    /// Creates a strategy based on the provided configuration.
    /// </summary>
    /// <param name="config">The reference rendering configuration</param>
    /// <returns>A configured strategy instance</returns>
    public static IReferenceRenderingStrategy Create(ReferenceRenderingConfig config)
    {
        var circular = config.CircularReferenceDisplay;
        var shared = config.SharedReferenceDisplay;

        // Determine what needs to be tracked based on what will be displayed
        bool needsIds = circular.HasFlag(CircularReferenceDisplay.Id) || shared == SharedReferenceDisplay.ShowReference;

        bool needsPaths = circular.HasFlag(CircularReferenceDisplay.Path) || shared == SharedReferenceDisplay.ShowReference;

        bool tracksShared = shared == SharedReferenceDisplay.ShowReference;

        return new ReferenceRenderingStrategy( circular, shared, needsIds, needsPaths, tracksShared);
    }
}