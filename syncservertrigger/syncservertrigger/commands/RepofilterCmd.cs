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

        const string HELP = "";
    }
}
