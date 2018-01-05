using System;
using System.Collections.Generic;

using Codice.SyncServerTrigger.Configuration;

namespace Codice.SyncServerTrigger.Commands
{
    internal class ServerCmd : ICmd
    {
        public string Help { get { return HELP; } }

        public string CommandName { get { return "server"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length == 1
                || (args.Length >= 2 && args[1].Contains("help")))
            {
                Console.Error.WriteLine(Help);
                Environment.Exit(1);
            }

            if (args.Length == 2 && args[1] == "list")
            {
                ListServers();
                return;
            }

            if (args.Length == 3 && args[1] == "add")
            {
                AddServer(args[2]);
                return;
            }

            if (args.Length == 3 && args[1] == "delete")
            {
                DeleteServer(args[2]);
                return;
            }
        }

        void ListServers()
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();
            List<string> servers = toolConfig.ServerConfig.GetServers();

            if (servers.Count == 0)
            {
                Console.WriteLine("There are no configured destination servers.");
                return;
            }

            servers.ForEach(server => Console.WriteLine(server));
        }

        void AddServer(string server)
        {
            Console.WriteLine("Adding server '{0}'.", server);

            ToolConfiguration toolConfig = ToolConfiguration.Load();
            toolConfig.ServerConfig.AddServer(server);
            toolConfig.Save();

            Console.WriteLine("Server '{0}' correctly added!", server);
        }

        void DeleteServer(string server)
        {
            Console.WriteLine("Deleting server '{0}'", server);

            ToolConfiguration toolConfig = ToolConfiguration.Load();
            toolConfig.ServerConfig.RemoveServer(server);
            toolConfig.Save();

            Console.WriteLine("Server '{0}' correctly removed!", server);
        }

        const string HELP = "";
    }
}
