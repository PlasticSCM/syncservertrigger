using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Codice.SyncServerTrigger.Models
{
    internal class Changeset
    {
        internal double CsetId { get; private set; }
        internal string BranchName { get; private set; }
        internal string RepositoryName { get; private set; }
        internal string ServerName { get; private set; }

        internal Changeset(
            double csetId,
            string branchName,
            string repositoryName,
            string serverName)
        {
            CsetId = csetId;
            BranchName = branchName;
            RepositoryName = repositoryName;
            ServerName = serverName;
        }

        internal static List<Changeset> ParsePlasticChangesetEnvironVar(
            string environVarValue)
        {
            if (string.IsNullOrEmpty(environVarValue))
                return new List<Changeset>();

            string[] csetSpecs = environVarValue.Split(
                    CSET_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);

            List<Changeset> result = new List<Changeset>();
            foreach (string csetSpec in csetSpecs)
            {
                Changeset parsedCset = ParseChangesetSpec(csetSpec);
                if (parsedCset != null)
                    result.Add(parsedCset);
            }

            return result;
        }

        static Changeset ParseChangesetSpec(string csetSpec)
        {
            Match match = SPEC_REGEX.Match(csetSpec);
            if (!match.Success)
                return null;

            return new Changeset(
                double.Parse(match.Groups["id"].Value),
                match.Groups["branchName"].Value,
                match.Groups["repName"].Value,
                match.Groups["repServer"].Value);
        }

        static readonly char[] CSET_SEPARATOR = { ';' };

        // From https://www.plasticscm.com/documentation/triggers/plastic-scm-version-control-triggers-guide.shtml#Checkin
        // cs:<id>@br:/<brName>@rep:<repName>@repserver:<repServer>
        static readonly Regex SPEC_REGEX = new Regex(
            @"cs:(?<id>[1-9]+)\@br:(?<brName>.+)\@rep:(?<repName>.+)\@repserver:(?<repServer>.+)");
    }
}
