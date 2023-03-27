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
        var enumerable = (IEnumerable)obj;

        var table = new Table();
        table.Collapse();
        table.AddColumn(Markup.Escape($"IEnumerable<{descriptor.ValuesType?.Name ?? ""}>"));

        foreach (var item in enumerable)
        {
            var type = descriptor.ValuesType ?? item?.GetType();
            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

            var renderedItem = RenderDescriptor(item, itemsDescriptor, config, tracker, currentDepth);
            table.AddRow(renderedItem);
        }

        return table;
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
