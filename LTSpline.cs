using System;
using UnityEngine;

[Serializable]
public class LTSpline
{
	public Vector3[] pts;

	public bool orientToPath;

	private float[] lengthRatio;

	private float[] lengths;

	private int numSections;

	private int currPt;

	private float totalLength;

	public LTSpline(params Vector3[] pts)
	{
		this.pts = new Vector3[pts.Length];
		Array.Copy(pts, this.pts, pts.Length);
		numSections = pts.Length - 3;
		int num = 20;
		lengthRatio = new float[num];
		lengths = new float[num];
		Vector3 b = new Vector3(float.PositiveInfinity, 0f, 0f);
		totalLength = 0f;
		for (int i = 0; i < num; i++)
		{
			float t = (float)i * 1f / (float)num;
			Vector3 vector = interp(t);
			if (i >= 1)
			{
				lengths[i] = (vector - b).magnitude;
			}
			totalLength += lengths[i];
			b = vector;
		}
		float num2 = 0f;
		for (int j = 0; j < lengths.Length; j++)
		{
			float num3 = (float)j * 1f / (float)(lengths.Length - 1);
			currPt = Mathf.Min(Mathf.FloorToInt(num3 * (float)numSections), numSections - 1);
			float num4 = lengths[j] / totalLength;
			num2 += num4;
			lengthRatio[j] = num2;
		}
	}

	public float map(float t)
	{
		for (int i = 0; i < lengthRatio.Length; i++)
		{
			if (lengthRatio[i] >= t)
			{
				return lengthRatio[i] + (t - lengthRatio[i]);
			}
		}
		return 1f;
	}

	public Vector3 interp(float t)
	{
		currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
		float num = t * (float)numSections - (float)currPt;
		Vector3 a = pts[currPt];
		Vector3 a2 = pts[currPt + 1];
		Vector3 vector = pts[currPt + 2];
		Vector3 b = pts[currPt + 3];
		return 0.5f * ((-a + 3f * a2 - 3f * vector + b) * (num * num * num) + (2f * a - 5f * a2 + 4f * vector - b) * (num * num) + (-a + vector) * num + 2f * a2);
	}

	public Vector3 point(float ratio)
	{
		float t = map(ratio);
		return interp(t);
	}

	public void place(Transform transform, float ratio)
	{
		place(transform, ratio, Vector3.up);
	}

	public void place(Transform transform, float ratio, Vector3 worldUp)
	{
		transform.position = point(ratio);
		ratio += 0.001f;
		if (ratio <= 1f)
		{
			transform.LookAt(point(ratio), worldUp);
		}
	}

	public void placeLocal(Transform transform, float ratio)
	{
		placeLocal(transform, ratio, Vector3.up);
	}

	public void placeLocal(Transform transform, float ratio, Vector3 worldUp)
	{
		transform.localPosition = point(ratio);
		ratio += 0.001f;
		if (ratio <= 1f)
		{
			transform.LookAt(transform.parent.TransformPoint(point(ratio)), worldUp);
		}
	}

	public void gizmoDraw(float t = -1f)
	{
		if (lengthRatio != null && lengthRatio.Length > 0)
		{
			Vector3 to = point(0f);
			for (int i = 1; i <= 120; i++)
			{
				float ratio = (float)i / 120f;
				Vector3 vector = point(ratio);
				Gizmos.DrawLine(vector, to);
				to = vector;
			}
		}
	}

	public Vector3 Velocity(float t)
	{
		t = map(t);
		int num = pts.Length - 3;
		int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
		float num3 = t * (float)num - (float)num2;
		Vector3 a = pts[num2];
		Vector3 a2 = pts[num2 + 1];
		Vector3 a3 = pts[num2 + 2];
		Vector3 b = pts[num2 + 3];
		return 1.5f * (-a + 3f * a2 - 3f * a3 + b) * (num3 * num3) + (2f * a - 5f * a2 + 4f * a3 - b) * num3 + 0.5f * a3 - 0.5f * a;
	}
}
