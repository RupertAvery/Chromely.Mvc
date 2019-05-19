using System.Collections.Generic;
using System;

namespace Tests
{
    public class Event
    {
        public string Activity { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public Person Instructor { get; set; }
        public IEnumerable<Person> Participants { get; set; }
    }
}