using Dumpify.Descriptors;
using Dumpify.Extensions;
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

        int maxCollectionCount = context.Config.TableConfig.MaxCollectionCount;
        int length = pairs.Count > maxCollectionCount ? maxCollectionCount : pairs.Count;

        foreach (var pair in pairs.Take(length))
        {
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

        if (pairs.Count > maxCollectionCount)
        {
            string truncatedNotificationText = $"... truncated {pairs.Count - maxCollectionCount} items";

            var labelDescriptor = new LabelDescriptor(typeof(string), null);
            var renderedValue = _handler.RenderDescriptor(truncatedNotificationText, labelDescriptor, context);
            var keyRenderable = _handler.RenderDescriptor(string.Empty, labelDescriptor, context);
            tableBuilder.AddRow(labelDescriptor, truncatedNotificationText, keyRenderable, renderedValue);
        }

        return tableBuilder.Build();
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