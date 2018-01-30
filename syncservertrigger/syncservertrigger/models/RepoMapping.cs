using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Codice.SyncServerTrigger.Models
{
    public class RepoMapping
    {
        public string SrcRepo { get; private set; }
        public string DstRepo { get; private set; }
        public string DstServer { get; private set; }

        public RepoMapping(
            string srcRepo, string dstRepo, string dstServer)
        {
            SrcRepo = srcRepo;
            DstRepo = dstRepo;
            DstServer = dstServer;
        }

        public string ToConfigurationString()
        {
            return string.Format(FORMAT_STR, SrcRepo, DstRepo, DstServer);
        }

        public static List<RepoMapping> ParseConfiguration(
            List<string> configuration)
        {
            List<RepoMapping> result = new List<RepoMapping>();

            foreach (string configLine in configuration)
            {
                RepoMapping repoMapping = FromConfigurationString(configLine);
                if (repoMapping != null)
                    result.Add(repoMapping);
            }

            return result;
        }

        public static RepoMapping FromConfigurationString(string line)
        {
            Match match = CONFIG_REGEX.Match(line);

            if (!match.Success)
                return null;

            return new RepoMapping(
                match.Groups["srcRepo"].Value,
                match.Groups["dstRepo"].Value,
                match.Groups["dstServer"].Value);
        }

        static readonly Regex CONFIG_REGEX = new Regex(
            @"(?<srcRepo>.+)#(?<dstRepo>.+)\@(?<dstServer>.+)");

        const string FORMAT_STR = "{0}#{1}@{2}";
    }
}
