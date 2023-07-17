using Dumpify.Descriptors;
using Dumpiyf;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections;
using System.Collections.Concurrent;

namespace Dumpify;

internal class SpectreConsoleTableRenderer : SpectreConsoleRendererBase
{
    public SpectreConsoleTableRenderer()
        : base(new ConcurrentDictionary<RuntimeTypeHandle, IList<ICustomTypeRenderer<IRenderable>>>())
    {
        AddCustomTypeDescriptor(new DictionaryTypeRenderer(this));
        AddCustomTypeDescriptor(new ArrayTypeRenderer(this));
        AddCustomTypeDescriptor(new TupleTypeRenderer(this));
        AddCustomTypeDescriptor(new EnumTypeRenderer(this));
        AddCustomTypeDescriptor(new DataTableTypeRenderer(this));
        AddCustomTypeDescriptor(new DataSetTypeRenderer(this));
        AddCustomTypeDescriptor(new SystemReflectionTypeRenderer(this));
        AddCustomTypeDescriptor(new TimeTypesRenderer(this));
        AddCustomTypeDescriptor(new GuidTypeRenderer(this));
    }

    protected override IRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext<SpectreRendererState> context)
        => RenderIEnumerable((IEnumerable)obj, descriptor, context);

    private IRenderable RenderIEnumerable(IEnumerable obj, MultiValueDescriptor descriptor, RenderContext<SpectreRendererState> context)
    {
        var builder = new ObjectTableBuilder(context, descriptor, obj)
            .HideTitle();

        var typeName = GetTypeName(descriptor.Type);

        builder.AddColumnName(typeName + "", new Style(foreground: context.State.Colors.TypeNameColor));

        foreach (var item in obj)
        {
            var type = descriptor.ElementsType ?? item?.GetType();

            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null, context.Config.MemberProvider) : null;

            var renderedItem = RenderDescriptor(item, itemsDescriptor, context);
            builder.AddRow(itemsDescriptor, item, renderedItem);
        }

        return builder.Build();

        string GetTypeName(Type type)
        {
            if (!type.IsArray)
            {
                return context.Config.TypeNameProvider.GetTypeName(type);
            }

            var (name, rank) = context.Config.TypeNameProvider.GetJaggedArrayNameWithRank(type);

            return $"{name}[{new string(',', rank + 1)}]";
        }
    }

    protected override IRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext<SpectreRendererState> context)
    {
        var builder = new ObjectTableBuilder(context, descriptor, obj);
        builder.AddDefaultColumnNames();

        foreach (var property in descriptor.Properties)
        {
            var renderedValue = GetValueAndRender(obj, property.ValueProvider!, property, context with { CurrentDepth = context.CurrentDepth + 1 });
            builder.AddRowWithTypeName(property, obj, renderedValue);
        }

        return builder.Build();
    }
}