using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#if USE_SYSTEM_TEXT_JSON
using System.Text.Json;
using System.Text.Encodings.Web;
#else
using Newtonsoft.Json;
#endif

namespace MOD.Scripts.Core.MODJSONWrapper
{
	internal class MODJSONWrapper
	{

#if USE_SYSTEM_TEXT_JSON
		static JsonSerializerOptions options = new JsonSerializerOptions
		{
			// Include plain fields (otherwise only Properties (with {get; set;} are included)
			IncludeFields = true,
			// Ensure special characters like "&" are output as-is and not encoded as "\u0026"
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		};
#endif

		static public void Serialize(string outputPath, object objectToSerialize)
		{
#if USE_SYSTEM_TEXT_JSON
			File.WriteAllText(outputPath, JsonSerializer.Serialize(objectToSerialize, options));
#else
			JsonSerializer jsonSerializer = new JsonSerializer();
			using (StreamWriter sw = new StreamWriter(outputPath))
			using (JsonTextWriter writer = new JsonTextWriter(sw))
			{
				jsonSerializer.Serialize(writer, objectToSerialize);
			}
#endif
		}

		static public T Deserialize<T>(string inputPath)
		{
#if USE_SYSTEM_TEXT_JSON
			return  JsonSerializer.Deserialize<T>(File.ReadAllText(inputPath), options);
#else
			JsonSerializer jsonSerializer = new JsonSerializer();
			using (StreamReader sw = new StreamReader(inputPath))
			using (JsonReader reader = new JsonTextReader(sw))
			{
				return jsonSerializer.Deserialize<T>(reader);
			}
#endif
		}
	}
}
