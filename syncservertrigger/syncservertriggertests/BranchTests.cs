using Microsoft.VisualStudio.TestTools.UnitTesting;

using Codice.SyncServerTrigger.Models;

namespace syncservertriggertests
{
    [TestClass]
    public class BranchTests
    {
        [TestMethod]
        public void ParsePlasticBranchEnvironTest()
        {
            Branch branch = Branch.ParsePlasticBranchEnvironVar(
                "/main/scm29545@rep:codice@skull.factoria.com:9095");

            Assert.AreEqual(
                "/main/scm29545", branch.BranchName,
                "The BranchName does not match the expected one.");

            Assert.AreEqual(
                "codice", branch.RepositoryName,
                "The RepositoryName does not match the expected one.");

            Assert.AreEqual(
                "skull.factoria.com:9095", branch.ServerName,
                "The ServerName does not match the expected one.");
        }
    }
}
