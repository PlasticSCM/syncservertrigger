using System;
using System.IO;

namespace Codice.SyncServerTrigger
{
    internal static class Logger
    {
        internal static void InitializeLoggerFile(string installationPath)
        {
            try
            {
                string folder = Path.Combine(installationPath, "logs");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                int pid = System.Diagnostics.Process.GetCurrentProcess().Id;

                mLogFilePath = Path.Combine(
                    folder, string.Format("gmaster-install.{0}.log", pid));
            }
            catch
            {
                // if log fails, there is nothing more to do :-S
            }
        }

        internal static void LogException(string message, Exception e)
        {
            if (mLogFilePath == null)
                return;

            AppendLog(
                string.Format("{0} - ERROR - {1}. Exception:{2}{3}{4}{5}",
                    DateTime.Now,
                    message,
                    e.Message,
                    Environment.NewLine,
                    e.StackTrace,
                    Environment.NewLine));
        }

        internal static void LogInfo(string message)
        {
            if (mLogFilePath == null)
                return;

            AppendLog(
                string.Format("{0} - INFO - {1}{2}.",
                    DateTime.Now,
                    message,
                    Environment.NewLine));
        }

        static void AppendLog(string message)
        {
            try
            {
                File.AppendAllText(mLogFilePath, message);
            }
            catch
            {
                // if log fails, there is nothing more to do :-S
            }
        }

        static string mLogFilePath = null;
    }
}