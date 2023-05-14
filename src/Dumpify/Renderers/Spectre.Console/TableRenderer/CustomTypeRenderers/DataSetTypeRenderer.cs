using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Data;

namespace Dumpify.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;

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

        var table = new Table();

        var title = (context.Config.TypeNamingConfig.ShowTypeNames, dataSet.DataSetName) switch
        {
            (_, { Length: > 0 }) => dataSet.DataSetName,
            (true, _) => context.Config.TypeNameProvider.GetTypeName(descriptor.Type),
            (false, _) => "",
        };

        table.AddColumn(new TableColumn(new Markup(Markup.Escape(title), new Style(foreground: context.Config.ColorConfig.TypeNameColor.ToSpectreColor()))));

        foreach (DataTable dataTable in dataSet.Tables)
        {
            var tableDescriptor = DumpConfig.Default.Generator.Generate(typeof(DataTable), null, context.Config.MemberProvider);
            var renderedItem = _handler.RenderDescriptor(dataTable, tableDescriptor, context);

            table.AddRow(renderedItem);
        }

        return table.Collapse();
    }

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type == typeof(DataSet), null);
}