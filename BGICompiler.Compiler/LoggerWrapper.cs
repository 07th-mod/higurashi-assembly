using System;

namespace BGICompiler.Compiler.Logger
{
	class Debug
	{
#if STANDALONE_SCRIPT_COMPILER
		public static void Log(object message)
		{
			print($"INFO", message);
		}

		public static void LogWarning(object message)
		{
			print("WARN", message);
		}

		public static void LogError(object message)
		{
			print("ERROR", message);
		}

		private static void print(string level, object message)
		{
			Console.WriteLine($"[{level}] {message}", message);
		}
#else
		public static void Log(object message)
		{
			UnityEngine.Debug.Log(message);
		}

		public static void LogWarning(object message)
		{
			UnityEngine.Debug.LogWarning(message);
		}

		public static void LogError(object message)
		{
			UnityEngine.Debug.Log(message);
		}
#endif
	}
}
