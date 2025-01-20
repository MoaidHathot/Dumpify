namespace Dumpify;

public class ObjectIdReferenceTracker : IObjectIdTracker
{
	private readonly Dictionary<object, long> _objects = new(ReferenceEqualityComparer.Instance);

    public (bool firstTime, long id) Track(object obj)
	{
		if(_objects.TryAdd(obj, _objects.Count + 1))
		{
			return (true, _objects.Count);
		}
		else
		{
			return (false, _objects[obj]);
		}
	}
}