using Dumpify.Descriptors;
using Spectre.Console.Rendering;

namespace Dumpify;

public interface ITableBuilderBehavior
{
    IEnumerable<IRenderable> GetAdditionalColumns(RenderContext<SpectreRendererState> context);
    IEnumerable<IRenderable> GetAdditionalRowElements(BehaviorContext behaviorContext, RenderContext<SpectreRendererState> context);

    IEnumerable<IRenderable> GetAdditionalCells(object? obj, IDescriptor? currentDescriptor, RenderContext<SpectreRendererState> context);
}