using System;
using System.IO;
using System.Reflection;

namespace Codice.SyncServerTrigger
{
    internal static class Utils
    {
        internal static string GetAssemblyLocation()
        {
            return Assembly.GetCallingAssembly().Location;
        }

        internal static bool CheckServerSpec(string serverSpec)
        {
            return true; // TODO
        }

        internal static string GetHomeBasedDirectoryPath(string directoryName)
        {
            return PlatformUtils.IsWindows
                ? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    directoryName)
                : Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    string.Format(".{0}", directoryName));
        }
    }
}
