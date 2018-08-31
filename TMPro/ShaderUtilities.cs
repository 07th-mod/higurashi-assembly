using System.Linq;
using UnityEngine;

namespace TMPro
{
	public static class ShaderUtilities
	{
		public static int ID_MainTex;

		public static int ID_FaceColor;

		public static int ID_FaceDilate;

		public static int ID_Shininess;

		public static int ID_UnderlayOffsetX;

		public static int ID_UnderlayOffsetY;

		public static int ID_UnderlayDilate;

		public static int ID_UnderlaySoftness;

		public static int ID_WeightNormal;

		public static int ID_WeightBold;

		public static int ID_OutlineWidth;

		public static int ID_OutlineSoftness;

		public static int ID_OutlineColor;

		public static int ID_GradientScale;

		public static int ID_ScaleX;

		public static int ID_ScaleY;

		public static int ID_PerspectiveFilter;

		public static int ID_TextureWidth;

		public static int ID_TextureHeight;

		public static int ID_BevelAmount;

		public static int ID_GlowColor;

		public static int ID_GlowOffset;

		public static int ID_GlowPower;

		public static int ID_GlowOuter;

		public static int ID_LightAngle;

		public static int ID_EnvMap;

		public static int ID_EnvMatrix;

		public static int ID_EnvMatrixRotation;

		public static int ID_MaskID;

		public static int ID_MaskCoord;

		public static int ID_MaskSoftnessX;

		public static int ID_MaskSoftnessY;

		public static int ID_VertexOffsetX;

		public static int ID_VertexOffsetY;

		public static int ID_StencilID;

		public static int ID_StencilComp;

		public static int ID_ShaderFlags;

		public static int ID_ScaleRatio_A;

		public static int ID_ScaleRatio_B;

		public static int ID_ScaleRatio_C;

		public static string Keyword_Bevel = "BEVEL_ON";

		public static string Keyword_Glow = "GLOW_ON";

		public static string Keyword_Underlay = "UNDERLAY_ON";

		public static string Keyword_Ratios = "RATIOS_OFF";

		public static string Keyword_MaskSoft = "MASK_SOFT";

		public static string Keyword_MaskHard = "MASK_HARD";

		private static float m_clamp = 1f;

		public static bool isInitialized;

		public static void GetShaderPropertyIDs()
		{
			if (!isInitialized)
			{
				isInitialized = true;
				ID_MainTex = Shader.PropertyToID("_MainTex");
				ID_FaceColor = Shader.PropertyToID("_FaceColor");
				ID_FaceDilate = Shader.PropertyToID("_FaceDilate");
				ID_Shininess = Shader.PropertyToID("_FaceShininess");
				ID_UnderlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
				ID_UnderlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
				ID_UnderlayDilate = Shader.PropertyToID("_UnderlayDilate");
				ID_UnderlaySoftness = Shader.PropertyToID("_UnderlaySoftness");
				ID_WeightNormal = Shader.PropertyToID("_WeightNormal");
				ID_WeightBold = Shader.PropertyToID("_WeightBold");
				ID_OutlineWidth = Shader.PropertyToID("_OutlineWidth");
				ID_OutlineSoftness = Shader.PropertyToID("_OutlineSoftness");
				ID_OutlineColor = Shader.PropertyToID("_OutlineColor");
				ID_GradientScale = Shader.PropertyToID("_GradientScale");
				ID_ScaleX = Shader.PropertyToID("_ScaleX");
				ID_ScaleY = Shader.PropertyToID("_ScaleY");
				ID_PerspectiveFilter = Shader.PropertyToID("_PerspectiveFilter");
				ID_TextureWidth = Shader.PropertyToID("_TextureWidth");
				ID_TextureHeight = Shader.PropertyToID("_TextureHeight");
				ID_BevelAmount = Shader.PropertyToID("_Bevel");
				ID_LightAngle = Shader.PropertyToID("_LightAngle");
				ID_EnvMap = Shader.PropertyToID("_Cube");
				ID_EnvMatrix = Shader.PropertyToID("_EnvMatrix");
				ID_EnvMatrixRotation = Shader.PropertyToID("_EnvMatrixRotation");
				ID_GlowColor = Shader.PropertyToID("_GlowColor");
				ID_GlowOffset = Shader.PropertyToID("_GlowOffset");
				ID_GlowPower = Shader.PropertyToID("_GlowPower");
				ID_GlowOuter = Shader.PropertyToID("_GlowOuter");
				ID_MaskID = Shader.PropertyToID("_MaskID");
				ID_MaskCoord = Shader.PropertyToID("_MaskCoord");
				ID_MaskSoftnessX = Shader.PropertyToID("_MaskSoftnessX");
				ID_MaskSoftnessY = Shader.PropertyToID("_MaskSoftnessY");
				ID_VertexOffsetX = Shader.PropertyToID("_VertexOffsetX");
				ID_VertexOffsetY = Shader.PropertyToID("_VertexOffsetY");
				ID_StencilID = Shader.PropertyToID("_Stencil");
				ID_StencilComp = Shader.PropertyToID("_StencilComp");
				ID_ShaderFlags = Shader.PropertyToID("_ShaderFlags");
				ID_ScaleRatio_A = Shader.PropertyToID("_ScaleRatioA");
				ID_ScaleRatio_B = Shader.PropertyToID("_ScaleRatioB");
				ID_ScaleRatio_C = Shader.PropertyToID("_ScaleRatioC");
			}
		}

