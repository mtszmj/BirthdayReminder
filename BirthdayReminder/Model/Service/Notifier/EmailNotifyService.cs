using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace BirthdayReminder.Model.Service
{
    public class EmailNotifyService : INotifyService
    {
        public bool Enabled { get; set; }
        private string From { get; }
        private string FromName { get; }
        private List<string> To { get; } = new List<string>();
        private string Subject { get; }
        private string Path { get; }
        private int Port { get; }
        private string Smtp { get; }

        internal EmailNotifyService(bool enabled, string from, string fromName, IEnumerable<string> to, string subject, string path, string smtp, int port)
        {
            if (to == null)
                throw new ArgumentNullException(nameof(to));
            Enabled = enabled;
            From = from ?? throw new ArgumentNullException(nameof(from));
            FromName = fromName ?? throw new ArgumentNullException(nameof(fromName));
            To.AddRange(to);
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Smtp = smtp ?? throw new ArgumentNullException(nameof(path));
            Port = port;
        }

        public void Notify(IEnumerable<Person> peopleWithBirthdayToday, IEnumerable<Person> peopleWithBirthdayInFuture)
        {
            Logger.Log.LogDebug("TEST WATKOW");
            var message = PrepareMessage(peopleWithBirthdayToday, peopleWithBirthdayInFuture);
            var password = File.ReadAllText(Path);
            using (var client = new SmtpClient())
            {
                // accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(Smtp, Port, true);
                client.Authenticate(From, password);

                if (Enabled)
                    client.Send(message);
                client.Disconnect(true);
                Logger.Log.LogDebug("Wysłano poprawnie");
            }
        }

        private MimeMessage PrepareMessage(IEnumerable<Person> peopleWithBirthdayToday, IEnumerable<Person> peopleWithBirthdayInFuture)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(FromName, From));
            message.To.AddRange(To.Select(t => new MailboxAddress(t)));
            message.Subject = Subject;

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
                sb.Append(string.Join("\n", 
                    peopleWithBirthdayInFuture
                        .OrderBy(p => p.Month)
                        .ThenBy(p => p.Day)
                        .Select(person => $"{person.Name} ({person.Birthday})")
                    )
                );
            }
            sb.AppendLine("\n\n--- BirthdayReminder :)");

            message.Body = new TextPart() { Text = sb.ToString() };
            return message;
        }
    }
}
