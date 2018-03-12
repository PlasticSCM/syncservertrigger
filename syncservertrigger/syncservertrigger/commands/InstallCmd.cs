using System;

using Codice.SyncServerTrigger.Configuration;
using Codice.SyncServerTrigger.Models;

namespace Codice.SyncServerTrigger.Commands
{
    internal class InstallCmd : ICmd
    {
        string ICmd.Help { get { return HELP; } }

        string ICmd.CommandName { get { return "install"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length == 1 || args.Length > 3)
            {
                Console.Error.WriteLine(HELP);
                Environment.Exit(1);
            }

            string srcServer = args[1];
            if (!Utils.CheckServerSpec(srcServer))
            {
                Console.Error.WriteLine(
                    "The server spec is not correct: {0}", srcServer);
                Environment.Exit(1);
            }

            Console.WriteLine(
                "Installing the necessary triggers on server {1}:{0}" +
                "* after-checkin{0}" +
                "* after-replicationwrite{0}" +
                "* after-mklabel{0}" +
                "* after-chattvalue",
                Environment.NewLine,
                srcServer);

            string executablePath = Utils.GetAssemblyLocation();

            Console.WriteLine(
                "Using syncservertrigger located at {0}.",
                executablePath);

            if (!PlatformUtils.IsWindows)
                InitializeMonoRuntimePath();

            if (!InstallTrigger(Trigger.Types.AfterCi, Trigger.Names.AfterCi, executablePath, srcServer)
                || !InstallTrigger(Trigger.Types.AfterRW, Trigger.Names.AfterRW, executablePath, srcServer)
                || !InstallTrigger(Trigger.Types.AfterMkLb, Trigger.Names.AfterMkLb, executablePath, srcServer)
                || !InstallTrigger(Trigger.Types.AfterChAttVal, Trigger.Names.AfterChAttVal, executablePath, srcServer))
            {
                Environment.Exit(1);
            }

            Console.WriteLine(
                "Triggers successfully installed in {0}!", srcServer);

            if (args.Length != 3)
                return;

            string dstServer = args[2];
            if (!Utils.CheckServerSpec(dstServer))
            {
                Console.Error.WriteLine(
                    "The server spec is not correct: {0}",
                    dstServer);
                Environment.Exit(1);
            }

            Console.WriteLine(
                "Adding '{0}' as the first destination server.",
                dstServer);

            ToolConfiguration toolConfig = ToolConfiguration.Load();
            toolConfig.ServerConfig.AddServer(dstServer);
            toolConfig.Save();

            Console.WriteLine(
                "Server '{0}' correctly added!",
                dstServer);
        }

        static void InitializeMonoRuntimePath()
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();
            toolConfig.RuntimeConfig.MonoRuntimePath =
                ConsoleUtils.ReadLine(
                    "Enter Mono runtime path",
                    PlatformUtils.DefaultMonoRuntimePath);

            toolConfig.Save();
        }

        static bool InstallTrigger(
            string triggerType,
            string triggerName,
            string executablePath,
            string server)
        {
            int result;
            string stdOut, stdErr;

            string cmdLine = GetCommandLine(
                triggerType, triggerName, executablePath, server);

            Console.WriteLine(
                "Installing trigger '{0}' (type '{1}').",
                triggerName,
                triggerType);

            result = CmdRunner.CmdRunner.ExecuteCommandWithResult(
                cmdLine,
                Environment.CurrentDirectory,
                out stdOut,
                out stdErr,
                false);

            if (result == 0)
                return true;

            WriteError(
                cmdLine,
                server,
                stdOut,
                stdErr);

            return false;
        }

        static void WriteError(
            string commandLine, string server, string output, string error)
        {
            Console.Error.WriteLine(
                "Installing a trigger in {1} failed.{0}" +
                "Command line: '{2}'{0}" +
                "cm stdout: {3}{0}" +
                "cm stderr: {4}",
                Environment.NewLine,
                server,
                commandLine,
                output,
                error);
        }

        static string GetCommandLine(
            string triggerType, string triggerName, string executablePath, string server)
        {
            if (PlatformUtils.IsWindows)
            {
                return string.Format(
                    "cm maketrigger {0} \"{1}\" \"{2} trigger {0}\" --server={3}",
                    triggerType,
                    triggerName,
                    executablePath,
                    server);
            }

            ToolConfiguration toolConfig = ToolConfiguration.Load();

            return string.Format(
                "cm maketrigger {0} \"{1}\" \"{2} {3} trigger {0}\" --server={4}",
                triggerType,
                triggerName,
                toolConfig.RuntimeConfig.MonoRuntimePath,
                executablePath,
                server);
        }

        const string HELP =
@"install         Autoinstalls the required triggers to have the two servers in
                sync.

Usage: 
    install <src_server> [dst_server]
    
    src_server  The source repository, where the server-side triggers will
                actually be installed.
    dst_server  The destination repository, where the changes will be pushed to.

Examples:
    synservertrigger install localhost:8087 skull:9097
    (Installs the server-side triggers on the 'localhost:8087' server and adds
    'skull:9097' as the first destination repository in the configuration.)

Remarks:
    This command must be executed only for the first destination repository.
    If you want to have multiple destination repositories, check the 'server add'
    command.

    By default, every repository on the source server is expected to exist with
    the same name on the destination server.If that is not the case, check the
    'repomap' command.

    By default, every repository on the source server is synced with the remote
    server.If that is not the desired behavior, check the 'repofilter' command.";
    }
}
