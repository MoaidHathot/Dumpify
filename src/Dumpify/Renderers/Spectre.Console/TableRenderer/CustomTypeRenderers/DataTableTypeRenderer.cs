using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Data;
using System.Data.Common;

namespace Dumpify;

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

        int maxCollectionCount = context.Config.TableConfig.MaxCollectionCount;
        int rows = dataTable.Rows.Count > maxCollectionCount ? maxCollectionCount : dataTable.Rows.Count;
        int columns = dataTable.Columns.Count > maxCollectionCount ? maxCollectionCount : dataTable.Columns.Count;

        var dataTableContents = new object[rows, columns];

        for(int column = 0; column < columns; column++)
        {
            for(int row = 0; row < rows; row++)
            {
                dataTableContents[row, column] = dataTable.Rows[row][column];
            }
        }

        for (int column = 0; column < columns; column++)
        {
            builder.AddColumnName(dataTable.Columns[column].ColumnName);
        }

        if (dataTable.Columns.Count > maxCollectionCount)
        {
            builder.AddColumnName($"... +{dataTable.Columns.Count - maxCollectionCount}");
        }

        for (int row = 0; row < rows; row++)
        {
            List<object?> rowContents = new();
            List<IRenderable> renderables = new();
            for (int column = 0; column < columns; column++)
            {
                rowContents.Add(dataTableContents[row, column]);
                renderables.Add(RenderTableCell(dataTableContents[row, column], context));
            }

            if (dataTable.Columns.Count > maxCollectionCount)
            {
                rowContents.Add(string.Empty);
                renderables.Add(RenderTableCell(string.Empty, context, true));
            }

            builder.AddRow(null, rowContents, renderables);
        }

        if (dataTable.Rows.Count > maxCollectionCount)
        {
            var rowCells = new List<IRenderable>();
            for (int i = 0; i <= columns; i++)
            {
                var truncatedNotificationText = $"... +{dataTable.Rows.Count - maxCollectionCount}";
                if (i > 0)
                {
                    truncatedNotificationText = string.Empty;
                }

                var renderedCell = RenderTableCell(truncatedNotificationText, context, true);
                rowCells.Add(renderedCell);
            }

            builder.AddRow(null, null, rowCells);
        }

        return builder.Build();
    }

    private IRenderable RenderTableCell(object? obj, RenderContext<SpectreRendererState> context, bool asLabel = false)
    {
        if (obj is null)
        {
            return _handler.RenderNullValue(null, context);
        }

        var descriptor = asLabel
            ? new LabelDescriptor(obj.GetType(), null)
            : DumpConfig.Default.Generator.Generate(obj.GetType(), null, context.Config.MemberProvider);
        var rendered = _handler.RenderDescriptor(obj, descriptor, context);

        return rendered;
    }

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type == typeof(DataTable), null);
}