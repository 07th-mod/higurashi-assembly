namespace UMP.Services.Helpers
{
	internal static class Text
	{
		public static string StringBetween(string prefix, string suffix, string parent)
		{
			int num = parent.IndexOf(prefix) + prefix.Length;
			if (num < prefix.Length)
			{
				return string.Empty;
			}
			int num2 = parent.IndexOf(suffix, num);
			if (num2 == -1)
			{
				num2 = parent.Length;
			}
			return parent.Substring(num, num2 - num);
		}

		public static int SkipWhitespace(this string text, int start)
		{
			int i;
			for (i = start; char.IsWhiteSpace(text[i]); i++)
			{
			}
			return i;
		}
	}
}
