using System;
using UnityEngine;

namespace Assets.Scripts.Core
{
	public static class Logger
	{
		private static LogOnMainThread logDelegate;

		public static void RegisterLogOnMainThread(LogOnMainThread d)
		{
			logDelegate = (LogOnMainThread)Delegate.Combine(logDelegate, d);
		}

		private static bool CheckConsoleLogLevel(LoggingLevel targetLevel)
		{
			return GameSystem.Instance.ConsoleLoggingLevel >= targetLevel;
		}

		public static void Log(string message)
		{
			if (CheckConsoleLogLevel(LoggingLevel.All))
			{
				Debug.Log(message);
			}
		}

		public static void LogWarning(string message)
		{
			if (CheckConsoleLogLevel(LoggingLevel.WarningsAndErrors))
			{
				Debug.LogWarning(message);
			}
		}

		public static void LogError(string message)
		{
			if (CheckConsoleLogLevel(LoggingLevel.ErrorOnly))
			{
				Debug.LogError(message);
			}
		}

		public static void Update()
		{
			if (logDelegate != null)
			{
				logDelegate();
				logDelegate = null;
			}
		}
	}
}
