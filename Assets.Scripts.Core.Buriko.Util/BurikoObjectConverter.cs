using Assets.Scripts.Core.Buriko.VarTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Assets.Scripts.Core.Buriko.Util
{
	public class BurikoObjectConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is BurikoString)
			{
				JToken jToken = JToken.FromObject(value);
				JObject jObject = (JObject)jToken;
				jObject.AddFirst(new JProperty("type", new JArray(new string[1]
				{
					"BurikoString"
				})));
				jToken.WriteTo(writer);
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return true;
		}
	}
}
