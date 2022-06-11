using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Mesh/Polygon")]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Polygon : MonoBehaviour
{
	[Serializable]
	public class ShapeData
	{
		public bool GenerateFront = true;

		public bool GenerateBack = true;

		public bool GenerateSides = true;

		public float Extrude = 1f;

		public float Elevation = 0.5f;

		public bool Enabled = true;
	}

	public List<Vector2> Points = new List<Vector2>();

	public HashSet<int> Selected = new HashSet<int>();

	public float SmoothAngle = 35f;

	public Vector2 FrontUVScale = new Vector2(0.25f, 0.25f);

	public Vector2 BackUVScale = new Vector2(0.25f, 0.25f);

	public Vector2 SideUVScale = new Vector2(0.25f, 0.25f);

	public ShapeData PolygonMesh = new ShapeData();

	public ShapeData PolygonCollider = new ShapeData();

	public int InsertBefore;

	private void Start()
	{
		UpdateComponents();
	}

	public void UpdateComponents()
	{
		if (Application.isPlaying)
		{
			GetComponent<MeshCollider>().sharedMesh = GenerateMesh(PolygonCollider.GenerateFront, PolygonCollider.GenerateBack, PolygonCollider.GenerateSides, PolygonCollider.Extrude, PolygonCollider.Elevation, useNormals: true, useUVS: true, Vector2.one, Vector2.one, Vector2.one);
		}
		Mesh mesh = GenerateMesh(PolygonMesh.GenerateFront, PolygonMesh.GenerateBack, PolygonMesh.GenerateSides, PolygonMesh.Extrude, PolygonMesh.Elevation, useNormals: true, useUVS: true, FrontUVScale, BackUVScale, SideUVScale);
		GetComponent<MeshFilter>().mesh = mesh;
	}

	private Mesh GenerateMesh(bool front, bool back, bool sides, float extrude, float elevate, bool useNormals, bool useUVS, Vector2 frontUVScale, Vector2 backUVScale, Vector2 sideUVScale)
	{
		Mesh mesh = new Mesh();
		if (Points.Count == 0)
		{
			return mesh;
		}
		int num = 0;
		int num2 = 0;
		Vector3 b = Vector3.back * elevate;
		List<Vector3> list = new List<Vector3>();
		if (front)
		{
			foreach (Vector2 point in Points)
			{
				Vector3 a = point;
				list.Add(a + b);
			}
		}
		num = list.Count;
		if (back)
		{
			foreach (Vector2 point2 in Points)
			{
				Vector3 a2 = point2;
				list.Add(a2 + Vector3.forward * extrude + b);
			}
		}
		num2 = list.Count;
		if (sides)
		{
			foreach (Vector2 point3 in Points)
			{
				Vector3 a3 = point3;
				list.Add(a3 + b);
				list.Add(a3 + b);
			}
			foreach (Vector2 point4 in Points)
			{
				Vector3 a4 = point4;
				list.Add(a4 + Vector3.forward * extrude + b);
				list.Add(a4 + Vector3.forward * extrude + b);
			}
		}
		mesh.vertices = list.ToArray();
		if (useUVS)
		{
			List<Vector2> list2 = new List<Vector2>();
			float num3 = 0f;
			Vector2 b2 = Points[Points.Count - 1];
			foreach (Vector2 point5 in Points)
			{
				num3 += (point5 - b2).magnitude;
				b2 = point5;
			}
			if (front)
			{
				for (int i = 0; i < Points.Count; i++)
				{
					Vector2 vector = Points[i];
					list2.Add(new Vector2(vector.x * frontUVScale.x, vector.y * frontUVScale.y));
				}
			}
			if (back)
			{
				for (int j = 0; j < Points.Count; j++)
				{
					Vector2 vector2 = Points[j];
					list2.Add(new Vector2(vector2.x * backUVScale.x, vector2.y * backUVScale.y));
				}
			}
			if (sides)
			{
				for (int k = 0; k < 2; k++)
				{
					Vector2 b3 = Points[0];
					float num4 = 0f;
					for (int l = 0; l < Points.Count; l++)
					{
						Vector2 vector3 = Points[l];
						num4 += (vector3 - b3).magnitude;
						list2.Add(new Vector2((float)k * extrude * sideUVScale.x, ((l == 0) ? num3 : num4) * sideUVScale.y));
						list2.Add(new Vector2((float)k * extrude * sideUVScale.x, num4 * sideUVScale.y));
						b3 = vector3;
					}
					num4 += (Points[0] - b3).magnitude;
				}
			}
			mesh.uv = list2.ToArray();
		}
		List<Vector2> contour = new List<Vector2>();
		foreach (Vector2 point6 in Points)
		{
			Vector3 v = point6;
			contour.Add(v);
		}
		List<int> result = new List<int>();
		Triangulate.Process(ref contour, ref result, out bool counterClockwise);
		List<int> list3 = new List<int>();
		if (front)
		{
			list3.AddRange(result);
		}
		if (back)
		{
			for (int m = 0; m < result.Count; m += 3)
			{
				list3.Add(result[m + 2] + num);
				list3.Add(result[m + 1] + num);
				list3.Add(result[m] + num);
			}
		}
		if (sides)
		{
			int num5 = num2;
			_ = Points.Count;
			int item = num5 + 0 + Points.Count * 2 - 1;
			int item2 = num2 + Points.Count * 2 + Points.Count * 2 - 1;
			for (int n = 0; n < Points.Count; n++)
			{
				int num6 = num2;
				_ = Points.Count;
				int num7 = num6 + 0 + n * 2;
				int num8 = num2 + Points.Count * 2 + n * 2;
				if (counterClockwise)
				{
					list3.Add(item);
					list3.Add(num7);
					list3.Add(item2);
					list3.Add(num7);
					list3.Add(num8);
					list3.Add(item2);
				}
				else
				{
					list3.Add(item);
					list3.Add(item2);
					list3.Add(num7);
					list3.Add(num7);
					list3.Add(item2);
					list3.Add(num8);
				}
				item = num7 + 1;
				item2 = num8 + 1;
			}
		}
		mesh.triangles = list3.ToArray();
		if (useNormals)
		{
			List<Vector3> list4 = new List<Vector3>();
			if (front)
			{
				for (int num9 = 0; num9 < Points.Count; num9++)
				{
					list4.Add(Vector3.back);
				}
			}
			if (back)
			{
				for (int num10 = 0; num10 < Points.Count; num10++)
				{
					list4.Add(Vector3.forward);
				}
			}
			if (sides)
			{
				for (int num11 = 0; num11 < 2; num11++)
				{
					for (int num12 = 0; num12 < Points.Count; num12++)
					{
						Vector2 a5 = Points[(num12 + Points.Count - 1) % Points.Count];
						Vector2 vector4 = Points[num12];
						Vector2 b4 = Points[(num12 + 1) % Points.Count];
						Vector2 normalized = (a5 - vector4).normalized;
						Vector2 normalized2 = (vector4 - b4).normalized;
						Vector2 vector5 = new Vector3(normalized.y, 0f - normalized.x, 0f).normalized;
						Vector2 vector6 = new Vector3(normalized2.y, 0f - normalized2.x, 0f).normalized;
						Vector2 normalized3 = (vector5 + vector6).normalized;
						if (counterClockwise)
						{
							normalized3 *= -1f;
							vector5 *= -1f;
							vector6 *= -1f;
						}
						if (Vector2.Dot(vector5, vector6) > Mathf.Cos((float)Math.PI / 180f * SmoothAngle))
						{
							list4.Add(normalized3);
							list4.Add(normalized3);
						}
						else
						{
							list4.Add(vector5);
							list4.Add(vector6);
						}
					}
				}
			}
			mesh.normals = list4.ToArray();
		}
		mesh.RecalculateBounds();
		return mesh;
	}
}
