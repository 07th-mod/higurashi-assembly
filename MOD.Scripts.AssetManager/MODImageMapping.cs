using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MOD.Scripts.AssetManager
{
	internal class MODImageMapping
	{
		// Internal class only used for deserializing the JSON
		private class ImageMappingFromJSON
		{
			// {modPath: ogPath}
			public Dictionary<string, string> global_fallback;
			// {scriptPath : { modPath: ogPath } }
			public Dictionary<string, Dictionary<string, string>> script_fallback;
			// {scriptPath : { voicePath : { modPath: ogPath } } }
			public Dictionary<string, Dictionary<string, Dictionary<string, string>>> voice_database;
		}

		private class ModToOGMapping
		{
			// A dictionary mapping mod paths to a OG paths
			// eg.modToOG could be { "background/hina_bus_03": "bg/hina/bus_03", "sprite/hyos1a_09_": "sprites/yos/yos_d3" }
			public Dictionary<string, string> modToOG;

			public ModToOGMapping(Dictionary<string, string> modToOG)
			{
				this.modToOG = modToOG;
			}
		}

		private class ScriptFallback
		{
			// Tells you which fallbacks to use for a given script file name
			// "&opening.txt": ModToOGMapping,
			public Dictionary<string, ModToOGMapping> scriptToFallback;

			public ScriptFallback(Dictionary<string, Dictionary<string, string>> scriptFallbackJSON)
			{
				scriptToFallback = scriptFallbackJSON.ToDictionary(kvp => kvp.Key, kvp => new ModToOGMapping(kvp.Value));
			}
		}

		private class VoiceMapping
		{
			// Tells you which mapping to use given the last played voice file
			// "ps3/s26/00/tkak_0110" : ModToOGMapping
			public Dictionary<string, ModToOGMapping> voiceToMapping;

			public VoiceMapping(Dictionary<string, Dictionary<string, string>> voiceToMappingJSON)
			{
				voiceToMapping = voiceToMappingJSON.ToDictionary(kvp => kvp.Key, kvp => new ModToOGMapping(kvp.Value));
			}
		}

		private class VoiceDatabase
		{
			// Tells you which voice mapping to use, given the script file name
			// "&opening.txt": VoiceMapping,
			public Dictionary<string, VoiceMapping> scriptToVoiceMapping;

			public VoiceDatabase(Dictionary<string, Dictionary<string, Dictionary<string, string>>> scriptToVoiceMappingJSON)
			{
				scriptToVoiceMapping = scriptToVoiceMappingJSON.ToDictionary(kvp => kvp.Key, kvp => new VoiceMapping(kvp.Value));
			}
		}

		public class ImageMapping
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
			public static ImageMapping GetVoiceBasedMapping(string path)
			{
				return new ImageMapping(DeserializeFromJSONFile<ImageMappingFromJSON>(path));
			}

			ImageMapping(ImageMappingFromJSON mappingJSON)
			{
				globalFallback = new ModToOGMapping(mappingJSON.global_fallback);
				scriptFallback = new ScriptFallback(mappingJSON.script_fallback);
				voiceDatabase = new VoiceDatabase(mappingJSON.voice_database);
			}
		}
	}
}
