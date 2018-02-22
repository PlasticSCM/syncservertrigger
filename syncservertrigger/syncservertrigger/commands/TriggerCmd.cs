﻿using System;
using System.Diagnostics;
#if MONO
using Mono.Unix.Native;
#endif
using Codice.SyncServerTrigger.Configuration;
using Codice.SyncServerTrigger.Models;

namespace Codice.SyncServerTrigger.Commands
{
    internal class TriggerCmd : ICmd
    {
        string ICmd.Help { get { return HELP; } }

        string ICmd.CommandName { get { return "trigger"; } }

        void ICmd.Execute(string[] args)
        {
            if (args.Length == 1 || args.Length > 2)
            {
                Console.Error.WriteLine(HELP);
                Environment.Exit(1);
            }

            string runArgs = string.Empty;
            if (args.Length == 2 && args[1] == Trigger.Names.AfterCi)
            {
                runArgs = string.Format(
                    "run {0} \"{1}\"",
                    Trigger.Names.AfterCi,
                    PlasticEnvironment.PlasticChangeset);
            }

            if (args.Length == 2 && args[1] == Trigger.Names.AfterRW)
            {
                runArgs = string.Format(
                    "run {0} \"{1}\"",
                    Trigger.Names.AfterRW,
                    PlasticEnvironment.PlasticBranch);
            }

            if (args.Length == 2 && args[1] == Trigger.Names.AfterMkLb)
            {
                runArgs = string.Format(
                    "run {0} \"{1}\" \"{2}\" \"{3}\"",
                    Trigger.Names.AfterMkLb,
                    PlasticEnvironment.PlasticLabelName,
                    PlasticEnvironment.PlasticRepositoryName,
                    PlasticEnvironment.PlasticServer);
            }

            if (args.Length == 2 && args[1] == Trigger.Names.AfterChAttVal)
            {
                runArgs = string.Format(
                    "run {0} \"{1}\" \"{2}\" \"{3}\"",
                    Trigger.Names.AfterChAttVal,
                    Utils.ReadStdInToEnd(),
                    PlasticEnvironment.PlasticRepositoryName,
                    PlasticEnvironment.PlasticServer);
            }

            if (string.IsNullOrEmpty(runArgs))
            {
                Console.Error.WriteLine(HELP);
                Environment.Exit(1);
            }

            // IMPORTANT: Build the process BEFORE dettaching from StdIO.
            Process p = BuildSyncServerTriggerProcess(runArgs);

#if MONO
            if (PlatformUtils.CurrentPlatform != Platform.Windows)
            {
                Logger.LogInfo("Dettaching from stdio.");
                DettachFromStdIO();
            }
#endif

            try
            {
                p.Start();
            }
            finally
            {
                p.Close();
                p.Dispose();
            }
        }

#if MONO
        static void DettachFromStdIO()
        {
            try
            {
                Stdlib.fclose(Stdlib.stdin);
                Stdlib.fclose(Stdlib.stdout);
                Stdlib.fclose(Stdlib.stderr);
            }
            catch (Exception ex)
            {
                Logger.LogException("Could not dettach from stdio.", ex);
            }
        }
#endif

        static Process BuildSyncServerTriggerProcess(string runArgs)
        {
            Process result = PlatformUtils.IsWindows
                ? BuildSyncServerTriggerProcessForWindows(runArgs)
                : BuildSyncServerTriggerProcessForUnixLike(runArgs);

            result.StartInfo.UseShellExecute = true;
            return result;
        }

        static Process BuildSyncServerTriggerProcessForWindows(string runArgs)
        {
            Process result = new Process();
            result.StartInfo.FileName = Utils.GetAssemblyLocation();
            result.StartInfo.Arguments = runArgs;
            return result;
        }

        static Process BuildSyncServerTriggerProcessForUnixLike(string runArgs)
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();

            Process result = new Process();
            result.StartInfo.FileName = toolConfig.RuntimeConfig.MonoRuntimePath;
            result.StartInfo.Arguments = $"{Utils.GetAssemblyLocation()} {runArgs}";
            return result;
        }

        const string HELP =
@"This command is intended to be directly run by the Plastic SCM server. Run
this command manually only for debug purposes.";
    }
}
