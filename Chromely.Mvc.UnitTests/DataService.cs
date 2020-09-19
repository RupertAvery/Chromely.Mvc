using System;
using System.Collections.Generic;

namespace Tests
{
    public class DataService : IDataService
    {
        public Person GetPerson()
        {
            return new Person()
            {
                Name = "Rupert Avery",
                Age = 21,
                BirthDate = new DateTime(1982, 06, 12)
            };
        }

        public IEnumerable<Person> GetPeople()
        {
            return new List<Person>()
            {
                new Person()
                {
                    Name = "Rupert Avery",
                    Age = 21,
                    BirthDate = new DateTime(1982, 06, 12)
                },
                new Person()
                {
                    Name = "Jemma Avery",
                    Age = 18,
                    BirthDate = new DateTime(1989, 08, 27)
                }
            };

        }
    }
}