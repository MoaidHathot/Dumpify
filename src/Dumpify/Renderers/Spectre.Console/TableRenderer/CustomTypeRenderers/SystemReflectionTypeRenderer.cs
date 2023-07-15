using Dumpify;
using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Reflection;

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
        var metadataColor = context.Config.ColorConfig.MetadataInfoColor?.HexString ?? "default";
        var propertyValueColor = context.Config.ColorConfig.PropertyValueColor?.HexString ?? "default";

        if (obj is PropertyInfo property)
        {
            var typeName = context.Config.TypeNameProvider.GetTypeName(property.PropertyType).EscapeMarkup();
            var propertyDescription = $"[{typeColor}]{typeName}[/] [{propertyValueColor}]{property.Name.EscapeMarkup()}[/] {{ {(property.SetMethod is not null ? $"[{metadataColor}]set[/]; " : "")}{(property.GetMethod is not null ? $"[{metadataColor}]get[/]; " : "")}}}";
            return new Markup(propertyDescription);
            //return GeneralMarkup(propertyDescription, context);
        }

        if (obj is ConstructorInfo ctor)
        {
            var ctorDescription = $"Constructor {ctor.Name}({string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType} {p.Name}"))})".EscapeMarkup();
            return GeneralMarkup(ctorDescription, context);
        }

        if (obj is MethodInfo method)
        {

            var methodDescription = $"{method.ReturnType.Name} {method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType} {p.Name}"))})";
            return GeneralMarkup(methodDescription, context);
        }

        if (obj is FieldInfo field)
        {
            var typeName = context.Config.TypeNameProvider.GetTypeName(field.FieldType).EscapeMarkup();
            var fieldDescription = $"[{typeColor}]{typeName}[/] [{propertyValueColor}]{field.Name.EscapeMarkup()}[/];";
            return new Markup(fieldDescription);
        }

        var formattedTypeName = context.Config.TypeNameProvider.GetTypeName(obj.GetType());
        return GeneralMarkup(formattedTypeName, context);
    }

    private Markup GeneralMarkup(string interpolated, RenderContext context)
    {
        var typeColor = context.Config.ColorConfig.TypeNameColor?.HexString ?? "default";
        var escaped = interpolated.EscapeMarkup();
        var markup = $"[{typeColor}]{escaped}[/]";

        return new Markup(markup);
    }

    public (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type.Namespace?.StartsWith("System.Reflection") ?? false || obj is MemberInfo, null);
}
