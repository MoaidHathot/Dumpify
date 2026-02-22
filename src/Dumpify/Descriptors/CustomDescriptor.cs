using Dumpify.Descriptors.ValueProviders;

namespace Dumpify.Descriptors;

internal record CustomDescriptor(Type Type, IValueProvider? ValueProvider) : IDescriptor
{
    public string Name => ValueProvider?.Name ?? Type.Name;
}