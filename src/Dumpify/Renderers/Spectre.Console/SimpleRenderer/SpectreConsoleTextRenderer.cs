using Dumpify.Descriptors;
using Dumpify.Extensions;
using Spectre.Console;
using System.Collections;
using System.Collections.Concurrent;
using System.Text;

namespace Dumpify.Renderers.Spectre.Console.SimpleRenderer;

internal class SpectreConsoleTextRenderer : RendererBase<string>
{
    public SpectreConsoleTextRenderer()
        : base(new ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<string>>>())
    {
    }

    protected override string RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext context)
    {
        var builder = new StringBuilder();

        builder.Append($"{{{descriptor.Type.GetGenericTypeName()}}}");

        var indent = new string(' ', (context.CurrentDepth + 1) * 2);
        foreach (var property in descriptor.Properties)
        {
            builder.AppendLine();
            var renderedValue = RenderDescriptor(property.PropertyInfo!.GetValue(obj), property, context with { CurrentDepth = context.CurrentDepth + 1 });
            builder.Append($"{indent}{property.Name}: {renderedValue}");
        }

        return builder.ToString();
    }

    protected override string RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext context)
    {
        var items = (IEnumerable)obj;

        var renderedItems = new List<string>();
        foreach (var item in items)
        {
            var renderedItem = obj switch
            {
                null => RenderNullValue(null, context),
                not null => GetRenderedValue(item, descriptor.ElementsType, context),
            };

            renderedItems.Add(renderedItem);

            string GetRenderedValue(object item, Type? elementType, RenderContext context)
            {
                var itemType = descriptor.ElementsType ?? item.GetType();
                var itemDescriptor = DumpConfig.Default.Generator.Generate(itemType, null);

                return RenderDescriptor(item, itemDescriptor, context);
            }
        }

        var joined = string.Join(", ", renderedItems);

        return $"[{joined}]";
    }

    protected override void PublishRenderables(string renderable, RenderContext context)
    {
        var text = new Text(renderable);
        AnsiConsole.Write(text);
        System.Console.WriteLine();
        System.Console.WriteLine();
    }

    public override string RenderNullValue(IDescriptor? descriptor, RenderContext context)
        => "null";

    public override string RenderExceededDepth(object obj, IDescriptor? descriptor, RenderContext context)
        => $"[Exceeded max depth {context.Config.MaxDepth}]";

    protected override string RenderCircularDependency(object @object, IDescriptor? descriptor, RenderContext context)
        => "[Circular Reference]";

    protected override string RenderNullDescriptor(object obj, RenderContext context) 
        => $"[null descriptor] {obj}";

    protected override string RenderIgnoredDescriptor(object obj, IgnoredDescriptor descriptor, RenderContext context)
        => $"[Ignored {descriptor.Name}]";

    protected override string RenderSingleValueDescriptor(object obj, SingleValueDescriptor descriptor, RenderContext context) 
        => obj is string ? $"\"{obj}\"" : obj.ToString() ?? "";

    protected override string RenderUnsupportedDescriptor(object obj, IDescriptor descriptor, RenderContext context) 
        => $"[Unsupported {descriptor.GetType().Name} for {descriptor.Name}]";

    protected override string RenderUnfamiliarCustomDescriptor(object obj, CustomDescriptor descriptor, RenderContext context) 
        => $"[Unfamiliar descriptor {descriptor.Name}]";
}
