using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Codice.SyncServerTrigger.Configuration;

namespace Codice.SyncServerTrigger.Commands
{
    internal class RepofilterCmd : ICmd
    {
        public string Help { get { return HELP; } }

        public string CommandName { get { return "repofilter"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length == 1
                || args.Length >= 2 && args[1].Contains("help"))
            {
                Console.Error.WriteLine(Help);
                Environment.Exit(1);
            }

            if (args.Length == 2 && args[1] == "list")
            {
                ListRepoFilters();
                return;
            }

            if (args.Length == 3 && args[1] == "add")
            {
                AddRepoFilter(args[2]);
                return;
            }

            if (args.Length == 3 && args[1] == "delete")
            {
                DeleteRepoFilter(args[2]);
                return;
            }

            Console.Error.WriteLine(Help);
            Environment.Exit(1);
        }

        void ListRepoFilters()
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();
            List<string> filters = toolConfig.RepoFilterConfig.GetFilters();

            if (filters.Count == 0)
            {
                Console.WriteLine("There are no configured repo filters.");
                return;
            }

            filters.ForEach(repoFilter => Console.WriteLine(repoFilter));
        }

        void AddRepoFilter(string repo)
        {
            Console.WriteLine("Adding repository '{0}' to the filter list.", repo);

            ToolConfiguration toolConfig = ToolConfiguration.Load();
            toolConfig.RepoFilterConfig.AddRepository(repo);
            toolConfig.Save();

            Console.WriteLine("Repository '{0}' correctly added!", repo);
        }

        void DeleteRepoFilter(string repo)
        {
            Console.WriteLine("Deleting repository '{0}' from the filter list.", repo);

            ToolConfiguration toolConfig = ToolConfiguration.Load();
            toolConfig.RepoFilterConfig.DeleteRepository(repo);
            toolConfig.Save();

            Console.WriteLine("Repository '{0}' correctly deleted!");
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
