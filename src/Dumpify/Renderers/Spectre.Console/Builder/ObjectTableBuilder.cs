using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpify.Renderers.Spectre.Console.Builder;

internal class ObjectTableBuilder
{
    private readonly RenderContext<SpectreRendererState> _context;
    private readonly IDescriptor _descriptor;
    private readonly object _sourceObject;

    private Table _table = new Table();

    private readonly List<IEnumerable<IRenderable>> _rows = new();
    private readonly List<IRenderable> _columnNames = new(2);

    public ObjectTableBuilder(RenderContext<SpectreRendererState> context, IDescriptor descriptor, object sourceObject)
    {
        _context = context;
        _descriptor = descriptor;
        _sourceObject = sourceObject;

        InitTable();
    }

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

    private void InitTable()
    {
        if (_context.Config.TypeNamingConfig.ShowTypeNames is true)
        {
            var type = _descriptor.Type == _sourceObject.GetType() ? _descriptor.Type : _sourceObject.GetType();
            var typeName = _context.Config.TypeNameProvider.GetTypeName(type);
            _table.Title = new TableTitle(Markup.Escape(typeName), new Style(foreground: _context.State.Colors.TypeNameColor));
        }

        if (_context.Config.TableConfig.ShowTableHeaders is false)
        {
            _table.HideHeaders();
        }

        if (_context.Config.Label is { } label && _context.CurrentDepth == 0 && object.ReferenceEquals(_context.RootObject, _sourceObject))
        {
            _table.Caption = new TableTitle(Markup.Escape(label));
        }
    }

    public ObjectTableBuilder SetTitle(string? title, Style? style = null)
    {
        var tableTitle = title switch
        {
            null => null,
            not null => new TableTitle(Markup.Escape(title), style),
        };

        _table.Title = tableTitle;

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

    private IRenderable ToPropertyNameMarkup(string str)
        => new Markup(Markup.Escape(str), new Style(foreground: _context.State.Colors.PropertyNameColor));

    private IRenderable ToColumnNameMarkup(string str, Style? style)
        => new Markup(Markup.Escape(str), style);

    public Table Build()
    {
        foreach (var column in _columnNames)
        {
            _table.AddColumn(new TableColumn(column));
        }

        foreach (var row in _rows)
        {
            _table.AddRow(row);
        }

        return _table.Collapse();
    }
}