using System;
using System.Collections.Generic;

namespace Codice.SyncServerTrigger.Models
{
    internal class Replica
    {
        internal string SrcBranch { get; private set; }
        internal string SrcRepo { get; private set; }
        internal string SrcServer { get; private set; }
        internal string DstRepo { get; private set; }
        internal string DstServer { get; private set; }

        internal Replica(
            string srcBranch,
            string srcRepo,
            string srcServer,
            string dstRepo,
            string dstServer)
        {
            SrcBranch = srcBranch;
            SrcRepo = srcRepo;
            SrcServer = srcServer;
            DstRepo = dstRepo;
            DstServer = dstServer;
        }

        internal static List<Replica> BuildPendingReplicas(
            string srcBranch,
            string srcRepo,
            string srcServer,
            List<string> filteredRepos,
            List<string> dstServers,
            List<RepoMapping> repoMappings)
        {
            List<Replica> result = new List<Replica>();

            if (filteredRepos.Contains(srcRepo))
                return result;

            foreach (string server in dstServers)
            {
                result.Add(new Replica(
                    srcBranch,
                    srcRepo,
                    srcServer,
                    GetDstRepo(srcRepo, server, repoMappings),
                    server));
            }

            return result;
        }

        static string GetDstRepo(
            string srcRepo, string dstServer, List<RepoMapping> repoMappings)
        {
            foreach (RepoMapping mapping in repoMappings)
            {
                if (!dstServer.Equals(
                    mapping.DstServer,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (!srcRepo.Equals(
                    mapping.SrcRepo,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                return mapping.DstRepo;
            }

            return srcRepo;
        }
    }
}
