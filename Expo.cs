using System;

internal class Expo : Ease
{
	public static double EaseIn(double t, double b, double c, double d)
	{
		return (t != 0.0) ? (c * Math.Pow(2.0, 10.0 * (t / d - 1.0)) + b - c * 0.001) : b;
	}

	public static double EaseOut(double t, double b, double c, double d)
	{
		return (t != d) ? (c * (0.0 - Math.Pow(2.0, -10.0 * t / d) + 1.0) + b) : (b + c);
	}

	public static double EaseInOut(double t, double b, double c, double d)
	{
		if (t == 0.0)
		{
			return b;
		}
		if (t == d)
		{
			return b + c;
		}
		if ((t /= d / 2.0) < 1.0)
		{
			return c / 2.0 * Math.Pow(2.0, 10.0 * (t - 1.0)) + b;
		}
		return c / 2.0 * (0.0 - Math.Pow(2.0, -10.0 * (t -= 1.0)) + 2.0) + b;
	}
}
