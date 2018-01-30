using System;
using System.Collections.Generic;

using Codice.SyncServerTrigger.Configuration;

namespace Codice.SyncServerTrigger.Commands
{
    internal class RepofilterCmd : ICmd
    {
        string ICmd.Help { get { return HELP; } }

        string ICmd.CommandName { get { return "repofilter"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length == 1 || args.Length > 3)
            {
                Console.Error.WriteLine(HELP);
                Environment.Exit(1);
            }

            if (args.Length == 2 && args[1] == "list")
            {
                ListFilteredRepos();
                return;
            }

            if (args.Length == 3 && args[1] == "add")
            {
                AddFilteredRepo(args[2]);
                return;
            }

            if (args.Length == 3 && args[1] == "delete")
            {
                DeleteFilteredRepo(args[2]);
                return;
            }

            Console.Error.WriteLine(HELP);
            Environment.Exit(1);
        }

        void ListFilteredRepos()
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();
            List<string> filteredRepos =
                toolConfig.RepoFilterConfig.GetFilteredRepos();

            if (filteredRepos.Count == 0)
            {
                Console.WriteLine("There are no configured repo filters.");
                return;
            }

            filteredRepos.ForEach(repoFilter => Console.WriteLine(repoFilter));
        }

        void AddFilteredRepo(string repo)
        {
            Console.WriteLine("Adding repository '{0}' to the filter list.", repo);

            ToolConfiguration toolConfig = ToolConfiguration.Load();
            toolConfig.RepoFilterConfig.AddFilteredRepo(repo);
            toolConfig.Save();

            Console.WriteLine("Repository '{0}' correctly added!", repo);
        }

        void DeleteFilteredRepo(string repo)
        {
            Console.WriteLine("Deleting repository '{0}' from the filter list.", repo);

            ToolConfiguration toolConfig = ToolConfiguration.Load();
            toolConfig.RepoFilterConfig.DeleteFilteredRepo(repo);
            toolConfig.Save();

            Console.WriteLine("Repository '{0}' correctly deleted!", repo);
        }

        const string HELP =
@"repofilter      Used to operate on the local repositories synchronization
                configuration.

Usage:
    repofilter <list | <delete|add> repository>


    list        Lists the repositories that will be pushed to the destination
                servers. These are your local repositories, without the ones you
                manually removed.
    delete      Deletes a repository from the filter list.
    add         Adds a repository to the filter list.
    repository  The name of the repository on the source server.

Examples:
    syncservertrigger repofilter add codice_local
    (Adds the 'codice_local' repository to the list of filtered-out repositories,
    so it won't be synced with any of the destination servers.)

    syncservertrigger repofilter remove codice_local
    (Removes the 'codice_local' repository from the filtered-out repositories,
    so it will be synced with the destination servers.)

Remarks:
    By default, all of the local repositories are synchronized,
    including new ones.You have to manually add a repository to the filter list
    if you don't want it to be synced. The 'remove' command only applies for
    filtered repositories, to re-admit them in the synchronization process.";
    }
}
