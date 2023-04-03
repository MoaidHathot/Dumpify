using Dumpify.Descriptors;
using Dumpify.Renderers;
using System.Reflection;

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
            RenderSafely(obj, null, rendererConfig);
            return obj;
        }

        if(!TryGenerate(obj.GetType(), null, out var descriptor))
        {
            return obj;
        }

        RenderSafely(obj, descriptor, rendererConfig);

        return obj;

        static bool TryGenerate(Type type, PropertyInfo? info, out IDescriptor? descriptor)
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

        static void RenderSafely(T? obj, IDescriptor? descriptor, RendererConfig config)
        {
            try
            {

                DumpConfig.Default.Renderer.Render(obj, descriptor, config);
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"[Failed to Render {typeof(T)} - {obj}]. {ex.Message}");

#if DEBUG
                Console.WriteLine(ex);
#endif
            }
        }
    }
}
