﻿using System;
using System.Collections.Generic;
using System.Linq;

using Codice.SyncServerTrigger.Models;

namespace Codice.SyncServerTrigger.Commands
{
    internal class UninstallCmd : ICmd
    {
        public string Help { get { return HELP; } }

        public string CommandName { get { return "uninstall"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length != 2 || args[1].Contains("help"))
            {
                Console.Error.WriteLine(HELP);
                Environment.Exit(1);
            }

            string srcServer = args[1];

            if (!Utils.CheckServerSpec(srcServer))
            {
                Console.Error.WriteLine(
                    "The server spec is not correct: {0}", srcServer);
                Environment.Exit(1);
            }

            List<Trigger> triggers;
            if (!ListTriggers(srcServer, out triggers))
                Environment.Exit(1);

            string[] triggerNames =
            {
                TriggerNames.AfterCi,
                TriggerNames.AfterReplication,
                TriggerNames.AfterMkLabel,
                TriggerNames.AfterChAtt
            };

            List<Trigger> triggersToUninstall = triggers.FindAll(
                trigger => triggerNames.Contains(trigger.Name));

            if (triggersToUninstall.Count == 0)
            {
                Console.WriteLine("No triggers to uninstall!");
                return;
            }

            foreach (Trigger trigger in triggersToUninstall)
            {
                if (!UninstallTrigger(trigger, srcServer))
                    Environment.Exit(1);
            }

            Console.WriteLine(
                "Triggers successfully uninstalled from {0}!", srcServer);
        }

        static bool ListTriggers(string server, out List<Trigger> triggers)
        {
            int result;
            string stdOut, stdErr;

            string cmdLine = string.Format(
                "cm listtriggers --format={0} --server={1}",
                Trigger.CmFormat,
                server);

            result = CmdRunner.CmdRunner.ExecuteCommandWithResult(
                cmdLine,
                Environment.CurrentDirectory,
                out stdOut,
                out stdErr,
                false);

            if (result == 0)
            {
                triggers = Trigger.ParseFind(stdOut);
                return true;
            }

            Console.Error.WriteLine(
                "Listing the triggers in {1} failed.{0}" +
                "Command line: '{2}'{0}" +
                "cm stdout: {3}{0}" +
                "cm stderr: {4}{0}",
                Environment.NewLine,
                server,
                cmdLine,
                stdOut,
                stdErr);

            triggers = null;
            return false;
        }

        static bool UninstallTrigger(Trigger trigger, string server)
        {
            int result;
            string stdOut, stdErr;

            Console.WriteLine(
                "Uninstalling trigger '{0}' (type '{1}' at position {2})",
                trigger.Name, trigger.Type, trigger.Position);

            string cmdLine = string.Format(
                "cm removetrigger {0} {1} --server={2}",
                trigger.Type,
                trigger.Position,
                server);

            result = CmdRunner.CmdRunner.ExecuteCommandWithResult(
                cmdLine,
                Environment.CurrentDirectory,
                out stdOut,
                out stdErr,
                false);

            if (result == 0)
                return true;

            Console.Error.WriteLine(
                "Uninstalling a trigger from {1} failed.{0}" +
                "Command line: '{2}'{0}" +
                "cm stdout: {3}{0}" +
                "cm stderr: {4}{0}",
                server,
                cmdLine,
                stdOut,
                stdErr);

            return false;
        }

        const string HELP =
            @"uninstall       Autoremoves the triggers from the source server.

Usage:
    uninstall <src_server>

    src_server  The source repository, where the server-side triggers will be
                uninstalled from.

Examples:
    syncservertrigger uninstall
    (Uninstalls the server-side triggers, so it won't be automatically synced
    again.)

Remarks:
    This command only needs to be executed once, regardless of the
    number of destination repositories configured using the install command.

    After uninstalling the syncservertrigger, all of the configuration remains
    intact, in case you want to re-install it later.";
    }
}