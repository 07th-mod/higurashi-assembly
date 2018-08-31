using System;

internal class Sine : Ease
{
	public static double EaseIn(double t, double b, double c, double d)
	{
		return (0.0 - c) * Math.Cos(t / d * 1.5707963267948966) + c + b;
	}

	public static double EaseOut(double t, double b, double c, double d)
	{
		return c * Math.Sin(t / d * 1.5707963267948966) + b;
	}

	public static double EaseInOut(double t, double b, double c, double d)
	{
		return (0.0 - c) / 2.0 * (Math.Cos(3.1415926535897931 * t / d) - 1.0) + b;
	}
}
