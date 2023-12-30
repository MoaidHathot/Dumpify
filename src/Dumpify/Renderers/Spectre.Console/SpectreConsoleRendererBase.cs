using Dumpify.Descriptors;
using Dumpify.Descriptors.ValueProviders;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Dumpify;

internal abstract class SpectreConsoleRendererBase : RendererBase<IRenderable, SpectreRendererState>
{
    protected SpectreConsoleRendererBase(ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<IRenderable>>> customTypeRenderers)
        : base(customTypeRenderers)
    {
    }

    protected override IRenderable RenderFailedValueReading(Exception ex, IValueProvider valueProvider, IDescriptor? descriptor, RenderContext<SpectreRendererState> context)
        => RenderSingleValue("[Failed to read value]", context, context.State.Colors.MetadataErrorColor);

    protected override IRenderedObject CreateRenderedObject(IRenderable rendered)
        => new SpectreConsoleRenderedObject(rendered);

    protected virtual IRenderable RenderSingleValue(object value, RenderContext<SpectreRendererState> context, Color? color)
        => new Markup(Markup.Escape(value.ToString() ?? ""), new Style(foreground: color));

    protected override IRenderable RenderSingleValueDescriptor(object obj, SingleValueDescriptor descriptor, RenderContext<SpectreRendererState> context)
    {
        var stringQuotationChar = context.Config.TypeRenderingConfig.StringQuotationChar;
        var charQuotationChar = context.Config.TypeRenderingConfig.CharQuotationChar;
        var renderValue = obj switch
        {
            string str when context.Config.TypeRenderingConfig.QuoteStringValues => $"{stringQuotationChar}{str}{stringQuotationChar}",
            char ch when context.Config.TypeRenderingConfig.QuoteCharValues => $"{charQuotationChar}{ch}{charQuotationChar}",
            _ => obj,
        };

        var singleValue = RenderSingleValue(renderValue, context, context.State.Colors.PropertyValueColor);

        if (context.Config.Label is { } label && context.CurrentDepth == 0 && (object.ReferenceEquals(context.RootObject, obj) || (context.RootObjectTransform is not null && object.ReferenceEquals(context.RootObjectTransform, obj))))
        {
            var builder = new ObjectTableBuilder(context, descriptor, obj);
            return builder
                .AddColumnName("Value")
                .AddRow(descriptor, obj, singleValue)
                .HideTitle()
                .HideHeaders()
                .Build();
        }

        return singleValue;
    }

    public override IRenderable RenderNullValue(IDescriptor? descriptor, RenderContext<SpectreRendererState> context)
        => RenderSingleValue("null", context, context.State.Colors.NullValueColor);

    public override IRenderable RenderExceededDepth(object obj, IDescriptor? descriptor, RenderContext<SpectreRendererState> context)
        => RenderSingleValue($"[Exceeded max depth {context.Config.MaxDepth}]", context, context.State.Colors.MetadataInfoColor);

    protected override IRenderable RenderCircularDependency(object @object, IDescriptor? descriptor, RenderContext<SpectreRendererState> context)
            => RenderSingleValue($"[Circular Reference #{context.Config.TypeNameProvider.GetTypeName(@object.GetType())}]", context, context.State.Colors.MetadataInfoColor);

    protected override IRenderable RenderNullDescriptor(object obj, RenderContext<SpectreRendererState> context)
            => RenderSingleValue($"[null descriptor] {obj}", context, context.State.Colors.MetadataErrorColor);

    protected override IRenderable RenderIgnoredDescriptor(object obj, IgnoredDescriptor descriptor, RenderContext<SpectreRendererState> context)
            => RenderSingleValue($"[Ignored {descriptor.Name}] {obj}", context, context.State.Colors.IgnoredValueColor);

    protected override IRenderable RenderUnsupportedDescriptor(object obj, IDescriptor descriptor, RenderContext<SpectreRendererState> context)
        => RenderSingleValue($"[Unsupported {descriptor.GetType().Name} for {descriptor.Name}]", context, context.State.Colors.MetadataErrorColor);

    protected override IRenderable RenderUnfamiliarCustomDescriptor(object obj, CustomDescriptor descriptor, RenderContext<SpectreRendererState> context)
        => RenderSingleValue($"[Unfamiliar descriptor {descriptor.GetType().Name} for {descriptor.Name}]", context, context.State.Colors.MetadataErrorColor);

    protected override SpectreRendererState CreateState(object? obj, IDescriptor? descriptor, RendererConfig config)
        => new SpectreRendererState(config);
}