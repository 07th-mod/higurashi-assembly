using Newtonsoft.Json.Bson;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Newtonsoft.Json.Converters
{
	public class RegexConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Regex regex = (Regex)value;
			BsonWriter bsonWriter = writer as BsonWriter;
			if (bsonWriter != null)
			{
				WriteBson(bsonWriter, regex);
			}
			else
			{
				WriteJson(writer, regex);
			}
		}

		private bool HasFlag(RegexOptions options, RegexOptions flag)
		{
			return (options & flag) == flag;
		}

		private void WriteBson(BsonWriter writer, Regex regex)
		{
			string str = null;
			if (HasFlag(regex.Options, RegexOptions.IgnoreCase))
			{
				str += "i";
			}
			if (HasFlag(regex.Options, RegexOptions.Multiline))
			{
				str += "m";
			}
			if (HasFlag(regex.Options, RegexOptions.Singleline))
			{
				str += "s";
			}
			str += "u";
			if (HasFlag(regex.Options, RegexOptions.ExplicitCapture))
			{
				str += "x";
			}
			writer.WriteRegex(regex.ToString(), str);
		}

		private void WriteJson(JsonWriter writer, Regex regex)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("Pattern");
			writer.WriteValue(regex.ToString());
			writer.WritePropertyName("Options");
			writer.WriteValue(regex.Options);
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			BsonReader bsonReader = reader as BsonReader;
			if (bsonReader != null)
			{
				return ReadBson(bsonReader);
			}
			return ReadJson(reader);
		}

		private object ReadBson(BsonReader reader)
		{
			string text = (string)reader.Value;
			int num = text.LastIndexOf("/");
			string pattern = text.Substring(1, num - 1);
			string text2 = text.Substring(num + 1);
			RegexOptions regexOptions = RegexOptions.None;
			string text3 = text2;
			for (int i = 0; i < text3.Length; i++)
			{
				switch (text3[i])
				{
				case 'i':
					regexOptions |= RegexOptions.IgnoreCase;
					break;
				case 'm':
					regexOptions |= RegexOptions.Multiline;
					break;
				case 's':
					regexOptions |= RegexOptions.Singleline;
					break;
				case 'x':
					regexOptions |= RegexOptions.ExplicitCapture;
					break;
				}
			}
			return new Regex(pattern, regexOptions);
		}

		private Regex ReadJson(JsonReader reader)
		{
			reader.Read();
			reader.Read();
			string pattern = (string)reader.Value;
			reader.Read();
			reader.Read();
			int options = Convert.ToInt32(reader.Value, CultureInfo.InvariantCulture);
			reader.Read();
			return new Regex(pattern, (RegexOptions)options);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Regex);
		}
	}
}
