using Dumpify.Renderers;

namespace Dumpify;

public static class DumpExtensions
{
    public static T? Dump<T>(this T? obj, string? label = null, int? maxDepth = null, IRenderer? renderer = null, bool? useDescriptors = null)
    {
        var defaultConfig = DumpConfig.Default;

        var rendererConfig = new RendererConfig
        {
            Label = label,
            MaxDepth = maxDepth ?? defaultConfig.MaxDepth,
        };

        var createDescriptor = useDescriptors ?? defaultConfig.useDescriptors;

        if(obj is null || createDescriptor is false)
        {
            DumpConfig.Default.Renderer.Render(obj, null, rendererConfig);
            return obj;
        }

        var descriptor = DumpConfig.Default.Generator.Generate(obj.GetType(), propertyInfo: null);

        DumpConfig.Default.Renderer.Render(obj, descriptor, rendererConfig);

        return obj;
    }
}
