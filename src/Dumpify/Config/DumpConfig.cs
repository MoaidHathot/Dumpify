using Dumpify.Extensions;
using Dumpify.Descriptors.Generators;
using Dumpify.Renderers;
using System.Reflection;
using System.Collections.Concurrent;
using Dumpify.Config;
using Dumpify.Descriptors.ValueProviders;
using Dumpify.Outputs;

namespace Dumpify;

public class DumpConfig
{
    private int _maxDepth = 7;

    public static DumpConfig Default { get; } = new DumpConfig();

    internal ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>> CustomDescriptorHandlers { get; }

    private DumpConfig()
    {
        CustomDescriptorHandlers = new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, IValueProvider?, IMemberProvider, object?>>();
        Generator = new CompositeDescriptorGenerator(CustomDescriptorHandlers);
        Renderer = Config.Renderers.Table;
        Output = Config.Outputs.Console;
        ColorConfig = new ColorConfig();
        TableConfig = new TableConfig();
        MembersConfig = new MembersConfig();
        TypeNamingConfig = new TypeNamingConfig();
    }

    public void AddCustomTypeHandler(Type type, Func<object, Type, IValueProvider?, IMemberProvider, object?> valueFactory)
        => CustomDescriptorHandlers[type.TypeHandle] = valueFactory;

    public void RemoveCustomTypeHandler(Type type)
        => CustomDescriptorHandlers.TryRemove(type.TypeHandle, out _);

    public IDescriptorGenerator Generator { get; set; }
    public IRenderer Renderer { get; set; }
    public IDumpOutput Output { get; set; }

    public int MaxDepth { get => _maxDepth; set => _maxDepth = value.MustBeGreaterThan(0); }

    public bool UseDescriptors { get; set; } = true;
    public bool ShowHeaders { get; set; } = true;

    public ColorConfig ColorConfig { get; set; }
    public TableConfig TableConfig { get; }
    public MembersConfig MembersConfig { get; }
    public TypeNamingConfig TypeNamingConfig { get; }
}
