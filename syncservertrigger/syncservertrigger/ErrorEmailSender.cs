using System.Net;
using System.Net.Mail;

using Codice.SyncServerTrigger.Configuration;

namespace Codice.SyncServerTrigger
{
    internal class ErrorEmailSender
    {
        internal ErrorEmailSender(EmailConfiguration emailConfiguration)
        {
            mEmailConfiguration = emailConfiguration;
        }

        internal void SendErrorEmail(string subject, string body)
        {
            if (string.IsNullOrEmpty(mEmailConfiguration.SourceEmail))
                return;

            SmtpClient client = BuildSmtpClient(mEmailConfiguration);
            foreach (string toAddress in mEmailConfiguration.GetDestinationEmails())
            {
                client.Send(
                    BuildMailMessage(
                        mEmailConfiguration.SourceEmail,
                        toAddress,
                        subject,
                        body));
            }
        }

        static MailMessage BuildMailMessage(
            string from,
            string to,
            string subject,
            string body)
        {
            MailMessage result = new MailMessage
            {
                Sender = new MailAddress(from),
                Subject = subject,
                Body = body
            };
            result.To.Add(to);

            return result;
        }

        static SmtpClient BuildSmtpClient(EmailConfiguration emailConfiguration)
        {
            return new SmtpClient
            {
                Port = emailConfiguration.Port,
                Credentials = new NetworkCredential(
                    emailConfiguration.SourceEmail,
                    emailConfiguration.Password),
                EnableSsl = emailConfiguration.EnableSsl
            };
        }

        EmailConfiguration mEmailConfiguration;
    }
}
