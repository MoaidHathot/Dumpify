using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

public class RowIndicesTableBuilderBehavior : ITableBuilderBehavior
{
    private readonly string _indexColumnName = Markup.Escape("#");

    public IEnumerable<IRenderable> GetAdditionalCells(object? obj, IDescriptor? currentDescriptor, RenderContext<SpectreRendererState> context)
    {
        yield break;
    }

    public IEnumerable<IRenderable> GetAdditionalColumns(RenderContext<SpectreRendererState> context)
    {
        yield return new Markup(_indexColumnName, new Style(foreground: context.State.Colors.ColumnNameColor));
    }

    public IEnumerable<IRenderable> GetAdditionalRowElements(BehaviorContext behaviorContext, RenderContext<SpectreRendererState> context)
    {
        var index = behaviorContext.AddedRows;

        yield return new Markup(Markup.Escape(index.ToString()), new Style(foreground: context.State.Colors.ColumnNameColor));
    }
}