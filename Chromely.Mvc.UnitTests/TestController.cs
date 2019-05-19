using Chromely.Mvc;
using Chromely.Mvc.Attributes;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Tests
{
    public class TestController : Controller
    {

        [HttpGet]
        public void GetUrl(string name, int age, DateTime birthdate)
        {
            if (name != "Rupert Avery" || age != 21 || birthdate != new DateTime(1982, 06, 12))
            {
                throw new Exception("Test failed!");
            }
        }

        [HttpGet]
        public void ParameterTest(string name, int age, DateTime birthdate)
        {
            if (name != "Rupert Avery" || age != 21 || birthdate != new DateTime(1982, 06, 12))
            {
                throw new Exception("Test failed!");
            }
        }

        [HttpPost]
        public void PostTest(Person person)
        {
            if (person.Name != "Rupert Avery" || person.Age != 21 || person.BirthDate != new DateTime(1982, 06, 12))
            {
                throw new Exception("Test failed!");
            }
        }

        [HttpPost]
        public void PostArrayTest(IEnumerable<Person> persons)
        {
            var personArray = persons.ToArray();
            var person = personArray[0];

            if (person.Name != "Rupert Avery" || person.Age != 21 || person.BirthDate != new DateTime(1982, 06, 12))
            {
                throw new Exception("Test failed!");
            }

            person = personArray[1];
            if (person.Name != "Jemma Avery" || person.Age != 18 || person.BirthDate != new DateTime(1989, 08, 27))
            {
                throw new Exception("Test failed!");
            }

        }


        [HttpPost]
        public void ComplexObjectTest(Event evt)
        {
            if(evt.Activity != "Swimming")
            {
                throw new Exception("Test failed!");
            }

            if (evt.Location != "Swimming Pool")
            {
                throw new Exception("Test failed!");
            }

            var person = evt.Instructor;
            if (person.Name != "Laarni Avery" || person.Age != 38 || person.BirthDate != new DateTime(1975, 12, 02))
            {
                throw new Exception("Test failed!");
            }

            var personArray = evt.Participants.ToArray();
            person = personArray[0];
            if (person.Name != "Rupert Avery" || person.Age != 21 || person.BirthDate != new DateTime(1982, 06, 12))
            {
                throw new Exception("Test failed!");
            }
            person = personArray[1];
            if (person.Name != "Jemma Avery" || person.Age != 18 || person.BirthDate != new DateTime(1989, 08, 27))
            {
                throw new Exception("Test failed!");
            }
        }

    }
}