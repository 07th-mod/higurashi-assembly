using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMP.Services.Helpers
{
	internal static class HttpUtility
	{
		public static IEnumerable<string> GetUrisFromManifest(string source)
		{
			string text = "<BaseURL>";
			string closing = "</BaseURL>";
			int num = source.IndexOf(text);
			if (num != -1)
			{
				string text2 = source.Substring(num);
				return from v in text2.Split(new string[1]
					{
						text
					}, StringSplitOptions.RemoveEmptyEntries)
					select v.Substring(0, v.IndexOf(closing));
			}
			throw new NotSupportedException();
		}

		public static string UrlDecode(string str)
		{
			return UrlDecode(str, Encoding.UTF8);
		}

		public static string UrlDecode(string s, Encoding e)
		{
			if (s == null)
			{
				return null;
			}
			if (s.IndexOf('%') == -1 && s.IndexOf('+') == -1)
			{
				return s;
			}
			if (e == null)
			{
				e = Encoding.UTF8;
			}
			long num = s.Length;
			List<byte> list = new List<byte>();
			for (int i = 0; i < num; i++)
			{
				char c = s[i];
				if (c == '%' && i + 2 < num && s[i + 1] != '%')
				{
					int @char;
					if (s[i + 1] == 'u' && i + 5 < num)
					{
						@char = GetChar(s, i + 2, 4);
						if (@char != -1)
						{
							WriteCharBytes(list, (char)@char, e);
							i += 5;
						}
						else
						{
							WriteCharBytes(list, '%', e);
						}
					}
					else if ((@char = GetChar(s, i + 1, 2)) != -1)
					{
						WriteCharBytes(list, (char)@char, e);
						i += 2;
					}
					else
					{
						WriteCharBytes(list, '%', e);
					}
				}
				else if (c == '+')
				{
					WriteCharBytes(list, ' ', e);
				}
				else
				{
					WriteCharBytes(list, c, e);
				}
			}
			byte[] bytes = list.ToArray();
			list.Clear();
			list = null;
			return e.GetString(bytes);
		}

		private static void WriteCharBytes(IList buf, char ch, Encoding e)
		{
			if (ch > 'ÿ')
			{
				byte[] bytes = e.GetBytes(new char[1]
				{
					ch
				});
				foreach (byte b in bytes)
				{
					buf.Add(b);
				}
			}
			else
			{
				buf.Add((byte)ch);
			}
		}

		private static int GetChar(string str, int offset, int length)
		{
			int num = 0;
			int num2 = length + offset;
			for (int i = offset; i < num2; i++)
			{
				char c = str[i];
				if (c > '\u007f')
				{
					return -1;
				}
				int @int = GetInt((byte)c);
				if (@int == -1)
				{
					return -1;
				}
				num = (num << 4) + @int;
			}
			return num;
		}

		private static int GetInt(byte b)
		{
			char c = (char)b;
			if (c >= '0' && c <= '9')
			{
				return c - 48;
			}
			if (c >= 'a' && c <= 'f')
			{
				return c - 97 + 10;
			}
			if (c >= 'A' && c <= 'F')
			{
				return c - 65 + 10;
			}
			return -1;
		}
	}
}
