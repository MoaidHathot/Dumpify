using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify;

public class InvertedTableBuilder : IRenderableBuilder
{
    private readonly RenderContext<SpectreRendererState> _context;
    private readonly IDescriptor _descriptor;
    private readonly object _sourceObject;

    private readonly HashSet<(string header, Style? style)> _headers = new();
    private readonly List<(string header, IDescriptor? descriptor, object? obj, IRenderable? renderedObj)> _entries = [];

    private (string? title, Style? style)? _title = default;
    private (string? label, Style? style)? _label = default;

    public InvertedTableBuilder(RenderContext<SpectreRendererState> context, IDescriptor descriptor, object sourceObject)
    {
        _context = context;
        _descriptor = descriptor;
        _sourceObject = sourceObject;
    }

    public IRenderable Build()
    {
        var table = new Table();
        BuildTitle(table);

        var columnMapping = BuildColumns(table);
        BuildRows(table, columnMapping);

        table.RoundedBorder();

        if (_context.Config.TableConfig.ShowRowSeparators)
        {
            table.ShowRowSeparators();
        }

        table = _context.Config.TableConfig.ExpandTables ? table.Expand() : table.Collapse();
        return table;
    }

    private void BuildLabel(Table table)
    {
        if (_label?.label is { } label && _context.CurrentDepth == 0 && (object.ReferenceEquals(_context.RootObject, _sourceObject) || _context.RootObjectTransform is not null && object.ReferenceEquals(_context.RootObjectTransform, _sourceObject)))
        {
            table.Caption = new TableTitle(Markup.Escape(label), new Style(foreground: _context.State.Colors.LabelValueColor));
            var panel = new Panel(table);
            panel.Header = new PanelHeader(label, Justify.Center);
            panel.BorderStyle = new Style(foreground: _context.State.Colors.LabelValueColor);
        }
    }

    private void BuildRows(Table table, Dictionary<string, int> columnMap)
    {
        var row = new IRenderable[_headers.Count];

        foreach (var entry in _entries)
        {
            if (columnMap.TryGetValue(entry.header, out var index))
            {
                Console.WriteLine(index);
                row[index] = entry.renderedObj!;
            }
            else
            {
                Console.WriteLine("No index found for " + entry.header);
            }

        }

        table.AddRow(row);
    }

    private Dictionary<string, int> BuildColumns(Table table)
    {
        var columnMap = new Dictionary<string, int>();

        foreach (var (header, style) in _headers)
        {
            var tableColumn = new TableColumn(header);

            if (_context.Config.TableConfig.NoColumnWrapping)
            {
                tableColumn.NoWrap();
            }

            columnMap.Add(header, columnMap.Count);
            table.AddColumn(tableColumn);
        }

        return columnMap;
    }

    private void BuildTitle(Table table)
    {
        if (_context.Config.TypeNamingConfig.ShowTypeNames is true)
        {
            var type = _descriptor.Type == _sourceObject.GetType() ? _descriptor.Type : _sourceObject.GetType();
            var typeName = _context.Config.TypeNameProvider.GetTypeName(type);

            var title = _title switch
            {
                null => new TableTitle(Markup.Escape(typeName), new Style(foreground: _context.State.Colors.TypeNameColor)),
                not null => string.IsNullOrWhiteSpace(_title.Value.title) ? null : new TableTitle(_title.Value.title!, _title.Value.style),
            };

            if (title is not null)
            {

                table.Title = title;
            }
        }
    }

    public IRenderableBuilder WithEntry(IDescriptor? descriptor, object? obj, IRenderable? renderedObj, string header, IEnumerable<IRenderable> entry, Style? headerStyle = null)
    {
        Console.WriteLine(_headers.Add((header, headerStyle)));
        _entries.Add((header, descriptor, obj, renderedObj));
        return this;
    }

    public IRenderableBuilder WithHeader(string header, Style? style = null)
    {
        _headers.Add((header, style));
        return this;
    }

    public IRenderableBuilder WithLabel(string? label)
    {
        _label = (label, null);
        return this;
    }

    public IRenderableBuilder WithTitle(string? title, Style? style = null)
    {
        _title = (title, style);
        return this;
    }
}
