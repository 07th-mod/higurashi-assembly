using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Newtonsoft.Json
{
	public static class JsonConvert
	{
		public static readonly string True = "true";

		public static readonly string False = "false";

		public static readonly string Null = "null";

		public static readonly string Undefined = "undefined";

		public static readonly string PositiveInfinity = "Infinity";

		public static readonly string NegativeInfinity = "-Infinity";

		public static readonly string NaN = "NaN";

		internal static readonly long InitialJavaScriptDateTicks = 621355968000000000L;

		public static string ToString(DateTime value)
		{
			using (StringWriter stringWriter = StringUtils.CreateStringWriter(64))
			{
				WriteDateTimeString(stringWriter, value, GetUtcOffset(value), value.Kind);
				return stringWriter.ToString();
				IL_0028:
				string result;
				return result;
			}
		}

		public static string ToString(DateTimeOffset value)
		{
			using (StringWriter stringWriter = StringUtils.CreateStringWriter(64))
			{
				WriteDateTimeString(stringWriter, value.UtcDateTime, value.Offset, DateTimeKind.Local);
				return stringWriter.ToString();
				IL_0029:
				string result;
				return result;
			}
		}

		private static TimeSpan GetUtcOffset(DateTime dateTime)
		{
			return TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
		}

		internal static void WriteDateTimeString(TextWriter writer, DateTime value)
		{
			WriteDateTimeString(writer, value, GetUtcOffset(value), value.Kind);
		}

		internal static void WriteDateTimeString(TextWriter writer, DateTime value, TimeSpan offset, DateTimeKind kind)
		{
			long value2 = ConvertDateTimeToJavaScriptTicks(value, offset);
			writer.Write("\"\\/Date(");
			writer.Write(value2);
			switch (kind)
			{
			case DateTimeKind.Unspecified:
			case DateTimeKind.Local:
			{
				writer.Write((offset.Ticks < 0) ? "-" : "+");
				int num = Math.Abs(offset.Hours);
				if (num < 10)
				{
					writer.Write(0);
				}
				writer.Write(num);
				int num2 = Math.Abs(offset.Minutes);
				if (num2 < 10)
				{
					writer.Write(0);
				}
				writer.Write(num2);
				break;
			}
			}
			writer.Write(")\\/\"");
		}

		private static long ToUniversalTicks(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime.Ticks;
			}
			return ToUniversalTicks(dateTime, GetUtcOffset(dateTime));
		}

		private static long ToUniversalTicks(DateTime dateTime, TimeSpan offset)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime.Ticks;
			}
			long num = dateTime.Ticks - offset.Ticks;
			if (num > 3155378975999999999L)
			{
				return 3155378975999999999L;
			}
			if (num < 0)
			{
				return 0L;
			}
			return num;
		}

		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, TimeSpan offset)
		{
			long universialTicks = ToUniversalTicks(dateTime, offset);
			return UniversialTicksToJavaScriptTicks(universialTicks);
		}

		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime)
		{
			return ConvertDateTimeToJavaScriptTicks(dateTime, convertToUtc: true);
		}

		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, bool convertToUtc)
		{
			long universialTicks = (!convertToUtc) ? dateTime.Ticks : ToUniversalTicks(dateTime);
			return UniversialTicksToJavaScriptTicks(universialTicks);
		}

		private static long UniversialTicksToJavaScriptTicks(long universialTicks)
		{
			return (universialTicks - InitialJavaScriptDateTicks) / 10000;
		}

		internal static DateTime ConvertJavaScriptTicksToDateTime(long javaScriptTicks)
		{
			return new DateTime(javaScriptTicks * 10000 + InitialJavaScriptDateTicks, DateTimeKind.Utc);
		}

		public static string ToString(bool value)
		{
			return (!value) ? False : True;
		}

		public static string ToString(char value)
		{
			return ToString(char.ToString(value));
		}

		public static string ToString(Enum value)
		{
			return value.ToString("D");
		}

		public static string ToString(int value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(short value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(ushort value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(uint value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(long value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(ulong value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(float value)
		{
			return EnsureDecimalPlace((double)value, value.ToString("R", CultureInfo.InvariantCulture));
		}

		public static string ToString(double value)
		{
			return EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture));
		}

		private static string EnsureDecimalPlace(double value, string text)
		{
			if (double.IsNaN(value) || double.IsInfinity(value) || text.IndexOf('.') != -1 || text.IndexOf('E') != -1)
			{
				return text;
			}
			return text + ".0";
		}

		private static string EnsureDecimalPlace(string text)
		{
			if (text.IndexOf('.') != -1)
			{
				return text;
			}
			return text + ".0";
		}

		public static string ToString(byte value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(sbyte value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToString(decimal value)
		{
			return EnsureDecimalPlace(value.ToString(null, CultureInfo.InvariantCulture));
		}

		public static string ToString(Guid value)
		{
			return '"' + value.ToString("D", CultureInfo.InvariantCulture) + '"';
		}

		public static string ToString(TimeSpan value)
		{
			return '"' + value.ToString() + '"';
		}

		public static string ToString(Uri value)
		{
			return '"' + value.ToString() + '"';
		}

		public static string ToString(string value)
		{
			return ToString(value, '"');
		}

		public static string ToString(string value, char delimter)
		{
			return JavaScriptUtils.ToEscapedJavaScriptString(value, delimter, appendDelimiters: true);
		}

		public static string ToString(object value)
		{
			if (value == null)
			{
				return Null;
			}
			IConvertible convertible = value as IConvertible;
			if (convertible != null)
			{
				switch (convertible.GetTypeCode())
				{
				case TypeCode.String:
					return ToString(convertible.ToString(CultureInfo.InvariantCulture));
				case TypeCode.Char:
					return ToString(convertible.ToChar(CultureInfo.InvariantCulture));
				case TypeCode.Boolean:
					return ToString(convertible.ToBoolean(CultureInfo.InvariantCulture));
				case TypeCode.SByte:
					return ToString(convertible.ToSByte(CultureInfo.InvariantCulture));
				case TypeCode.Int16:
					return ToString(convertible.ToInt16(CultureInfo.InvariantCulture));
				case TypeCode.UInt16:
					return ToString(convertible.ToUInt16(CultureInfo.InvariantCulture));
				case TypeCode.Int32:
					return ToString(convertible.ToInt32(CultureInfo.InvariantCulture));
				case TypeCode.Byte:
					return ToString(convertible.ToByte(CultureInfo.InvariantCulture));
				case TypeCode.UInt32:
					return ToString(convertible.ToUInt32(CultureInfo.InvariantCulture));
				case TypeCode.Int64:
					return ToString(convertible.ToInt64(CultureInfo.InvariantCulture));
				case TypeCode.UInt64:
					return ToString(convertible.ToUInt64(CultureInfo.InvariantCulture));
				case TypeCode.Single:
					return ToString(convertible.ToSingle(CultureInfo.InvariantCulture));
				case TypeCode.Double:
					return ToString(convertible.ToDouble(CultureInfo.InvariantCulture));
				case TypeCode.DateTime:
					return ToString(convertible.ToDateTime(CultureInfo.InvariantCulture));
				case TypeCode.Decimal:
					return ToString(convertible.ToDecimal(CultureInfo.InvariantCulture));
				case TypeCode.DBNull:
					return Null;
				}
			}
			else
			{
				if (value is DateTimeOffset)
				{
					return ToString((DateTimeOffset)value);
				}
				if (value is Guid)
				{
					return ToString((Guid)value);
				}
				if (value is Uri)
				{
					return ToString((Uri)value);
				}
				if (value is TimeSpan)
				{
					return ToString((TimeSpan)value);
				}
			}
			throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
		}

		private static bool IsJsonPrimitiveTypeCode(TypeCode typeCode)
		{
			switch (typeCode)
			{
			case TypeCode.DBNull:
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
			case TypeCode.String:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsJsonPrimitiveType(Type type)
		{
			if (ReflectionUtils.IsNullableType(type))
			{
				type = Nullable.GetUnderlyingType(type);
			}
			if (type == typeof(DateTimeOffset))
			{
				return true;
			}
			if (type == typeof(byte[]))
			{
				return true;
			}
			if (type == typeof(Uri))
			{
				return true;
			}
			if (type == typeof(TimeSpan))
			{
				return true;
			}
			if (type == typeof(Guid))
			{
				return true;
			}
			return IsJsonPrimitiveTypeCode(Type.GetTypeCode(type));
		}

		internal static bool IsJsonPrimitive(object value)
		{
			if (value == null)
			{
				return true;
			}
			IConvertible convertible = value as IConvertible;
			if (convertible != null)
			{
				return IsJsonPrimitiveTypeCode(convertible.GetTypeCode());
			}
			if (value is DateTimeOffset)
			{
				return true;
			}
			if (value is byte[])
			{
				return true;
			}
			if (value is Uri)
			{
				return true;
			}
			if (value is TimeSpan)
			{
				return true;
			}
			if (value is Guid)
			{
				return true;
			}
			return false;
		}

		public static string SerializeObject(object value)
		{
			return SerializeObject(value, Formatting.None, (JsonSerializerSettings)null);
		}

		public static string SerializeObject(object value, Formatting formatting)
		{
			return SerializeObject(value, formatting, (JsonSerializerSettings)null);
		}

		public static string SerializeObject(object value, params JsonConverter[] converters)
		{
			return SerializeObject(value, Formatting.None, converters);
		}

		public static string SerializeObject(object value, Formatting formatting, params JsonConverter[] converters)
		{
			object obj;
			if (converters != null && converters.Length > 0)
			{
				JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
				jsonSerializerSettings.Converters = converters;
				obj = jsonSerializerSettings;
			}
			else
			{
				obj = null;
			}
			JsonSerializerSettings settings = (JsonSerializerSettings)obj;
			return SerializeObject(value, formatting, settings);
		}

		public static string SerializeObject(object value, Formatting formatting, JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.Create(settings);
			StringBuilder sb = new StringBuilder(128);
			StringWriter stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture);
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.Formatting = formatting;
				jsonSerializer.Serialize(jsonTextWriter, value);
			}
			return stringWriter.ToString();
		}

		public static object DeserializeObject(string value)
		{
			return DeserializeObject(value, (Type)null, (JsonSerializerSettings)null);
		}

		public static object DeserializeObject(string value, JsonSerializerSettings settings)
		{
			return DeserializeObject(value, null, settings);
		}

		public static object DeserializeObject(string value, Type type)
		{
			return DeserializeObject(value, type, (JsonSerializerSettings)null);
		}

		public static T DeserializeObject<T>(string value)
		{
			return JsonConvert.DeserializeObject<T>(value, (JsonSerializerSettings)null);
		}

		public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject)
		{
			return DeserializeObject<T>(value);
		}

		public static T DeserializeObject<T>(string value, params JsonConverter[] converters)
		{
			return (T)DeserializeObject(value, typeof(T), converters);
		}

		public static T DeserializeObject<T>(string value, JsonSerializerSettings settings)
		{
			return (T)DeserializeObject(value, typeof(T), settings);
		}

		public static object DeserializeObject(string value, Type type, params JsonConverter[] converters)
		{
			object obj;
			if (converters != null && converters.Length > 0)
			{
				JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
				jsonSerializerSettings.Converters = converters;
				obj = jsonSerializerSettings;
			}
			else
			{
				obj = null;
			}
			JsonSerializerSettings settings = (JsonSerializerSettings)obj;
			return DeserializeObject(value, type, settings);
		}

		public static object DeserializeObject(string value, Type type, JsonSerializerSettings settings)
		{
			StringReader reader = new StringReader(value);
			JsonSerializer jsonSerializer = JsonSerializer.Create(settings);
			using (JsonReader jsonReader = new JsonTextReader(reader))
			{
				object result = jsonSerializer.Deserialize(jsonReader, type);
				if (!jsonReader.Read())
				{
					return result;
				}
				if (jsonReader.TokenType != JsonToken.Comment)
				{
					throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
				}
				return result;
			}
		}

		public static void PopulateObject(string value, object target)
		{
			PopulateObject(value, target, null);
		}

		public static void PopulateObject(string value, object target, JsonSerializerSettings settings)
		{
			StringReader reader = new StringReader(value);
			JsonSerializer jsonSerializer = JsonSerializer.Create(settings);
			using (JsonReader jsonReader = new JsonTextReader(reader))
			{
				jsonSerializer.Populate(jsonReader, target);
				if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
				{
					throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
				}
			}
		}

		public static string SerializeXmlNode(XmlNode node)
		{
			return SerializeXmlNode(node, Formatting.None);
		}

		public static string SerializeXmlNode(XmlNode node, Formatting formatting)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter();
			return SerializeObject(node, formatting, xmlNodeConverter);
		}

		public static string SerializeXmlNode(XmlNode node, Formatting formatting, bool omitRootObject)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter();
			xmlNodeConverter.OmitRootObject = omitRootObject;
			XmlNodeConverter xmlNodeConverter2 = xmlNodeConverter;
			return SerializeObject(node, formatting, xmlNodeConverter2);
		}

		public static XmlDocument DeserializeXmlNode(string value)
		{
			return DeserializeXmlNode(value, null);
		}

		public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName)
		{
			return DeserializeXmlNode(value, deserializeRootElementName, writeArrayAttribute: false);
		}

		public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName, bool writeArrayAttribute)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter();
			xmlNodeConverter.DeserializeRootElementName = deserializeRootElementName;
			xmlNodeConverter.WriteArrayAttribute = writeArrayAttribute;
			return (XmlDocument)DeserializeObject(value, typeof(XmlDocument), xmlNodeConverter);
		}
	}
}
