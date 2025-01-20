namespace Dumpify;

public interface IObjectIdTracker
{
	public (bool firstTime, long id) Track(object obj);
}