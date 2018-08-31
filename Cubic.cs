internal class Cubic : Ease
{
	public static double EaseIn(double t, double b, double c, double d)
	{
		return c * (t /= d) * t * t + b;
	}

	public static double EaseOut(double t, double b, double c, double d)
	{
		return c * ((t = t / d - 1.0) * t * t + 1.0) + b;
	}

	public static double EaseInOut(double t, double b, double c, double d)
	{
		if ((t /= d / 2.0) < 1.0)
		{
			return c / 2.0 * t * t * t + b;
		}
		return c / 2.0 * ((t -= 2.0) * t * t + 2.0) + b;
	}
}
