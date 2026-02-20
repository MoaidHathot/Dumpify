using Dumpify.Descriptors;
using Dumpify.Extensions;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections;
using System.Reflection;

namespace Dumpify;

internal class DictionaryTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;

    public Type DescriptorType { get; } = typeof(MultiValueDescriptor);

    public DictionaryTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
    {
        _handler = handler;
    }

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        handleContext.MustNotBeNull();

        var tableBuilder = new ObjectTableBuilder(context, descriptor, obj);
        tableBuilder.SetColumnNames("Key", "Value");

        Type? valueType = null;

        var pairs = ((IEnumerable<(object? key, object? value)>)handleContext!).ToList();

        // Use centralized truncation
        var truncated = CollectionTruncator.Truncate(
            pairs,
            context.Config.TruncationConfig);

        // Render start marker if present (for Tail or HeadAndTail modes)
        if (truncated.StartMarker is { } startMarker)
        {
            var markerRenderable = RenderTruncationMarker(startMarker, context);
            var emptyRenderable = new Markup("");
            tableBuilder.AddRow(null, null, emptyRenderable, markerRenderable);
        }

        // Render items with middle marker support
        for (int i = 0; i < truncated.Items.Count; i++)
        {
            // Insert middle marker at the appropriate position (for HeadAndTail mode)
            if (truncated.MiddleMarkerIndex == i && truncated.MiddleMarker is { } middleMarker)
            {
                var markerRenderable = RenderTruncationMarker(middleMarker, context);
                var emptyRenderable = new Markup("");
                tableBuilder.AddRow(null, null, emptyRenderable, markerRenderable);
            }

            var pair = truncated.Items[i];

            var keyType = pair.key?.GetType();
            var keyDescriptor = keyType is null ? null : DumpConfig.Default.Generator.Generate(keyType, null, context.Config.MemberProvider);
            var keyRenderable = _handler.RenderDescriptor(pair.key, keyDescriptor, context);

            var value = pair.value;

            if (value is not null && valueType is null)
            {
                valueType = value.GetType();
            }

            (IDescriptor? valueDescriptor, IRenderable renderedValue) = value switch
            {
                null => (null, _handler.RenderNullValue(null, context)),
                not null => GetDescriptorAndRender(value, context),
            };

            tableBuilder.AddRow(valueDescriptor, value, keyRenderable, renderedValue);
        }

        // Render end marker if present (for Head mode)
        if (truncated.EndMarker is { } endMarker)
        {
            var markerRenderable = RenderTruncationMarker(endMarker, context);
            var emptyRenderable = new Markup("");
            tableBuilder.AddRow(null, null, emptyRenderable, markerRenderable);
        }

        return tableBuilder.Build();
    }

    private IRenderable RenderTruncationMarker(TruncationMarker marker, RenderContext<SpectreRendererState> context)
    {
        var color = context.State.Colors.MetadataInfoColor;
        return new Markup(Markup.Escape(marker.GetDefaultMessage()), new Style(foreground: color));
    }

    private (IDescriptor? descriptor, IRenderable renderedValue) GetDescriptorAndRender(object value, RenderContext<SpectreRendererState> context)
    {
        var descriptor = DumpConfig.Default.Generator.Generate(value.GetType(), null, context.Config.MemberProvider);
        var rendered = _handler.RenderDescriptor(value, descriptor, context);

        return (descriptor, rendered);
    }

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
    {
        if (obj is IDictionary map)
        {
            var list = new List<(object? key, object? value)>(map.Count);

            foreach (var key in map.Keys)
            {
                list.Add((key, map[key]));
            }

            return (true, list);
        }

        var nameProvider = new TypeNameProvider(false, true, false, false);

        var enumerableInterfaces = descriptor.Type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        foreach (var i in enumerableInterfaces)
        {
            var genericArgument = i.GetGenericArguments()[0];

            if (genericArgument.IsGenericType)
            {
                if (genericArgument.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    var method = i.GetMethod("GetEnumerator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
                    var enumerator = (IEnumerator)method.Invoke(obj, null)!;

                    var (canHandle, propertyProvider) = GetItemProvider(obj, genericArgument, i, enumerator);

                    if (canHandle is not true)
                    {
                        return (false, default);
                    }

                    var list = new List<(object? key, object? value)>();

                    while (enumerator.MoveNext())
                    {
                        var item = propertyProvider!.Invoke(enumerator);
                        var itemType = item!.GetType();

                        var keyProperty = itemType.GetProperty("Key", BindingFlags.Instance | BindingFlags.Public)!;
                        var key = keyProperty.GetValue(item);

                        var valueProperty = itemType.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public)!;
                        var value = valueProperty.GetValue(item);

                        list.Add((key, value));
                    }

                    return (true, list);
                }
            }
        }

        return (false, null);
    }

    private static (bool canHandle, Func<object, object>? currentValueProvider) GetItemProvider(object? obj, Type genericArgument, Type enumerableInterface, IEnumerator enumerator)
    {
        var currentProperty = enumerator.GetType().GetProperty("Current", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        //Notice how the if also checks for `currentProperty` nullability
        if (currentProperty?.PropertyType.IsGenericType is true && currentProperty.PropertyType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
        {
            return (true, enumeratorSource => currentProperty.GetValue(enumeratorSource)!);
        }

        //Should we "fail" gracefully without special rendering and never knowing an issue has happened, or should we really fail and indicate via a rendering failure?
        if (2 != genericArgument.GenericTypeArguments.Length)
        {
            return (false, default);
        }

        var nameProvider = new TypeNameProvider(false, true, false, false);

        var currentPropertyExternalImplementationName = $"System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<{nameProvider.GetTypeName(genericArgument.GenericTypeArguments[0])},{genericArgument.GenericTypeArguments[1]}>>.Current";
        currentProperty = enumerator.GetType().GetProperty(currentPropertyExternalImplementationName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        return (currentProperty is not null, enumerableSource => currentProperty!.GetValue(enumerableSource)!);
    }
}
