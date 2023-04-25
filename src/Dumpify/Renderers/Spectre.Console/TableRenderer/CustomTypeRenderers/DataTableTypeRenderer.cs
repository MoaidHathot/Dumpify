using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Data;

namespace Dumpify.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;

internal class DataTableTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable> _handler;
    public Type DescriptorType => typeof(CustomDescriptor);

    public DataTableTypeRenderer(IRendererHandler<IRenderable> handler)
    {
        _handler = handler;
    }

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext context, object? handleContext)
    {
        var dataTable = (DataTable)obj;

        var table = new Table();

        var title = (context.Config.TypeNamingConfig.ShowTypeNames, dataTable.TableName) switch
        {
            (_, { Length: > 0 }) => dataTable.TableName,
            (true, _) => context.Config.TypeNameProvider.GetTypeName(descriptor.Type),
            (false, _) => null,
        };

        if(title is not null)
        {
            table.Title = new TableTitle(Markup.Escape(title), new Style(foreground: context.Config.ColorConfig.TypeNameColor.ToSpectreColor()));
        }

        foreach(DataColumn column in dataTable.Columns)
        {
            table.AddColumn(new TableColumn(new Markup(Markup.Escape(column.ColumnName), new Style(foreground: context.Config.ColorConfig.ColumnNameColor.ToSpectreColor()))));
        }

        foreach (DataRow row in dataTable.Rows)
        {
            var renderables = row.ItemArray.Select(item => RenderTableCell(item, context)).ToArray();
            table.AddRow(renderables);
        }

        return table.Collapse();
    }

    private IRenderable RenderTableCell(object? obj, RenderContext context)
    {
        if(obj is null)
        {
            return _handler.RenderNullValue(null, context);
        }

        var descriptor = DumpConfig.Default.Generator.Generate(obj.GetType(), null, context.Config.MemberProvider);
        var rendered = _handler.RenderDescriptor(obj, descriptor, context);

        return rendered;
    }

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type == typeof(DataTable), null);
}
