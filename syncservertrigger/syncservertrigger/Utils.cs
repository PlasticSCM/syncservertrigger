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
    }
}
