using System;

namespace Codice.SyncServerTrigger
{
    internal static class PlasticEnvironment
    {
        internal static string PlasticChangeset
        {
            get
            {
                string result = Environment.GetEnvironmentVariable("PLASTIC_CHANGESET");
                Logger.LogInfo($"$PLASTIC_CHANGESET value: {result}");
                return result;
            }
        }

        internal static string PlasticBranch
        {
            get
            {
                string result = Environment.GetEnvironmentVariable("PLASTIC_BRANCH");
                Logger.LogInfo($"$PLASTIC_BRANCH value: {result}");
                return result;
            }
        }

        internal static string PlasticLabelName
        {
            get
            {
                string result = Environment.GetEnvironmentVariable("PLASTIC_LABEL_NAME"); 
                Logger.LogInfo($"$PLASTIC_LABEL_NAME value: {result}");
                return result;
            }
        }

        internal static string PlasticRepositoryName
        {
            get
            {
                string result = Environment.GetEnvironmentVariable("PLASTIC_REPOSITORY_NAME");
                Logger.LogInfo($"$PLASTIC_REPOSITORY_NAME value: {result}");
                return result;
            }
        }

        internal static string PlasticServer
        {
            get
            {
                string result = Environment.GetEnvironmentVariable("PLASTIC_SERVER");
                Logger.LogInfo($"PLASTIC_SERVER value: {result}");
                return result;
            }
        }
    }
}
