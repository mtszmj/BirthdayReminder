using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BirthdayReminder.Model.Service.Notifier
{
    public class NotifyIconNotifyService : INotifyService
    {
        private NotifyIcon _NotifyIcon;
        private int _Time;

        internal NotifyIconNotifyService(bool enabled, NotifyIcon notifyIcon, int time)
        {
            Enabled = enabled;
            _NotifyIcon = notifyIcon;
            _Time = time;
        }

        public bool Enabled { get; set; }

        public void Notify(IEnumerable<Person> peopleWithBirthdayToday, IEnumerable<Person> peopleWithBirthdayInFuture)
        {
            _NotifyIcon?.ShowBalloonTip(_Time, "BirthdayReminder", PrepareMessage(peopleWithBirthdayToday), ToolTipIcon.None);
            Logger.Log.LogDebug($"Notify for notify icon");
        }

        private string PrepareMessage(IEnumerable<Person> peopleWithBirthdayToday)
        {
            StringBuilder sb = new StringBuilder();
            if (peopleWithBirthdayToday.Any())
            {
                sb.Append("Dzisiejsze urodziny:\n");
                sb.Append(string.Join("\n", peopleWithBirthdayToday.Select(person => $"{person.Name} ({person.Age})")));
            }
            return sb.ToString();
        }

    }
}
