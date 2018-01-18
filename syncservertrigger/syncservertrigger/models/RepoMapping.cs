using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Codice.SyncServerTrigger.Models
{
    internal class RepoMapping
    {
        internal string SrcRepo { get; private set; }
        internal string DstRepo { get; private set; }
        internal string DstServer { get; private set; }

        internal RepoMapping(
            string srcRepo, string dstRepo, string dstServer)
        {
            SrcRepo = srcRepo;
            DstRepo = dstRepo;
            DstServer = dstServer;
        }

        internal string ToConfigurationString()
        {
            return string.Format(FORMAT_STR, SrcRepo, DstRepo, DstServer);
        }

        internal static List<RepoMapping> ParseFromConfiguration
            (List<string> configuration)
        {
            List<RepoMapping> result = new List<RepoMapping>();

            foreach (string configLine in configuration)
            {
                RepoMapping repoMapping = ParseFromLine(configLine);
                if (repoMapping != null)
                    result.Add(repoMapping);
            }

            return result;
        }

        internal static RepoMapping ParseFromLine(string line)
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
