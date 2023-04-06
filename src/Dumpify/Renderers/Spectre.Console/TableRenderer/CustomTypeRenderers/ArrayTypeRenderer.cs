using Dumpify.Config;
using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;

internal class ArrayTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable> _handler;

    public ArrayTypeRenderer(IRendererHandler<IRenderable> handler)
    {
        _handler = handler;
    }

    public Type DescriptorType { get; } = typeof(MultiValueDescriptor);

    public bool ShouldHandle(IDescriptor descriptor, object obj)
        => descriptor.Type.IsArray && ((Array)obj).Rank < 3;

    public IRenderable Render(IDescriptor descriptor, object @object, RenderContext context)
    {
        var mvd = (MultiValueDescriptor)descriptor;
        var obj = (Array)@object;

        return obj.Rank switch
        {
            1 => RenderSingleDimensionArray(obj, mvd, context),
            2 => RenderTwoDimensionalArray(obj, mvd, context),
            > 2 => RenderHighRankArrays(obj, mvd, context),
            _ => throw new NotImplementedException($"Rendering Array of rank {obj.Rank}")
        };
    }

    private IRenderable RenderSingleDimensionArray(Array obj, MultiValueDescriptor mvd, RenderContext context)
    {
        var table = new Table();

        var showIndexes = context.Config.TableConfig.ShowArrayIndices;

        if (showIndexes)
        {
            table.AddColumn(new TableColumn(new Markup("")));
        }

        table.AddColumn(new TableColumn(new Markup(Markup.Escape($"{mvd.ElementsType?.Name ?? ""}[{obj.GetLength(0)}]"), new Style(foreground: context.Config.ColorConfig.TypeNameColor.ToSpectreColor()))));

        if(context.Config.ShowHeaders is not true || context.Config.ShowTypeNames is not true)
        {
            table.HideHeaders();
        }

        for(var index = 0; index < obj.Length; ++index)
        {
            var item = obj.GetValue(index);

            var type = mvd.ElementsType ?? item?.GetType();
            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

            var renderedItem = _handler.RenderDescriptor(item, itemsDescriptor, context);

            if (showIndexes)
            {
                var renderedIndex = new Markup(index.ToString(), new Style(foreground: context.Config.ColorConfig.ColumnNameColor.ToSpectreColor()));
                table.AddRow(renderedIndex, renderedItem);
            }
            else
            {
                table.AddRow(renderedItem);
            }
        }

        table.Collapse();
        return table;
    }

    private IRenderable RenderTwoDimensionalArray(Array obj, MultiValueDescriptor descriptor, RenderContext context)
    {
        if (obj.Rank != 2)
        {
            return RenderHighRankArrays(obj, descriptor, context);
        }

        var table = new Table();

        var rows = obj.GetLength(0);
        var collumns = obj.GetLength(1);

        var colorConfig = context.Config.ColorConfig;

        if(context.Config.ShowTypeNames is true)
        {
            table.Title = new TableTitle(Markup.Escape($"{descriptor.ElementsType?.Name ?? ""}[{rows},{collumns}]"), new Style(foreground: colorConfig.TypeNameColor.ToSpectreColor()));
        }

        table.AddColumn(new TableColumn(""));

        for (var col = 0; col < collumns; ++col)
        {
            table.AddColumn(new TableColumn(new Markup(col.ToString(), new Style(foreground: colorConfig.ColumnNameColor.ToSpectreColor()))));
        }

        for (var row = 0; row < rows; ++row)
        {
            var cells = new List<IRenderable>() { new Markup(row.ToString(), new Style(foreground: colorConfig.ColumnNameColor.ToSpectreColor())) };

            for (var col = 0; col < collumns; ++col)
            {
                var item = obj.GetValue(row, col);

                var type = descriptor.ElementsType ?? item?.GetType();
                IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

                var renderedItem = _handler.RenderDescriptor(item, itemsDescriptor, context);
                cells.Add(renderedItem);
            }

            table.AddRow(cells);
        }

        return table.Collapse();
    }

    private IRenderable RenderHighRankArrays(Array arr, MultiValueDescriptor descriptor, RenderContext context) 
        => _handler.RenderDescriptor(arr, descriptor, context);
}
