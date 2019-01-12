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
            yield return new Person("Adam Smith", new DateTime(1999, 08, 10), true);
            yield return new Person("John Doe", new DateTime(2018, 11, 23), false);
            yield return new Person("Celcius", new DateTime(2018, 6, 23), false);

            DateTime today = DateTime.Today;
            yield return new Person("Yesterday Man", today.AddDays(-1).AddYears(-10), true);
            yield return new Person("Today Man", new DateTime(2011, today.Month, today.Day), true);
            yield return new Person("Tomorrow Man", today.AddDays(1).AddYears(-20), true);
        }

        public void SavePeople(ObservableCollection<Person> people)
        {
            // nothing
        }
    }
}
