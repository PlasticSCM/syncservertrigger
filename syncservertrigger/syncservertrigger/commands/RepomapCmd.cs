using System;
using System.Collections.Generic;

using Codice.SyncServerTrigger.Configuration;
using Codice.SyncServerTrigger.Models;

namespace Codice.SyncServerTrigger.Commands
{
    internal class RepomapCmd : ICmd
    {
        public string Help { get { return HELP; } }

        public string CommandName { get { return "repomap"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length <= 1
                || args.Length >= 3
                || args[1].Contains("help"))
            {
                Console.Error.WriteLine(Help);
                Environment.Exit(1);
            }

            if (args.Length == 2 && args[1] == "list")
            {
                ListRepoMaps();
                return;
            }

            if (args.Length == 5 && args[1] == "add")
            {
                AddRepoMap(args[2], args[3], args[4]);
                return;
            }

            if (args.Length == 5 && args[1] == "delete")
            {
                DeleteRepoMap(args[2], args[3], args[4]);
                return;
            }

            Console.Error.WriteLine(Help);
            Environment.Exit(1);
        }

        void ListRepoMaps()
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();
            List<RepoMapping> mappings = toolConfig.RepoMapConfig.GetMappedRepos();

            if (mappings.Count == 0)
            {
                Console.WriteLine("There are no configured repo mappings.");
                return;
            }

            mappings.ForEach(
                repoMapping => Console.WriteLine(
                    "{0} -> {1}@{2}",
                    repoMapping.SrcRepo,
                    repoMapping.DstRepo,
                    repoMapping.DstServer));
        }

        void AddRepoMap(string srcRepo, string dstRepo, string dstServer)
        {
            RepoMapping mapping = new RepoMapping(srcRepo, dstRepo, dstServer);

            Console.WriteLine(
                "Adding repository mapping '{0}' -> '{1}@{2}'",
                mapping.SrcRepo,
                mapping.DstRepo,
                mapping.DstServer);

            ToolConfiguration toolConfig = ToolConfiguration.Load();
            toolConfig.RepoMapConfig.AddMappedRepo(mapping);
            toolConfig.Save();

            Console.WriteLine("Repository mapping correctly added!");
        }

        void DeleteRepoMap(string srcRepo, string dstRepo, string dstServer)
        {
            RepoMapping mapping = new RepoMapping(srcRepo, dstRepo, dstServer);

            Console.WriteLine(
                "Deleting repository mapping '{0}' -> '{1}@{2}'",
                mapping.SrcRepo,
                mapping.DstRepo,
                mapping.DstServer);

            ToolConfiguration toolConfig = ToolConfiguration.Load();
            toolConfig.RepoMapConfig.DeleteMappedRepo(mapping);
            toolConfig.Save();

            Console.WriteLine("Repository mapping correctly deleted!");
        }

        const string HELP =
@"repomap         Used to operate on the local repositories mapping with remote
                repositories.

Usage:
    repomap <list | <add|delete> <src_repo> <dst_repo> <dst_server>>

    list        Lists the repository name mappings.
    add         Adds a new mapping to the list.
    delete      Deletes the specified mapping from the list.
    src_repo    The name of the repository in the source server.
    dst_repo    The name of the repostiroy in the destination server.
    dst_server  The destination server, including port.

Examples:
    syncservertrigger repomap add codice_local codice @skull:9097
    (Maps the 'codice_local' source repository to the 'codice' destination
    repository only for the 'skull:9097' destination server.)

    synservertrigger repomap remove codice_local codice @skull:9097
    (Removes the mapping between the source repository 'codice_local' and the
    destination repository 'codice' only for the 'skull:9097' destination server.)

    synservertrigger repomap list
    (Shows a list of the configured mappings.)

Remarks:
    By default, synservertrigger expect your source and destination
    repositories to be named the same.Use this command only if that is not the
    case.

    The 'dst_server' param must match one of the servers in the
    configuration, use the 'server list' command to list the destination servers.";
    }
}
