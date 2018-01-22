using System.Collections.Generic;

using Codice.SyncServerTrigger.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace syncservertriggertests
{
    [TestClass]
    public class ChangesetTests
    {
        [TestMethod]
        public void ParseSingleChangesetSpec()
        {
            string plasticChangesetEnvironVar =
                "cs:19@br:/main/test2/@rep:codice@repserver:skull.mydomain.com:9095";

            Changeset expectedChangeset = new Changeset(
                19,
                "/main/test2/",
                "codice",
                "skull.mydomain.com:9095");

            List<Changeset> changesets =
                Changeset.ParsePlasticChangesetEnvironVar(plasticChangesetEnvironVar);

            Assert.AreEqual(1, changesets.Count);

            AssertChangesetEquals(expectedChangeset, changesets[0]);
        }

        [TestMethod]
        public void ParseMultipleChangesetSpecs()
        {
            string plasticChangesetEnvironVar =
                "cs:19@br:/main/test2/@rep:codice@repserver:skull.mydomain.com:9095; " +
                "cs:21@br:/main/test2@rep:codice/osc@repserver:skull.mydomain.com:9095;" +
                "cs:32@br:/main@rep:xlinked_repo@repserver:myrepo.local:8086";

            Changeset firstExpectedChangeset = new Changeset(
                19,
                "/main/test2/",
                "codice",
                "skull.mydomain.com:9095");

            Changeset secondExpectedChangeset = new Changeset(
                21,
                "/main/test2",
                "codice/osc",
                "skull.mydomain.com:9095");

            Changeset thirdExpectedChangeset = new Changeset(
                32,
                "/main",
                "xlinked_repo",
                "myrepo.local:8086");

            List<Changeset> changesets =
                Changeset.ParsePlasticChangesetEnvironVar(plasticChangesetEnvironVar);

            Assert.AreEqual(3, changesets.Count);

            AssertChangesetEquals(firstExpectedChangeset, changesets[0]);
            AssertChangesetEquals(secondExpectedChangeset, changesets[1]);
            AssertChangesetEquals(thirdExpectedChangeset, changesets[2]);
        }

        static void AssertChangesetEquals(Changeset left, Changeset right)
        {
            Assert.AreEqual(
                left.CsetId, right.CsetId,
                "The CsetId property does not match");

            Assert.AreEqual(
                left.BranchName, right.BranchName,
                "The BranchName property does not match");

            Assert.AreEqual(
                left.RepositoryName, right.RepositoryName,
                "The RepositoryName property does not match");

            Assert.AreEqual(
                left.ServerName, right.ServerName,
                "The ServerName property does not match");
        }
    }
}
