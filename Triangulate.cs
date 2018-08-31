using System.Collections.Generic;
using UnityEngine;

public class Triangulate
{
	private static float EPSILON = 1E-10f;

	public static float Area(ref List<Vector2> contour)
	{
		int count = contour.Count;
		float num = 0f;
		int index = count - 1;
		int num2 = 0;
		while (num2 < count)
		{
			float num3 = num;
			Vector2 vector = contour[index];
			float x = vector.x;
			Vector2 vector2 = contour[num2];
			float num4 = x * vector2.y;
			Vector2 vector3 = contour[num2];
			float x2 = vector3.x;
			Vector2 vector4 = contour[index];
			num = num3 + (num4 - x2 * vector4.y);
			index = num2++;
		}
		return num * 0.5f;
	}

	public static bool InsideTriangle(float Ax, float Ay, float Bx, float By, float Cx, float Cy, float Px, float Py)
	{
		float num = Cx - Bx;
		float num2 = Cy - By;
		float num3 = Ax - Cx;
		float num4 = Ay - Cy;
		float num5 = Bx - Ax;
		float num6 = By - Ay;
		float num7 = Px - Ax;
		float num8 = Py - Ay;
		float num9 = Px - Bx;
		float num10 = Py - By;
		float num11 = Px - Cx;
		float num12 = Py - Cy;
		float num13 = num * num10 - num2 * num9;
		float num14 = num5 * num8 - num6 * num7;
		float num15 = num3 * num12 - num4 * num11;
		return num13 >= 0f && num15 >= 0f && num14 >= 0f;
	}

	public static bool Snip(ref List<Vector2> contour, int u, int v, int w, int n, ref List<int> V)
	{
		Vector2 vector = contour[V[u]];
		float x = vector.x;
		Vector2 vector2 = contour[V[u]];
		float y = vector2.y;
		Vector2 vector3 = contour[V[v]];
		float x2 = vector3.x;
		Vector2 vector4 = contour[V[v]];
		float y2 = vector4.y;
		Vector2 vector5 = contour[V[w]];
		float x3 = vector5.x;
		Vector2 vector6 = contour[V[w]];
		float y3 = vector6.y;
		if (EPSILON > (x2 - x) * (y3 - y) - (y2 - y) * (x3 - x))
		{
			return false;
		}
		for (int i = 0; i < n; i++)
		{
			if (i != u && i != v && i != w)
			{
				Vector2 vector7 = contour[V[i]];
				float x4 = vector7.x;
				Vector2 vector8 = contour[V[i]];
				float y4 = vector8.y;
				if (InsideTriangle(x, y, x2, y2, x3, y3, x4, y4))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool Process(ref List<Vector2> contour, ref List<int> result, out bool counterClockwise)
	{
		counterClockwise = false;
		int count = contour.Count;
		if (count < 3)
		{
			return false;
		}
		List<int> V = new List<int>();
		if (0f < Area(ref contour))
		{
			counterClockwise = true;
			for (int i = 0; i < count; i++)
			{
				V.Add(i);
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				V.Add(count - 1 - j);
			}
		}
		int num = count;
		int num2 = 2 * num;
		int num3 = 0;
		int num4 = num - 1;
		while (num > 2)
		{
			if (0 >= num2--)
			{
				return false;
			}
			int num6 = num4;
			if (num <= num6)
			{
				num6 = 0;
			}
			num4 = num6 + 1;
			if (num <= num4)
			{
				num4 = 0;
			}
			int num7 = num4 + 1;
			if (num <= num7)
			{
				num7 = 0;
			}
			if (Snip(ref contour, num6, num4, num7, num, ref V))
			{
				int item = V[num6];
				int item2 = V[num4];
				int item3 = V[num7];
				result.Add(item3);
				result.Add(item2);
				result.Add(item);
				num3++;
				int num8 = num4;
				for (int k = num4 + 1; k < num; k++)
				{
					V[num8] = V[k];
					num8++;
				}
				num--;
				num2 = 2 * num;
			}
		}
		return true;
	}
}
