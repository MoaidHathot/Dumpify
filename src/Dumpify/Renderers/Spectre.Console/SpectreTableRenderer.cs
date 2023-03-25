using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers.Spectre.Console;
internal class SpectreTableRenderer : IRenderer
{
    public void Render(object? obj, IDescriptor? descriptor, RendererConfig config)
    {
        var renderable = obj switch
        {
            null => RenderNull(descriptor, config),
            _ => RenderDescriptor(obj, descriptor, config),
        };

        AnsiConsole.Write(renderable);
        System.Console.WriteLine();
    }

    private IRenderable RenderNull(IDescriptor? descriptor, RendererConfig config)
    {
        return Markup.FromInterpolated($"null", new Style(foreground: Color.DarkSlateGray2));
    }

    private IRenderable RenderDescriptor(object? @object, IDescriptor? descriptor, in RendererConfig config)
    {
        return descriptor switch
        {
            null => Markup.FromInterpolated($"[null] {@object}", new Style(foreground: Color.Yellow)),
            IgnoredDescriptor ignoredDescriptor => Markup.FromInterpolated($"Ignored[{ignoredDescriptor.Name}]", new Style(foreground: Color.DarkSlateGray1)),
            //SingleValueDescriptor singleDescriptor => Markup.FromInterpolated($"{singleDescriptor.PropertyInfo?.GetValue(@object) ?? "[missing]"}"),
            SingleValueDescriptor singleDescriptor => Markup.FromInterpolated($"{@object ?? "[missing]"}"),
            ObjectDescriptor objDescriptor => RenderObject(@object, objDescriptor, config),
            MultiValueDescriptor multiDescriptor => RenderMultiValue(@object, multiDescriptor, config),
            CustomDescriptor customDescriptor => new Markup(Markup.Escape($"Custom[{customDescriptor.Name}]"), new Style(foreground: Color.Orange3)),
            _ => Markup.FromInterpolated($"Unsuppored descriptor[{descriptor.GetType().Name}] for [{descriptor.Name}]", new Style(foreground: Color.Red1))
        };
    }

    private IRenderable RenderObject(object? obj, ObjectDescriptor descriptor, in RendererConfig config)
    {
        var table = new Table();
        table.Title = new TableTitle(Markup.Escape(descriptor.Type.ToString()));

        table.Collapse();
        table.AddColumn("Name");
        table.AddColumn("Value");

        foreach(var property in descriptor.Properties)
        {
            var renderedValue = RenderDescriptor(property.PropertyInfo!.GetValue(obj), property, config);
            table.AddRow(new Markup(Markup.Escape(property.Name)), renderedValue);
        }

        return table;
    }

    private IRenderable RenderMultiValue(object? obj, MultiValueDescriptor descriptor, in RendererConfig config)
    {
        if(obj is null)
        {
            return RenderNull(descriptor, config);
        }

        var enumerable = ((IEnumerable)obj);

        var table = new Table();
        table.AddColumn(Markup.Escape(descriptor.ValuesType?.Name ?? ""));

        foreach(var item in enumerable)
        {
            var type = descriptor.ValuesType ?? item?.GetType();
            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

            var renderedItem = RenderDescriptor(item, itemsDescriptor, config);
            table.AddRow(renderedItem);
        }

        return table;
    }
}
