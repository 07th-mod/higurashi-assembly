using Assets.Scripts.Core.AssetManagement;
using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	public class LayerOld : MonoBehaviour
	{
		public Shader LayerShader;

		public Shader LayerShaderNormal;

		public Shader LayerAddative;

		public Shader LayerCrossfade;

		public Shader LayerFadeWithMask;

		private Mesh mesh;

		private MeshFilter meshFilter;

		private MeshRenderer meshRenderer;

		private Material material;

		private Texture2D primary;

		private Texture2D secondary;

		private float targetalpha;

		private Vector3 endpos = new Vector3(0f, 0f, 0f);

		public int LayerNum;

		private void Prepare(int width, int height, Shader shader, LayerAlignment align, bool retainMainTexture)
		{
			if (meshFilter == null)
			{
				meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (meshRenderer == null)
			{
				meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
			mesh = MGHelper.CreateMesh(width, height, align,false, 0);
			if (material != null)
			{
				if (!retainMainTexture)
				{
					Object.Destroy(primary);
				}
				Object.Destroy(material);
			}
			material = new Material(shader);
			meshFilter.mesh = mesh;
			meshRenderer.material = material;
		}

		public void HideLayer()
		{
			ChangeBlendMode(0);
			iTween.Stop(base.gameObject);
			base.transform.localPosition = new Vector3(0f, 0f, 0f);
			Object.Destroy(material);
			Object.Destroy(meshRenderer);
			Object.Destroy(meshFilter);
			if (primary != null)
			{
				Object.Destroy(primary);
				primary = null;
			}
			if (secondary != null)
			{
				Object.Destroy(secondary);
				secondary = null;
			}
		}

		public void UpdateAlpha(float a)
		{
			material.SetFloat("_Alpha", a);
		}

		public void UpdateOpaque()
		{
			material.SetFloat("_Alpha", 0f);
		}

		public void FinalizeAlpha()
		{
			material.SetFloat("_Alpha", targetalpha);
		}

		public void UpdateRange(float r)
		{
			material.SetFloat("_Range", r);
		}

		public void FinalizeFade()
		{
			Object.Destroy(secondary);
			material.shader = LayerShader;
			material.SetTexture("_MainTex", primary);
			material.SetFloat("_Alpha", 0f);
		}

		public void FinalizeMove()
		{
			iTween.Stop(base.gameObject);
			base.transform.localPosition = endpos;
		}

		public void FinalizeAll()
		{
			FinalizeFade();
			FinalizeMove();
		}

		public void ChangeBlendMode(int mode)
		{
			switch (mode)
			{
			case 0:
				LayerShader = LayerShaderNormal;
				break;
			case 1:
				LayerShader = LayerAddative;
				break;
			default:
				Logger.LogWarning("Unknown Blend Mode: " + mode);
				break;
			}
		}

		public void FadeOutSprite(float wait, bool isblocking)
		{
			iTween.Stop(base.gameObject);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", wait, "onupdate", "UpdateAlpha", "oncomplete", "HideLayer"));
			if (isblocking)
			{
				GameSystem.Instance.AddWait(new Wait(wait / 1000f, WaitTypes.WaitForMove, HideLayer));
			}
		}

		public void FadeInSprite(float wait, bool isblocking)
		{
			material.SetFloat("_Alpha", 1f);
			iTween.Stop(base.gameObject);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", 1f, "to", 0f, "time", wait, "onupdate", "UpdateAlpha", "oncomplete", "UpdateOpaque"));
			if (isblocking)
			{
				GameSystem.Instance.AddWait(new Wait(wait / 1000f, WaitTypes.WaitForMove, UpdateOpaque));
			}
		}

		public void MoveSprite(int x, int y, int z, int alpha, float wait, bool isblocking)
		{
			targetalpha = 0f + (float)alpha / 256f;
			float @float = material.GetFloat("_Alpha");
			if (!Mathf.Approximately(targetalpha, @float))
			{
				iTween.ValueTo(base.gameObject, iTween.Hash("from", @float, "to", targetalpha, "time", wait, "onupdate", "UpdateAlpha", "oncomplete", "FinalizeAlpha"));
			}
			float targetscale = 1f;
			if (z > 0)
			{
				targetscale = 1f - (float)z / 100f;
			}
			if (z < 0)
			{
				targetscale = 1f + (float)z / -100f;
			}
			float x2 = (float)x;
			float y2 = (float)y;
			Vector3 localPosition = base.transform.localPosition;
			endpos = new Vector3(x2, y2, localPosition.z);
			iTween.ScaleTo(base.gameObject, iTween.Hash("scale", new Vector3(targetscale, targetscale, 1f), "time", wait, "islocal", true));
			iTween.MoveTo(base.gameObject, iTween.Hash("position", endpos, "time", wait, "islocal", true));
			if (isblocking)
			{
				GameSystem.Instance.AddWait(new Wait(wait, WaitTypes.WaitForMove, delegate
				{
					iTween.Stop(base.gameObject);
					base.transform.localScale = new Vector3(targetscale, targetscale, 1f);
					FinalizeAll();
				}));
			}
		}

		public void DrawSprite(string textureName, int x, int y, int z, int alpha, int priority, float wait, bool isblocking)
		{
			iTween.Stop(base.gameObject);
			Texture2D texture2D = AssetManager.Instance.LoadTexture(textureName);
			if (!(texture2D == null))
			{
				LayerAlignment align = LayerAlignment.AlignCenter;
				if (x != 0 || y != 0)
				{
					align = LayerAlignment.AlignTopleft;
				}
				Prepare(texture2D.width, texture2D.height, LayerShader, align, retainMainTexture: false);
				float num = 1f;
				if (z > 0)
				{
					num = 1f - (float)z / 100f;
				}
				if (z < 0)
				{
					num = 1f + (float)z / -100f;
				}
				base.transform.localPosition = new Vector3((float)x, (float)(y * -1), (float)priority * -0.1f);
				base.transform.localScale = new Vector3(num, num, 1f);
				material.SetTexture("_MainTex", texture2D);
				if (wait > 0f)
				{
					FadeInSprite(wait, isblocking);
				}
				else
				{
					material.SetFloat("_Alpha", 0f + (float)alpha / 256f);
				}
			}
		}

		public void DrawBustshot(string textureName, float x, float y, float z, float oldx, float oldy, float oldz, bool move, int priority, float wait, bool isblocking)
		{
			Vector3 localPosition = (!move) ? new Vector3(x, y, (float)priority * -0.1f) : new Vector3(oldx, oldy, (float)priority * -0.1f);
			base.transform.localPosition = localPosition;
			endpos = new Vector3(x, y, (float)priority * -0.1f);
			SetTextureCrossFade(textureName, wait, LayerAlignment.AlignCenter);
			if (move)
			{
				iTween.MoveTo(base.gameObject, iTween.Hash("position", endpos, "time", wait, "islocal", true));
			}
			if (isblocking)
			{
				GameSystem.Instance.AddWait(new Wait(wait, WaitTypes.WaitForMove, FinalizeAll));
			}
		}

		public void SetTextureCrossFade(string textureName, float time, LayerAlignment align)
		{
			Texture2D texture2D = AssetManager.Instance.LoadTexture(textureName);
			if (!(texture2D == null))
			{
				if (primary != null)
				{
					Prepare(texture2D.width, texture2D.height, LayerCrossfade, align, retainMainTexture: true);
					secondary = primary;
					primary = texture2D;
					material.SetTexture("_Primary", primary);
					material.SetTexture("_Secondary", secondary);
					material.SetFloat("_Range", 0f);
					iTween.Stop(base.gameObject);
					iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", time, "onupdate", "UpdateRange", "oncomplete", "FinalizeFade"));
				}
				else
				{
					SetTextureImmediate(textureName, align);
					FadeInSprite(time, isblocking: false);
				}
			}
		}

		public void SetTextureImmediate(string textureName, LayerAlignment align)
		{
			Texture2D texture2D = AssetManager.Instance.LoadTexture(textureName);
			if (!(texture2D == null))
			{
				Prepare(texture2D.width, texture2D.height, LayerShader, align, retainMainTexture: false);
				primary = texture2D;
				material.SetTexture("_MainTex", texture2D);
				material.SetFloat("_Alpha", 0f);
			}
		}
	}
}
