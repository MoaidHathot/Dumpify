using Dumpify.Descriptors.Generators;
using Dumpify.Renderers;
using Dumpify.Renderers.Spectre.Console;
using System.Reflection;

namespace Dumpify;

public class DumpConfig
{
    public static DumpConfig Default { get; } = new DumpConfig();
    internal Dictionary<RuntimeTypeHandle, Func<object, Type, PropertyInfo?, object>> CustomDescriptorHandlers { get; }

    public DumpConfig()
    {
        CustomDescriptorHandlers = new Dictionary<RuntimeTypeHandle, Func<object, Type, PropertyInfo?, object>>();
        Generator = new CompositeDescriptorGenerator(CustomDescriptorHandlers);
        Renderer = new SpectreConsoleTableRenderer();
    }

    public void AddCustomTypeHandler(Type type, Func<object, Type, PropertyInfo?, object> valueFactory)
    {
        CustomDescriptorHandlers[type.TypeHandle] = valueFactory;
    }

    public void RemoveCustomTypeHandler(Type type)
    {
        CustomDescriptorHandlers.Remove(type.TypeHandle);
    }

    public IDescriptorGenerator Generator { get; set; }
    public IRenderer Renderer { get; set; }


    public bool UseDescriptors { get; set; } = true;
    public int MaxDepth { get; set; } = 7;
    public bool ShowTypeNames { get; set; } = true;
    public bool ShowHeaders { get; set; } = true;
}
