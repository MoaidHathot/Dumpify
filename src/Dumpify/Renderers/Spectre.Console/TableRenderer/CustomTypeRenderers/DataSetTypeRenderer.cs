using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Data;

namespace Dumpify;


internal class DataSetTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;
    public Type DescriptorType => typeof(CustomDescriptor);

    public DataSetTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
    {
        _handler = handler;
    }

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        var dataSet = (DataSet)obj;

        var tableBuilder = new ObjectTableBuilder(context, descriptor, obj);

        var title = (showTypeNames: (bool)context.Config.TypeNamingConfig.ShowTypeNames, dataSet.DataSetName) switch
        {
            (_, { Length: > 0 }) => dataSet.DataSetName,
            (true, _) => context.Config.TypeNameProvider.GetTypeName(descriptor.Type),
            (false, _) => "",
        };

        tableBuilder.AddColumnName(title, new Style(foreground: context.State.Colors.TypeNameColor));

        // Use centralized truncation for tables
        var maxCount = context.Config.TruncationConfig.MaxCollectionCount.Value;
        var mode = context.Config.TruncationConfig.Mode.Value;

        var truncated = CollectionTruncator.Truncate(
            Enumerable.Range(0, dataSet.Tables.Count),
            maxCount,
            mode);

        // Render start marker if present
        if (truncated.StartMarker != null)
        {
            var markerRenderable = RenderMarker(truncated.StartMarker, context);
            tableBuilder.AddRow(null, null, markerRenderable);
        }

        // Render tables
        for (int i = 0; i < truncated.Items.Count; i++)
        {
            // Render middle marker if present
            if (truncated.MiddleMarkerIndex == i && truncated.MiddleMarker != null)
            {
                var markerRenderable = RenderMarker(truncated.MiddleMarker, context);
                tableBuilder.AddRow(null, null, markerRenderable);
            }

            var tableIndex = truncated.Items[i];
            var dataTable = dataSet.Tables[tableIndex];
            var tableDescriptor = DumpConfig.Default.Generator.Generate(typeof(DataTable), null, context.Config.MemberProvider);
            var renderedItem = _handler.RenderDescriptor(dataTable, tableDescriptor, context);

            tableBuilder.AddRow(tableDescriptor, dataTable, renderedItem);
        }

        // Render end marker if present
        if (truncated.EndMarker != null)
        {
            var markerRenderable = RenderMarker(truncated.EndMarker, context);
            tableBuilder.AddRow(null, null, markerRenderable);
        }

        return tableBuilder.Build();
    }

    private IRenderable RenderMarker(TruncationMarker marker, RenderContext<SpectreRendererState> context)
    {
        var color = context.State.Colors.MetadataInfoColor;
        return new Markup(Markup.Escape(marker.GetDefaultMessage()), new Style(foreground: color));
    }

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type == typeof(DataSet), null);
}