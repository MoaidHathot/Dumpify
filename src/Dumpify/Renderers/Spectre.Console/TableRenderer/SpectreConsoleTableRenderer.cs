using Dumpify.Descriptors;
using Dumpify.Extensions;
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

    protected override IRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext context)
        => RenderIEnumerable((IEnumerable)obj, descriptor, context);

    private IRenderable RenderIEnumerable(IEnumerable obj, MultiValueDescriptor descriptor, RenderContext context)
    {
        var table = new Table();

        var typeName = context.Config.TypeNameProvider.GetTypeName(descriptor.Type);
        table.AddColumn(new TableColumn(new Markup(Markup.Escape(typeName), new Style(foreground: context.Config.ColorConfig.TypeNameColor.ToSpectreColor()))));

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

    protected override IRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext context)
    {
        var table = new Table();

        var colorConfig = context.Config.ColorConfig;

        if (context.Config.TypeNamingConfig.ShowTypeNames)
        {
            var type = descriptor.Type == obj.GetType() ? descriptor.Type : obj.GetType();
            var typeName = context.Config.TypeNameProvider.GetTypeName(type);
            table.Title = new TableTitle(Markup.Escape(typeName), new Style(foreground: colorConfig.TypeNameColor.ToSpectreColor()));
        }

        var columnColor = colorConfig.ColumnNameColor.ToSpectreColor();
        table.AddColumn(new TableColumn(new Markup("Name", new Style(foreground: columnColor))));
        table.AddColumn(new TableColumn(new Markup("Value", new Style(foreground: columnColor))));

        if (context.Config.TableConfig.ShowTableHeaders is false)
        {
            table.HideHeaders();
        }

        foreach (var property in descriptor.Properties)
        {
            var renderedValue = GetValueAndRender(obj, property.ValueProvider!, property, context with { CurrentDepth = context.CurrentDepth + 1 });
            table.AddRow(new Markup(Markup.Escape(property.Name), new Style(foreground: colorConfig.PropertyNameColor.ToSpectreColor())), renderedValue);
        }

        table.Collapse();
        return table;
    }
}
