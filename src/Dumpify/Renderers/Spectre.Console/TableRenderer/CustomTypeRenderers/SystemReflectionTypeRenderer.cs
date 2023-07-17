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

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        var typeColor = context.Config.ColorConfig.TypeNameColor?.HexString ?? "default";
        var metadataColor = context.Config.ColorConfig.MetadataInfoColor?.HexString ?? "default";
        var propertyValueColor = context.Config.ColorConfig.PropertyValueColor?.HexString ?? "default";

        var nameProvider = context.Config.TypeNameProvider;

        if (obj is Type type)
        {
            var typeName = nameProvider.GetTypeName((Type)obj).EscapeMarkup();
            var c = $"[{metadataColor}]typeof([/][{typeColor}]{Markup.Escape(typeName)}[/][{metadataColor}])[/]";
            return RenderMarkup(new Markup(c), context, descriptor, obj);
        }

        if (obj is PropertyInfo property)
        {
            var typeName = nameProvider.GetTypeName(property.PropertyType).EscapeMarkup();
            var propertyDescription = $"[{typeColor}]{typeName}[/] [{propertyValueColor}]{property.Name.EscapeMarkup()}[/] {{ {(property.SetMethod is not null ? $"[{metadataColor}]set[/]; " : "")}{(property.GetMethod is not null ? $"[{metadataColor}]get[/]; " : "")}}}";
            return RenderMarkup(new Markup(propertyDescription), context, descriptor, obj);
        }

        if (obj is ConstructorInfo ctor)
        {
            var argumentList = string.Join(", ", ctor.GetParameters().Select(p => $"[{typeColor}]{nameProvider.GetTypeName(p.ParameterType).EscapeMarkup()}[/] {p.Name.EscapeMarkup()}"));
            var ctorTypeName = ctor.DeclaringType is not null ? nameProvider.GetTypeName(ctor.DeclaringType) : ctor.Name;
            var ctorDescription = $"[{metadataColor}]ctor[/] [{typeColor}]{ctorTypeName.EscapeMarkup()}[/]({argumentList})";
            return RenderMarkup(new Markup(ctorDescription), context, descriptor, obj);
        }

        if (obj is MethodInfo method)
        {
            var argumentList = string.Join(", ", method.GetParameters().Select(p => $"[{typeColor}]{nameProvider.GetTypeName(p.ParameterType).EscapeMarkup()}[/] {p.Name.EscapeMarkup()}"));
            var methodDescription = $"[{typeColor}]{nameProvider.GetTypeName(method.ReturnType).EscapeMarkup()}[/] [{propertyValueColor}]{method.Name.EscapeMarkup()}[/]({argumentList})";
            return RenderMarkup(new Markup(methodDescription), context, descriptor, obj);
        }

        if (obj is FieldInfo field)
        {
            var typeName = context.Config.TypeNameProvider.GetTypeName(field.FieldType).EscapeMarkup();
            var fieldDescription = $"[{typeColor}]{typeName}[/] [{propertyValueColor}]{field.Name.EscapeMarkup()}[/];";
            return RenderMarkup(new Markup(fieldDescription), context, descriptor, obj);
        }

        var formattedTypeName = nameProvider.GetTypeName(obj.GetType());
        return RenderMarkup(GeneralMarkup(formattedTypeName, context), context, descriptor, obj);
    }

    private IRenderable RenderMarkup(Markup markup, RenderContext<SpectreRendererState> context, IDescriptor descriptor, object sourceObj)
    {
        if (context.Config.Label is { } label && context.CurrentDepth == 0 && object.ReferenceEquals(context.RootObject, sourceObj))
        {
            var table = new ObjectTableBuilder(context, descriptor, sourceObj)
                .AddColumnName("Values")
                .AddRow(descriptor, sourceObj, markup)
                .HideTitle()
                .HideHeaders()
                .Build();

            return table;
        }

        return markup;
    }

    private Markup GeneralMarkup(string interpolated, RenderContext context)
    {
        var typeColor = context.Config.ColorConfig.TypeNameColor?.HexString ?? "default";
        var escaped = interpolated.EscapeMarkup();
        var markup = $"[{typeColor}]{escaped}[/]";

        return new Markup(markup);
    }

    public (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj)
        => (obj is Type || descriptor.Type.FullName == "System.RuntimeType" || (descriptor.Type.Namespace?.StartsWith("System.Reflection") ?? false) || obj is MemberInfo, null);
}