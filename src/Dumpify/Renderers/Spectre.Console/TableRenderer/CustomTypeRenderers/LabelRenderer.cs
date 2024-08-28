using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;
internal class LabelRenderer : ICustomTypeRenderer<IRenderable>
{
    public Type DescriptorType => typeof(LabelDescriptor);

    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;

    public LabelRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
    {
        _handler = handler;
    }

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        var propertyValueColor = context.Config.ColorConfig.PropertyValueColor?.HexString ?? "default";
        
        var markup = new Markup($"[{propertyValueColor}]{obj}[/]");
        return markup;
    }

    public (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type == typeof(string), null);
}
