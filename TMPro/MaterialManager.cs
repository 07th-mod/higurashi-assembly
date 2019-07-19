using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	public static class MaterialManager
	{
		private class MaskingMaterial
		{
			public Material baseMaterial;

			public Material stencilMaterial;

			public int count;

			public int stencilID;
		}

		private static List<MaskingMaterial> m_materialList = new List<MaskingMaterial>();

		private static Mask[] m_maskComponents = new Mask[0];

		public static Material GetStencilMaterial(Material baseMaterial, int stencilID)
		{
			if (!baseMaterial.HasProperty(ShaderUtilities.ID_StencilID))
			{
				Debug.LogWarning("Selected Shader does not support Stencil Masking. Please select the Distance Field or Mobile Distance Field Shader.");
				return baseMaterial;
			}
			Material material = null;
			int num = m_materialList.FindIndex((MaskingMaterial item) => item.baseMaterial == baseMaterial && item.stencilID == stencilID);
			if (num == -1)
			{
				material = new Material(baseMaterial);
				material.hideFlags = HideFlags.HideAndDontSave;
				Material material2 = material;
				material2.name = material2.name + " Masking ID:" + stencilID;
				material.shaderKeywords = baseMaterial.shaderKeywords;
				ShaderUtilities.GetShaderPropertyIDs();
				material.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
				material.SetFloat(ShaderUtilities.ID_StencilComp, 4f);
				MaskingMaterial maskingMaterial = new MaskingMaterial();
				maskingMaterial.baseMaterial = baseMaterial;
				maskingMaterial.stencilMaterial = material;
				maskingMaterial.stencilID = stencilID;
				maskingMaterial.count = 1;
				m_materialList.Add(maskingMaterial);
			}
			else
			{
				material = m_materialList[num].stencilMaterial;
				m_materialList[num].count++;
			}
			ListMaterials();
			return material;
		}

		public static Material GetBaseMaterial(Material stencilMaterial)
		{
			int num = m_materialList.FindIndex((MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num == -1)
			{
				return null;
			}
			return m_materialList[num].baseMaterial;
		}

		public static Material SetStencil(Material material, int stencilID)
		{
			material.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
			if (stencilID == 0)
			{
				material.SetFloat(ShaderUtilities.ID_StencilComp, 8f);
			}
			else
			{
				material.SetFloat(ShaderUtilities.ID_StencilComp, 4f);
			}
			return material;
		}

		public static void AddMaskingMaterial(Material baseMaterial, Material stencilMaterial, int stencilID)
		{
			int num = m_materialList.FindIndex((MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num == -1)
			{
				MaskingMaterial maskingMaterial = new MaskingMaterial();
				maskingMaterial.baseMaterial = baseMaterial;
				maskingMaterial.stencilMaterial = stencilMaterial;
				maskingMaterial.stencilID = stencilID;
				maskingMaterial.count = 1;
				m_materialList.Add(maskingMaterial);
			}
			else
			{
				stencilMaterial = m_materialList[num].stencilMaterial;
				m_materialList[num].count++;
			}
		}

		public static void RemoveStencilMaterial(Material stencilMaterial)
		{
			int num = m_materialList.FindIndex((MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num != -1)
			{
				m_materialList.RemoveAt(num);
			}
			ListMaterials();
		}

		public static void ReleaseBaseMaterial(Material baseMaterial)
		{
			int num = m_materialList.FindIndex((MaskingMaterial item) => item.baseMaterial == baseMaterial);
			if (num == -1)
			{
				Debug.Log("No Masking Material exists for " + baseMaterial.name);
			}
			else if (m_materialList[num].count > 1)
			{
				m_materialList[num].count--;
				Debug.Log("Removed (1) reference to " + m_materialList[num].stencilMaterial.name + ". There are " + m_materialList[num].count + " references left.");
			}
			else
			{
				Debug.Log("Removed last reference to " + m_materialList[num].stencilMaterial.name + " with ID " + m_materialList[num].stencilMaterial.GetInstanceID());
				Object.DestroyImmediate(m_materialList[num].stencilMaterial);
				m_materialList.RemoveAt(num);
			}
			ListMaterials();
		}

		public static void ReleaseStencilMaterial(Material stencilMaterial)
		{
			int num = m_materialList.FindIndex((MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num != -1)
			{
				if (m_materialList[num].count > 1)
				{
					m_materialList[num].count--;
				}
				else
				{
					Object.DestroyImmediate(m_materialList[num].stencilMaterial);
					m_materialList.RemoveAt(num);
				}
			}
			ListMaterials();
		}

		public static void ClearMaterials()
		{
			if (m_materialList.Count() == 0)
			{
				Debug.Log("Material List has already been cleared.");
				return;
			}
			for (int i = 0; i < m_materialList.Count(); i++)
			{
				Material stencilMaterial = m_materialList[i].stencilMaterial;
				Object.DestroyImmediate(stencilMaterial);
				m_materialList.RemoveAt(i);
			}
		}

		public static void ListMaterials()
		{
		}

		public static int GetStencilID(GameObject obj)
		{
			int num = 0;
			m_maskComponents = obj.GetComponentsInParent<Mask>();
			for (int i = 0; i < m_maskComponents.Length; i++)
			{
				if (m_maskComponents[i].MaskEnabled())
				{
					num++;
				}
			}
			return Mathf.Min((1 << num) - 1, 255);
		}
	}
}
