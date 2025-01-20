using Dumpify.Descriptors;
using Spectre.Console.Rendering;
using System.Runtime.CompilerServices;

namespace Dumpify;

internal class TupleTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;

    public Type DescriptorType { get; } = typeof(ObjectDescriptor);

    public TupleTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
        => _handler = handler;

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        var tableBuilder = new ObjectTableBuilder(context, descriptor, obj);
        tableBuilder.SetColumnNames("Item", "Value");

        var tuple = (ITuple)obj;

        //var genericArguments = descriptor.Type.GetGenericArguments();
        var genericArguments = GetGenericArgumentExpansions(descriptor.Type.GetGenericArguments(), context).ToArray();

        var memberProvider = context.Config.MemberProvider;
        for (var index = 0; index < tuple.Length; ++index)
        {
            var value = tuple[index];
            var type = genericArguments[index];

            var typeDescriptor = DumpConfig.Default.Generator.Generate(type, null, context.Config.MemberProvider);

            var renderedValue = value switch
            {
                null => _handler.RenderNullValue(typeDescriptor, context),
                not null => _handler.RenderDescriptor(value, typeDescriptor, context),
            };

            tableBuilder.AddRow(typeDescriptor, value, $"Item{index + 1}", renderedValue);
        }

        return tableBuilder.Build();
    }

    private static IEnumerable<Type> GetGenericArgumentExpansions(Type[] genericArguments, RenderContext<SpectreRendererState> context)
    {
        var expansion = genericArguments;
        var index = 0;

        while (index < expansion.Length)
        {
            var type = expansion[index];
            if(IsValueTuple(type))
            {
                var descriptor = DumpConfig.Default.Generator.Generate(type, null, context.Config.MemberProvider)!;
                expansion = descriptor.Type.GetGenericArguments();
                index = 0;
                continue;
            }

            yield return type;
            ++index;
        }
    }

    private static bool IsValueTuple(Type type)
    {
        if (type is null)
        {
            return false;
        }

        if (type == typeof(ValueTuple))
        {
            return true;
        }

        if (type.IsGenericType)
        {
            Type genericDefinition = type.GetGenericTypeDefinition();

            return genericDefinition == typeof(ValueTuple<>) ||
                   genericDefinition == typeof(ValueTuple<,>) ||
                   genericDefinition == typeof(ValueTuple<,,>) ||
                   genericDefinition == typeof(ValueTuple<,,,>) ||
                   genericDefinition == typeof(ValueTuple<,,,,>) ||
                   genericDefinition == typeof(ValueTuple<,,,,,>) ||
                   genericDefinition == typeof(ValueTuple<,,,,,,>) ||
                   genericDefinition == typeof(ValueTuple<,,,,,,,>);
        }

        return false;
    }

    public (bool, object?) ShouldHandle(IDescriptor descriptor, object obj)
        => (obj is ITuple, null);
}