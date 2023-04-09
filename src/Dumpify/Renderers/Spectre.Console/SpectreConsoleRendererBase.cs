using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections.Concurrent;

namespace Dumpify.Renderers.Spectre.Console;

internal abstract class SpectreConsoleRendererBase : RendererBase<IRenderable>
{
    protected SpectreConsoleRendererBase(ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<IRenderable>>> customTypeRenderers)
        : base(customTypeRenderers)
    {
    }

    protected override void PublishRenderables(IRenderable renderable, RenderContext context)
    {
        AnsiConsole.Write(renderable);
        System.Console.WriteLine();
    }

    public override IRenderable RenderNullValue(IDescriptor? descriptor, RenderContext context)
        => Markup.FromInterpolated($"null", new Style(foreground: context.Config.ColorConfig.NullValueColor.ToSpectreColor()));

    public override IRenderable RenderExceededDepth(object obj, IDescriptor? descriptor, RenderContext context)
        => new Markup(Markup.Escape($"[Exceeded max depth {context.Config.MaxDepth}]"), new Style(foreground: context.Config.ColorConfig.MetadataInfoColor.ToSpectreColor()));

    protected override IRenderable RenderCircularDependency(object @object, IDescriptor? descriptor, RenderContext context)
        => new Markup(Markup.Escape("[Circular Reference]"), new Style(foreground: context.Config.ColorConfig.MetadataInfoColor.ToSpectreColor()));

    protected override IRenderable RenderNullDescriptor(object obj, RenderContext context)
        => new Markup(Markup.Escape($"[null descriptor] {obj}"), new Style(foreground: context.Config.ColorConfig.MetadataInfoColor.ToSpectreColor()));

    protected override IRenderable RenderIgnoredDescriptor(object obj, IgnoredDescriptor descriptor, RenderContext context)
        => new Markup(Markup.Escape($"[Ignored {descriptor.Name}]"), new Style(foreground: context.Config.ColorConfig.IgnoredValueColor.ToSpectreColor()));

    protected override IRenderable RenderSingleValueDescriptor(object obj, SingleValueDescriptor descriptor, RenderContext context)
        => new Markup(Markup.Escape( obj is string str ? $"\"{obj}\"" : obj.ToString() ?? ""), new Style(foreground: context.Config.ColorConfig.PropertyValueColor?.ToSpectreColor()));

    protected override IRenderable RenderUnsupportedDescriptor(object obj, IDescriptor descriptor, RenderContext context)
        => new Markup(Markup.Escape($"[Unsupported {descriptor.GetType().Name} for {descriptor.Name}]"), new Style(foreground: context.Config.ColorConfig.MetadataErrorColor.ToSpectreColor()));

    protected override IRenderable RenderUnfamiliarCustomDescriptor(object obj, CustomDescriptor descriptor, RenderContext context)
        => new Markup(Markup.Escape($"[Unfamiliar descriptor {descriptor.Name}]"), new Style(foreground: context.Config.ColorConfig.MetadataErrorColor.ToSpectreColor()));
}
