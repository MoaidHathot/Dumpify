using Dumpify.Descriptors;
using Dumpify.Renderers.Spectre.Console.Builder;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Data;

namespace Dumpify.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;

internal class DataTableTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;
    public Type DescriptorType => typeof(CustomDescriptor);

    public DataTableTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
    {
        _handler = handler;
    }

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        var dataTable = (DataTable)obj;

        var builder = new ObjectTableBuilder(context, descriptor, obj);

        var title = (context.Config.TypeNamingConfig.ShowTypeNames, dataTable.TableName) switch
        {
            (_, { Length: > 0 }) => dataTable.TableName,
            (true, _) => context.Config.TypeNameProvider.GetTypeName(descriptor.Type),
            (false, _) => null,
        };

        builder.SetTitle(title);

        foreach (DataColumn column in dataTable.Columns)
        {
            builder.AddColumnName(column.ColumnName);
        }

        foreach (DataRow row in dataTable.Rows)
        {
            var renderables = row.ItemArray.Select(item => RenderTableCell(item, context)).ToArray();
            builder.AddRow(null, row, renderables);
        }

        return builder.Build();
    }

    private IRenderable RenderTableCell(object? obj, RenderContext<SpectreRendererState> context)
    {
        if (obj is null)
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