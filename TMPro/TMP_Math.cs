namespace TMPro
{
	public static class TMP_Math
	{
		public static bool Equals(float a, float b)
		{
			return b - 1E-06f < a && a < b + 1E-06f;
		}
	}
}
