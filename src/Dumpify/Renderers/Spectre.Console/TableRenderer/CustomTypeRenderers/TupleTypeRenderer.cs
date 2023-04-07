using Dumpify.Config;
using Dumpify.Descriptors;
using Dumpify.Extensions;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Runtime.CompilerServices;

namespace Dumpify.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;

internal class TupleTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable> _handler;

    public Type DescriptorType { get; } = typeof(ObjectDescriptor);

    public TupleTypeRenderer(IRendererHandler<IRenderable> handler)
        => _handler = handler;

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext context)
    {
        var table = new Table();

        var colorConfig = context.Config.ColorConfig;
        var columnColor = colorConfig.ColumnNameColor.ToSpectreColor();

        table.AddColumn(new TableColumn(new Markup(Markup.Escape("Item"), new Style(columnColor))));
        table.AddColumn(new TableColumn(new Markup(Markup.Escape("Value"), new Style(columnColor))));

        var tuple = (ITuple)obj;

        var genericArguments = descriptor.Type.GetGenericArguments();

        for(var index = 0; index < tuple.Length; ++index)
        {
            var value = tuple[index];
            var type = genericArguments[index];

            var typeDescriptor = DumpConfig.Default.Generator.Generate(type, null);

            var renderedValue = value switch
            {
                null => _handler.RenderNullValue(typeDescriptor, context),
                not null => _handler.RenderDescriptor(value, typeDescriptor, context),
            };

            var keyRenderable = new Markup($"Item{index + 1}", new Style(foreground: colorConfig.PropertyNameColor.ToSpectreColor()));

            table.AddRow(keyRenderable, renderedValue);
        }

        if(context.Config.ShowHeaders is false)
        {
            table.HideHeaders();
        }

        if(context.Config.ShowTypeNames is true)
        {
            table.Title = new TableTitle(Markup.Escape(descriptor.Type.GetGenericTypeName()), new Style(foreground: colorConfig.TypeNameColor.ToSpectreColor()));
        }

        return table;
    }

    public bool ShouldHandle(IDescriptor descriptor, object obj) 
        => obj is ITuple;
}
