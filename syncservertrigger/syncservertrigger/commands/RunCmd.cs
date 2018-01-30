using System;
using System.Collections.Generic;
using System.Text;

using Codice.SyncServerTrigger.Configuration;
using Codice.SyncServerTrigger.Models;

namespace Codice.SyncServerTrigger.Commands
{
    internal class RunCmd : ICmd
    {
        string ICmd.Help { get { return HELP; } }

        string ICmd.CommandName { get { return "run"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length == 1 || args.Length > 5)
            {
                Console.Error.WriteLine(HELP);
                Environment.Exit(1);
            }

            ToolConfiguration toolConfig = ToolConfiguration.Load();
            List<string> filteredRepos = toolConfig.RepoFilterConfig.GetFilteredRepos();
            List<string> dstServers = toolConfig.ServerConfig.GetServers();
            List<RepoMapping> repoMappings = toolConfig.RepoMapConfig.GetMappedRepos();

            ErrorEmailSender emailSender =
                new ErrorEmailSender(toolConfig.EmailConfig);

            if (args.Length == 3 && args[1] == Trigger.Names.AfterCi)
            {
                RunAfterCheckin(
                    filteredRepos,
                    dstServers,
                    repoMappings,
                    args[2],
                    emailSender);
            }

            if (args.Length == 3 && args[1] == Trigger.Names.AfterRW)
            {
                RunAfterReplicationWrite(
                    filteredRepos,
                    dstServers,
                    repoMappings,
                    args[2],
                    emailSender);
            }

            if (args.Length == 5 && args[1] == Trigger.Names.AfterMkLb)
            {
                RunAfterMakeLabel(
                    filteredRepos,
                    dstServers,
                    repoMappings,
                    args[2],
                    args[3],
                    args[4],
                    emailSender);
            }

            if (args.Length == 5 && args[1] == Trigger.Names.AfterChAttVal)
            {
                RunAfterChangeAttributeValue(
                    filteredRepos,
                    dstServers,
                    repoMappings,
                    args[2],
                    args[3],
                    args[4],
                    emailSender);
            }
        }

        static void RunAfterCheckin(
            List<string> filteredRepos,
            List<string> dstServers,
            List<RepoMapping> mappings,
            string csetSpecs,
            ErrorEmailSender emailSender)
        {
            Console.WriteLine("Running as after-checkin trigger...");

            List<Changeset> csets =
                Changeset.ParsePlasticChangesetEnvironVar(csetSpecs);

            List<Replica> pendingReplicas = new List<Replica>();

            foreach (Changeset cset in csets)
            {
                pendingReplicas.AddRange(
                    Replica.BuildPendingReplicas(
                        cset.BranchName,
                        cset.RepositoryName,
                        cset.ServerName,
                        filteredRepos,
                        dstServers,
                        mappings));
            }

            Console.WriteLine(
                "Found {0} destinations to replicate to.",
                pendingReplicas.Count);

            List<Replica> failedReplicas = new List<Replica>();
            foreach (Replica pendingReplica in pendingReplicas)
            {
                if (!Replicate(pendingReplica))
                    failedReplicas.Add(pendingReplica);
            }

            NotifyFailedReplicas(
                "after-checkin",
                failedReplicas,
                emailSender);
        }

        static void RunAfterReplicationWrite(
            List<string> filteredRepos,
            List<string> dstServers,
            List<RepoMapping> mappings,
            string branchSpec,
            ErrorEmailSender emailSender)
        {
            Console.WriteLine("Running as after-replicationwrite trigger...");

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

            Console.WriteLine(
                "Found {0} destinations to replicate to.",
                pendingReplicas.Count);

            List<Replica> failedReplicas = new List<Replica>();
            foreach (Replica pendingReplica in pendingReplicas)
            {
                if (!Replicate(pendingReplica))
                    failedReplicas.Add(pendingReplica);
            }

            NotifyFailedReplicas(
                "after-replicationwrite",
                failedReplicas,
                emailSender);
        }

        static void RunAfterMakeLabel(
            List<string> filteredRepos,
            List<string> dstServers,
            List<RepoMapping> mappings,
            string labelName,
            string repoName,
            string serverName,
            ErrorEmailSender emailSender)
        {
            Console.WriteLine("Running as after-makelabel trigger...");

            Branch branchToReplicate = null;
            if (!FindBranchForLabel(
                labelName, repoName, serverName, out branchToReplicate))
            {
                return;
            }

            List<Replica> pendingReplicas =
                Replica.BuildPendingReplicas(
                    branchToReplicate.BranchName,
                    branchToReplicate.RepositoryName,
                    branchToReplicate.ServerName,
                    filteredRepos,
                    dstServers,
                    mappings);

            Console.WriteLine(
                "Found {0} destinations to replicate to.",
                pendingReplicas.Count);

            List<Replica> failedReplicas = new List<Replica>();
            foreach (Replica pendingReplica in pendingReplicas)
            {
                if (!Replicate(pendingReplica))
                    failedReplicas.Add(pendingReplica);
            }

            NotifyFailedReplicas(
                "after-makelabel",
                failedReplicas,
                emailSender);
        }

