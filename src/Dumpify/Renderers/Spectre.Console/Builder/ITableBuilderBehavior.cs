using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify.Renderers.Spectre.Console.Builder;

public interface ITableBuilderBehavior
{
    IEnumerable<IRenderable> GetAdditionalColumns(RenderContext<SpectreRendererState> context);
    IEnumerable<IRenderable> GetAdditionalRowElements(BehaviorContext behaviorContext, RenderContext<SpectreRendererState> context);
}