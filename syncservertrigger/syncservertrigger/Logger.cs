using System;
using System.IO;

namespace Codice.SyncServerTrigger
{
    internal static class Logger
    {
        internal static void InitializeLoggerFile(string destinationDirectory)
        {
            try
            {
                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                mLogFilePath = Path.Combine(
                    destinationDirectory,
                    $"{DateTime.Now.ToString("yyyy-MM-dd")}.log.txt");
            }
            catch { }
        }

        internal static void LogError(string message)
        {
            if (mLogFilePath == null)
                return;

            AppendLog(
                string.Format("{0} - ERROR - {1}{2}",
                    DateTime.Now,
                    message,
                    Environment.NewLine));
        }

        internal static void LogException(string message, Exception e)
        {
            if (mLogFilePath == null)
                return;

            AppendLog(
                string.Format("{0} - EXCEPTION - {1}. Exception:{2}{3}{4}{5}",
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
            catch { }
        }

        static string mLogFilePath = null;
    }
}