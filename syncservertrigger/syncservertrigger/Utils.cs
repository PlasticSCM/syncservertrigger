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

        internal static string ReadStdInToEnd()
        {
            if (!Console.IsInputRedirected)
                return string.Empty;

            using (StreamReader reader = new StreamReader(
                Console.OpenStandardInput(), Console.InputEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        internal static bool CheckServerSpec(string serverSpec)
        {
            return true; // TODO
        }
    }
}
