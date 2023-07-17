using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Globalization;

namespace Dumpify;

internal class TimeTypesRenderer : ICustomTypeRenderer<IRenderable>
{
    public Type DescriptorType => typeof(CustomDescriptor);

    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;
    private readonly Dictionary<RuntimeTypeHandle, Func<IDescriptor, object, RenderContext<SpectreRendererState>, object?, IRenderable>> _typeHandlers;

    public TimeTypesRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
    {
        _handler = handler;

        _typeHandlers = new()
        {
            [typeof(DateTime).TypeHandle] = RenderDateTime,
            [typeof(TimeSpan).TypeHandle] = RenderTimeSpan,
            [typeof(DateTimeOffset).TypeHandle] = RenderDateTimeOffset,

#if NET6_0_OR_GREATER
            [typeof(DateOnly).TypeHandle] = RenderDateOnly,
            [typeof(TimeOnly).TypeHandle] = RenderTimeOnly,
#endif
        };
    }

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        return _typeHandlers[descriptor.Type.TypeHandle](descriptor, obj, context, handleContext);
    }

    private IRenderable RenderDateTime(IDescriptor descriptor, object obj, RenderContext<SpectreRendererState> context, object? handleContext)
        => RenderCustomTimingType(descriptor, obj, context, handleContext, datetime => ((DateTime)datetime).ToString(CultureInfo.CurrentCulture));

    private IRenderable RenderTimeSpan(IDescriptor descriptor, object obj, RenderContext<SpectreRendererState> context, object? handleContext)
        => RenderCustomTimingType(descriptor, obj, context, handleContext, timespan => ((TimeSpan)timespan).ToString());

    private IRenderable RenderDateTimeOffset(IDescriptor descriptor, object obj, RenderContext<SpectreRendererState> context, object? handleContext)
        => RenderCustomTimingType(descriptor, obj, context, handleContext, datetimeOffset => ((DateTimeOffset)datetimeOffset).ToString());

#if NET6_0_OR_GREATER
    private IRenderable RenderDateOnly(IDescriptor descriptor, object obj, RenderContext<SpectreRendererState> context, object? handleContext)
        => RenderCustomTimingType(descriptor, obj, context, handleContext, dateOnly => ((DateOnly)dateOnly).ToString());

    private IRenderable RenderTimeOnly(IDescriptor descriptor, object obj, RenderContext<SpectreRendererState> context, object? handleContext)
        => RenderCustomTimingType(descriptor, obj, context, handleContext, timeOnly => ((TimeOnly)timeOnly).ToString());
#endif

    private IRenderable RenderCustomTimingType(IDescriptor descriptor, object obj, RenderContext<SpectreRendererState> context, object? handleContext, Func<object, string> customValueProvider)
    {
        var valueColor = context.Config.ColorConfig.PropertyValueColor?.HexString ?? "default";

        var customValue = customValueProvider(obj);
        var markupValue = $"[[[{valueColor}]{customValue.EscapeMarkup()}[/]]]";

        var markup = new Markup(markupValue);

        if (context.Config.Label is { } label && context.CurrentDepth == 0 && object.ReferenceEquals(context.RootObject, obj))
        {
            var table = new ObjectTableBuilder(context, descriptor, obj)
                .AddColumnName("Values")
                .AddRow(descriptor, obj, markup)
                .HideTitle()
                .HideHeaders()
                .Build();

            return table;
        }

        return markup;
    }

    public (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj)
        => (_typeHandlers.ContainsKey(descriptor.Type.TypeHandle), null);
}