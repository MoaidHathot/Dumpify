using Dumpify.Descriptors;
using Dumpify.Extensions;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections;
using System.Collections.Concurrent;
using System.Text;

namespace Dumpify.Renderers.Spectre.Console.TextRenderer;

internal class SpectreConsoleTextRenderer : SpectreConsoleRendererBase
{
    public SpectreConsoleTextRenderer()
        : base(new ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<IRenderable>>>())
    {
    }

    protected override IRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext context)
    {
        var builder = new StringBuilder();

        builder.Append($"{{{descriptor.Type.GetGenericTypeName()}}}");

        var indent = new string(' ', (context.CurrentDepth + 1) * 2);
        foreach (var property in descriptor.Properties)
        {
            builder.AppendLine();
            var renderedValue = RenderDescriptor(property.ValueProvider!.GetValue(obj), property, context with { CurrentDepth = context.CurrentDepth + 1 });
            builder.Append($"{indent}{property.Name}: {renderedValue}");
        }

        return RenderSingleValue(builder.ToString(), context, null);
    }

    protected override IRenderable RenderSingleValue(object value, RenderContext context, Color? color)
        => new TextRenderableAdapter(value.ToString() ?? "", new Style(foreground: color));

    protected override IRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext context)
    {
        var items = (IEnumerable)obj;

        var memberProvider = context.Config.MemberProvider;
        var renderedItems = new List<IRenderable>();

        foreach (var item in items)
        {
            var renderedItem = obj switch
            {
                null => RenderNullValue(null, context),
                not null => GetRenderedValue(item, descriptor.ElementsType),
            };

            renderedItems.Add(renderedItem);

            IRenderable GetRenderedValue(object item, Type? elementType)
            {
                var itemType = descriptor.ElementsType ?? item.GetType();
                var itemDescriptor = DumpConfig.Default.Generator.Generate(itemType, null, memberProvider);

                return RenderDescriptor(item, itemDescriptor, context with { CurrentDepth = context.CurrentDepth + 1});
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
}
