﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MOD.Scripts.Core.UnityLoggerShim;

namespace MOD.ImageMapping
{
	// Internal class only used for deserializing the JSON
	internal class ImageMappingFromJSON
	{
		// {modPath: ogPath}
		public Dictionary<string, string> global_fallback;
		// {scriptPath : { modPath: ogPath } }
		public Dictionary<string, Dictionary<string, string>> script_fallback;
		// {scriptPath : { voicePath : { modPath: ogPath } } }
		public Dictionary<string, Dictionary<string, Dictionary<string, string>>> voice_database;
	}

	internal class ModToOGMapping
	{
		// A dictionary mapping mod paths to a OG paths
		// eg.modToOG could be { "background/hina_bus_03": "bg/hina/bus_03", "sprite/hyos1a_09_": "sprites/yos/yos_d3" }
		Dictionary<string, string> modToOG;

		public ModToOGMapping(Dictionary<string, string> modToOG)
		{
			this.modToOG = modToOG;
		}

		public bool GetOGImage(string modImagePath, out string ogImagePath)
		{
			return modToOG.TryGetValue(modImagePath, out ogImagePath);
		}
	}

	internal class ScriptFallback
	{
		// Tells you which fallbacks to use for a given script file name
		// "&opening.txt": ModToOGMapping,
		public Dictionary<string, ModToOGMapping> scriptToFallback;

		public ScriptFallback(Dictionary<string, Dictionary<string, string>> scriptFallbackJSON)
		{
			scriptToFallback = scriptFallbackJSON.ToDictionary(kvp => kvp.Key, kvp => new ModToOGMapping(kvp.Value));
		}

		public bool GetModToOGMapping(string scriptFileName, string modImagePath, out string ogImagePath)
		{
			if(scriptToFallback.TryGetValue(scriptFileName, out ModToOGMapping modToOGMapping))
			{
				return modToOGMapping.GetOGImage(modImagePath, out ogImagePath);
			}

			ogImagePath = null;
			return false;
		}
	}

	internal class VoiceMapping
	{
		// Tells you which mapping to use given the last played voice file
		// "ps3/s26/00/tkak_0110" : ModToOGMapping
		public Dictionary<string, ModToOGMapping> voiceToMapping;

		public VoiceMapping(Dictionary<string, Dictionary<string, string>> voiceToMappingJSON)
		{
			voiceToMapping = voiceToMappingJSON.ToDictionary(kvp => kvp.Key, kvp => new ModToOGMapping(kvp.Value));
		}
		public bool GetModToOGMapping(string lastPlayedVoice, string modImagePath, out string ogImagePath)
		{
			if(voiceToMapping.TryGetValue(lastPlayedVoice, out ModToOGMapping mapping))
			{
				return mapping.GetOGImage(modImagePath, out ogImagePath);
			}

			ogImagePath = null;
			return false;
		}
	}

	internal class VoiceDatabase
	{
		// Tells you which voice mapping to use, given the script file name
		// "&opening.txt": VoiceMapping,
		public Dictionary<string, VoiceMapping> scriptToVoiceMapping;

		public VoiceDatabase(Dictionary<string, Dictionary<string, Dictionary<string, string>>> scriptToVoiceMappingJSON)
		{
			scriptToVoiceMapping = scriptToVoiceMappingJSON.ToDictionary(kvp => kvp.Key, kvp => new VoiceMapping(kvp.Value));
		}
		public bool GetVoiceMapping(string scriptFileName, string lastPlayedVoice, string modImagePath, out string ogImagePath)
		{
			if(scriptToVoiceMapping.TryGetValue(scriptFileName, out VoiceMapping mapping))
			{
				return mapping.GetModToOGMapping(lastPlayedVoice, modImagePath, out ogImagePath);
			}

			ogImagePath = null;
			return false;
		}
	}

	public class MODImageMapping
	{
		ModToOGMapping globalFallback;
		ScriptFallback scriptFallback;
		VoiceDatabase voiceDatabase;

		// Move this function to mod utils
		public static T DeserializeFromJSONFile<T>(string path)
		{
			using (StreamReader sw = new StreamReader(path))
			using (JsonReader reader = new JsonTextReader(sw))
			{
				return new JsonSerializer().Deserialize<T>(reader);
			}
		}

		// Throws exception if unable to decode the .json (eg missing file, .json formatting error etc.)
		public static MODImageMapping GetVoiceBasedMapping(string path)
		{
			return new MODImageMapping(DeserializeFromJSONFile<ImageMappingFromJSON>(path));
		}

		public bool GetOGImage(string scriptFileName, string lastPlayedVoice, string modImagePath, out string ogImagePath, out string debugInfo)
		{
			// Since JSON doesn't support 'null' keys, lastPlayedVoice to the empty string ("") before continuing
			if(lastPlayedVoice == null)
			{
				lastPlayedVoice = "";
			}

			// 1) Try to get from voice database
			if (voiceDatabase.GetVoiceMapping(scriptFileName, lastPlayedVoice, modImagePath, out ogImagePath))
			{
				debugInfo = "Source: Voice Mapping";
				return true;
			}

			// 2) Try script fallback
			if (scriptFallback.GetModToOGMapping(scriptFileName, modImagePath, out ogImagePath))
			{
				debugInfo = "Source: Script Fallback";
				return true;
			}

			// 3) Try global fallback
			if(globalFallback.GetOGImage(modImagePath,out ogImagePath))
			{
				debugInfo = "Source: Global Fallback";
				return true;
			}

			debugInfo = "No match found";
			ogImagePath = null;
			return false;
		}

		public static void TEST_Lookup(string JSONPath)
		{
			MODImageMapping mapping = GetVoiceBasedMapping(JSONPath);

			string scriptName = "busstop01";
			string lastVoice = null;
			string modImage = "background/hina_bus_01";
			Debug.Log($"Looking up {scriptName} - {(lastVoice == null ? "[null]" : lastVoice)} - {modImage}");
			if (mapping.GetOGImage(scriptName, lastVoice, modImage, out string ogImagePath, out string debug))
			{
				Debug.Log($"Got match {ogImagePath} - Debug: [{debug}]");
			}
			else
			{
				Debug.Log($"Match failed - Debug: [{debug}]");
			}
		}

		MODImageMapping(ImageMappingFromJSON mappingJSON)
		{
			globalFallback = new ModToOGMapping(mappingJSON.global_fallback);
			scriptFallback = new ScriptFallback(mappingJSON.script_fallback);
			voiceDatabase = new VoiceDatabase(mappingJSON.voice_database);
		}
	}
}
