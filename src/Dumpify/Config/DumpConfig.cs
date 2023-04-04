using Dumpify.Extensions;
using Dumpify.Descriptors.Generators;
using Dumpify.Renderers;
using Dumpify.Renderers.Spectre.Console;
using System.Reflection;
using System.Collections.Concurrent;

namespace Dumpify;

public class DumpConfig
{
    private int _maxDepth = 7;

    public static DumpConfig Default { get; } = new DumpConfig();
    internal ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, PropertyInfo?, object>> CustomDescriptorHandlers { get; }

    public DumpConfig()
    {
        CustomDescriptorHandlers = new ConcurrentDictionary<RuntimeTypeHandle, Func<object, Type, PropertyInfo?, object>>();
        Generator = new CompositeDescriptorGenerator(CustomDescriptorHandlers);
        Renderer = new SpectreConsoleTableRenderer();
    }

    public void AddCustomTypeHandler(Type type, Func<object, Type, PropertyInfo?, object> valueFactory)
    {
        CustomDescriptorHandlers[type.TypeHandle] = valueFactory;
    }

    public void RemoveCustomTypeHandler(Type type)
    {
        CustomDescriptorHandlers.TryRemove(type.TypeHandle, out _);
    }

    public IDescriptorGenerator Generator { get; set; }
    public IRenderer Renderer { get; set; }

    public int MaxDepth { get => _maxDepth; set => _maxDepth = value.MustBeGreaterThan(0); }

    public bool UseDescriptors { get; set; } = true;
    public bool ShowTypeNames { get; set; } = true;
    public bool ShowHeaders { get; set; } = true;
}
