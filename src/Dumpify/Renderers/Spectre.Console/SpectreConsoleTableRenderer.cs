using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers.Spectre.Console;

internal class SpectreConsoleTableRenderer : RendererBase<IRenderable>
{
    public SpectreConsoleTableRenderer()
        : base(new Dictionary<RuntimeTypeHandle, IList<CustomTypeRenderer<IRenderable>>>())
    {
        AddCustomTypeDescriptor(typeof(MultiValueDescriptor));
    }

    private void AddCustomTypeDescriptor(Type descriptorType)
    {
        if(!_customTypeRenderers.TryGetValue(descriptorType.TypeHandle, out var renderersList)) 
        { 
            renderersList = new List<CustomTypeRenderer<IRenderable>>();

            _customTypeRenderers.Add(descriptorType.TypeHandle, renderersList);
        }

        renderersList.Add(new CustomTypeRenderer<IRenderable>(descriptorType,
            shouldHandleFunc: (d, o) => d.Type.IsGenericType && d.Type.GetGenericTypeDefinition() == typeof(Dictionary<,>),
            rendererFunc: (d, o, c) => RenderDictionary(o, (MultiValueDescriptor)d, c))
        );
            
    }

    private IRenderable RenderDictionary(object obj, MultiValueDescriptor descriptor, RenderContext context)
    {
        var table = new Table();
        table.AddColumn("Key");
        table.AddColumn("Value");

        var dictionary = (IDictionary)obj;

        foreach (var keyValue in dictionary.Keys)
        {
            var keyDescriptor = DumpConfig.Default.Generator.Generate(keyValue.GetType(), null);
            var keyRenderable = RenderDescriptor(keyValue, keyDescriptor, context);

            var value = dictionary[keyValue];

            var valueRenderable = value switch
            {
                null => RenderNullValue(null, context.Config),
                not null => RenderDescriptor(value, DumpConfig.Default.Generator.Generate(value.GetType(), null), context),
            };

            table.AddRow(keyRenderable, valueRenderable);
        }

        return table;
    }

    protected override void PublishRenderables(IRenderable renderable)
    {
        AnsiConsole.Write(renderable);
        System.Console.WriteLine();
    }

    protected override IRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext context)
    {
        if(descriptor.Type.IsArray)
        {
            return RenderArray((Array)obj, descriptor, context);
        }

        return RenderIEnumerable((IEnumerable)obj, descriptor, context);
    }

    private IRenderable RenderIEnumerable(IEnumerable obj, MultiValueDescriptor descriptor, RenderContext context)
    {
        var table = new Table();
        table.AddColumn(Markup.Escape($"IEnumerable<{descriptor.ElementsType?.Name ?? ""}>"));

        foreach (var item in obj)
        {
            var type = descriptor.ElementsType ?? item?.GetType();
            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

            var renderedItem = RenderDescriptor(item, itemsDescriptor, context);
            table.AddRow(renderedItem);
        }

        table.Collapse();
        return table;
    }

    private IRenderable RenderArray(Array obj, MultiValueDescriptor descriptor, RenderContext context)
    {
        if(!descriptor.Type.IsSZArray)
        {
            return RenderMultiDimentionalArray(obj, descriptor, context);
        }

        var table = new Table();
        table.AddColumn(Markup.Escape($"{descriptor.ElementsType?.Name ?? ""}[{obj.GetLength(0)}]"));

        for(var index = 0; index < obj.Length; ++index)
        {
            var item = obj.GetValue(index);

            var type = descriptor.ElementsType ?? item?.GetType();
            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

            var renderedItem = RenderDescriptor(item, itemsDescriptor, context);
            table.AddRow(renderedItem);
        }

        table.Collapse();
        return table;
    }

    private IRenderable RenderMultiDimentionalArray(Array obj, MultiValueDescriptor descriptor, RenderContext context)
    {
        if(obj.Rank == 2)
        {
            var table = new Table();

            var rows = obj.GetLength(0);
            var collumns = obj.GetLength(1);

            table.Title = new TableTitle(Markup.Escape($"{descriptor.ElementsType?.Name ?? ""}[{rows},{collumns}]"));
            table.AddColumn(new TableColumn(""));

            for (var col = 0; col < collumns; ++col)
            {
                table.AddColumn(new TableColumn(new Markup(col.ToString(), new Style(foreground: Color.DarkSeaGreen3))));
            }

            for(var row = 0; row < rows; ++row)
            {
                var cells = new List<IRenderable>() { new Markup(row.ToString(), new Style(foreground: Color.DarkSlateGray3)) };

                for(var  col = 0; col < collumns; ++col)
                {
                    var item = obj.GetValue(row, col);

                    var type = descriptor.ElementsType ?? item?.GetType();
                    IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

                    var renderedItem = RenderDescriptor(item, itemsDescriptor, context);
                    cells.Add(renderedItem);
                }

                table.AddRow(cells);
            }

            table.Collapse();
            return table;
        }

        return RenderIEnumerable(obj, descriptor, context);
    }

    protected override IRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext context)
    {
        if (ObjectAlreadyRendered(obj, context.ObjectTracker))
        {
            return RenderCircularDependency(obj, descriptor, context.Config);
        }

        var table = new Table();
        table.Title = new TableTitle(Markup.Escape(descriptor.Type.ToString()));

        table.Collapse();
        table.AddColumn("Name");
        table.AddColumn("Value");

        foreach (var property in descriptor.Properties)
        {
            var renderedValue = RenderDescriptor(property.PropertyInfo!.GetValue(obj), property, context with { CurrentDepth = context.CurrentDepth + 1});
            table.AddRow(new Markup(Markup.Escape(property.Name)), renderedValue);
        }

        table.Collapse();
        return table;
    }

    protected override IRenderable RenderCircularDependency(object @object, IDescriptor? descriptor, in RendererConfig config)
    => new Markup(Markup.Escape("[Circular Dependency]"), new Style(foreground: Color.DarkSlateGray3));

    protected override IRenderable RenderExeededDepth(object obj, IDescriptor? descriptor, in RendererConfig config)
    => new Markup(Markup.Escape($"[Exceeded max depth {config.MaxDepth}]"), new Style(foreground: Color.DarkSlateGray3));

    protected override IRenderable RenderIgnoredDescriptor(object obj, IgnoredDescriptor descriptor, RenderContext context)
    => new Markup(Markup.Escape($"[Ignored {descriptor.Name}]"), new Style(foreground: Color.DarkSlateGray1));

    protected override IRenderable RenderNullDescriptor(object obj) 
        => Markup.FromInterpolated($"[null descriptor] {obj}", new Style(foreground: Color.Yellow));

    protected override IRenderable RenderNullValue(IDescriptor? descriptor, in RendererConfig config) 
        => Markup.FromInterpolated($"null", new Style(foreground: Color.DarkSlateGray2));

    protected override IRenderable RenderSingleValueDescriptor(object obj, SingleValueDescriptor descriptor, RenderContext context) 
        => new Markup(Markup.Escape($"{obj ?? "[missing]"}"));

    protected override IRenderable RenderUnfamiliarCustomDescriptor(object obj, CustomDescriptor descriptor, in RendererConfig config) 
        => new Markup(Markup.Escape($"[Unfamiliar {descriptor.Name}]"), new Style(foreground: Color.Orange3));

    protected override IRenderable RenderUnsupportedDescriptor(object obj, IDescriptor descriptor) 
        => new Markup(Markup.Escape($"[Unsupported {descriptor.GetType().Name} for {descriptor.Name}]"), new Style(foreground: Color.Red1));
}
