using System;
using System.IO;

using Codice.SyncServerTrigger.Configuration;

namespace Codice.SyncServerTrigger.Commands
{
    public class LogCmd : ICmd
    {
        string ICmd.Help { get { return HELP; } }

        string ICmd.CommandName { get { return "log"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length == 1 || args.Length > 3)
            {
                Console.Error.WriteLine(HELP);
                Environment.Exit(1);
            }

            if (args[1] == "enable")
            {
                EnableLoging(args.Length == 3 ? args[2] : string.Empty);
                return;
            }

            if (args[1] == "disable")
            {
                DisableLogging();
                return;
            }

            if (args[1] == "status")
            {
                ShowLoggingStatus();
                return;
            }
            
            Console.Error.WriteLine(HELP);
            Environment.Exit(1);
        }

        void EnableLoging(string destinationDirectory)
        {
            Console.WriteLine("Enabling logging.");

            if (!string.IsNullOrEmpty((destinationDirectory)))
            {
                if (File.Exists(destinationDirectory))
                {
                    Console.Error.WriteLine("{0} is a file, not a directory!");
                    Environment.Exit(1);
                }
                
                EnsureDirectoryExists(destinationDirectory);
            }

            ToolConfiguration toolConfig = ToolConfiguration.Load();

            LoggingConfiguration loggingConfig = toolConfig.LoggingConfig;
            loggingConfig.Enabled = true;
            loggingConfig.DestinationPath = destinationDirectory;
            toolConfig.Save();
            
            Console.WriteLine(
                "Logging enabled. Logs will be saved to {0}.",
                loggingConfig.DestinationPath);
            
        }

        void DisableLogging()
        {
            Console.WriteLine("Disabling logging.");
            
            ToolConfiguration toolConfig = ToolConfiguration.Load();
            toolConfig.LoggingConfig.Enabled = false;
            toolConfig.Save();

            Console.WriteLine("Logging disabled.");
        }

        void ShowLoggingStatus()
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();
            LoggingConfiguration loggingConfig = toolConfig.LoggingConfig;
            
            Console.WriteLine(toolConfig.LoggingConfig.Enabled
                ? "Logging enabled."
                : "Logging disabled.");
            
            Console.WriteLine(
                "Logs destination path: {0}",
                toolConfig.LoggingConfig.DestinationPath);
        }

        static void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path))
                return;

            Directory.CreateDirectory(path);
        }
        

        const string HELP =
@"log             Used to enable or disable the replication log, useful to
                debug errors in the replication process.

Usage:
    log <enable [path]| disable | status>

    enable      Enables the logging.
    path        The destination directory to where the logs should be saved.
    disable     Disables the logging.
    status      Shows the current status of the logging.

Examples:
    log enable
    (Enables the logging. By default, the log will be saved in the HOME of
    the user.)

    log enable /home/sluisp/logs/
    (Enables the logging, and sets the specified directory as the destination
    directory of the logs.)

    log disable
    (Disables the logging.)

    log status
    (Prints the status of the logging -enabled/disabled- and the directory
    where it is set to be saved.)";
    }
}