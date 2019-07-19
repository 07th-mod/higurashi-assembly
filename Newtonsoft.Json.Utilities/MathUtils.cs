using System;

namespace Newtonsoft.Json.Utilities
{
	internal class MathUtils
	{
		public static int IntLength(int i)
		{
			if (i < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (i == 0)
			{
				return 1;
			}
			return (int)Math.Floor(Math.Log10(i)) + 1;
		}

		public static int HexToInt(char h)
		{
			if (h >= '0' && h <= '9')
			{
				return h - 48;
			}
			if (h >= 'a' && h <= 'f')
			{
				return h - 97 + 10;
			}
			if (h >= 'A' && h <= 'F')
			{
				return h - 65 + 10;
			}
			return -1;
		}

		public static char IntToHex(int n)
		{
			if (n <= 9)
			{
				return (char)(n + 48);
			}
			return (char)(n - 10 + 97);
		}

		public static int GetDecimalPlaces(double value)
		{
			int num = 10;
			double num2 = Math.Pow(0.1, num);
			if (value == 0.0)
			{
				return 0;
			}
			int num3 = 0;
			while (value - Math.Floor(value) > num2 && num3 < num)
			{
				value *= 10.0;
				num3++;
			}
			return num3;
		}

		public static int? Min(int? val1, int? val2)
		{
			if (!val1.HasValue)
			{
				return val2;
			}
			if (!val2.HasValue)
			{
				return val1;
			}
			return Math.Min(val1.Value, val2.Value);
		}

		public static int? Max(int? val1, int? val2)
		{
			if (!val1.HasValue)
			{
				return val2;
			}
			if (!val2.HasValue)
			{
				return val1;
			}
			return Math.Max(val1.Value, val2.Value);
		}

		public static double? Min(double? val1, double? val2)
		{
			if (!val1.HasValue)
			{
				return val2;
			}
			if (!val2.HasValue)
			{
				return val1;
			}
			return Math.Min(val1.Value, val2.Value);
		}

		public static double? Max(double? val1, double? val2)
		{
			if (!val1.HasValue)
			{
				return val2;
			}
			if (!val2.HasValue)
			{
				return val1;
			}
			return Math.Max(val1.Value, val2.Value);
		}

		public static bool ApproxEquals(double d1, double d2)
		{
			return Math.Abs(d1 - d2) < Math.Abs(d1) * 1E-06;
		}
	}
}
