using Dumpify.Descriptors;
using Dumpify.Extensions;
using Dumpify.Renderers.Spectre.Console.CustomTypeRenderers;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Dumpify.Renderers.Spectre.Console;

internal class SpectreConsoleTableRenderer : RendererBase<IRenderable>
{
    public SpectreConsoleTableRenderer()
        : base(new ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<IRenderable>>>())
    {
        AddCustomTypeDescriptor(new DictionaryTypeRenderer(this));
        AddCustomTypeDescriptor(new ArrayTypeRenderer(this));
        AddCustomTypeDescriptor(new TupleTypeRenderer(this));
    }

    private void AddCustomTypeDescriptor(ICustomTypeRenderer<IRenderable> handler)
    {
        _customTypeRenderers.AddOrUpdate(handler.DescriptorType.TypeHandle, new List<ICustomTypeRenderer<IRenderable>>() { handler }, (k, list) =>
        {
            list.Add(handler);
            return list;
        });
    }

    protected override void PublishRenderables(IRenderable renderable)
    {
        AnsiConsole.Write(renderable);
        System.Console.WriteLine();
    }

    protected override IRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext context) 
        => RenderIEnumerable((IEnumerable)obj, descriptor, context);

    private IRenderable RenderIEnumerable(IEnumerable obj, MultiValueDescriptor descriptor, RenderContext context)
    {
        var table = new Table();

        var typeName = descriptor.Type.GetGenericTypeName();
        table.AddColumn(new TableColumn(new Markup(Markup.Escape(typeName), new Style(foreground: context.Config.ColorConfig.TypeNameColor.ToSpectreColor()))));

        foreach (var item in obj)
        {        
            var type = descriptor.ElementsType ?? item?.GetType();

            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

            var renderedItem = RenderDescriptor(item, itemsDescriptor, context);
            table.AddRow(renderedItem);
        }

        if (context.Config.ShowHeaders is not true)
        {
            table.HideHeaders();
        }

        table.Collapse();
        return table;
    }


    protected override IRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext context)
    {
        if (ObjectAlreadyRendered(obj, context.ObjectTracker))
        {
            return RenderCircularDependency(obj, descriptor, context.Config);
        }

        var table = new Table();

        var colorConfig = context.Config.ColorConfig;

        if (context.Config.ShowTypeNames is true)
        {
            table.Title = new TableTitle(Markup.Escape(descriptor.Type.GetGenericTypeName()), new Style(foreground: colorConfig.TypeNameColor.ToSpectreColor()));
        }

        var columnColor = colorConfig.ColumnNameColor.ToSpectreColor();
        table.AddColumn(new TableColumn(new Markup("Name", new Style(foreground: columnColor))));
        table.AddColumn(new TableColumn(new Markup("Value", new Style(foreground: columnColor))));

        if(context.Config.ShowHeaders is not true)
        {
            table.HideHeaders();
        }

        foreach (var property in descriptor.Properties)
        {
            var renderedValue = RenderDescriptor(property.PropertyInfo!.GetValue(obj), property, context with { CurrentDepth = context.CurrentDepth + 1});
            table.AddRow(new Markup(Markup.Escape(property.Name), new Style(foreground: colorConfig.PropertyNameColor.ToSpectreColor())), renderedValue);
        }

        table.Collapse();
        return table;
    }

    protected override IRenderable RenderCircularDependency(object @object, IDescriptor? descriptor, in RendererConfig config)
        => new Markup(Markup.Escape("[Circular Reference]"), new Style(foreground: config.ColorConfig.MetadataInfoColor.ToSpectreColor()));

    public override IRenderable RenderExeededDepth(object obj, IDescriptor? descriptor, in RendererConfig config)
        => new Markup(Markup.Escape($"[Exceeded max depth {config.MaxDepth}]"), new Style(foreground: config.ColorConfig.MetadataInfoColor.ToSpectreColor()));

    protected override IRenderable RenderIgnoredDescriptor(object obj, IgnoredDescriptor descriptor, RenderContext context)
        => new Markup(Markup.Escape($"[Ignored {descriptor.Name}]"), new Style(foreground: context.Config.ColorConfig.IgnoredValueColor.ToSpectreColor()));

    protected override IRenderable RenderNullDescriptor(object obj, RenderContext context) 
        => new Markup(Markup.Escape($"[null descriptor] {obj}"), new Style(foreground: context.Config.ColorConfig.MetadataInfoColor.ToSpectreColor()));

    public override IRenderable RenderNullValue(IDescriptor? descriptor, in RendererConfig config) 
        => Markup.FromInterpolated($"null", new Style(foreground: config.ColorConfig.NullValueColor.ToSpectreColor()));

    protected override IRenderable RenderSingleValueDescriptor(object obj, SingleValueDescriptor descriptor, RenderContext context) 
        => new Markup(Markup.Escape($"{obj ?? "[missing]"}"), new Style(foreground: obj is not null ? context.Config.ColorConfig.PropertyValueColor?.ToSpectreColor() : context.Config.ColorConfig?.MetadataErrorColor.ToSpectreColor()));

    protected override IRenderable RenderUnfamiliarCustomDescriptor(object obj, CustomDescriptor descriptor, in RendererConfig config) 
        => new Markup(Markup.Escape($"[Unfamiliar descriptor {descriptor.Name}]"), new Style(foreground: config.ColorConfig.MetadataErrorColor.ToSpectreColor()));

    protected override IRenderable RenderUnsupportedDescriptor(object obj, IDescriptor descriptor, RenderContext context) 
        => new Markup(Markup.Escape($"[Unsupported {descriptor.GetType().Name} for {descriptor.Name}]"), new Style(foreground: context.Config.ColorConfig.MetadataErrorColor.ToSpectreColor()));
}
