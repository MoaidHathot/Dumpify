using Dumpify.Descriptors;
using Dumpify.Extensions;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections;
using System.Collections.Concurrent;
using System.Text;

namespace Dumpify;

internal class SpectreConsoleTextRenderer : SpectreConsoleRendererBase
{
    public SpectreConsoleTextRenderer()
        : base(new ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<IRenderable>>>())
    {
    }

    protected override IRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext<SpectreRendererState> context)
    {
        var builder = new StringBuilder();

        var typeName = context.Config.TypeNameProvider.GetTypeName(descriptor.Type);
        builder.Append($"{{{typeName}}}");

        var indent = new string(' ', (context.CurrentDepth + 1) * 2);
        foreach (var property in descriptor.Properties)
        {
            builder.AppendLine();
            // Update path with property name for reference tracking
            var propertyContext = context.WithProperty(property.Name) with { CurrentDepth = context.CurrentDepth + 1 };
            var renderedValue = GetValueAndRender(obj, property.ValueProvider!, property, propertyContext);
            builder.Append($"{indent}{property.Name}: {renderedValue}");
        }

        return RenderSingleValue(builder.ToString(), context, null);
    }

    protected override IRenderable RenderSingleValue(object value, RenderContext<SpectreRendererState> context, Color? color)
        => new TextRenderableAdapter(value.ToString() ?? "", new Style(foreground: color));

    protected override IRenderable RenderLabelDescriptor(object obj, LabelDescriptor descriptor, RenderContext<SpectreRendererState> context)
        => new Markup(Markup.Escape(obj.ToString() ?? ""));

    protected override IRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext<SpectreRendererState> context)
    {
        var items = (IEnumerable)obj;

        var memberProvider = context.Config.MemberProvider;
        var renderedItems = new List<IRenderable>();

        int index = 0;
        foreach (var item in items)
        {
            var renderedItem = obj switch
            {
                null => RenderNullValue(null, context),
                not null => GetRenderedValue(item, descriptor.ElementsType, index),
            };

            renderedItems.Add(renderedItem);
            index++;

            IRenderable GetRenderedValue(object item, Type? elementType, int idx)
            {
                var itemType = descriptor.ElementsType ?? item.GetType();
                var itemDescriptor = DumpConfig.Default.Generator.Generate(itemType, null, memberProvider);

                // Update path with index for reference tracking
                var itemContext = context.WithIndex(idx) with { CurrentDepth = context.CurrentDepth + 1 };
                return RenderDescriptor(item, itemDescriptor, itemContext);
            }
        }

        if (renderedItems.None())
        {
            return new TextRenderableAdapter("[]");
        }

        var itemIndent = new string(' ', (context.CurrentDepth + 1) * 2);
        var itemsStr = string.Join($",{Environment.NewLine}{itemIndent}", renderedItems);

        var result = $"[{Environment.NewLine}{itemIndent}{itemsStr}{Environment.NewLine}{new string(' ', (context.CurrentDepth) * 2)}]";

        return new TextRenderableAdapter(result);
    }

    protected override IRenderedObject CreateRenderedObject(IRenderable rendered)
    {
        var markup = ((TextRenderableAdapter)rendered).ToMarkup();

        return base.CreateRenderedObject(markup);
    }

    // protected override IRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext<SpectreRendererState> context) => throw new NotImplementedException();
    // protected override IRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext<SpectreRendererState> context) => throw new NotImplementedException();
}