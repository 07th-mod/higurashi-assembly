using UnityEngine;

namespace TMPro
{
	[ExecuteInEditMode]
	public class TMPro_AdvancedLayout : MonoBehaviour
	{
		[SerializeField]
		private bool m_isEnabled = true;

		[SerializeField]
		private bool isRedrawRequired;

		public AnimationCurve TranslationCurve;

		public AnimationCurve ScaleCurve;

		private TextMeshPro m_textMeshProComponent;

		private Renderer m_renderer;

		private MeshFilter m_meshFilter;

		private Material m_sharedMaterial;

		private Mesh m_mesh;

		[SerializeField]
		private bool propertiesChanged;

		public bool isEnabled => m_isEnabled;

		private void Awake()
		{
			Debug.Log("Advanced Layout Component was added.");
			m_renderer = GetComponent<Renderer>();
			m_sharedMaterial = m_renderer.sharedMaterial;
			m_textMeshProComponent = GetComponent<TextMeshPro>();
		}

		private void OnDestroy()
		{
			Debug.Log("Advanced Layout Component was removed.");
		}

		private void Update()
		{
			if (propertiesChanged)
			{
				if (m_isEnabled)
				{
					m_textMeshProComponent.enableWordWrapping = false;
					m_textMeshProComponent.alignment = TextAlignmentOptions.Left;
				}
				propertiesChanged = false;
			}
			if (!isRedrawRequired)
			{
			}
		}

		public void DrawMesh()
		{
			TMP_MeshInfo meshInfo = m_textMeshProComponent.textInfo.meshInfo;
			TMP_TextInfo textInfo = m_textMeshProComponent.textInfo;
			TMP_CharacterInfo[] characterInfo = textInfo.characterInfo;
			int characterCount = textInfo.characterCount;
			Vector3[] vertices = meshInfo.vertices;
			Vector2[] uv0s = meshInfo.uv0s;
			Vector2[] uv2s = meshInfo.uv2s;
			Color32[] vertexColors = meshInfo.vertexColors;
			Vector3[] normals = meshInfo.normals;
			Vector4[] tangents = meshInfo.tangents;
			float num = 1f;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < characterCount; i++)
			{
				char character = characterInfo[i].character;
				GlyphInfo glyphInfo = m_textMeshProComponent.font.characterDictionary[character];
				int vertexIndex = characterInfo[i].vertexIndex;
				if (characterInfo[i].isVisible)
				{
					float x = (characterInfo[i].bottomLeft.x + characterInfo[i].topRight.x) / 2f;
					Vector3 a = new Vector3(x, 0f, 0f);
					vertices[vertexIndex] += -a;
					vertices[vertexIndex + 1] += -a;
					vertices[vertexIndex + 2] += -a;
					vertices[vertexIndex + 3] += -a;
					float time = (characterCount <= 1) ? 0f : ((float)i / (float)(characterCount - 1));
					num2 = ScaleCurve.Evaluate(time);
					Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), new Vector3(num2, num2, 1f));
					vertices[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex]);
					vertices[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 1]);
					vertices[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 2]);
					vertices[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 3]);
					float num4 = glyphInfo.xOffset * characterInfo[i].scale * num2 + (characterInfo[i].topRight.x - characterInfo[i].bottomLeft.x) / 2f * num2;
					a = new Vector3(num4 + num3, 0f, 0f);
					vertices[vertexIndex] += a;
					vertices[vertexIndex + 1] += a;
					vertices[vertexIndex + 2] += a;
					vertices[vertexIndex + 3] += a;
					uv2s[vertexIndex].y *= num2;
					uv2s[vertexIndex + 1].y *= num2;
					uv2s[vertexIndex + 2].y *= num2;
					uv2s[vertexIndex + 3].y *= num2;
				}
				num3 += glyphInfo.xAdvance * characterInfo[i].scale * num2 + m_textMeshProComponent.characterSpacing;
			}
			int vertexIndex2 = characterInfo[characterCount - 1].vertexIndex;
			float x2 = vertices[0].x;
			float x3 = vertices[vertexIndex2 + 2].x;
			for (int j = 0; j < characterCount; j++)
			{
				int vertexIndex3 = characterInfo[j].vertexIndex;
				if (characterInfo[j].isVisible)
				{
					float num5 = (vertices[vertexIndex3].x + vertices[vertexIndex3 + 2].x) / 2f;
					Vector3 vector = new Vector3(num5, 0f, 0f);
					vertices[vertexIndex3] += -vector;
					vertices[vertexIndex3 + 1] += -vector;
					vertices[vertexIndex3 + 2] += -vector;
					vertices[vertexIndex3 + 3] += -vector;
					float num6 = (num5 - x2) / (x3 - x2);
					float num7 = num6 + 0.0001f;
					float num8 = TranslationCurve.Evaluate(num6) * num;
					float num9 = TranslationCurve.Evaluate(num7) * num;
					Vector3 lhs = new Vector3(1f, 0f, 0f);
					Debug.DrawLine(end: new Vector3(0f - (num9 - num8), num7 * (x3 - x2) + x2 - num5, 0f), start: new Vector3(num5, num8, 0f), color: Color.green, duration: 60f);
					Vector3 rhs = new Vector3(num7 * (x3 - x2) + x2, num9) - new Vector3(num5, num8);
					float num10 = Mathf.Acos(Vector3.Dot(lhs, rhs.normalized)) * 57.29578f;
					Vector3 vector2 = Vector3.Cross(lhs, rhs);
					float z = (!(vector2.z > 0f)) ? (360f - num10) : num10;
					Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0f, num8, 0f), Quaternion.Euler(0f, 0f, z), new Vector3(1f, 1f, 1f));
					vertices[vertexIndex3] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex3]);
					vertices[vertexIndex3 + 1] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex3 + 1]);
					vertices[vertexIndex3 + 2] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex3 + 2]);
					vertices[vertexIndex3 + 3] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex3 + 3]);
					vertices[vertexIndex3] += vector;
					vertices[vertexIndex3 + 1] += vector;
					vertices[vertexIndex3 + 2] += vector;
					vertices[vertexIndex3 + 3] += vector;
				}
			}
			Mesh mesh = m_textMeshProComponent.mesh;
			mesh.vertices = vertices;
			mesh.uv = uv0s;
			mesh.uv2 = uv2s;
			mesh.colors32 = vertexColors;
			if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_Shininess))
			{
				mesh.normals = normals;
				mesh.tangents = tangents;
			}
			mesh.RecalculateBounds();
		}
	}
}
