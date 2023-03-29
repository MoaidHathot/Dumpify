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
    protected override void PublishRenderables(IRenderable renderable)
    {
        AnsiConsole.Write(renderable);
        System.Console.WriteLine();
    }

    protected override IRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentDepth)
    {
        if(descriptor.Type.IsArray)
        {
            return RenderArray((Array)obj, descriptor, config, tracker, currentDepth);
        }

        return RenderIEnumerable((IEnumerable)obj, descriptor, config, tracker, currentDepth);
    }

    private IRenderable RenderIEnumerable(IEnumerable obj, MultiValueDescriptor descriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentDepth)
    {
        var table = new Table();
        table.AddColumn(Markup.Escape($"IEnumerable<{descriptor.ElementsType?.Name ?? ""}>"));

        foreach (var item in obj)
        {
            var type = descriptor.ElementsType ?? item?.GetType();
            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

            var renderedItem = RenderDescriptor(item, itemsDescriptor, config, tracker, currentDepth);
            table.AddRow(renderedItem);
        }

        table.Collapse();
        return table;
    }

    private IRenderable RenderArray(Array obj, MultiValueDescriptor descriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentDepth)
    {
        if(!descriptor.Type.IsSZArray)
        {
            return RenderMultiDimentionalArray(obj, descriptor, config, tracker, currentDepth);
        }

        var table = new Table();
        table.AddColumn(Markup.Escape($"{descriptor.ElementsType?.Name ?? ""}[{obj.GetLength(0)}]"));

        for(var index = 0; index < obj.Length; ++index)
        {
            var item = obj.GetValue(index);

            var type = descriptor.ElementsType ?? item?.GetType();
            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

            var renderedItem = RenderDescriptor(item, itemsDescriptor, config, tracker, currentDepth);
            table.AddRow(renderedItem);
        }

        table.Collapse();
        return table;
    }


    private IRenderable RenderMultiDimentionalArray(Array obj, MultiValueDescriptor descriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentDepth)
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

                    var renderedItem = RenderDescriptor(item, itemsDescriptor, config, tracker, currentDepth);
                    cells.Add(renderedItem);
                }

                table.AddRow(cells);
            }

            table.Collapse();
            return table;
        }

        return RenderIEnumerable(obj, descriptor, config, tracker, currentDepth);
    }

    protected override IRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentDepth)
    {
        if (ObjectAlreadyRendered(obj, tracker))
        {
            return RenderCircularDependency(obj, descriptor, config);
        }

        var table = new Table();
        table.Title = new TableTitle(Markup.Escape(descriptor.Type.ToString()));

        table.Collapse();
        table.AddColumn("Name");
        table.AddColumn("Value");

        foreach (var property in descriptor.Properties)
        {
            var renderedValue = RenderDescriptor(property.PropertyInfo!.GetValue(obj), property, config, tracker, currentDepth + 1);
            table.AddRow(new Markup(Markup.Escape(property.Name)), renderedValue);
        }

        table.Collapse();
        return table;
    }

    protected override IRenderable RenderCircularDependency(object @object, IDescriptor? descriptor, in RendererConfig config)
    => new Markup(Markup.Escape("[Circular Dependency]"), new Style(foreground: Color.DarkSlateGray3));

    protected override IRenderable RenderExeededDepth(object obj, IDescriptor? descriptor, in RendererConfig config)
    => new Markup(Markup.Escape($"[Exceeded max depth {config.MaxDepth}]"), new Style(foreground: Color.DarkSlateGray3));

    protected override IRenderable RenderIgnoredDescriptor(object obj, IgnoredDescriptor descriptor)
    => new Markup(Markup.Escape($"Ignored[{descriptor.Name}]"), new Style(foreground: Color.DarkSlateGray1));

    protected override IRenderable RenderNullDescriptor(object obj) 
        => Markup.FromInterpolated($"[null descriptor] {obj}", new Style(foreground: Color.Yellow));

    protected override IRenderable RenderNullValue(IDescriptor? descriptor, in RendererConfig config) 
        => Markup.FromInterpolated($"null", new Style(foreground: Color.DarkSlateGray2));

    protected override IRenderable RenderSingleValueDescriptor(object obj, SingleValueDescriptor descriptor) 
        => new Markup(Markup.Escape($"{obj ?? "[missing]"}"));

    protected override IRenderable RenderUnfamiliarCustomDescriptor(object obj, CustomDescriptor descriptor, in RendererConfig config) 
        => new Markup(Markup.Escape($"Custom[{descriptor.Name}]"), new Style(foreground: Color.Orange3));

    protected override IRenderable RenderUnsupportedDescriptor(object obj, IDescriptor descriptor) 
        => new Markup(Markup.Escape($"Unsuppored descriptor[{descriptor.GetType().Name}] for [{descriptor.Name}]"), new Style(foreground: Color.Red1));
}
