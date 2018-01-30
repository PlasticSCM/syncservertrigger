using System;
using System.Collections.Generic;

using Codice.SyncServerTrigger.Configuration;

namespace Codice.SyncServerTrigger.Commands
{
    internal class WarnEmailCmd : ICmd
    {
        string ICmd.Help { get { return HELP; } }

        string ICmd.CommandName { get { return "warnemail"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length == 1 || args.Length > 4)
            {
                Console.Error.WriteLine(HELP);
                Environment.Exit(1);
            }

            if (args.Length == 2 && args[1] == "configure")
            {
                ConfigureEmailWarnings();
                return;
            }

            if (args.Length == 2 && args[1] == "list")
            {
                ListDestinationAddresses();
                return;
            }

            if (args.Length == 3 && args[1] == "add")
            {
                AddDestinationAddress(args[2]);
                return;
            }

            if (args.Length == 3 && args[1] == "remove")
            {
                RemoveDestinationAddress(args[2]);
                return;
            }

            Console.Error.WriteLine(HELP);
            Environment.Exit(1);
        }

        void ConfigureEmailWarnings()
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();
            EmailConfiguration emailConfig = toolConfig.EmailConfig;

            emailConfig.SmptServer = ConsoleUtils.ReadLine("SMTP server address");
            emailConfig.EnableSsl = ConsoleUtils.ReadBool("Enable SSL");
            emailConfig.Port = ConsoleUtils.ReadInt("Port number");
            emailConfig.SourceEmail = ConsoleUtils.ReadLine("Email account");
            emailConfig.Password = ConsoleUtils.ReadPassword("Password");

            toolConfig.Save();
            Console.WriteLine("The account was correctly configured!");
        }

        void ListDestinationAddresses()
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();
            EmailConfiguration emailConfig = toolConfig.EmailConfig;

            List<string> destinationAddresses = emailConfig.GetDestinationEmails();

            destinationAddresses.ForEach(address => Console.WriteLine(address));
        }

        void AddDestinationAddress(string destinationAddress)
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();
            EmailConfiguration emailConfig = toolConfig.EmailConfig;

            emailConfig.AddDestinationEmail(destinationAddress);
            toolConfig.Save();

            Console.WriteLine(
                "Correctly added '{0}' to the destination addresses.",
                destinationAddress);
        }

        void RemoveDestinationAddress(string destinationAddress)
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();
            EmailConfiguration emailConfig = toolConfig.EmailConfig;

            emailConfig.RemoveDestinationEmail(destinationAddress);
            toolConfig.Save();

            Console.WriteLine(
                "Correctly removed '{0}' from the destination addresses.",
                destinationAddress);
        }

        const string HELP =
@"warnemail       Used to configure the warning email notifications when
                synchronization fails.

Usage:
    syncservertrigger warnemail <<configure> | <list> | <add|remove> <email>>

    configure   Enters the configuration wizard, to configure the email account
                from which the notifications should be sent.
    list        Lists the destination email addresses that receive
                notifications.
    add         Adds a destination email address to receive notifications.
    delete      Deletes a destination email address to receive notifications.
    email       Specifies the detination email address to add or remove from the
                notifications recipients.

Examples:
    syncservertrigger configure
    (Enters the configuration wizard.You will be asked for a email address,
    if it should use SMTP or POP3, the email server, and a password.The password
    will be stored cyphered, while the rest of the configuration will be saved
    as plain text. However, the cypher is reversible, so don't use this on a
    machine you don't trust to be secure.)

    synservertrigger add sluisp @somedomain.org
    (Adds the 'sluisp@somedomain.org' to the recipients of warning emails
    regarding synchronization.)

Remarks:
    Emails are only sent when the synchronization process failed. No news is
    good news!";
    }
}
