using Dumpify.Descriptors;
using Spectre.Console.Rendering;

namespace Dumpify;

internal class DirectoryInfoRenderer : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler;

    public Type DescriptorType => typeof(CustomDescriptor);

    public DirectoryInfoRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler)
    {
        _handler = handler;
    }

    public (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj)
        => (descriptor.Type == typeof(DirectoryInfo), null);

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;
        var directoryInfo = (DirectoryInfo)obj;

        var table = new ObjectTableBuilder(context, descriptor, obj)
            .AddColumnName("Name")
            .AddColumnName("FullName")
            .AddColumnName("CreationTime")
            .AddColumnName("LastAccessTime")
            .AddColumnName("LastWriteTime")
            .AddColumnName("Exists");

        table.AddRow(descriptor, obj, "Name", directoryInfo.Name);

        return table.Build();
    }
}