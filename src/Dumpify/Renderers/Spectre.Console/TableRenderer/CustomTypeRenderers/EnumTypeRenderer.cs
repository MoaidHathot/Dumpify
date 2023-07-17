using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

internal class EnumTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;
    public Type DescriptorType => typeof(CustomDescriptor);

    public EnumTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
    {
        _handler = handler;
    }

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        var propertyValueColor = context.Config.ColorConfig.PropertyValueColor?.HexString ?? "default";
        var typeColor = context.Config.ColorConfig.TypeNameColor?.HexString ?? "default";

        var name = context.Config.TypeNameProvider.GetTypeName(descriptor.Type);

        var markup = new Markup($"[{typeColor}]{name}[/].[{propertyValueColor}]{obj}[/]");


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

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type.IsEnum, null);
}