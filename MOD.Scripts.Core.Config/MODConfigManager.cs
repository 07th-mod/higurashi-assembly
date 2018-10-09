using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace MOD.Scripts.Core.Config
{
	public static class MODConfigManager
	{
		private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
		};

		private static readonly string ConfigFilePath = Path.Combine(MODSystem.BaseDirectory, "nipah.json");

		private static TextReader EmptyConfigReader => new StringReader("{}");

		public static MODConfig Read()
		{
			MODConfig config = null;
			if (File.Exists(ConfigFilePath))
			{
				config = ReadFrom(new StreamReader(ConfigFilePath));
				if (config == null)
				{
					Debug.LogWarning($"Error reading configuration file at {ConfigFilePath}.  This file is probably malformed; using defaults");
				}
			}
			else
			{
				Debug.LogWarning($"Config file not present at {ConfigFilePath}; using defaults");
			}
			return config ?? ReadFrom(EmptyConfigReader);
		}

		private static MODConfig ReadFrom(TextReader reader)
		{
			try
			{
				JsonSerializer jsonSerializer = JsonSerializer.Create(Settings);
				using (JsonTextReader reader2 = new JsonTextReader(reader))
				{
					return jsonSerializer.Deserialize<MODConfig>(reader2);
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Failed to read configuration: " + e.Message);
				return null;
			}
		}
	}
}
