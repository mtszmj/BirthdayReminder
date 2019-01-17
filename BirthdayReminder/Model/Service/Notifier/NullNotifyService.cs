using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayReminder.Model.Service.Notifier
{
    public class NullNotifyService : INotifyService
    {
        public bool Enabled { get; set; }

        public void Notify(IEnumerable<Person> peopleWithBirthdayToday, IEnumerable<Person> peopleWithBirthdayInFuture) { }
    }
}
