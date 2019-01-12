using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayReminder.Model.Service
{
    public interface IDataService
    {
        IEnumerable<Person> GetPeople();
        void SavePeople(ObservableCollection<Person> people);
    }
}
