using System;
using System.Collections.Generic;
using System.Threading;

namespace BirthdayReminder.Model.Service.Notifier
{
    public class NotifierRetryDecorator : INotifyService
    {
        public NotifierRetryDecorator(INotifyService service, int retries = 3)
        {
            _NotifyService = service;
            _Retries = retries;
        }

        private int _Retries { get; }
        private INotifyService _NotifyService { get; }

        public bool Enabled { get => _NotifyService.Enabled; set => _NotifyService.Enabled = value; }

        public void Notify(IEnumerable<Person> peopleWithBirthdayToday, IEnumerable<Person> peopleWithBirthdayInFuture)
        {
            for (var i = 1; i <= _Retries; i++)
            {
                try
                {
                    _NotifyService.Notify(peopleWithBirthdayToday, peopleWithBirthdayInFuture);
                    break;
                }
                catch (Exception e)
                {
                    if (i < _Retries)
                    { 
                        Thread.Sleep(10000);
                        continue;
                    }

                    throw; // throw only if this is the last try
                }
            }
        }
    }
}
