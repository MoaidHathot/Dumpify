using Dumpify;
using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dumpiyf;

internal class SystemReflectionTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;

    public SystemReflectionTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
        => _handler = handler;

    public Type DescriptorType { get; } = typeof(CustomDescriptor);

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext context, object? handleContext)
    {
        var typeColor = context.Config.ColorConfig.TypeNameColor?.HexString ?? "default";
        var typeName = context.Config.TypeNameProvider.GetTypeName(obj.GetType());
        var c = $"[[[{typeColor}]{Markup.Escape(typeName)}[/]]]";
        return new Markup(c);
    }

    public (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type.Namespace?.StartsWith("System.Reflection") ?? false || obj is MemberInfo, null);
}
