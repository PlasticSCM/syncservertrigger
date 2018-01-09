using System;

namespace Codice.SyncServerTrigger.Commands
{
    internal class RunCmd : ICmd
    {
        public string Help { get { return HELP; } }

        public string CommandName { get { return "run"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length == 1 || args.Length > 2)
            {
                Console.Error.WriteLine(Help);
                Environment.Exit(1);
            }

            while (true)
            {
                Console.WriteLine(args[1]);
                System.Threading.Thread.Sleep(1000);
            }
        }

        const string HELP =
@"This command is intended to be directly run by the syncservertrigger program
itself. Run this command manually only for debug purposes.";
    }
}
