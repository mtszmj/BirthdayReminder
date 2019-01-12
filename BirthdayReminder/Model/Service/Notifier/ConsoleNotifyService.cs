using System;

namespace BirthdayReminder.Model.Service
{
    public class ConsoleNotifyService : INotifyService
    {
        public string PrepareMessage()
        {
            return "Dzisiaj urodziny ma { Contact.Name }. To { Contact.Age } urodziny!";
        }

        public void Notify()
        {
            Console.WriteLine(PrepareMessage());
        }
    }
}