		public static void UpdateShaderRatios(Material mat, bool isBold)
		{
			float num = 1f;
			float num2 = 1f;
			float num3 = 1f;
			bool flag = !mat.shaderKeywords.Contains(Keyword_Ratios);
			float @float = mat.GetFloat(ID_GradientScale);
			float float2 = mat.GetFloat(ID_FaceDilate);
			float float3 = mat.GetFloat(ID_OutlineWidth);
			float float4 = mat.GetFloat(ID_OutlineSoftness);
			float num4 = isBold ? (mat.GetFloat(ID_WeightBold) * 2f / @float) : (mat.GetFloat(ID_WeightNormal) * 2f / @float);
			float num5 = Mathf.Max(1f, num4 + float2 + float3 + float4);
			num = ((!flag) ? 1f : ((@float - m_clamp) / (@float * num5)));
			mat.SetFloat(ID_ScaleRatio_A, num);
			if (mat.HasProperty(ID_GlowOffset))
			{
				float float5 = mat.GetFloat(ID_GlowOffset);
				float float6 = mat.GetFloat(ID_GlowOuter);
				float num6 = (num4 + float2) * (@float - m_clamp);
				num5 = Mathf.Max(1f, float5 + float6);
				num2 = ((!flag) ? 1f : (Mathf.Max(0f, @float - m_clamp - num6) / (@float * num5)));
				mat.SetFloat(ID_ScaleRatio_B, num2);
			}
			if (mat.HasProperty(ID_UnderlayOffsetX))
			{
				float float7 = mat.GetFloat(ID_UnderlayOffsetX);
				float float8 = mat.GetFloat(ID_UnderlayOffsetY);
				float float9 = mat.GetFloat(ID_UnderlayDilate);
				float float10 = mat.GetFloat(ID_UnderlaySoftness);
				float num7 = (num4 + float2) * (@float - m_clamp);
				num5 = Mathf.Max(1f, Mathf.Max(Mathf.Abs(float7), Mathf.Abs(float8)) + float9 + float10);
				num3 = ((!flag) ? 1f : (Mathf.Max(0f, @float - m_clamp - num7) / (@float * num5)));
				mat.SetFloat(ID_ScaleRatio_C, num3);
			}
		}

		public static Vector4 GetFontExtent(Material material)
		{
			if (!material.HasProperty(ID_GradientScale))
			{
				return Vector4.zero;
			}
			float @float = material.GetFloat(ID_ScaleRatio_A);
			float num = material.GetFloat(ID_FaceDilate) * @float;
			float num2 = material.GetFloat(ID_OutlineWidth) * @float;
			float num3 = Mathf.Min(1f, num + num2);
			num3 *= material.GetFloat(ID_GradientScale);
			return new Vector4(num3, num3, num3, num3);
		}

