internal class Back : Ease
{
	public static double EaseIn(double t, double b, double c, double d)
	{
		double num = 1.70158;
		return c * (t /= d) * t * ((num + 1.0) * t - num) + b;
	}

	public static double EaseOut(double t, double b, double c, double d)
	{
		double num = 1.70158;
		return c * ((t = t / d - 1.0) * t * ((num + 1.0) * t + num) + 1.0) + b;
	}

	public static double EaseInOut(double t, double b, double c, double d)
	{
		double num = 1.70158;
		if ((t /= d / 2.0) < 1.0)
		{
			return c / 2.0 * (t * t * (((num *= 1.525) + 1.0) * t - num)) + b;
		}
		return c / 2.0 * ((t -= 2.0) * t * (((num *= 1.525) + 1.0) * t + num) + 2.0) + b;
	}

	public static double EaseIn(double t, double b, double c, double d, double s)
	{
		return c * (t /= d) * t * ((s + 1.0) * t - s) + b;
	}

	public static double EaseOut(double t, double b, double c, double d, double s)
	{
		return c * ((t = t / d - 1.0) * t * ((s + 1.0) * t + s) + 1.0) + b;
	}

	public static double EaseInOut(double t, double b, double c, double d, double s)
	{
		if ((t /= d / 2.0) < 1.0)
		{
			return c / 2.0 * (t * t * (((s *= 1.525) + 1.0) * t - s)) + b;
		}
		return c / 2.0 * ((t -= 2.0) * t * (((s *= 1.525) + 1.0) * t + s) + 2.0) + b;
	}
}
