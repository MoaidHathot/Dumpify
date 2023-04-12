using Dumpify.Config;

namespace Dumpify.Descriptors.ValueProviders;

public interface IMemberProvider
{
    IEnumerable<IValueProvider> GetMembers(Type type);
}
