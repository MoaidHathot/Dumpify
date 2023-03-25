using Dumpify;
using System.Text;

var moaid = new Person { FirstName = "Moaid", LastName = "Hathot" };
var arr = new[] { 1, 2, 3, 4 }; 

new StringBuilder("sdfsdf").Dump();

public class Person
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}