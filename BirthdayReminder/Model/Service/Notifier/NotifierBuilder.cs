using BirthdayReminder.Model.Service.Password;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BirthdayReminder.Model.Service.Notifier
{
    public class NotifierBuilder
    {
        private bool enabled;
        private NotifierType type = NotifierType.Null;
        private string from;
        private string fromName;
        private Lazy<List<string>> to = new Lazy<List<string>>();
        private string subject;
        private string smtp;
        private int port;
        private NotifyIcon notifyIcon;
        private int time = 2000;
        private ILoginHandler loginHandler;

        public NotifierBuilder OfType(NotifierType type)
        {
            this.type = type;
            return this;
        }

        public NotifierBuilder SetEmailFrom(string pathToFile)
        {
            if (pathToFile == null)
                throw new ArgumentNullException(nameof(pathToFile));
            if (!File.Exists(pathToFile))
                throw new ArgumentException(nameof(pathToFile));

            var mail = File.ReadLines(pathToFile).FirstOrDefault()?.Trim() 
                ?? throw new InvalidOperationException("Brak podanego maila w pliku.");
            from = mail;
            return this;
        }

        public NotifierBuilder SetEmailFromName(string name)
        {
            fromName = name;
            return this;
        }

        public NotifierBuilder AddEmailTo(string pathToFile)
        {
            if (pathToFile == null)
                throw new ArgumentNullException(nameof(pathToFile));
            if (!File.Exists(pathToFile))
                throw new ArgumentException(nameof(pathToFile));

            foreach (var email in File.ReadAllLines(pathToFile))
            {
                to.Value.Add(email.Trim());
            }
            if(!to.Value.Any())
            {
                throw new InvalidOperationException("Brak podanych maili w pliku.");
            }

            return this;
        }

        public NotifierBuilder SetSubject(string subject)
        {
            this.subject = subject;
            return this;
        }

        public NotifierBuilder WithSmtp(string smtp, int port)
        {
            this.smtp = smtp;
            this.port = port;
            return this;
        }

        public NotifierBuilder WithLoginHandler(ILoginHandler loginHandler)
        {
            this.loginHandler = loginHandler;
            return this;
        }

        public NotifierBuilder Enabled()
        {
            enabled = true;
            return this;
        }

        public NotifierBuilder Disabled()
        {
            enabled = false;
            return this;
        }

        public NotifierBuilder WithNotifyIcon(NotifyIcon notifyIcon)
        {
            this.notifyIcon = notifyIcon;
            return this;
        }

        public NotifierBuilder WithNotifyTime(int time)
        {
            this.time = time;
            return this;
        }

        public INotifyService Build()
        {
            switch (type)
            {
                case NotifierType.Console:
                    return new ConsoleNotifyService { Enabled = enabled };
                case NotifierType.Email:
                    return new EmailNotifyService(enabled, from, fromName, to.Value, subject, smtp, port, loginHandler);
                case NotifierType.NotifyIcon:
                    if (notifyIcon != null)
                        return new NotifyIconNotifyService(enabled, notifyIcon, time);
                    else throw new ArgumentNullException("NotifyIcon");
            }
            return new NullNotifyService();
        }
    }

    public enum NotifierType
    {
        Null, Console, Email, NotifyIcon
    }
}
