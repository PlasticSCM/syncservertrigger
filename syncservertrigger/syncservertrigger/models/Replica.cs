using System;
using System.Collections.Generic;

namespace Codice.SyncServerTrigger.Models
{
    public class Replica
    {
        public string SrcBranch { get; private set; }
        public string SrcRepo { get; private set; }
        public string SrcServer { get; private set; }
        public string DstRepo { get; private set; }
        public string DstServer { get; private set; }

        public Replica(
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

        public static List<Replica> BuildPendingReplicas(
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
