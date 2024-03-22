using Dumpify.Descriptors;
using Dumpify.Descriptors.ValueProviders;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

internal class ArrayTypeRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;

    public ArrayTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
    {
        _handler = handler;
    }

    public Type DescriptorType { get; } = typeof(MultiValueDescriptor);

    public (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type.IsArray && ((Array)obj).Rank < 3, null);

    public IRenderable Render(IDescriptor descriptor, object @object, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        var mvd = (MultiValueDescriptor)descriptor;
        var obj = (Array)@object;

        return obj.Rank switch
        {
            1 => RenderSingleDimensionArray(obj, mvd, context),
            2 => RenderTwoDimensionalArray(obj, mvd, context),
            > 2 => RenderHighRankArrays(obj, mvd, context),
            _ => throw new NotImplementedException($"Rendering Array of rank {obj.Rank}")
        };
    }

    private IRenderable RenderSingleDimensionArray(Array obj, MultiValueDescriptor mvd, RenderContext<SpectreRendererState> context)
    {
        var builder = new ObjectTableBuilder(context, mvd, obj)
            .HideTitle();

        var showIndexes = context.Config.TableConfig.ShowArrayIndices;

        RowIndicesTableBuilderBehavior? rowIndicesBehavior = showIndexes ? new() : null;
        if (rowIndicesBehavior != null)
        {
            builder.AddBehavior(rowIndicesBehavior);
        }

        var elementName = mvd.ElementsType is null ? "" : context.Config.TypeNameProvider.GetTypeName(mvd.ElementsType);
        builder.AddColumnName($"{elementName}[{obj.GetLength(0)}]", new Style(foreground: context.Config.ColorConfig.TypeNameColor.ToSpectreColor()));

        if (context.Config.TableConfig.ShowTableHeaders is not true || context.Config.TypeNamingConfig.ShowTypeNames is not true)
        {
            builder.HideHeaders();
        }

        int maxCollectionCount = context.Config.TableConfig.MaxCollectionCount;
        int length = obj.Length > maxCollectionCount ? maxCollectionCount : obj.Length;

        for (var index = 0; index < length; ++index)
        {
            var item = obj.GetValue(index);

            //var type = mvd.ElementsType ?? item?.GetType();
            var type =  item?.GetType() ?? mvd.ElementsType;
            IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null, context.Config.MemberProvider) : null;

            var renderedItem = _handler.RenderDescriptor(item, itemsDescriptor, context);

            builder.AddRow(itemsDescriptor, item, renderedItem);
        }

        if(obj.Length > maxCollectionCount)
        {
            if (rowIndicesBehavior != null)
            {
                rowIndicesBehavior.AddHideIndexForRow(maxCollectionCount);
            }

            string truncatedNotificationText = $"... truncated {obj.Length - maxCollectionCount} items";
            var labelDescriptor = new LabelDescriptor(typeof(string), null);
            var renderedItem = _handler.RenderDescriptor(truncatedNotificationText, labelDescriptor, context);
            builder.AddRow(labelDescriptor, truncatedNotificationText, renderedItem);
        }

        return builder.Build();
    }

    private IRenderable RenderTwoDimensionalArray(Array obj, MultiValueDescriptor descriptor, RenderContext baseContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;

        if (obj.Rank != 2)
        {
            return RenderHighRankArrays(obj, descriptor, context);
        }

        RowIndicesTableBuilderBehavior rowIndicesBehavior = new ();
        var builder = new ObjectTableBuilder(context, descriptor, obj)
            .HideTitle()
            .AddBehavior(rowIndicesBehavior);

        var rowsAll = obj.GetLength(0);
        var columnsAll = obj.GetLength(1);

        int maxCollectionCount = context.Config.TableConfig.MaxCollectionCount;
        int rows = rowsAll > maxCollectionCount ? maxCollectionCount : rowsAll;
        int columns = columnsAll > maxCollectionCount ? maxCollectionCount : columnsAll;

        var colorConfig = context.Config.ColorConfig;

        if (context.Config.TypeNamingConfig.ShowTypeNames is true)
        {
            var (typeName, rank) = context.Config.TypeNameProvider.GetJaggedArrayNameWithRank(descriptor.Type);
            builder.SetTitle($"{typeName}[{rowsAll},{columnsAll}]");
        }

        var columnStyle = new Style(foreground: colorConfig.ColumnNameColor.ToSpectreColor());
        for (var col = 0; col < columns; ++col)
        {
            builder.AddColumnName(col.ToString(), columnStyle);
        }

        if (columnsAll > maxCollectionCount)
        {
            builder.AddColumnName($"... +{columnsAll - maxCollectionCount}", columnStyle);
        }

        for (var row = 0; row < rows; ++row)
        {
            var cells = new List<IRenderable>(2);

            for (var col = 0; col < columns; ++col)
            {
                var item = obj.GetValue(row, col);

                var type = descriptor.ElementsType ?? item?.GetType();
                IDescriptor? itemsDescriptor = type is not null ? DumpConfig.Default.Generator.Generate(type, null, context.Config.MemberProvider) : null;

                var renderedItem = _handler.RenderDescriptor(item, itemsDescriptor, context);
                cells.Add(renderedItem);
            }

            if (columnsAll > maxCollectionCount)
            {
                var labelDescriptor = new LabelDescriptor(typeof(string), null);
                var renderedItem = _handler.RenderDescriptor(string.Empty, labelDescriptor, context);
                cells.Add(renderedItem);
            }

            builder.AddRow(null, null, cells);
        }

        if (rowsAll > maxCollectionCount)
        {
            var cells = new List<IRenderable>(2);
            var labelDescriptor = new LabelDescriptor(typeof(string), null);

            for (int i = 0; i <= columns; i++)
            {
                var renderedCell = _handler.RenderDescriptor(string.Empty, labelDescriptor, context);
                cells.Add(renderedCell);
            }

            var truncatedNotificationText = $"... +{rowsAll - maxCollectionCount}";
            rowIndicesBehavior.AddHideIndexForRow(maxCollectionCount, truncatedNotificationText);

            builder.AddRow(null, null, cells);
        }

        return builder.Build();
    }

    private IRenderable RenderHighRankArrays(Array arr, MultiValueDescriptor descriptor, RenderContext context)
        => _handler.RenderDescriptor(arr, descriptor, (RenderContext<SpectreRendererState>)context);
}