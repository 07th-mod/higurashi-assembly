using System.IO;

namespace Newtonsoft.Json.Utilities
{
	internal static class JavaScriptUtils
	{
		public static void WriteEscapedJavaScriptString(TextWriter writer, string value, char delimiter, bool appendDelimiters)
		{
			if (appendDelimiters)
			{
				writer.Write(delimiter);
			}
			if (value != null)
			{
				int num = 0;
				int num2 = 0;
				char[] array = null;
				for (int i = 0; i < value.Length; i++)
				{
					char c = value[i];
					string text;
					switch (c)
					{
					default:
						text = ((c == '\u2028') ? "\\u2028" : ((c == '\u2029') ? "\\u2029" : ((c == '"') ? ((delimiter != '"') ? null : "\\\"") : ((c == '\'') ? ((delimiter != '\'') ? null : "\\'") : ((c == '\\') ? "\\\\" : ((c != '\u0085') ? ((c > '\u001f') ? null : StringUtils.ToCharAsUnicode(c)) : "\\u0085"))))));
						break;
					case '\t':
						text = "\\t";
						break;
					case '\n':
						text = "\\n";
						break;
					case '\r':
						text = "\\r";
						break;
					case '\f':
						text = "\\f";
						break;
					case '\b':
						text = "\\b";
						break;
					}
					if (text != null)
					{
						if (array == null)
						{
							array = value.ToCharArray();
						}
						if (num2 > 0)
						{
							writer.Write(array, num, num2);
							num2 = 0;
						}
						writer.Write(text);
						num = i + 1;
					}
					else
					{
						num2++;
					}
				}
				if (num2 > 0)
				{
					if (num == 0)
					{
						writer.Write(value);
					}
					else
					{
						writer.Write(array, num, num2);
					}
				}
			}
			if (appendDelimiters)
			{
				writer.Write(delimiter);
			}
		}

		public static string ToEscapedJavaScriptString(string value)
		{
			return ToEscapedJavaScriptString(value, '"', appendDelimiters: true);
		}

		public static string ToEscapedJavaScriptString(string value, char delimiter, bool appendDelimiters)
		{
			int? length = StringUtils.GetLength(value);
			using (StringWriter stringWriter = StringUtils.CreateStringWriter((!length.HasValue) ? 16 : length.Value))
			{
				WriteEscapedJavaScriptString(stringWriter, value, delimiter, appendDelimiters);
				return stringWriter.ToString();
			}
		}
	}
}
