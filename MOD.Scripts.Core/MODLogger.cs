using MOD.Scripts.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.Core
{
    class MODLogger
    {
        private const int MAX_LOG_FILES = 10;
        private const string LOG_PREFIX = "mod_log";

        private static StreamWriter _currentLogFile = null;

        private static string GetLogPath(int i) => Path.Combine(MODActions.GetLogFolder(), $"{LOG_PREFIX}_{i}.txt");

        /// NOTE: The returned FileInfo object may or may not exist, but it does contain the correct path to use.
        private static FileInfo GetLogFilePath()
        {
            // Make log folder if it doesn't already exist
            Directory.CreateDirectory(MODActions.GetLogFolder());

            DateTime oldestWriteTime = DateTime.MaxValue;
            FileInfo oldestFile = new FileInfo(GetLogPath(0));

            for (int i = 0; i < MAX_LOG_FILES; i++)
            {
                FileInfo fileInfo = new FileInfo(GetLogPath(i + 1));

                // If the log file doesn't exist yet, just use that.
                if (!fileInfo.Exists)
                {
                    return fileInfo;
                }

                // Find the oldest log file
                if (fileInfo.LastWriteTime < oldestWriteTime)
                {
                    oldestWriteTime = fileInfo.LastWriteTime;
                    oldestFile = fileInfo;
                }
            }

            return oldestFile;
        }

        /// <summary>
        /// Create or reuse a log file to write to, if it doesn't already exist
        /// </summary>
        /// <returns>A StreamWriter for the log file.</returns>
        private static StreamWriter Writer()
        {
            if (_currentLogFile == null)
            {
                var logFile = GetLogFilePath();
                Debug.Log($"[MOD] Begin Logging to {logFile}");
                _currentLogFile = new StreamWriter(logFile.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));
                _currentLogFile.WriteLine($"---- Logging started at [{DateTime.Now}|{Time.realtimeSinceStartup}] ----");
            }

            return _currentLogFile;
        }

        public static void Log(string message, bool stackTrace)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                // Log the time
                sb.Append($"[{DateTime.Now}|{Time.realtimeSinceStartup}] ");
                sb.Append(message);

                // Optionally, log a stack trace
                if (stackTrace)
                {
                    sb.Append("\n");
                    sb.Append(StackTraceUtility.ExtractStackTrace());
                }

                // Log to the regular unity output
                Debug.Log(sb);

                // Also log to the mod log file
                Writer().WriteLine(sb);

                // Flush after every log, in case game crashes.
                Writer().Flush();
            }
            catch (Exception e)
            {
                Debug.Log($"[MOD] Failed to write to mod log file! {e}");
            }
        }
    }
}
