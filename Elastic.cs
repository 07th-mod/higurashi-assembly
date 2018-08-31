using System;

internal class Elastic : Ease
{
	public static double EaseIn(double t, double b, double c, double d)
	{
		double num = 0.0;
		double num2 = d * 0.3;
		if (t == 0.0)
		{
			return b;
		}
		if ((t /= d) == 1.0)
		{
			return b + c;
		}
		double num3;
		if (num < Math.Abs(c))
		{
			num = c;
			num3 = num2 / 4.0;
		}
		else
		{
			num3 = num2 / 6.2831853071795862 * Math.Asin(c / num);
		}
		return 0.0 - num * Math.Pow(2.0, 10.0 * (t -= 1.0)) * Math.Sin((t * d - num3) * 6.2831853071795862 / num2) + b;
	}

	public static double EaseOut(double t, double b, double c, double d)
	{
		double num = 0.0;
		double num2 = d * 0.3;
		if (t == 0.0)
		{
			return b;
		}
		if ((t /= d) == 1.0)
		{
			return b + c;
		}
		double num3;
		if (num < Math.Abs(c))
		{
			num = c;
			num3 = num2 / 4.0;
		}
		else
		{
			num3 = num2 / 6.2831853071795862 * Math.Asin(c / num);
		}
		return num * Math.Pow(2.0, -10.0 * t) * Math.Sin((t * d - num3) * 6.2831853071795862 / num2) + c + b;
	}

	public static double EaseInOut(double t, double b, double c, double d)
	{
		double num = 0.0;
		double num2 = d * 0.44999999999999996;
		if (t == 0.0)
		{
			return b;
		}
		if ((t /= d / 2.0) == 2.0)
		{
			return b + c;
		}
		double num3;
		if (num < Math.Abs(c))
		{
			num = c;
			num3 = num2 / 4.0;
		}
		else
		{
			num3 = num2 / 6.2831853071795862 * Math.Asin(c / num);
		}
		if (t < 1.0)
		{
			return -0.5 * (num * Math.Pow(2.0, 10.0 * (t -= 1.0)) * Math.Sin((t * d - num3) * 6.2831853071795862 / num2)) + b;
		}
		return num * Math.Pow(2.0, -10.0 * (t -= 1.0)) * Math.Sin((t * d - num3) * 6.2831853071795862 / num2) * 0.5 + c + b;
	}

	public static double EaseIn(double t, double b, double c, double d, double a, double p)
	{
		if (t == 0.0)
		{
			return b;
		}
		if ((t /= d) == 1.0)
		{
			return b + c;
		}
		if (p == 0.0)
		{
			p = d * 0.3;
		}
		double num;
		if (a < Math.Abs(c))
		{
			a = c;
			num = p / 4.0;
		}
		else
		{
			num = p / 6.2831853071795862 * Math.Asin(c / a);
		}
		return 0.0 - a * Math.Pow(2.0, 10.0 * (t -= 1.0)) * Math.Sin((t * d - num) * 6.2831853071795862 / p) + b;
	}

	public static double EaseOut(double t, double b, double c, double d, double a, double p)
	{
		if (t == 0.0)
		{
			return b;
		}
		if ((t /= d) == 1.0)
		{
			return b + c;
		}
		if (p == 0.0)
		{
			p = d * 0.3;
		}
		double num;
		if (a < Math.Abs(c))
		{
			a = c;
			num = p / 4.0;
		}
		else
		{
			num = p / 6.2831853071795862 * Math.Asin(c / a);
		}
		return a * Math.Pow(2.0, -10.0 * t) * Math.Sin((t * d - num) * 6.2831853071795862 / p) + c + b;
	}

	public static double EaseInOut(double t, double b, double c, double d, double a, double p)
	{
		if (t == 0.0)
		{
			return b;
		}
		if ((t /= d / 2.0) == 2.0)
		{
			return b + c;
		}
		if (p == 0.0)
		{
			p = d * 0.44999999999999996;
		}
		double num;
		if (a < Math.Abs(c))
		{
			a = c;
			num = p / 4.0;
		}
		else
		{
			num = p / 6.2831853071795862 * Math.Asin(c / a);
		}
		if (t < 1.0)
		{
			return -0.5 * (a * Math.Pow(2.0, 10.0 * (t -= 1.0)) * Math.Sin((t * d - num) * 6.2831853071795862 / p)) + b;
		}
		return a * Math.Pow(2.0, -10.0 * (t -= 1.0)) * Math.Sin((t * d - num) * 6.2831853071795862 / p) * 0.5 + c + b;
	}
}
