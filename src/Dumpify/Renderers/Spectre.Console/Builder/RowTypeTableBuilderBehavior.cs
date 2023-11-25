using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

public class RowTypeTableBuilderBehavior : ITableBuilderBehavior
{
    private readonly string _columnTypeName;

    public RowTypeTableBuilderBehavior(string columnTypeName = "Type")
    {
        _columnTypeName = Markup.Escape(columnTypeName);
    }

    public IEnumerable<IRenderable> GetAdditionalCells(object? obj, IDescriptor? currentDescriptor, RenderContext<SpectreRendererState> context)
    {
        var type = obj?.GetType() ?? currentDescriptor?.Type;
        var typeName = type is not null ? context.Config.TypeNameProvider.GetTypeName(type) : "";

        yield return new Markup(Markup.Escape(typeName), new Style(foreground: context.State.Colors.TypeNameColor));
    }

    public IEnumerable<IRenderable> GetAdditionalColumns(RenderContext<SpectreRendererState> context)
    {
        yield return new Markup(_columnTypeName, new Style(foreground: context.State.Colors.ColumnNameColor));
    }

    public IEnumerable<IRenderable> GetAdditionalRowElements(BehaviorContext behaviorContext, RenderContext<SpectreRendererState> context)
    {
        yield break;
    }
}