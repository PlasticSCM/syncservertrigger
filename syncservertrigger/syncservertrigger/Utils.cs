using System.Reflection;

namespace syncservertrigger
{
    internal static class Utils
    {
        internal static string GetAssemblyLocation()
        {
            return Assembly.GetCallingAssembly().Location;
        }
    }
}
