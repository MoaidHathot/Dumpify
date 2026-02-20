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
            var renderedValue = GetValueAndRender(obj, property.ValueProvider!, property, context with { CurrentDepth = context.CurrentDepth + 1 });
            builder.Append($"{indent}{property.Name}: {renderedValue}");
        }

        return RenderSingleValue(builder.ToString(), context, null);
    }

    protected override IRenderable RenderSingleValue(object value, RenderContext<SpectreRendererState> context, Color? color)
        => new TextRenderableAdapter(value.ToString() ?? "", new Style(foreground: color));

    protected override IRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext<SpectreRendererState> context)
    {
        var items = (IEnumerable)obj;
        var memberProvider = context.Config.MemberProvider;

        // Use the centralized truncation utility
        var truncated = CollectionTruncator.Truncate(
            items.Cast<object?>(),
            context.Config.TruncationConfig);

        var renderedItems = new List<string>();
        var itemIndent = new string(' ', (context.CurrentDepth + 1) * 2);

        // Render start marker if present (for Tail or HeadAndTail modes)
        if (truncated.StartMarker is { } startMarker)
        {
            renderedItems.Add(startMarker.GetDefaultMessage());
        }

        // Render items with middle marker support
        for (int i = 0; i < truncated.Items.Count; i++)
        {
            // Insert middle marker at the appropriate position (for HeadAndTail mode)
            if (truncated.MiddleMarkerIndex == i && truncated.MiddleMarker is { } middleMarker)
            {
                renderedItems.Add(middleMarker.GetDefaultMessage());
            }

            var item = truncated.Items[i];
            var renderedItem = item switch
            {
                null => RenderNullValue(null, context).ToString() ?? "null",
                not null => GetRenderedValue(item, descriptor.ElementsType).ToString() ?? ""
            };

            renderedItems.Add(renderedItem);
        }

        // Render end marker if present (for Head mode)
        if (truncated.EndMarker is { } endMarker)
        {
            renderedItems.Add(endMarker.GetDefaultMessage());
        }

        if (renderedItems.Count == 0)
        {
            return new TextRenderableAdapter("[]");
        }

        var itemsStr = string.Join($",{Environment.NewLine}{itemIndent}", renderedItems);
        var result = $"[{Environment.NewLine}{itemIndent}{itemsStr}{Environment.NewLine}{new string(' ', context.CurrentDepth * 2)}]";

        return new TextRenderableAdapter(result);

        IRenderable GetRenderedValue(object item, Type? elementType)
        {
            var itemType = elementType ?? item.GetType();
            var itemDescriptor = DumpConfig.Default.Generator.Generate(itemType, null, memberProvider);
            return RenderDescriptor(item, itemDescriptor, context with { CurrentDepth = context.CurrentDepth + 1 });
        }
    }

    protected override IRenderedObject CreateRenderedObject(IRenderable rendered)
    {
        var markup = ((TextRenderableAdapter)rendered).ToMarkup();
        return base.CreateRenderedObject(markup);
    }
}
