using Dumpify.Descriptors;
using Dumpify.Descriptors.Generators;
using Dumpify.Renderers;
using Dumpify.Renderers.Json;
using Dumpify.Renderers.Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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


    public bool useDescriptors = true;
    public int MaxDepth { get; set; } = 7;
}
