using System;

internal class Circ : Ease
{
	public static double EaseIn(double t, double b, double c, double d)
	{
		return (0.0 - c) * (Math.Sqrt(1.0 - (t /= d) * t) - 1.0) + b;
	}

	public static double EaseOut(double t, double b, double c, double d)
	{
		return c * Math.Sqrt(1.0 - (t = t / d - 1.0) * t) + b;
	}

	public static double EaseInOut(double t, double b, double c, double d)
	{
		if ((t /= d / 2.0) < 1.0)
		{
			return (0.0 - c) / 2.0 * (Math.Sqrt(1.0 - t * t) - 1.0) + b;
		}
		return c / 2.0 * (Math.Sqrt(1.0 - (t -= 2.0) * t) + 1.0) + b;
	}
}
