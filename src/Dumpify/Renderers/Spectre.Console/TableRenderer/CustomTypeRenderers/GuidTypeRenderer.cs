using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

internal class GuidTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;

    public Type DescriptorType => typeof(CustomDescriptor);


    public GuidTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
    {
        _handler = handler;
    }

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = ((RenderContext<SpectreRendererState>)baseContext);
        var valueColor = context.Config.ColorConfig.PropertyValueColor?.HexString ?? "default";

        var markupValue = $"[[[{valueColor}]{((Guid)obj).ToString().EscapeMarkup()}[/]]]";

        var markup = new Markup(markupValue);

        if (context.Config.Label is { } label && context.CurrentDepth == 0 && object.ReferenceEquals(context.RootObject, obj))
        {
            var table = new ObjectTableBuilder(context, descriptor, obj)
                .AddColumnName("Values")
                .AddRow(descriptor, obj, markup)
                .HideTitle()
                .HideHeaders()
                .Build();

            return table;
        }

        return markup;
    }

    public (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type == typeof(Guid), null);
}