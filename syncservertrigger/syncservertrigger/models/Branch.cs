using System.Text.RegularExpressions;

namespace Codice.SyncServerTrigger.Models
{
    public class Branch
    {
        public string BranchName { get; private set; }
        public string RepositoryName { get; private set; }
        public string ServerName { get; private set; }

        public Branch(
            string branchName, string repositoryName, string serverName)
        {
            BranchName = branchName;
            RepositoryName = repositoryName;
            ServerName = serverName;
        }

        public static Branch ParsePlasticBranchEnvironVar(
            string environVarValue)
        {
            if (string.IsNullOrEmpty(environVarValue))
                return null;

            Match match = SPEC_REGEX.Match(environVarValue);
            if (!match.Success)
                return null;

            return new Branch(
                match.Groups["brName"].Value,
                match.Groups["repName"].Value,
                match.Groups["serverName"].Value);
        }

        static readonly Regex SPEC_REGEX = new Regex(
            @"(?<brName>.+)\@rep:(?<repName>.+)\@(?<serverName>.+)");
    }
}
