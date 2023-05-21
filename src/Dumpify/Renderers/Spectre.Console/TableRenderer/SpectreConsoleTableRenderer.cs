using Dumpify.Descriptors;
using Dumpify.Renderers.Spectre.Console.Builder;
using Dumpify.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections;
using System.Collections.Concurrent;

namespace Dumpify.Renderers.Spectre.Console.TableRenderer;

internal class SpectreConsoleTableRenderer : SpectreConsoleRendererBase
{
    public SpectreConsoleTableRenderer()
        : base(new ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<IRenderable>>>())
    {
        AddCustomTypeDescriptor(new DictionaryTypeRenderer(this));
        AddCustomTypeDescriptor(new ArrayTypeRenderer(this));
        AddCustomTypeDescriptor(new TupleTypeRenderer(this));
        AddCustomTypeDescriptor(new SystemTypeRenderer(this));
        AddCustomTypeDescriptor(new EnumTypeRenderer(this));
        AddCustomTypeDescriptor(new DataTableTypeRenderer(this));
        AddCustomTypeDescriptor(new DataSetTypeRenderer(this));
    }

    protected override IRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext<SpectreRendererState> context)
        => RenderIEnumerable((IEnumerable)obj, descriptor, context);

    private IRenderable RenderIEnumerable(IEnumerable obj, MultiValueDescriptor descriptor, RenderContext<SpectreRendererState> context)
    {
        var table = new Table();

        var typeName = context.Config.TypeNameProvider.GetTypeName(descriptor.Type);
        table.AddColumn(new TableColumn(new Markup(Markup.Escape(typeName), new Style(foreground: context.State.Colors.TypeNameColor))));

        foreach (var item in obj)
        {
            var type = descriptor.ElementsType ?? item?.GetType();

            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null, context.Config.MemberProvider) : null;

            var renderedItem = RenderDescriptor(item, itemsDescriptor, context);
            table.AddRow(renderedItem);
        }

        if (context.Config.TableConfig.ShowTableHeaders is false)
        {
            table.HideHeaders();
        }

        table.Collapse();
        return table;
    }

    protected override IRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext<SpectreRendererState> context)
    {
        var builder = new ObjectTableBuilder(context, descriptor, obj);

        foreach (var property in descriptor.Properties)
        {
            var renderedValue = GetValueAndRender(obj, property.ValueProvider!, property, context with { CurrentDepth = context.CurrentDepth + 1 });
            builder.AddRow(property, obj, renderedValue);
        }

        return builder.Build();
    }
}