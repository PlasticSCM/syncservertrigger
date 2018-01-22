using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Codice.SyncServerTrigger.Models;

namespace CodiceSyncServerTriggerTests
{
    [TestClass]
    public class RepoMappingTests
    {
        [TestMethod]
        public void ParseRepoMappingTest()
        {
            string srcRepo = "default";
            string dstRepo = "default_dst";
            string dstServer = "skull.home:8087";

            RepoMapping repoMapping =
                new RepoMapping(srcRepo, dstRepo, dstServer);

            List<string> configuration =
                new List<string>() { repoMapping.ToConfigurationString() };

            List<RepoMapping> parsedRepoMappings =
                RepoMapping.ParseFromConfiguration(configuration);

            Assert.AreEqual(1, parsedRepoMappings.Count);

            AssertRepoMappingEqual(repoMapping, parsedRepoMappings[0]);
        }

        void AssertRepoMappingEqual(RepoMapping left, RepoMapping right)
        {
            Assert.AreEqual(
                left.SrcRepo, right.SrcRepo,
                "The SrcRepo property does not match");

            Assert.AreEqual(
                left.DstRepo, right.DstRepo,
                "The DstRepo property does not match.");

            Assert.AreEqual(
                left.DstServer, right.DstServer,
                "The DstServer property does not match");
        }
    }
}
