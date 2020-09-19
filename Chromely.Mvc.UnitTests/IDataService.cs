using System.Collections.Generic;

namespace Tests
{
    public interface IDataService
    {
        Person GetPerson();
        IEnumerable<Person> GetPeople(); 
    }
}