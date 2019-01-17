using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayReminder.Model.Service
{
    public interface INotifyService
    {
        bool Enabled { get; set; }
        void Notify(IEnumerable<Person> peopleWithBirthdayToday, IEnumerable<Person> peopleWithBirthdayInFuture);
    }
}
