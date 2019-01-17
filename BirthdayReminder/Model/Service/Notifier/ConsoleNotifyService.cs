using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirthdayReminder.Model.Service
{
    public class ConsoleNotifyService : INotifyService
    {
        public bool Enabled { get; set; } = true;

        public void Notify(IEnumerable<Person> peopleWithBirthdayToday, IEnumerable<Person> peopleWithBirthdayInFuture)
        {
            Console.WriteLine(PrepareMessage(peopleWithBirthdayToday, peopleWithBirthdayInFuture));
        }

        private string PrepareMessage(IEnumerable<Person> peopleWithBirthdayToday, IEnumerable<Person> peopleWithBirthdayInFuture)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Witaj,\n\n");
            if (peopleWithBirthdayToday.Any())
            {
                sb.Append("dzisiaj urodziny ma:\n");
                sb.Append(string.Join("\n", peopleWithBirthdayToday.Select(person => $"{person.Name} ({person.Age})")));
            }
            if (peopleWithBirthdayInFuture.Any())
            {
                sb.Append("\n\nW najbliższym czasie urodziny ma:\n");
                sb.Append(string.Join("\n", peopleWithBirthdayInFuture.Select(person => $"{person.Name} ({person.Birthday})")));
            }
            sb.AppendLine("\n\n--- BirthdayReminder :)");

            return sb.ToString();
        }
    }
}
