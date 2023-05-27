using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify.Renderers.Spectre.Console.Builder;

internal class ObjectTableBuilder
{
    private readonly RenderContext<SpectreRendererState> _context;
    private readonly IDescriptor _descriptor;
    private readonly object _sourceObject;

    private List<ITableBuilderBehavior> _behaviors = new();

    private readonly List<IEnumerable<IRenderable>> _rows = new();
    private readonly List<IRenderable> _columnNames = new(2);

    private TableTitle? _tableTitle = null;
    private bool? _showHeaders = null;

    public ObjectTableBuilder(RenderContext<SpectreRendererState> context, IDescriptor descriptor, object sourceObject)
    {
        _context = context;
        _descriptor = descriptor;
        _sourceObject = sourceObject;
    }

    public ObjectTableBuilder AddBehavior(ITableBuilderBehavior behavior)
    {
        _behaviors.Add(behavior);

        return this;
    }

    //todo: this should be an extension method
    public ObjectTableBuilder AddDefaultColumnNames()
        => SetColumnNames(new[] { "Name", "Value" });

    public ObjectTableBuilder AddColumnName(string columnName, Style? style)
    {
        _columnNames.Add(ToColumnNameMarkup(columnName, style));
        return this;
    }

    public ObjectTableBuilder AddColumnName(string columnName)
        => AddColumnName(columnName, GetColumnNameStyle());

    public ObjectTableBuilder SetColumnNames(IEnumerable<string> columnNames)
    {
        _columnNames.Clear();

        foreach (var columnName in columnNames)
        {
            AddColumnName(columnName);
        }

        return this;
    }

    private Style GetColumnNameStyle()
        => new Style(foreground: _context.Config.ColorConfig.ColumnNameColor.ToSpectreColor());

    public ObjectTableBuilder SetColumnNames(params string[] columnNames)
        => SetColumnNames((IEnumerable<string>)columnNames);

    public ObjectTableBuilder SetTitle(string? title, Style? style = null)
    {
        var tableTitle = title switch
        {
            null => null,
            not null => new TableTitle(Markup.Escape(title), style),
        };

        _tableTitle = tableTitle;

        return this;
    }

    public ObjectTableBuilder SetTitle(string? title)
        => SetTitle(title, new Style(foreground: _context.State.Colors.TypeNameColor));

    public ObjectTableBuilder AddRow(IDescriptor? descriptor, object? obj, IEnumerable<IRenderable> renderables)
    {
        _rows.Add(renderables);

        return this;
    }

    public ObjectTableBuilder AddRow(IDescriptor? descriptor, object? obj, params IRenderable[] renderables)
        => AddRow(descriptor, obj, (IEnumerable<IRenderable>)renderables);

    //todo: In the future, after the refactoring, this should be an extension method
    public ObjectTableBuilder AddRow(IDescriptor? descriptor, object? obj, string keyValue, IRenderable renderedValue)
        => AddRow(descriptor, obj, new[] { ToPropertyNameMarkup(keyValue), renderedValue });

    //todo: In the future, after the refactoring, this should be an extension method
    public ObjectTableBuilder AddRowWithTypeName(IDescriptor? descriptor, object? obj, IRenderable renderedValue)
        => AddRow(descriptor, obj, new[] { ToPropertyNameMarkup(descriptor?.Name ?? ""), renderedValue });

    public ObjectTableBuilder HideHeaders()
        => SetHeadersVisibility(false);

    public ObjectTableBuilder ShowHeaders()
        => SetHeadersVisibility(true);

    public ObjectTableBuilder SetHeadersVisibility(bool? showHeaders)
    {
        _showHeaders = showHeaders;

        return this;
    }

    private IRenderable ToPropertyNameMarkup(string str)
        => new Markup(Markup.Escape(str), new Style(foreground: _context.State.Colors.PropertyNameColor));

    private IRenderable ToColumnNameMarkup(string str, Style? style)
        => new Markup(Markup.Escape(str), style);

    private IEnumerable<IRenderable> GetBehaviorColumns()
        => _behaviors.SelectMany(b => b.GetAdditionalColumns(_context));

    private IEnumerable<IRenderable> GetBehaviorRows(Table table)
    {
        var behaviorContext = new BehaviorContext
        {
            TotalAvailableRows = _rows.Count,
            AddedRows = table.Rows.Count,
        };

        return _behaviors.SelectMany(b => b.GetAdditionalRowElements(behaviorContext, _context));
    }

    private Table CreateTable()
    {
        var table = new Table();

        if (_context.Config.TypeNamingConfig.ShowTypeNames is true)
        {
            var type = _descriptor.Type == _sourceObject.GetType() ? _descriptor.Type : _sourceObject.GetType();
            var typeName = _context.Config.TypeNameProvider.GetTypeName(type);
            table.Title = new TableTitle(Markup.Escape(typeName), new Style(foreground: _context.State.Colors.TypeNameColor));
        }

        var hideHeaders = _showHeaders switch
        {
            true => false,
            false => true,
            null => _context.Config.TableConfig.ShowTableHeaders is false,
        };

        if (hideHeaders)
        {
            table.HideHeaders();
        }

        // if (_shouldHideHeaders is true || _context.Config.TableConfig.ShowTableHeaders is false)
        // {
        //     table.HideHeaders();
        // }

        if (_context.Config.Label is { } label && _context.CurrentDepth == 0 && object.ReferenceEquals(_context.RootObject, _sourceObject))
        {
            table.Caption = new TableTitle(Markup.Escape(label));
        }

        table.Title = _tableTitle;

        var columns = GetBehaviorColumns().Concat(_columnNames);

        foreach (var column in columns)
        {
            table.AddColumn(new TableColumn(column));
        }

        foreach (var row in _rows)
        {
            var additional = GetBehaviorRows(table);
            var fullRow = additional.Concat(row);

            table.AddRow(fullRow);
        }

        return table.Collapse();
    }

    public Table Build()
        => CreateTable();
}