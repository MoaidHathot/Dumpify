using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers.Spectre.Console.TableRenderer.CustomTypeRenderers;

internal class EnumTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable> _handler;
    public Type DescriptorType => typeof(CustomDescriptor);

    public EnumTypeRenderer(IRendererHandler<IRenderable> handler)
    {
        _handler = handler;
    }

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext context, object? handleContext)
    {
        var color = context.Config.ColorConfig.PropertyValueColor?.HexString ?? "default";

        var name = context.Config.TypeNameProvider.GetTypeName(descriptor.Type);
        return new Markup($"[{color}]{name}.{obj}[/]");
    }

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type.IsEnum, null);
}
