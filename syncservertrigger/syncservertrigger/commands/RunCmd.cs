using System;
using System.Collections.Generic;

using Codice.SyncServerTrigger.Configuration;
using Codice.SyncServerTrigger.Models;

namespace Codice.SyncServerTrigger.Commands
{
    internal class RunCmd : ICmd
    {
        public string Help { get { return HELP; } }

        public string CommandName { get { return "run"; } }

        void ICmd.Execute(string[] args)
        {
            Console.WriteLine("Arguments received:");
            Array.ForEach(args, argument => Console.WriteLine(argument));

            if (args.Length == 1 || args.Length > 5)
            {
                Console.Error.WriteLine(Help);
                Environment.Exit(1);
            }

            ToolConfiguration toolConfig = ToolConfiguration.Load();
            List<string> filteredRepos = toolConfig.RepoFilterConfig.GetFilteredRepos();
            List<string> dstServers = toolConfig.ServerConfig.GetServers();
            List<RepoMapping> repoMappings = toolConfig.RepoMapConfig.GetMappedRepos();

            bool succeeded = false;
            if (args.Length == 3 && args[1] == Trigger.Names.AfterCi)
            {
                succeeded = RunAfterCheckin(
                    filteredRepos,
                    dstServers,
                    repoMappings,
                    args[2]);
            }

            if (args.Length == 3 && args[1] == Trigger.Names.AfterRW)
            {
                succeeded = RunAfterReplicationWrite(
                    filteredRepos,
                    dstServers,
                    repoMappings,
                    args[2]);
            }

            if (args.Length == 5 && args[1] == Trigger.Names.AfterMkLb)
            {
                succeeded = RunAfterMakeLabel(
                    filteredRepos,
                    dstServers,
                    repoMappings,
                    args[2],
                    args[3],
                    args[4]);
            }

            if (!succeeded)
            {
                Console.WriteLine("The replication process failed.");
                // TODO send failure email
            }
            else
            {
                Console.WriteLine("The replication process succeeded.");
            }

#if DEBUG
            Console.WriteLine("Finished! Press ENTER to leave.");
            Console.ReadLine();
#endif
        }

        static bool RunAfterCheckin(
            List<string> filteredRepos,
            List<string> dstServers,
            List<RepoMapping> mappings,
            string csetSpecs)
        {
            List<Changeset> csets =
                Changeset.ParsePlasticChangesetEnvironVar(csetSpecs);

            List<Replica> pendingReplicas = new List<Replica>();

            foreach (Changeset cset in csets)
            {
                pendingReplicas.AddRange(
                    Replica.BuildPendingReplicas(
                        cset.BranchName,
                        cset.RepositoryName,
                        cset.RepositoryName,
                        filteredRepos,
                        dstServers,
                        mappings));
            }

            bool succeeded = true;
            foreach (Replica pendingReplica in pendingReplicas)
            {
                succeeded = succeeded && Replicate(pendingReplica);
            }

            return succeeded;
        }

        static bool RunAfterReplicationWrite(
            List<string> filteredRepos,
            List<string> dstServers,
            List<RepoMapping> mappings,
            string branchSpec)
        {
            Branch branchToReplicate =
                Branch.ParsePlasticBranchEnvironVar(branchSpec);

            List<Replica> pendingReplicas =
                Replica.BuildPendingReplicas(
                    branchToReplicate.BranchName,
                    branchToReplicate.RepositoryName,
                    branchToReplicate.ServerName,
                    filteredRepos,
                    dstServers,
                    mappings);

            bool succeeded = true;
            foreach (Replica pendingReplica in pendingReplicas)
            {
                succeeded = succeeded && Replicate(pendingReplica);
            }

            return succeeded;
        }

        static bool RunAfterMakeLabel(
            List<string> filteredRepos,
            List<string> dstServers,
            List<RepoMapping> mappings,
            string labelName,
            string repoName,
            string serverName)
        {
            Branch branchToReplicate = null;
            if (!FindBranchForLabel(
                labelName, repoName, serverName, out branchToReplicate))
            {
                return false;
            }

            List<Replica> pendingReplicas =
                Replica.BuildPendingReplicas(
                    branchToReplicate.BranchName,
                    branchToReplicate.RepositoryName,
                    branchToReplicate.ServerName,
                    filteredRepos,
                    dstServers,
                    mappings);

            bool succeeded = true;
            foreach (Replica pendingReplica in pendingReplicas)
            {
                succeeded = succeeded && Replicate(pendingReplica);
            }

            return succeeded;
        }

        static bool FindBranchForLabel(
            string labelName,
            string repositoryName,
            string serverName,
            out Branch branch)
        {
            int result;
            string stdOut, stdErr;

            string cmdLine = string.Format(
                "cm find labels where name='{0}' --format=\"{{branch}}@rep:{1}@{2}\" on repository '{1}@{2}' --nototal",
                labelName,
                repositoryName,
                serverName);

            Console.WriteLine(
                "Searching for the branch of the label {0} on repository {1}@{2}",
                labelName,
                repositoryName,
                serverName);

            result = CmdRunner.CmdRunner.ExecuteCommandWithResult(
                cmdLine,
                Environment.CurrentDirectory,
                out stdOut,
                out stdErr,
                false);

            if (result == 0)
            {
                branch = Branch.ParsePlasticBranchEnvironVar(stdOut.Trim());
                return true;
            }

            Console.Error.WriteLine(
                "Searching for the branch containing a label failed.{0}" +
                "Command line: {1}{0}" +
                "cm stdout: {2}{0}" +
                "cm stderr: {3}{0}",
                Environment.NewLine,
                cmdLine,
                stdOut,
                stdErr);

            branch = null;
            return false;
        }

        static bool Replicate(Replica replica)
        {
            int result;
            string stdOut, stdErr;

            Console.WriteLine(
                "Replicating br:/{0}@{1}@{2} to {3}@{4}",
                replica.SrcBranch,
                replica.SrcRepo,
                replica.SrcServer,
                replica.DstRepo,
                replica.DstServer);

            string cmdLine = string.Format(
                "cm replicate --push br:/{0}@{1}@{2} {3}@{4}",
                replica.SrcBranch,
                replica.SrcRepo,
                replica.SrcServer,
                replica.DstRepo,
                replica.DstServer);

            result = CmdRunner.CmdRunner.ExecuteCommandWithResult(
                cmdLine,
                Environment.CurrentDirectory,
                out stdOut,
                out stdErr,
                false);

            if (result == 0)
                return true;

            Console.Error.WriteLine(
                "Replicating a branch failed.{0}" +
                "Command line: '{1}'{0}" +
                "cm stdout: {2}{0}" +
                "cm stderr: {3}{0}",
                cmdLine,
                stdOut,
                stdErr);

            return false;
        }

        const string HELP =
@"This command is intended to be directly run by the syncservertrigger program
itself. Run this command manually only for debug purposes.";
    }
}
