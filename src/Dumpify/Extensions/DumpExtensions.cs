using Dumpify.Renderers;

namespace Dumpify;

public static class DumpExtensions
{
    public static T Dump<T>(this T obj, string? label = null, int? maxNestingLevel = null, IRenderer? renderer = null)
    {
        if(obj is null)
        {
            //render
            return obj;
        }

        var descriptor = DumpConfig.Default.DescriptorGenerator.Generate(obj.GetType(), propertyInfo: null);
        return obj;
    }
}
