using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers.Spectre.Console;

internal class SpectreTableRenderer : IRenderer
{
    public void Render(object? obj, IDescriptor? descriptor, RendererConfig config)
    {
        var idGenerator = new ObjectIDGenerator();

        var renderable = obj switch
        {
            null => RenderNull(descriptor, config),
            _ => RenderDescriptor(obj, descriptor, config, idGenerator, 0),
        };

        AnsiConsole.Write(renderable);
        System.Console.WriteLine();
    }

    private IRenderable RenderNull(IDescriptor? descriptor, RendererConfig config)
        => Markup.FromInterpolated($"null", new Style(foreground: Color.DarkSlateGray2));

    private IRenderable RenderFilteredByNesting(object obj, IDescriptor? descriptor, RendererConfig config)
        => new Markup(Markup.Escape($"[Filtered by Nesting level {config.MaxNestingLevel}] {descriptor?.GetType().Name}"), new Style(foreground: Color.DarkSlateGray3));

    public IRenderable RenderCircularDependency(object @object, IDescriptor? descriptor, RendererConfig config)
        => new Markup(Markup.Escape("[Circular Dependency]"), new Style(foreground: Color.DarkSlateGray3));

    private bool ObjectAlreadyRendered(object @object, ObjectIDGenerator tracker)
    {
        tracker.GetId(@object, out var firstTime);

        return firstTime is false;
    }

    private IRenderable RenderDescriptor(object? @object, IDescriptor? descriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentNesting)
    {
        if(@object is null)
        {
            return RenderNull(descriptor, config);
        }

        if(config.MaxNestingLevel is not null && currentNesting > config.MaxNestingLevel)
        {
            return RenderFilteredByNesting(@object, descriptor, config);
        }

        return descriptor switch
        {
            null => Markup.FromInterpolated($"[null descriptor] {@object}", new Style(foreground: Color.Yellow)),
            CircularDependencyDescriptor circularDescriptor => RenderDescriptor(@object, circularDescriptor.Descriptor, config, tracker, currentNesting),
            IgnoredDescriptor ignoredDescriptor => new Markup(Markup.Escape($"Ignored[{ignoredDescriptor.Name}]"), new Style(foreground: Color.DarkSlateGray1)),
            SingleValueDescriptor singleDescriptor => new Markup(Markup.Escape($"{@object ?? "[missing]"}")),
            ObjectDescriptor objDescriptor => RenderObject(@object, objDescriptor, config, tracker, currentNesting),
            MultiValueDescriptor multiDescriptor => RenderMultiValue(@object, multiDescriptor, config, tracker, currentNesting),
            CustomDescriptor customDescriptor => new Markup(Markup.Escape($"Custom[{customDescriptor.Name}]"), new Style(foreground: Color.Orange3)),
            _ => new Markup(Markup.Escape($"Unsuppored descriptor[{descriptor.GetType().Name}] for [{descriptor.Name}]"), new Style(foreground: Color.Red1))
        };
    }

    private IRenderable RenderObject(object obj, ObjectDescriptor descriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentNesting)
    {
        if(ObjectAlreadyRendered(obj, tracker))
        {
            return RenderCircularDependency(obj, descriptor, config);
        }

        var table = new Table();
        table.Title = new TableTitle(Markup.Escape(descriptor.Type.ToString()));

        table.Collapse();
        table.AddColumn("Name");
        table.AddColumn("Value");

        foreach(var property in descriptor.Properties)
        {
            var renderedValue = RenderDescriptor(property.PropertyInfo!.GetValue(obj), property, config, tracker, currentNesting + 1);
            table.AddRow(new Markup(Markup.Escape(property.Name)), renderedValue);
        }

        return table;
    }

    private IRenderable RenderMultiValue(object? obj, MultiValueDescriptor descriptor, in RendererConfig config, ObjectIDGenerator tracker, int currentNesting)
    {
        if(obj is null)
        {
            return RenderNull(descriptor, config);
        }

        var enumerable = ((IEnumerable)obj);

        var table = new Table();
        table.Collapse();
        table.AddColumn(Markup.Escape($"IEnumerable<{descriptor.ValuesType?.Name ?? ""}>"));

        foreach(var item in enumerable)
        {
            var type = descriptor.ValuesType ?? item?.GetType();
            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null) : null;

            var renderedItem = RenderDescriptor(item, itemsDescriptor, config, tracker, currentNesting);
            table.AddRow(renderedItem);
        }

        return table;
    }
}
