using System;
using System.IO;

namespace Codice.SyncServerTrigger
{
    public enum Platform
    {
        Windows,
        MacOS,
        Linux
    }

    public static class PlatformUtils
    {
        public static bool IsWindows
        {
            get { return CurrentPlatform == Platform.Windows; }
        }

        public static string HomePath
        {
            get
            {
                return IsWindows
                    ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                    : Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }

        public static string DefaultMonoRuntimePath
        {
            get
            {
                switch (CurrentPlatform)
                {
                    case Platform.Windows:
                        return string.Empty;

                    case Platform.MacOS:
                        return "/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono";

                    default:
                    case Platform.Linux:
                        return "/opt/plasticscm5/mono/bin/mono";
                }
            }
        }

        public static Platform CurrentPlatform
        {
            get { return GetCurrentPlatform(); }
        }

        static Platform GetCurrentPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    return Platform.MacOS;

                case PlatformID.Unix:
                    return AreMacOSDirectoriesPresent()
                        ? Platform.MacOS
                        : Platform.Linux;

                default:
                    return Platform.Windows;
            }
        }

        static bool AreMacOSDirectoriesPresent()
        {
            return Directory.Exists("/Applications")
                && Directory.Exists("/System")
                && Directory.Exists("/Users")
                && Directory.Exists("/Volumes");
        }
    }
}