		public static bool IsMaskingEnabled(Material material)
		{
			if (!material.HasProperty(ID_MaskCoord))
			{
				return false;
			}
			if (material.shaderKeywords.Contains(Keyword_MaskSoft) || material.shaderKeywords.Contains(Keyword_MaskHard))
			{
				return true;
			}
			return false;
		}

		public static float GetPadding(Material material, bool enableExtraPadding, bool isBold)
		{
			if (!isInitialized)
			{
				GetShaderPropertyIDs();
			}
			int num = enableExtraPadding ? 4 : 0;
			if (!material.HasProperty(ID_GradientScale))
			{
				return (float)num;
			}
			Vector4 a = Vector4.zero;
			Vector4 zero = Vector4.zero;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			UpdateShaderRatios(material, isBold);
			string[] shaderKeywords = material.shaderKeywords;
			if (material.HasProperty(ID_ScaleRatio_A))
			{
				num5 = material.GetFloat(ID_ScaleRatio_A);
			}
			if (material.HasProperty(ID_FaceDilate))
			{
				num2 = material.GetFloat(ID_FaceDilate) * num5;
			}
			if (material.HasProperty(ID_OutlineSoftness))
			{
				num3 = material.GetFloat(ID_OutlineSoftness) * num5;
			}
			if (material.HasProperty(ID_OutlineWidth))
			{
				num4 = material.GetFloat(ID_OutlineWidth) * num5;
			}
			num10 = num4 + num3 + num2;
			if (material.HasProperty(ID_GlowOffset) && shaderKeywords.Contains(Keyword_Glow))
			{
				if (material.HasProperty(ID_ScaleRatio_B))
				{
					num6 = material.GetFloat(ID_ScaleRatio_B);
				}
				num8 = material.GetFloat(ID_GlowOffset) * num6;
				num9 = material.GetFloat(ID_GlowOuter) * num6;
			}
			num10 = Mathf.Max(num10, num2 + num8 + num9);
			if (material.HasProperty(ID_UnderlaySoftness) && shaderKeywords.Contains(Keyword_Underlay))
			{
				if (material.HasProperty(ID_ScaleRatio_C))
				{
					num7 = material.GetFloat(ID_ScaleRatio_C);
				}
				float num11 = material.GetFloat(ID_UnderlayOffsetX) * num7;
				float num12 = material.GetFloat(ID_UnderlayOffsetY) * num7;
				float num13 = material.GetFloat(ID_UnderlayDilate) * num7;
				float num14 = material.GetFloat(ID_UnderlaySoftness) * num7;
				a.x = Mathf.Max(a.x, num2 + num13 + num14 - num11);
				a.y = Mathf.Max(a.y, num2 + num13 + num14 - num12);
				a.z = Mathf.Max(a.z, num2 + num13 + num14 + num11);
				a.w = Mathf.Max(a.w, num2 + num13 + num14 + num12);
			}
			a.x = Mathf.Max(a.x, num10);
			a.y = Mathf.Max(a.y, num10);
			a.z = Mathf.Max(a.z, num10);
			a.w = Mathf.Max(a.w, num10);
			a.x += (float)num;
			a.y += (float)num;
			a.z += (float)num;
			a.w += (float)num;
			a.x = Mathf.Min(a.x, 1f);
			a.y = Mathf.Min(a.y, 1f);
			a.z = Mathf.Min(a.z, 1f);
			a.w = Mathf.Min(a.w, 1f);
			zero.x = ((!(zero.x < a.x)) ? zero.x : a.x);
			zero.y = ((!(zero.y < a.y)) ? zero.y : a.y);
			zero.z = ((!(zero.z < a.z)) ? zero.z : a.z);
			zero.w = ((!(zero.w < a.w)) ? zero.w : a.w);
			float @float = material.GetFloat(ID_GradientScale);
			a *= @float;
			num10 = Mathf.Max(a.x, a.y);
			num10 = Mathf.Max(a.z, num10);
			num10 = Mathf.Max(a.w, num10);
			return num10 + 0.25f;
		}

