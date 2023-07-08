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

        var color = context.Config.ColorConfig.PropertyValueColor?.HexString ?? "default";

        var name = context.Config.TypeNameProvider.GetTypeName(descriptor.Type);
        return new Markup($"[{color}]{name}.{obj}[/]");
    }

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type.IsEnum, null);
}