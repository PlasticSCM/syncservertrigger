using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Codice.SyncServerTrigger.Models;

namespace syncservertriggertests
{
    [TestClass]
    public class ReplicaTests
    {
        [TestMethod]
        public void BuildPendingReplicasForFilteredRepoTest()
        {
            string srcBranch = "br:/main/task-001";
            string srcRepo = "default";
            string srcServer = "JAIR:8087";

            List<string> filteredRepos = new List<string>() { "default" };
            List<string> dstServers = new List<string>()
            {
                "ssl://myserver.mydomain.com:9095",
                "calavera.home:7074"
            };
            List<RepoMapping> repoMappings = new List<RepoMapping>();

            List<Replica> pendingReplicas =
                Replica.BuildPendingReplicas(
                    srcBranch,
                    srcRepo,
                    srcServer,
                    filteredRepos,
                    dstServers,
                    repoMappings);

            Assert.AreEqual(0, pendingReplicas.Count);
        }

        [TestMethod]
        public void BuildPendingReplicasForMappedRepoTest()
        {
            string srcBranch = "br:/main/task-001";
            string srcRepo = "default";
            string srcServer = "JAIR:8087";

            List<string> filteredRepos = new List<string>();
            List<string> dstServers = new List<string>()
            {
                "ssl://myserver.mydomain.com:9095",
                "calavera.home:7074"
            };
            List<RepoMapping> repoMappings = new List<RepoMapping>();
            repoMappings.Add(new RepoMapping(
                srcRepo, "default_myserver", dstServers[0]));
            repoMappings.Add(new RepoMapping(
                srcRepo, "default_calavera", dstServers[1]));

            List<Replica> pendingReplicas =
                Replica.BuildPendingReplicas(
                    srcBranch,
                    srcRepo,
                    srcServer,
                    filteredRepos,
                    dstServers,
                    repoMappings);

            Assert.AreEqual(2, pendingReplicas.Count);

            Replica firstExpectedReplica = new Replica(
                srcBranch, srcRepo, srcServer, "default_myserver", dstServers[0]);

            Replica secondExpectedReplica = new Replica(
                srcBranch, srcRepo, srcServer, "default_calavera", dstServers[1]);

            AssertReplicasEqual(firstExpectedReplica, pendingReplicas[0]);
            AssertReplicasEqual(secondExpectedReplica, pendingReplicas[1]);
        }

        [TestMethod]
        public void BuildPendingReplicasForRepositoryTest()
        {
            string srcBranch = "br:/main/task-001";
            string srcRepo = "default";
            string srcServer = "JAIR:8087";

            List<string> filteredRepos = new List<string>();
            List<string> dstServers = new List<string>()
            {
                "ssl://myserver.mydomain.com:9095",
                "calavera.home:7074"
            };
            List<RepoMapping> repoMappings = new List<RepoMapping>();

            List<Replica> pendingReplicas =
                Replica.BuildPendingReplicas(
                    srcBranch,
                    srcRepo,
                    srcServer,
                    filteredRepos,
                    dstServers,
                    repoMappings);

            Assert.AreEqual(2, pendingReplicas.Count);

            Replica firstExpectedReplica = new Replica(
                srcBranch, srcRepo, srcServer, srcRepo, dstServers[0]);

            Replica secondExpectedReplica = new Replica(
                srcBranch, srcRepo, srcServer, srcRepo, dstServers[1]);

            AssertReplicasEqual(firstExpectedReplica, pendingReplicas[0]);
            AssertReplicasEqual(secondExpectedReplica, pendingReplicas[1]);
        }

        void AssertReplicasEqual(Replica left, Replica right)
        {
            Assert.AreEqual(
                left.SrcBranch, right.SrcBranch,
                "The SrcBranch property does not match.");

            Assert.AreEqual(
                left.SrcRepo, right.SrcRepo,
                "The SrcRepo property does not match.");

            Assert.AreEqual(
                left.SrcServer, right.SrcServer,
                "The SrcServer property does not match.");

            Assert.AreEqual(
                left.DstRepo, right.DstRepo,
                "The DstRepo property does not match.");

            Assert.AreEqual(
                left.DstServer, right.DstServer,
                "The DstServer property does not match.");
        }
    }
}
