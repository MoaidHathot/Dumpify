using Dumpify.Descriptors;
using Dumpify.Descriptors.ValueProviders;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

/// <summary>
/// Horizontal table layout strategy - LinqPad-style layout.
/// - Single object: Properties as columns, single value row
/// - Collection: Properties as columns, items as rows
/// </summary>
internal class HorizontalTableLayoutStrategy : ITableLayoutStrategy
{
    /// <summary>
    /// Horizontal layout shows row indices by default.
    /// </summary>
    public bool DefaultShowRowIndices => true;

    public void ConfigureObjectTable(
        ObjectTableBuilder builder,
        object obj,
        ObjectDescriptor descriptor,
        RenderContext<SpectreRendererState> context,
        Func<object, IDescriptor, RenderContext<SpectreRendererState>, (bool success, object? value, IRenderable rendered)> getValueAndRender)
    {
        var properties = descriptor.Properties.ToList();

        // Add columns for each property
        foreach (var property in properties)
        {
            builder.AddColumnName(property.Name);
        }

        // Add single row with all property values
        var rowCells = new List<IRenderable>();
        foreach (var property in properties)
        {
            var (success, value, renderedValue) = getValueAndRender(obj, property, context with { CurrentDepth = context.CurrentDepth + 1 });
            rowCells.Add(renderedValue);
        }

        builder.AddRow(descriptor, obj, rowCells);
    }

    public void ConfigureCollectionTable(
        ObjectTableBuilder builder,
        TruncatedEnumerable<object?> items,
        MultiValueDescriptor descriptor,
        RenderContext<SpectreRendererState> context,
        Func<object?, IDescriptor?, RenderContext<SpectreRendererState>, IRenderable> renderItem,
        Func<TruncationMarker, IRenderable> renderMarker)
    {
        var elementType = descriptor.ElementsType;

        // Check if we can use horizontal layout (element type has properties)
        if (elementType == null || !CanUseHorizontalLayout(elementType, context))
        {
            // Fall back to vertical layout for primitives/unknown types
            new VerticalTableLayoutStrategy().ConfigureCollectionTable(
                builder, items, descriptor, context, renderItem, renderMarker);
            return;
        }

        // Get the object descriptor for the element type
        var elementDescriptor = DumpConfig.Default.Generator.Generate(elementType, null, context.Config.MemberProvider) as ObjectDescriptor;

        if (elementDescriptor == null || !elementDescriptor.Properties.Any())
        {
            // Fall back to vertical layout
            new VerticalTableLayoutStrategy().ConfigureCollectionTable(
                builder, items, descriptor, context, renderItem, renderMarker);
            return;
        }

        // Set the table title to show the collection type and count
        var typeName = GetCollectionTypeName(descriptor.Type, items.TotalCount, context);
        builder.SetTitle(typeName);

        // Hide headers if configured
        if (!context.Config.TableConfig.ShowTableHeaders)
        {
            builder.HideHeaders();
        }

        // Apply MemberFilter to determine which properties to show as columns
        var properties = ApplyMemberFilter(elementDescriptor.Properties, items, context).ToList();

        if (properties.Count == 0)
        {
            // No properties after filtering, render "no members" message
            builder.AddColumnName(" ");
            builder.HideHeaders();
            var noMembersMessage = new Markup(Markup.Escape("[No members]"), new Style(foreground: context.State.Colors.MetadataInfoColor));
            builder.AddMarkerRow(new[] { noMembersMessage });
            return;
        }

        // Add columns for each property
        foreach (var property in properties)
        {
            builder.AddColumnName(property.Name);
        }

        // Render items
        items.ForEachWithMarkers(
            onMarker: marker =>
            {
                // For markers, add a row with the marker in first cell and empty cells for rest
                var markerCells = new List<IRenderable>();
                var renderedMarker = renderMarker(marker);
                markerCells.Add(renderedMarker);

                // Fill remaining cells with empty markup
                for (int i = 1; i < properties.Count; i++)
                {
                    markerCells.Add(new Markup(""));
                }

                builder.AddMarkerRow(markerCells);
            },
            onItem: (item, _) =>
            {
                if (item == null)
                {
                    // Null item - render null in first cell, empty in rest
                    // Null items are treated as marker rows (no index)
                    var nullCells = new List<IRenderable>();
                    nullCells.Add(renderItem(null, null, context));
                    for (int i = 1; i < properties.Count; i++)
                    {
                        nullCells.Add(new Markup(""));
                    }
                    builder.AddMarkerRow(nullCells);
                    return;
                }

                var rowCells = new List<IRenderable>();

                foreach (var property in properties)
                {
                    var value = GetPropertyValue(item, property);
                    var valueDescriptor = value != null
                        ? DumpConfig.Default.Generator.Generate(value.GetType(), null, context.Config.MemberProvider)
                        : property;

                    var renderedValue = renderItem(value, valueDescriptor, context with { CurrentDepth = context.CurrentDepth + 1 });
                    rowCells.Add(renderedValue);
                }

                builder.AddRow(elementDescriptor, item, rowCells);
            });
    }

    private static bool CanUseHorizontalLayout(Type type, RenderContext<SpectreRendererState> context)
    {
        // Use the descriptor system to determine the type category
        var descriptor = DumpConfig.Default.Generator.Generate(type, null, context.Config.MemberProvider);

        // SingleValueDescriptor types (primitives, strings, etc.) should use vertical
        if (descriptor is SingleValueDescriptor)
        {
            return false;
        }

        // ObjectDescriptor with properties can use horizontal layout
        return descriptor is ObjectDescriptor objDescriptor && objDescriptor.Properties.Any();
    }

    private static string GetCollectionTypeName(Type type, int itemCount, RenderContext<SpectreRendererState> context)
    {
        if (type.IsArray)
        {
            var elementTypeName = type.GetElementType() is { } elementType
                ? context.Config.TypeNameProvider.GetTypeName(elementType)
                : "";

            return $"{elementTypeName}[{itemCount}]";
        }

        return context.Config.TypeNameProvider.GetTypeName(type);
    }

    private static object? GetPropertyValue(object obj, IDescriptor property)
    {
        if (property.ValueProvider is IValueProvider valueProvider)
        {
            try
            {
                return valueProvider.GetValue(obj);
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Applies the MemberFilter from the context to filter which properties should be displayed as columns.
    /// Uses the first non-null item in the collection as a prototype for value-based filtering.
    /// When the collection has no non-null items there is no object to evaluate the filter against,
    /// so all properties are kept. This mirrors the vertical layout, where the filter is evaluated
    /// per item and therefore never runs for an empty collection.
    /// </summary>
    private static IEnumerable<IDescriptor> ApplyMemberFilter(
        IEnumerable<IDescriptor> properties,
        TruncatedEnumerable<object?> items,
        RenderContext<SpectreRendererState> context)
    {
        var filter = context.Config.MemberFilter;

        if (filter is null)
        {
            return properties;
        }

        // Find the first non-null item to use as a prototype for value-based filtering
        var prototypeItem = items.Items.FirstOrDefault(item => item is not null);

        if (prototypeItem is null)
        {
            return properties;
        }

        return properties.Where(property =>
        {
            if (property.ValueProvider is not IValueProvider valueProvider)
            {
                return true;
            }

            var filterContext = new MemberFilterContext(valueProvider, prototypeItem, context.CurrentDepth);
            return filter(filterContext);
        });
    }
}
