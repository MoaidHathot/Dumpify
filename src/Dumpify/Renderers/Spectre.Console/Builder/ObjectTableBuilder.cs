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

    private string _columnKeyName;
    private string _columnValueName;

    public ObjectTableBuilder(RenderContext<SpectreRendererState> context, IDescriptor descriptor, object sourceObject)
    {
        _context = context;
        _descriptor = descriptor;
        _sourceObject = sourceObject;

        (_columnKeyName, _columnValueName) = ("Name", "Value");

        InitTable();
    }

    public ObjectTableBuilder SetColumnNames(string keyName, string valueName)
    {
        _columnKeyName = keyName;
        _columnValueName = valueName;

        return this;
    }

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

    public ObjectTableBuilder AddRow(IDescriptor? descriptor, object? obj, IRenderable keyRenderable, IRenderable renderedValue)
    {
        _rows.Add(new[]
        {
            keyRenderable,
            renderedValue
       });

        return this;
    }

    public ObjectTableBuilder AddRow(IDescriptor? descriptor, object? obj, string keyValue, IRenderable renderedValue)
    {
        _rows.Add(new[]
        {
            new Markup(Markup.Escape(keyValue), new Style(foreground: _context.State.Colors.PropertyNameColor)),
            renderedValue
       });

        return this;
    }

    public ObjectTableBuilder AddRow(IDescriptor descriptor, object? obj, IRenderable renderedValue)
    {
        _rows.Add(new[]
        {
            new Markup(Markup.Escape(descriptor.Name), new Style(foreground: _context.State.Colors.PropertyNameColor)),
            renderedValue
       });
        return this;
    }

    public Table Build()
    {
        var colorConfig = _context.Config.ColorConfig;
        var columnColor = colorConfig.ColumnNameColor.ToSpectreColor();

        _table.AddColumn(new TableColumn(new Markup(_columnKeyName, new Style(foreground: columnColor))));
        _table.AddColumn(new TableColumn(new Markup(_columnValueName, new Style(foreground: columnColor))));

        foreach (var row in _rows)
        {
            _table.AddRow(row);
        }

        return _table.Collapse();
    }
}