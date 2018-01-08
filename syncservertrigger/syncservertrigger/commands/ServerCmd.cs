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

            Console.Error.WriteLine(Help);
            Environment.Exit(1);
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
            toolConfig.ServerConfig.DeleteServer(server);
            toolConfig.Save();

            Console.WriteLine("Server '{0}' correctly deleted!", server);
        }

        const string HELP =
@"server          Used to operate on the remote servers synchronization
                configuration.

Usage:
    server <list | <add|delete> dst_server>

    list        Lists the configured destination servers.
    add         Adds 'dst_server' to the list of destination servers.
    delete      Deletes 'dst_server' from the list of destination servers.

Examples:
    syncservertrigger server list
    (Shows a list of the configured destination servers.)

    syncservertrigger server add ssl://diana.mydomain:8088
    (Adds the specified server to the list of destination servers.)

    syncservertrigger server remove ssl://diana.mydomain:8088
    (Removes the specified server from the list of destination servers.)

Remarks:
    By default, every repository on the source server is expected to exist with
    the same name on the destination server.If that is not the case, check the
    'repomap' command.

    By default, every repository on the source server is synced with the remote
    server. If that is not the desired behavior, check the 'repofilter' command.";
    }
}
