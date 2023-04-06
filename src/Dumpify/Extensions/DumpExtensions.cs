using Dumpify.Config;
using Dumpify.Descriptors;
using Dumpify.Extensions;
using Dumpify.Renderers;
using System.Reflection;

namespace Dumpify;

public static class DumpExtensions
{
    public static T? Dump<T>(this T? obj, string? label = null, int? maxDepth = null, IRenderer? renderer = null, bool? useDescriptors = null, bool? showTypeNames = null, bool? showHeaders = null, ColorConfig? colors = null)
    {
        var defaultConfig = DumpConfig.Default;

        var rendererConfig = new RendererConfig
        {
            Label = label,
            MaxDepth = maxDepth.MustBeGreaterThan(0) ?? defaultConfig.MaxDepth,
            ShowTypeNames = showTypeNames ?? defaultConfig.ShowTypeNames,
            ShowHeaders = showHeaders ?? defaultConfig.ShowHeaders,
            ColorConfig = colors ?? defaultConfig.ColorConfig,
            TableConfig = defaultConfig.TableConfig,
        };

        var createDescriptor = useDescriptors ?? defaultConfig.UseDescriptors;
        renderer ??= defaultConfig.Renderer;

        if(obj is null || createDescriptor is false)
        {
            RenderSafely(obj, renderer, null, rendererConfig);
            return obj;
        }

        if(!TryGenerate<T>(obj.GetType(), null, out var descriptor))
        {
            return obj;
        }

        RenderSafely(obj, renderer, descriptor, rendererConfig);

        return obj;
    }

    private static void RenderSafely<T>(T? obj, IRenderer renderer, IDescriptor? descriptor, RendererConfig config)
    {
        try
        {

            renderer.Render(obj, descriptor, config);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Failed to Render {typeof(T)} - {obj}]. {ex.Message}");

#if DEBUG
            Console.WriteLine(ex);
#endif
        }
    }

    private static bool TryGenerate<T>(Type type, PropertyInfo? info, out IDescriptor? descriptor)
    {
        descriptor = null;

        try
        {
            descriptor = DumpConfig.Default.Generator.Generate(type, propertyInfo: info);
            return true;
        }
        catch (Exception ex)
        {

            Console.WriteLine($"[Failed to Generate descriptor for {typeof(T)} and Property: {info}]. {ex.Message}");

#if DEBUG
            Console.WriteLine(ex);
#endif
        }

        return false;
    }
}
