using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

internal class LazyTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler) : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler = handler;

    public Type DescriptorType => typeof(CustomDescriptor);

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;
        
        var (isValueCreated, value) = GetLazyState(obj);

        var tableBuilder = new ObjectTableBuilder(context, descriptor, obj)
            .AddDefaultColumnNames();

        // Render IsValueCreated row
        var isValueCreatedDescriptor = DumpConfig.Default.Generator.Generate(typeof(bool), null, context.Config.MemberProvider);
        var isValueCreatedRendered = _handler.RenderDescriptor(isValueCreated, isValueCreatedDescriptor, context);

        tableBuilder.AddRow(isValueCreatedDescriptor, isValueCreated, nameof(Lazy<>.IsValueCreated), isValueCreatedRendered);

        if (isValueCreated)
        {
            if (value is null)
            {
                tableBuilder.AddRow(null, null, nameof(Lazy<>.Value), _handler.RenderNullValue(null, context));
            }
            else
            {
                var valueDescriptor = DumpConfig.Default.Generator.Generate(value.GetType(), null, context.Config.MemberProvider);
                var valueRendered = _handler.RenderDescriptor(value, valueDescriptor, context);
                tableBuilder.AddRow(valueDescriptor, value, nameof(Lazy<>.Value), valueRendered);
            }
        }
        else
        {
            var notEvaluatedMarkup = new Markup(Markup.Escape("[Not Evaluated]"), new Style(foreground: context.State.Colors.MetadataInfoColor));
            tableBuilder.AddRow(null, null, nameof(Lazy<>.Value), notEvaluatedMarkup);
        }

        return tableBuilder.Build();
    }

    private static (bool isValueCreated, object? value) GetLazyState(object lazyObj)
    {
        var type = lazyObj.GetType();
        var isValueCreated = (bool)type.GetProperty(nameof(Lazy<>.IsValueCreated))!.GetValue(lazyObj)!;
        
        if (!isValueCreated)
        {
            return (false, null);
        }

        var value = type.GetProperty(nameof(Lazy<>.Value))!.GetValue(lazyObj);
        return (true, value);
    }

    public (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj)
    {
        var type = descriptor.Type;
        var isLazy = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>);
        return (isLazy, null);
    }
}