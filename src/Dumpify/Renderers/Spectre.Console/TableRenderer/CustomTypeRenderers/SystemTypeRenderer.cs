using Dumpify.Descriptors;
using Dumpify.Extensions;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;
internal class SystemTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable> _handler;

    public SystemTypeRenderer(IRendererHandler<IRenderable> handler)
        => _handler = handler;

    public Type DescriptorType { get; } = typeof(CustomDescriptor);

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext context)
    {
        var metadataColor = context.Config.ColorConfig.MetadataInfoColor?.HexString ?? "default";
        var typeColor = context.Config.ColorConfig.TypeNameColor?.HexString ?? "default";
        var typeName = context.Config.TypeNameProvider.GetTypeName((Type)obj);
        var c = $"[{metadataColor}]typeof([/][{typeColor}]{Markup.Escape(typeName)}[/][{metadataColor}])[/]";
        return new Markup(c);
    }

    public bool ShouldHandle(IDescriptor descriptor, object obj)
        => obj is Type || descriptor?.Type.FullName == "System.RuntimeType";
}
