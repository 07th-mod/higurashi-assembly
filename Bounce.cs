internal class Bounce : Ease
{
	public static double EaseOut(double t, double b, double c, double d)
	{
		if ((t /= d) < 0.36363636363636365)
		{
			return c * (7.5625 * t * t) + b;
		}
		if (t < 0.72727272727272729)
		{
			return c * (7.5625 * (t -= 0.54545454545454541) * t + 0.75) + b;
		}
		if (t < 0.90909090909090906)
		{
			return c * (7.5625 * (t -= 0.81818181818181823) * t + 0.9375) + b;
		}
		return c * (7.5625 * (t -= 21.0 / 22.0) * t + 63.0 / 64.0) + b;
	}

	public static double EaseIn(double t, double b, double c, double d)
	{
		return c - EaseOut(d - t, 0.0, c, d) + b;
	}

	public static double EaseInOut(double t, double b, double c, double d)
	{
		if (t < d / 2.0)
		{
			return EaseIn(t * 2.0, 0.0, c, d) * 0.5 + b;
		}
		return EaseOut(t * 2.0 - d, 0.0, c, d) * 0.5 + c * 0.5 + b;
	}
}
