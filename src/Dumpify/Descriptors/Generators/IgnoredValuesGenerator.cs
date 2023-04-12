using Dumpify.Descriptors.ValueProviders;

namespace Dumpify.Descriptors.Generators;
internal class IgnoredValuesGenerator : IDescriptorGenerator
{
    private readonly HashSet<RuntimeTypeHandle> _ignoredTypes = new ();

    public IDescriptor? Generate(Type type, IValueProvider? valueProvider, IMemberProvider memberProvider)
    {
        if(_ignoredTypes.Contains(type.TypeHandle))
        {
            return new IgnoredDescriptor(type, valueProvider);
        }

        return null;
    }
}
