using Dumpify.Descriptors.ValueProviders;

namespace Dumpify.Descriptors.Generators;
internal class KnownSingleValueGenerator : IDescriptorGenerator
{
    private static readonly HashSet<RuntimeTypeHandle> _singleValueTypes = new ()
    {
        typeof(bool).TypeHandle,
        typeof(byte).TypeHandle,
        typeof(short).TypeHandle,
        typeof(ushort).TypeHandle,
        typeof(int).TypeHandle,
        typeof(uint).TypeHandle,
        typeof(long).TypeHandle,
        typeof(ulong).TypeHandle,
        typeof(float).TypeHandle,
        typeof(double).TypeHandle,
        typeof(decimal).TypeHandle,
        typeof(string).TypeHandle,
        typeof(char).TypeHandle,
        typeof(nuint).TypeHandle,
        typeof(nint).TypeHandle,
#if NET5_0_OR_GREATER
        typeof(Half).TypeHandle,
#endif
    };

    public IDescriptor? Generate(Type type, IValueProvider? valueProvider, IMemberProvider memberProvider)
    {
        if (type.IsEnum)
        {
            return new SingleValueDescriptor(type, valueProvider);
        }

        if(!_singleValueTypes.Contains(type.TypeHandle))
        {
            return null;
        }

        return new SingleValueDescriptor(type, valueProvider);
    }
}
