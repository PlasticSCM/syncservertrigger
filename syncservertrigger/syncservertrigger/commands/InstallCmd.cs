using System;

namespace Codice.SyncServerTrigger.Commands
{
    internal static class TriggerNames
    {
        internal const string AfterCi = "sync afterci";
        internal const string AfterReplication = "sync afterreplication";
        internal const string AfterMkLabel = "sync aftermklabel";
        internal const string AfterChAtt = "sync afterchatt";
    }

    internal class InstallCmd : ICmd
    {
        public string Help { get { return HELP; } }

        public string CommandName { get { return "install"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length >= 2 && args[1].Contains("help"))
            {
                Console.Error.WriteLine(Help);
                Environment.Exit(1);
            }

            if (args.Length != 3)
            {
                Console.Error.WriteLine(HELP);
                Environment.Exit(1);
            }

            string srcServer = args[1];
            string dstServer = args[2];

            if (!Utils.CheckServerSpec(srcServer))
            {
                Console.Error.WriteLine(
                    "The server spec is not correct: {0}", srcServer);
                Environment.Exit(1);
            }

            if (!Utils.CheckServerSpec(dstServer))
            {
                Console.Error.WriteLine(
                    "The server spec is not correct: {0}", dstServer);
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

            if (!InstallTrigger("after-checkin", TriggerNames.AfterCi, executablePath, srcServer)
                || !InstallTrigger("after-replicationwrite", TriggerNames.AfterReplication, executablePath, srcServer)
                || !InstallTrigger("after-mklabel", TriggerNames.AfterMkLabel, executablePath, srcServer)
                || !InstallTrigger("after-chattvalue", TriggerNames.AfterChAtt, executablePath, srcServer))
            {
                Environment.Exit(1);
            }

            Console.WriteLine(
                "Triggers successfully installed in {0}!", srcServer);
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

            Console.WriteLine("Installing trigger '{0}' (type '{1}').",
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
                server,
                Environment.NewLine,
                commandLine,
                output,
                error);
        }

        static string GetCommandLine(
            string triggerType, string triggerName, string executablePath, string server)
        {
            return string.Format(
                "cm maketrigger {0} \"{1}\" \"{2} trigger {0}\" --server={3}",
                triggerType,
                triggerName,
                executablePath,
                server);
        }

        const string HELP =
@"install         Autoinstalls the required triggers to have the two servers in
                sync.

Usage: 
    install <src_server> <dst_server>
    
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
