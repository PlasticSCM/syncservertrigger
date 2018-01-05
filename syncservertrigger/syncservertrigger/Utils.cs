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

        internal static string GetHomeBasedPath(string fileName)
        {
            return PlatformUtils.IsWindows
                ? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    fileName)
                : Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    string.Format(".{0}", fileName));
        }
    }
}
