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

        var dataSet = ((DataSet)obj);

        var tableBuilder = new ObjectTableBuilder(context, descriptor, obj);

        var title = (context.Config.TypeNamingConfig.ShowTypeNames, dataSet.DataSetName) switch
        {
            (_, { Length: > 0 }) => dataSet.DataSetName,
            (true, _) => context.Config.TypeNameProvider.GetTypeName(descriptor.Type),
            (false, _) => "",
        };

        tableBuilder.AddColumnName(title, new Style(foreground: context.State.Colors.TypeNameColor));

        int maxCollectionCount = context.Config.TableConfig.MaxCollectionCount;
        int length = dataSet.Tables.Count > maxCollectionCount ? maxCollectionCount : dataSet.Tables.Count;

        for(int i = 0; i < length; i++)
        {
            var dataTable = dataSet.Tables[i];
            var tableDescriptor = DumpConfig.Default.Generator.Generate(typeof(DataTable), null, context.Config.MemberProvider);
            var renderedItem = _handler.RenderDescriptor(dataTable, tableDescriptor, context);

            tableBuilder.AddRow(tableDescriptor, dataTable, renderedItem);
        }

        if(dataSet.Tables.Count > maxCollectionCount)
        {
            tableBuilder.AddRow(null, null, new Markup($"... truncated {dataSet.Tables.Count - maxCollectionCount} more tables"));
        }

        return tableBuilder.Build();
    }

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type == typeof(DataSet), null);
}