		public static float GetPadding(Material[] materials, bool enableExtraPadding, bool isBold)
		{
			if (!isInitialized)
			{
				GetShaderPropertyIDs();
			}
			int num = enableExtraPadding ? 4 : 0;
			if (!materials[0].HasProperty(ID_GradientScale))
			{
				return (float)num;
			}
			Vector4 a = Vector4.zero;
			Vector4 zero = Vector4.zero;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			for (int i = 0; i < materials.Length; i++)
			{
				UpdateShaderRatios(materials[i], isBold);
				string[] shaderKeywords = materials[i].shaderKeywords;
				if (materials[i].HasProperty(ID_ScaleRatio_A))
				{
					num5 = materials[i].GetFloat(ID_ScaleRatio_A);
				}
				if (materials[i].HasProperty(ID_FaceDilate))
				{
					num2 = materials[i].GetFloat(ID_FaceDilate) * num5;
				}
				if (materials[i].HasProperty(ID_OutlineSoftness))
				{
					num3 = materials[i].GetFloat(ID_OutlineSoftness) * num5;
				}
				if (materials[i].HasProperty(ID_OutlineWidth))
				{
					num4 = materials[i].GetFloat(ID_OutlineWidth) * num5;
				}
				num10 = num4 + num3 + num2;
				if (materials[i].HasProperty(ID_GlowOffset) && shaderKeywords.Contains(Keyword_Glow))
				{
					if (materials[i].HasProperty(ID_ScaleRatio_B))
					{
						num6 = materials[i].GetFloat(ID_ScaleRatio_B);
					}
					num8 = materials[i].GetFloat(ID_GlowOffset) * num6;
					num9 = materials[i].GetFloat(ID_GlowOuter) * num6;
				}
				num10 = Mathf.Max(num10, num2 + num8 + num9);
				if (materials[i].HasProperty(ID_UnderlaySoftness) && shaderKeywords.Contains(Keyword_Underlay))
				{
					if (materials[i].HasProperty(ID_ScaleRatio_C))
					{
						num7 = materials[i].GetFloat(ID_ScaleRatio_C);
					}
					float num11 = materials[i].GetFloat(ID_UnderlayOffsetX) * num7;
					float num12 = materials[i].GetFloat(ID_UnderlayOffsetY) * num7;
					float num13 = materials[i].GetFloat(ID_UnderlayDilate) * num7;
					float num14 = materials[i].GetFloat(ID_UnderlaySoftness) * num7;
					a.x = Mathf.Max(a.x, num2 + num13 + num14 - num11);
					a.y = Mathf.Max(a.y, num2 + num13 + num14 - num12);
					a.z = Mathf.Max(a.z, num2 + num13 + num14 + num11);
					a.w = Mathf.Max(a.w, num2 + num13 + num14 + num12);
				}
				a.x = Mathf.Max(a.x, num10);
				a.y = Mathf.Max(a.y, num10);
				a.z = Mathf.Max(a.z, num10);
				a.w = Mathf.Max(a.w, num10);
				a.x += (float)num;
				a.y += (float)num;
				a.z += (float)num;
				a.w += (float)num;
				a.x = Mathf.Min(a.x, 1f);
				a.y = Mathf.Min(a.y, 1f);
				a.z = Mathf.Min(a.z, 1f);
				a.w = Mathf.Min(a.w, 1f);
				zero.x = ((!(zero.x < a.x)) ? zero.x : a.x);
				zero.y = ((!(zero.y < a.y)) ? zero.y : a.y);
				zero.z = ((!(zero.z < a.z)) ? zero.z : a.z);
				zero.w = ((!(zero.w < a.w)) ? zero.w : a.w);
			}
			float @float = materials[0].GetFloat(ID_GradientScale);
			a *= @float;
			num10 = Mathf.Max(a.x, a.y);
			num10 = Mathf.Max(a.z, num10);
			num10 = Mathf.Max(a.w, num10);
			return num10 + 0.25f;
		}
	}
}
