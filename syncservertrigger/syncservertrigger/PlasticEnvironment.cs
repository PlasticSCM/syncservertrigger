using System;

namespace Codice.SyncServerTrigger
{
    internal static class PlasticEnvironment
    {
        internal static string PlasticChangeset
        {
            get
            {
                return Environment.GetEnvironmentVariable("PLASTIC_CHANGESET");
            }
        }

        internal static string PlasticBranch
        {
            get
            {
                return Environment.GetEnvironmentVariable("PLASTIC_BRANCH");
            }
        }

        internal static string PlasticLabelName
        {
            get
            {
                return Environment.GetEnvironmentVariable("PLASTIC_LABEL_NAME");
            }
        }

        internal static string PlasticRepositoryName
        {
            get
            {
                return Environment.GetEnvironmentVariable("PLASTIC_REPOSITORY_NAME");
            }
        }
    }
}
