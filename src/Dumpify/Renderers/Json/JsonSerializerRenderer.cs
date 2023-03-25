using Dumpify.Descriptors;
using System.Text.Json;

namespace Dumpify.Renderers.Json;

internal class JsonSerializerRenderer : IRenderer
{
    public void Render(object? obj, IDescriptor? descriptor, RendererConfig config)
    {
        Console.WriteLine(JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true }));
    }
}
