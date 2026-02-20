using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Data;

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

        var title = (showTypeNames: (bool)context.Config.TypeNamingConfig.ShowTypeNames, dataTable.TableName) switch
        {
            (_, { Length: > 0 }) => dataTable.TableName,
            (true, _) => context.Config.TypeNameProvider.GetTypeName(descriptor.Type),
            (false, _) => null,
        };

        builder.SetTitle(title);

        // Use centralized truncation for rows and columns
        var maxCount = context.Config.TruncationConfig.MaxCollectionCount.Value;
        var mode = context.Config.TruncationConfig.Mode.Value;

        var rowTruncated = CollectionTruncator.Truncate(
            Enumerable.Range(0, dataTable.Rows.Count),
            maxCount,
            mode);

        var colTruncated = CollectionTruncator.Truncate(
            Enumerable.Range(0, dataTable.Columns.Count),
            maxCount,
            mode);

        // Add column headers
        // Start column marker
        if (colTruncated.StartMarker != null)
        {
            builder.AddColumnName(colTruncated.StartMarker.GetCompactMessage());
        }

        for (int i = 0; i < colTruncated.Items.Count; i++)
        {
            // Middle marker for columns
            if (colTruncated.MiddleMarkerIndex == i && colTruncated.MiddleMarker != null)
            {
                builder.AddColumnName(colTruncated.MiddleMarker.GetCompactMessage());
            }

            builder.AddColumnName(dataTable.Columns[colTruncated.Items[i]].ColumnName);
        }

        // End column marker
        if (colTruncated.EndMarker != null)
        {
            builder.AddColumnName(colTruncated.EndMarker.GetCompactMessage());
        }

        // Render start row marker
        if (rowTruncated.StartMarker != null)
        {
            var rowCells = CreateMarkerRow(rowTruncated.StartMarker, colTruncated, context);
            builder.AddRow(null, null, rowCells);
        }

        // Render rows
        for (int ri = 0; ri < rowTruncated.Items.Count; ri++)
        {
            // Middle marker for rows
            if (rowTruncated.MiddleMarkerIndex == ri && rowTruncated.MiddleMarker != null)
            {
                var markerCells = CreateMarkerRow(rowTruncated.MiddleMarker, colTruncated, context);
                builder.AddRow(null, null, markerCells);
            }

            var row = rowTruncated.Items[ri];
            var rowCells = new List<IRenderable>();
            var rowContents = new List<object?>();

            // Start column marker cell
            if (colTruncated.StartMarker != null)
            {
                rowCells.Add(new Markup(""));
                rowContents.Add(null);
            }

            for (int ci = 0; ci < colTruncated.Items.Count; ci++)
            {
                // Middle marker for columns
                if (colTruncated.MiddleMarkerIndex == ci && colTruncated.MiddleMarker != null)
                {
                    rowCells.Add(new Markup(""));
                    rowContents.Add(null);
                }

                var col = colTruncated.Items[ci];
                var cellValue = dataTable.Rows[row][col];
                rowContents.Add(cellValue);
                rowCells.Add(RenderTableCell(cellValue, context));
            }

            // End column marker cell
            if (colTruncated.EndMarker != null)
            {
                rowCells.Add(new Markup(""));
                rowContents.Add(null);
            }

            builder.AddRow(null, rowContents, rowCells);
        }

        // Render end row marker
        if (rowTruncated.EndMarker != null)
        {
            var rowCells = CreateMarkerRow(rowTruncated.EndMarker, colTruncated, context);
            builder.AddRow(null, null, rowCells);
        }

        return builder.Build();
    }

    private List<IRenderable> CreateMarkerRow(TruncationMarker marker, TruncatedEnumerable<int> colTruncated, RenderContext<SpectreRendererState> context)
    {
        var cells = new List<IRenderable>();
        var color = context.State.Colors.MetadataInfoColor;
        var markerText = new Markup(Markup.Escape(marker.GetCompactMessage()), new Style(foreground: color));

        // First cell gets the marker, rest are empty
        bool firstCell = true;

        if (colTruncated.StartMarker != null)
        {
            cells.Add(firstCell ? markerText : new Markup(""));
            firstCell = false;
        }

        for (int i = 0; i < colTruncated.Items.Count; i++)
        {
            if (colTruncated.MiddleMarkerIndex == i && colTruncated.MiddleMarker != null)
            {
                cells.Add(new Markup(""));
            }

            cells.Add(firstCell ? markerText : new Markup(""));
            firstCell = false;
        }

        if (colTruncated.EndMarker != null)
        {
            cells.Add(new Markup(""));
        }

        return cells;
    }

    private IRenderable RenderTableCell(object? obj, RenderContext<SpectreRendererState> context)
    {
        if (obj is null)
        {
            return _handler.RenderNullValue(null, context);
        }

        var descriptor = DumpConfig.Default.Generator.Generate(obj.GetType(), null, context.Config.MemberProvider);
        return _handler.RenderDescriptor(obj, descriptor, context);
    }

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type == typeof(DataTable), null);
}
