namespace Dumpify.Tests.DTO;

public class Person
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}

public class PersonWithSignificantOther
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public PersonWithSignificantOther? SignificantOther { get; set; }
}
