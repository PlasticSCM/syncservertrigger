using System;
using System.Diagnostics;

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
                runArgs = string.Format("run {0} \"{1}\"",
                    Trigger.Names.AfterCi,
                    PlasticEnvironment.PlasticChangeset);
            }

            if (args.Length == 2 && args[1] == Trigger.Names.AfterRW)
            {
                runArgs = string.Format("run {0} \"{1}\"",
                    Trigger.Names.AfterRW,
                    PlasticEnvironment.PlasticBranch);
            }

            if (args.Length == 2 && args[1] == Trigger.Names.AfterMkLb)
            {
                runArgs = string.Format("run {0} \"{1}\" \"{2}\" \"{3}\"",
                    Trigger.Names.AfterMkLb,
                    PlasticEnvironment.PlasticLabelName,
                    PlasticEnvironment.PlasticRepositoryName,
                    PlasticEnvironment.PlasticServer);
            }

            if (args.Length == 2 && args[1] == Trigger.Names.AfterChAttVal)
            {
                runArgs = string.Format("run {0} \"{1}\" \"{2}\" \"{3}\"",
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

            BuildSyncServerTriggerProcess(runArgs).Start();
        }

        static Process BuildSyncServerTriggerProcess(string args)
        {
            Process result = PlatformUtils.IsWindows
                ? BuildSyncServerTriggerProcessForWindows(args)
                : BuildSyncServerTriggerProcessForUnixLike(args);

#if !DEBUG
            result.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
#endif
            return result;
        }

        static Process BuildSyncServerTriggerProcessForWindows(string args)
        {
            Process result = new Process();
            result.StartInfo.FileName = Utils.GetAssemblyLocation();
            result.StartInfo.Arguments = args;
            return result;
        }

        static Process BuildSyncServerTriggerProcessForUnixLike(string args)
        {
            ToolConfiguration toolConfig = ToolConfiguration.Load();

            Process result = new Process();
            result.StartInfo.FileName = toolConfig.RuntimeConfig.MonoRuntimePath;
            result.StartInfo.Arguments = string.Format(
                "{0} {1}", Utils.GetAssemblyLocation(), args);
            return result;
        }

        const string HELP =
@"This command is intended to be directly run by the Plastic SCM server. Run
this command manually only for debug purposes.";
    }
}
