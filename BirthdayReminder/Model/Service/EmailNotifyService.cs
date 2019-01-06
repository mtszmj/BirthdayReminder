using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace BirthdayReminder.Model.Service
{
    public class EmailNotifyService : INotifyService
    {
        public bool IsSendOn = false;
        private string From = @"mtszmj.rpi@gmail.com";
        private string FromName = "Birthday Reminder";
        private List<string> To = new List<string>() {
            @"mateuszmaj@o2.pl"
        };
        private string Subject = "Urodziny";

        public EmailNotifyService(bool isSendOn)
        {
            IsSendOn = isSendOn;
        }

        public MimeMessage PrepareMessage()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(FromName, From));
            message.To.AddRange(To.Select(t => new MailboxAddress(t)));
            message.Subject = Subject;

            message.Body = new TextPart("plain")
            {
                Text = @"Witaj,\n\n" +
                @"dzisiaj urodziny ma {Contact.Name}. To {Contact.Age} urodziny!\n\n" +
                "--- BirthdayReminder :)"
            };

            return message;
        }

        public void Notify()
        {
            var password = File.ReadAllText("Auth/pwd.txt");
            using (var client = new SmtpClient())
            {
                // accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("mtszmj.rpi@gmail.com", password);

                if(IsSendOn)
                    client.Send(PrepareMessage());
                client.Disconnect(true);
            }
        }
    }
}
