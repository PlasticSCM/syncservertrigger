using System;

using Codice.CmdRunner;

namespace Codice.SyncServerTrigger.Commands
{
    internal class InstallCmd : ICmd
    {
        public string Help { get { return HELP; } }

        public string CommandName { get { return "install"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine(HELP);
                return;
            }

            string srcServer = args[1];
            string dstServer = args[2];

            string executablePath = Utils.GetAssemblyLocation();
        }

        static string GetCommandLine(
            string triggerType, string triggerName, string executablePath, string[] args)
        {
            return string.Format("cm maketrigger {0} \"{1}\" \"{2} {3}\"",
                triggerType,
                triggerName,
                executablePath,
                string.Join(" ", args));
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
