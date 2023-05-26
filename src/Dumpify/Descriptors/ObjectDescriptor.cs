using Dumpify.Descriptors.ValueProviders;

namespace Dumpify.Descriptors;

internal record ObjectDescriptor(Type Type, IValueProvider? ValueProvider, IEnumerable<IDescriptor> Properties) : IDescriptor
{
    public string Name => ValueProvider?.Name ?? Type.Name;
}