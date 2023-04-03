using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify.Renderers.Spectre.Console.CustomTypeRenderers;

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
            1 => RenderSingleDimentionArray(obj, mvd, context),
            2 => RenderTwoDimentionalArray(obj, mvd, context),
            > 2 => RenderHighRankArrays(obj, mvd, context),
            _ => throw new NotImplementedException($"Rendering Array of rank {obj.Rank}")
        };
    }

    private IRenderable RenderSingleDimentionArray(Array obj, MultiValueDescriptor mvd, RenderContext context)
    {
        var table = new Table();
        table.AddColumn(Markup.Escape($"{mvd.ElementsType?.Name ?? ""}[{obj.GetLength(0)}]"));

        if(context.Config.ShowHeaders is not true)
        {
            table.HideHeaders();
        }

        for(var index = 0; index < obj.Length; ++index)
        {
            var item = obj.GetValue(index);

            var type = mvd.ElementsType ?? item?.GetType();
            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

            var renderedItem = _handler.RenderDescriptor(item, itemsDescriptor, context);
            table.AddRow(renderedItem);
        }

        table.Collapse();
        return table;
    }

    private IRenderable RenderTwoDimentionalArray(Array obj, MultiValueDescriptor descriptor, RenderContext context)
    {
        if (obj.Rank != 2)
        {
            return RenderHighRankArrays(obj, descriptor, context);
        }

        var table = new Table();

        var rows = obj.GetLength(0);
        var collumns = obj.GetLength(1);

        if(context.Config.ShowTypeNames is true)
        {
            table.Title = new TableTitle(Markup.Escape($"{descriptor.ElementsType?.Name ?? ""}[{rows},{collumns}]"));
        }

        table.AddColumn(new TableColumn(""));

        for (var col = 0; col < collumns; ++col)
        {
            table.AddColumn(new TableColumn(new Markup(col.ToString(), new Style(foreground: Color.DarkSeaGreen3))));
        }

        for (var row = 0; row < rows; ++row)
        {
            var cells = new List<IRenderable>() { new Markup(row.ToString(), new Style(foreground: Color.DarkSlateGray3)) };

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
