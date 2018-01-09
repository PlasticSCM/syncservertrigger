using System;
using System.Diagnostics;

namespace Codice.SyncServerTrigger.Commands
{
    internal class TriggerCmd : ICmd
    {
        public string Help { get { return HELP; } }

        public string CommandName { get { return "trigger"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length == 1 || args.Length > 2)
            {
                Console.Error.WriteLine(Help);
                Environment.Exit(1);
            }

            Process process = new Process();
            process.StartInfo.FileName = Utils.GetAssemblyLocation();
            process.StartInfo.Arguments = string.Format("run {0}", args[1]);
#if !DEBUG
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
#endif
            process.Start();
        }

        const string HELP =
@"This command is intended to be directly run by the Plastic SCM server. Run
this command manually only for debug purposes.";
    }
}
