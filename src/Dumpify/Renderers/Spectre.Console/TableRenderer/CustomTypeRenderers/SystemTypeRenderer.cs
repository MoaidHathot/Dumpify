using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;
internal class SystemTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;

    public SystemTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
        => _handler = handler;

    public Type DescriptorType { get; } = typeof(CustomDescriptor);

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext context, object? handleContext)
    {
        var metadataColor = context.Config.ColorConfig.MetadataInfoColor?.HexString ?? "default";
        var typeColor = context.Config.ColorConfig.TypeNameColor?.HexString ?? "default";
        var typeName = context.Config.TypeNameProvider.GetTypeName((Type)obj);
        var c = $"[{metadataColor}]typeof([/][{typeColor}]{Markup.Escape(typeName)}[/][{metadataColor}])[/]";
        return new Markup(c);
    }

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
        => (obj is Type || descriptor?.Type.FullName == "System.RuntimeType", null);
}