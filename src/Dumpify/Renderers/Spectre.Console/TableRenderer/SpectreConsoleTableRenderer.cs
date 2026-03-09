using Dumpify.Descriptors;
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
        AddCustomTypeDescriptor(new LazyTypeRenderer(this));
        AddCustomTypeDescriptor(new TaskTypeRenderer(this));
    }

    protected override IRenderable RenderMultiValueDescriptor(object obj, MultiValueDescriptor descriptor, RenderContext<SpectreRendererState> context)
        => RenderIEnumerable((IEnumerable)obj, descriptor, context);

    private IRenderable RenderIEnumerable(IEnumerable obj, MultiValueDescriptor descriptor, RenderContext<SpectreRendererState> context)
    {
        // Resolve the layout strategy for the element type
        // Pass isCollectionElement=true and the container type so rules can check if rendering inside a collection
        var (strategy, layoutResult) = TableLayoutResolver.Resolve(
            descriptor.ElementsType,
            context,
            isCollectionElement: true,
            containerType: descriptor.Type);

        var builder = new ObjectTableBuilder(context, descriptor, obj);

        // Add row indices behavior if needed
        // Horizontal layout shows row indices by default, vertical does not
        // But explicit ShowRowIndices setting overrides the default
        if (TableLayoutResolver.ShouldShowRowIndices(context, strategy, layoutResult))
        {
            builder.AddBehavior(new RowIndicesTableBuilderBehavior());
        }

        // Use the centralized truncation utility
        var truncated = CollectionTruncator.Truncate(
            obj.Cast<object?>(),
            context.Config.TruncationConfig);

        // Delegate to the strategy
        strategy.ConfigureCollectionTable(
            builder,
            truncated,
            descriptor,
            context,
            (item, itemDescriptor, ctx) => RenderDescriptor(item, itemDescriptor, ctx),
            marker => RenderTruncationMarker(marker, context));

        return builder.Build();
    }

    protected override IRenderable RenderObjectDescriptor(object obj, ObjectDescriptor descriptor, RenderContext<SpectreRendererState> context)
    {
        // Check if there are no members to render
        if (!descriptor.Properties.Any())
        {
            var emptyBuilder = new ObjectTableBuilder(context, descriptor, obj);
            emptyBuilder.AddColumnName(" ");
            emptyBuilder.HideHeaders();
            emptyBuilder.AddRow(descriptor, obj, RenderNoMembers(context));
            return emptyBuilder.Build();
        }

        // Resolve the layout strategy for the object type
        var (strategy, layoutResult) = TableLayoutResolver.Resolve(descriptor.Type, context);

        var builder = new ObjectTableBuilder(context, descriptor, obj);

        // Add member types behavior if needed
        if (TableLayoutResolver.ShouldShowMemberTypes(context, layoutResult))
        {
            builder.AddBehavior(new RowTypeTableBuilderBehavior());
        }

        // Delegate to the strategy
        strategy.ConfigureObjectTable(
            builder,
            obj,
            descriptor,
            context,
            (source, property, ctx) => GetValueAndRender(source, property.ValueProvider!, property, ctx));

        return builder.Build();
    }
}
