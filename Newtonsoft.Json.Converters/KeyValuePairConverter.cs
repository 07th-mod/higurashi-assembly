using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Newtonsoft.Json.Converters
{
	public class KeyValuePairConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Type type = value.GetType();
			PropertyInfo property = type.GetProperty("Key");
			PropertyInfo property2 = type.GetProperty("Value");
			writer.WriteStartObject();
			writer.WritePropertyName("Key");
			serializer.Serialize(writer, ReflectionUtils.GetMemberValue(property, value));
			writer.WritePropertyName("Value");
			serializer.Serialize(writer, ReflectionUtils.GetMemberValue(property2, value));
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			IList<Type> genericArguments = objectType.GetGenericArguments();
			Type objectType2 = genericArguments[0];
			Type objectType3 = genericArguments[1];
			reader.Read();
			reader.Read();
			object obj = serializer.Deserialize(reader, objectType2);
			reader.Read();
			reader.Read();
			object obj2 = serializer.Deserialize(reader, objectType3);
			reader.Read();
			return ReflectionUtils.CreateInstance(objectType, obj, obj2);
		}

		public override bool CanConvert(Type objectType)
		{
			if (objectType.IsValueType && objectType.IsGenericType)
			{
				return objectType.GetGenericTypeDefinition() == typeof(KeyValuePair<, >);
			}
			return false;
		}
	}
}
