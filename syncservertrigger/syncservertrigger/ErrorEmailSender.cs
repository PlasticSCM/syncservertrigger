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
            MailMessage result = new MailMessage();
            result.Sender = new MailAddress(from);
            result.To.Add(to);
            result.Subject = subject;
            result.Body = body;

            return result;
        }

        static SmtpClient BuildSmtpClient(EmailConfiguration emailConfiguration)
        {
            SmtpClient result = new SmtpClient();
            result.Port = emailConfiguration.Port;
            result.Credentials = new NetworkCredential(
                emailConfiguration.SourceEmail, emailConfiguration.Password);
            result.EnableSsl = emailConfiguration.EnableSsl;

            return result;
        }

        EmailConfiguration mEmailConfiguration;
    }
}
