using Dumpify.Descriptors;
using Dumpify.Descriptors.ValueProviders;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections.Concurrent;
using System.Text;

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

    /// <summary>
    /// Formats a ReferenceLabel into a display string.
    /// </summary>
    /// <param name="label">The structured reference label</param>
    /// <param name="prefix">The prefix to use (e.g., "Cycle" or "Identical")</param>
    /// <returns>Formatted string like "[Cycle #Person:1 → Manager]"</returns>
    private static string FormatReferenceLabel(ReferenceLabel label, string prefix)
    {
        var sb = new StringBuilder();
        sb.Append('[');
        sb.Append(prefix);

        if (label.TypeName != null)
        {
            sb.Append(" #");
            sb.Append(label.TypeName);
        }

        if (label.Id != null)
        {
            sb.Append(':');
            sb.Append(label.Id);
        }

        // Show path if requested - use "(root)" for empty path (root object)
        if (label.Path != null)
        {
            sb.Append(" → ");
            sb.Append(string.IsNullOrEmpty(label.Path) ? "(root)" : label.Path);
        }

        sb.Append(']');
        return sb.ToString();
    }

    protected override IRenderable RenderCircularDependency(object @object, IDescriptor? descriptor, ReferenceLabel label, RenderContext<SpectreRendererState> context)
    {
        var text = FormatReferenceLabel(label, "Cycle");
        return RenderSingleValue(text, context, context.State.Colors.MetadataInfoColor);
    }

    protected override IRenderable RenderSharedReference(object @object, IDescriptor? descriptor, ReferenceLabel label, RenderContext<SpectreRendererState> context)
    {
        var text = FormatReferenceLabel(label, "Identical");
        return RenderSingleValue(text, context, context.State.Colors.MetadataInfoColor);
    }

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