        static void RunAfterChangeAttributeValue(
            List<string> filteredRepos,
            List<string> dstServers,
            List<RepoMapping> mappings,
            string triggerStdIn,
            string repoName,
            string serverName,
            ErrorEmailSender emailSender)
        {
            Console.WriteLine("Running as after-chattvalue trigger...");

            string objectSpec = triggerStdIn.Substring(
                startIndex: 0,
                length: triggerStdIn.IndexOf("attribute:")).Trim();

            string objectName = objectSpec.Substring(
                objectSpec.IndexOf(':') + 1);

            Branch branchToReplicate = null;
            if (objectSpec.StartsWith("br:"))
            {
                branchToReplicate = new Branch(
                    objectName,
                    repoName,
                    serverName);
            }

            if (objectSpec.StartsWith("lb:"))
            {
                if (!FindBranchForLabel(
                    objectName, repoName, serverName, out branchToReplicate))
                {
                    return;
                }
            }

            if (objectSpec.StartsWith("cs:"))
            {
                if (!FindBranchForChangeset(
                    objectName, repoName, serverName, out branchToReplicate))
                {
                    return;
                }
            }

            List<Replica> pendingReplicas =
                Replica.BuildPendingReplicas(
                    branchToReplicate.BranchName,
                    branchToReplicate.RepositoryName,
                    branchToReplicate.ServerName,
                    filteredRepos,
                    dstServers,
                    mappings);

            Console.WriteLine(
                "Found {0} destinations to replicate to.",
                pendingReplicas.Count);

            List<Replica> failedReplicas = new List<Replica>();
            foreach (Replica pendingReplica in pendingReplicas)
            {
                if (!Replicate(pendingReplica))
                    failedReplicas.Add(pendingReplica);
            }

            NotifyFailedReplicas(
                "after-chattvalue",
                failedReplicas,
                emailSender);
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
                "cm find labels where name=\"'{0}'\" --format=\"{{branch}}@rep:{1}@{2}\" on repository \"'{1}@{2}'\" --nototal",
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

            if (result == 0 && !string.IsNullOrEmpty(stdOut))
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

        static bool FindBranchForChangeset(
            string changesetId,
            string repositoryName,
            string serverName,
            out Branch branch)
        {
            int result;
            string stdOut, stdErr;

            string cmdLine = string.Format(
                "cm find changesets where changesetid={0} --format=\"{{branch}}@rep:{1}@{2}\" on repository \"'{1}@{2}'\" --nototal",
                changesetId,
                repositoryName,
                serverName);

            Console.WriteLine(
                "Searching for the branch of the changeset {0} on repository {1}@{2}",
                changesetId,
                repositoryName,
                serverName);

            result = CmdRunner.CmdRunner.ExecuteCommandWithResult(
                cmdLine,
                Environment.CurrentDirectory,
                out stdOut,
                out stdErr,
                false);

            if (result == 0 && !string.IsNullOrEmpty(stdOut))
            {
                branch = Branch.ParsePlasticBranchEnvironVar(stdOut.Trim());
                return true;
            }

            Console.Error.WriteLine(
                "Searching for the branch containing a changeset failed.{0}" +
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
                "Replicating br:{0}@{1}@{2} to {3}@{4}",
                replica.SrcBranch,
                replica.SrcRepo,
                replica.SrcServer,
                replica.DstRepo,
                replica.DstServer);

            string cmdLine = string.Format(
                "cm replicate --push \"br:{0}@{1}@{2}\" {3}@{4}",
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
                Environment.NewLine,
                cmdLine,
                stdOut,
                stdErr);

            return false;
        }

        static void NotifyFailedReplicas(
            string triggerType,
            List<Replica> failedReplicas,
            ErrorEmailSender emailSender)
        {
            if (failedReplicas.Count == 0)
                return;

            string subject = string.Format(
                "[syncservertrigger] {0} replication failed",
                triggerType);

            StringBuilder body = new StringBuilder();
            body.AppendLine("The following replicas failed:");

            foreach (Replica failedReplica in failedReplicas)
            {
                body.AppendLine(
                    string.Format(
                        "Source: br:{0}@rep:{1}@{2} --> Destination: rep:{3}@{4}",
                        failedReplica.SrcBranch,
                        failedReplica.SrcRepo,
                        failedReplica.SrcServer,
                        failedReplica.DstRepo,
                        failedReplica.DstServer));
            }

            emailSender.SendErrorEmail(subject, body.ToString());
        }

        const string HELP =
    @"This command is intended to be directly run by the syncservertrigger program
itself. Run this command manually only for debug purposes.";
    }
}
