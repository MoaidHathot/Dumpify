using Dumpify.Descriptors;

namespace Dumpify;

internal class CustomTypeRenderer<TRenderable> : ICustomTypeRenderer<TRenderable>
{
    public Type DescriptorType { get; init; }

    private readonly Func<IDescriptor, object, (bool shouldHandle, object handleContext)> _shouldHandleFunc;
    private readonly Func<IDescriptor, object, RenderContext, object?, TRenderable> _rendererFunc;

    public CustomTypeRenderer(Type descriptorType, Func<IDescriptor, object, (bool, object)> shouldHandleFunc, Func<IDescriptor, object, RenderContext, object?, TRenderable> rendererFunc)
    {
        DescriptorType = descriptorType;
        _shouldHandleFunc = shouldHandleFunc;
        _rendererFunc = rendererFunc;
    }

    public TRenderable Render(IDescriptor descriptor, object obj, RenderContext context, object? handleContext)
        => _rendererFunc(descriptor, obj, context, handleContext);

    (bool shouldHandle, object handleContext) ICustomTypeRenderer<TRenderable>.ShouldHandle(IDescriptor descriptor, object obj) 
        => _shouldHandleFunc(descriptor, obj);
}
