using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Newtonsoft.Json.Utilities
{
	internal static class StringUtils
	{
		private delegate void ActionLine(TextWriter textWriter, string line);

		public const string CarriageReturnLineFeed = "\r\n";

		public const string Empty = "";

		public const char CarriageReturn = '\r';

		public const char LineFeed = '\n';

		public const char Tab = '\t';

		public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
		{
			ValidationUtils.ArgumentNotNull(format, "format");
			return string.Format(provider, format, args);
		}

		public static bool ContainsWhiteSpace(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (char.IsWhiteSpace(s[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsWhiteSpace(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (!char.IsWhiteSpace(s[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static string EnsureEndsWith(string target, string value)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (target.Length >= value.Length)
			{
				if (string.Compare(target, target.Length - value.Length, value, 0, value.Length, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return target;
				}
				string text = target.TrimEnd(null);
				if (string.Compare(text, text.Length - value.Length, value, 0, value.Length, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return target;
				}
			}
			return target + value;
		}

		public static bool IsNullOrEmptyOrWhiteSpace(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return true;
			}
			if (IsWhiteSpace(s))
			{
				return true;
			}
			return false;
		}

		public static void IfNotNullOrEmpty(string value, Action<string> action)
		{
			IfNotNullOrEmpty(value, action, null);
		}

		private static void IfNotNullOrEmpty(string value, Action<string> trueAction, Action<string> falseAction)
		{
			if (!string.IsNullOrEmpty(value))
			{
				trueAction?.Invoke(value);
			}
			else
			{
				falseAction?.Invoke(value);
			}
		}

		public static string Indent(string s, int indentation)
		{
			return Indent(s, indentation, ' ');
		}

		public static string Indent(string s, int indentation, char indentChar)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (indentation <= 0)
			{
				throw new ArgumentException("Must be greater than zero.", "indentation");
			}
			StringReader textReader = new StringReader(s);
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			ActionTextReaderLine(textReader, stringWriter, delegate(TextWriter tw, string line)
			{
				tw.Write(new string(indentChar, indentation));
				tw.Write(line);
			});
			return stringWriter.ToString();
		}

		private static void ActionTextReaderLine(TextReader textReader, TextWriter textWriter, ActionLine lineAction)
		{
			bool flag = true;
			string line;
			while ((line = textReader.ReadLine()) != null)
			{
				if (!flag)
				{
					textWriter.WriteLine();
				}
				else
				{
					flag = false;
				}
				lineAction(textWriter, line);
			}
		}

		public static string NumberLines(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			StringReader textReader = new StringReader(s);
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			int lineNumber = 1;
			ActionTextReaderLine(textReader, stringWriter, delegate(TextWriter tw, string line)
			{
				tw.Write(lineNumber.ToString(CultureInfo.InvariantCulture).PadLeft(4));
				tw.Write(". ");
				tw.Write(line);
				lineNumber++;
			});
			return stringWriter.ToString();
		}

		public static string NullEmptyString(string s)
		{
			return (!string.IsNullOrEmpty(s)) ? s : null;
		}

		public static string ReplaceNewLines(string s, string replacement)
		{
			StringReader stringReader = new StringReader(s);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			string value;
			while ((value = stringReader.ReadLine()) != null)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(replacement);
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public static string Truncate(string s, int maximumLength)
		{
			return Truncate(s, maximumLength, "...");
		}

		public static string Truncate(string s, int maximumLength, string suffix)
		{
			if (suffix == null)
			{
				throw new ArgumentNullException("suffix");
			}
			if (maximumLength <= 0)
			{
				throw new ArgumentException("Maximum length must be greater than zero.", "maximumLength");
			}
			int num = maximumLength - suffix.Length;
			if (num <= 0)
			{
				throw new ArgumentException("Length of suffix string is greater or equal to maximumLength");
			}
			if (s != null && s.Length > maximumLength)
			{
				string text = s.Substring(0, num);
				text = text.Trim();
				return text + suffix;
			}
			return s;
		}

		public static StringWriter CreateStringWriter(int capacity)
		{
			StringBuilder sb = new StringBuilder(capacity);
			return new StringWriter(sb, CultureInfo.InvariantCulture);
		}

		public static int? GetLength(string value)
		{
			return value?.Length;
		}

		public static string ToCharAsUnicode(char c)
		{
			char c2 = MathUtils.IntToHex(((int)c >> 12) & 0xF);
			char c3 = MathUtils.IntToHex(((int)c >> 8) & 0xF);
			char c4 = MathUtils.IntToHex(((int)c >> 4) & 0xF);
			char c5 = MathUtils.IntToHex(c & 0xF);
			return new string(new char[6]
			{
				'\\',
				'u',
				c2,
				c3,
				c4,
				c5
			});
		}

		public static void WriteCharAsUnicode(TextWriter writer, char c)
		{
			ValidationUtils.ArgumentNotNull(writer, "writer");
			char value = MathUtils.IntToHex(((int)c >> 12) & 0xF);
			char value2 = MathUtils.IntToHex(((int)c >> 8) & 0xF);
			char value3 = MathUtils.IntToHex(((int)c >> 4) & 0xF);
			char value4 = MathUtils.IntToHex(c & 0xF);
			writer.Write('\\');
			writer.Write('u');
			writer.Write(value);
			writer.Write(value2);
			writer.Write(value3);
			writer.Write(value4);
		}

		public static TSource ForgivingCaseSensitiveFind<TSource>(this IEnumerable<TSource> source, Func<TSource, string> valueSelector, string testValue)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (valueSelector == null)
			{
				throw new ArgumentNullException("valueSelector");
			}
			TSource[] array = (from s in source
			where string.Compare(valueSelector(s), testValue, StringComparison.OrdinalIgnoreCase) == 0
			select s).ToArray();
			int num = array.Length;
			if (num <= 1)
			{
				return (num != 1) ? default(TSource) : array[0];
			}
			IEnumerable<TSource> source2 = from s in source
			where string.Compare(valueSelector(s), testValue, StringComparison.Ordinal) == 0
			select s;
			return source2.SingleOrDefault();
		}

		public static string ToCamelCase(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			if (!char.IsUpper(s[0]))
			{
				return s;
			}
			string text = char.ToLower(s[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
			if (s.Length > 1)
			{
				text += s.Substring(1);
			}
			return text;
		}
	}
}
