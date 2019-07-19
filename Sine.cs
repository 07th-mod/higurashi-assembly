using System;

internal class Sine : Ease
{
	public static double EaseIn(double t, double b, double c, double d)
	{
		return (0.0 - c) * Math.Cos(t / d * (Math.PI / 2.0)) + c + b;
	}

	public static double EaseOut(double t, double b, double c, double d)
	{
		return c * Math.Sin(t / d * (Math.PI / 2.0)) + b;
	}

	public static double EaseInOut(double t, double b, double c, double d)
	{
		return (0.0 - c) / 2.0 * (Math.Cos(Math.PI * t / d) - 1.0) + b;
	}
}
