using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayReminder.Model.Service
{
    class TestDataService : IDataService
    {
        public IEnumerable<Person> GetPeople()
        {
            yield return Person.Factory.CreatePerson("Adam Smith", new DateTime(1999, 08, 10), true);
            yield return Person.Factory.CreatePerson("John Doe", new DateTime(2018, 11, 23), false);
            yield return Person.Factory.CreatePerson("Celcius", new DateTime(2018, 6, 23), false);

            DateTime today = DateTime.Today;
            yield return Person.Factory.CreatePerson("Yesterday Man", today.AddDays(-1).AddYears(-10), true);
            yield return Person.Factory.CreatePerson("Today Man", new DateTime(2011, today.Month, today.Day), true);
            yield return Person.Factory.CreatePerson("Tomorrow Man", today.AddDays(1).AddYears(-20), true);
        }

        public void SavePeople(ObservableCollection<Person> people)
        {
            // nothing
        }
    }
}
