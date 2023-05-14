using Dumpify.Descriptors;
using Dumpify.Descriptors.ValueProviders;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections.Concurrent;

namespace Dumpify.Renderers.Spectre.Console;

internal abstract class SpectreConsoleRendererBase : RendererBase<IRenderable, SpectreRendererState>
{
    protected SpectreConsoleRendererBase(ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<IRenderable>>> customTypeRenderers)
        : base(customTypeRenderers)
    {
    }

    protected override IRenderable RenderFailedValueReading(Exception ex, IValueProvider valueProvider, IDescriptor? descriptor, RenderContext<SpectreRendererState> context)
        => RenderSingleValue("[Failed to read value]", context, context.Config.ColorConfig.MetadataErrorColor?.ToSpectreColor());

    protected override IRenderedObject CreateRenderedObject(IRenderable rendered)
        => new SpectreConsoleRenderedObject(rendered);

    protected virtual IRenderable RenderSingleValue(object value, RenderContext<SpectreRendererState> context, Color? color)
        => new Markup(Markup.Escape(value.ToString() ?? ""), new Style(foreground: color));

    protected override IRenderable RenderSingleValueDescriptor(object obj, SingleValueDescriptor descriptor, RenderContext<SpectreRendererState> context)
        => RenderSingleValue(obj is string str ? $"\"{str}\"" : obj, context, context.Config.ColorConfig.PropertyValueColor?.ToSpectreColor());

    public override IRenderable RenderNullValue(IDescriptor? descriptor, RenderContext<SpectreRendererState> context)
        => RenderSingleValue("null", context, context.Config.ColorConfig.NullValueColor.ToSpectreColor());

    public override IRenderable RenderExceededDepth(object obj, IDescriptor? descriptor, RenderContext<SpectreRendererState> context)
        => RenderSingleValue($"[Exceeded max depth {context.Config.MaxDepth}]", context, context.Config.ColorConfig.MetadataInfoColor.ToSpectreColor());

    protected override IRenderable RenderCircularDependency(object @object, IDescriptor? descriptor, RenderContext<SpectreRendererState> context)
            => RenderSingleValue($"[Circular Reference]", context, context.Config.ColorConfig.MetadataInfoColor.ToSpectreColor());

    protected override IRenderable RenderNullDescriptor(object obj, RenderContext<SpectreRendererState> context)
            => RenderSingleValue($"[null descriptor] {obj}", context, context.Config.ColorConfig.MetadataErrorColor.ToSpectreColor());

    protected override IRenderable RenderIgnoredDescriptor(object obj, IgnoredDescriptor descriptor, RenderContext<SpectreRendererState> context)
            => RenderSingleValue($"[Ignored {descriptor.Name}] {obj}", context, context.Config.ColorConfig.IgnoredValueColor.ToSpectreColor());

    protected override IRenderable RenderUnsupportedDescriptor(object obj, IDescriptor descriptor, RenderContext<SpectreRendererState> context)
        => RenderSingleValue($"[Unsupported {descriptor.GetType().Name} for {descriptor.Name}]", context, context.Config.ColorConfig.MetadataErrorColor.ToSpectreColor());

    protected override IRenderable RenderUnfamiliarCustomDescriptor(object obj, CustomDescriptor descriptor, RenderContext<SpectreRendererState> context)
        => RenderSingleValue($"[Unfamiliar descriptor {descriptor.GetType().Name} for {descriptor.Name}]", context, context.Config.ColorConfig.MetadataErrorColor.ToSpectreColor());

    protected override SpectreRendererState CreateState(object? obj, IDescriptor? descriptor, RendererConfig config)
        => new SpectreRendererState(config);
}