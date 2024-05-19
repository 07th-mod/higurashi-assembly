namespace UMP.Services.Helpers
{
	internal static class Json
	{
		public static string GetKey(string key, string source)
		{
			//Discarded unreachable code: IL_009a
			string text = '"' + key + '"';
			int num = 0;
			int start;
			while (true)
			{
				num = source.IndexOf(text, num);
				if (num == -1)
				{
					return string.Empty;
				}
				num += text.Length;
				start = num;
				start = source.SkipWhitespace(start);
				if (source[start++] == ':')
				{
					start = source.SkipWhitespace(start);
					if (source[start++] == '"')
					{
						break;
					}
				}
			}
			int i;
			for (i = start; source[i] != '"'; i++)
			{
			}
			return source.Substring(start, i - start);
		}
	}
}
