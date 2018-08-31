using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	public class FilmEffector : MonoBehaviour
	{
		public Material SceneMaterial;

		public float Power;

		private float targetPower;

		private bool isReady;

		private void Update()
		{
		}

		public void FadeOut(float time, bool isBlocking)
		{
			Debug.Log("Removing Effector");
			if (Mathf.Approximately(time, 0f))
			{
				RemoveEffector();
			}
			else
			{
				iTween.Stop(base.gameObject);
				iTween.ValueTo(base.gameObject, iTween.Hash("time", time, "from", Power, "to", 0, "onupdate", "UpdatePower", "oncomplete", "RemoveEffector"));
				if (isBlocking)
				{
					GameSystem.Instance.AddWait(new Wait(time, WaitTypes.WaitForScene, RemoveEffector));
				}
			}
		}

		private void UpdatePower(float p)
		{
			Power = p;
			SceneMaterial.SetFloat("_Power", Power);
		}

		private void SetFinalPower()
		{
			Power = targetPower;
			Debug.Log(Power);
			UpdatePower(Power);
		}

		public void RemoveEffector()
		{
			if (!(SceneMaterial == null) && !(base.gameObject == null))
			{
				iTween.Stop(base.gameObject);
				Object.Destroy(SceneMaterial);
				SceneMaterial = null;
				base.gameObject.GetComponent<Camera>().enabled = false;
				Object.Destroy(this);
			}
		}

		public void Prepare(int effecttype, Color targetColor, int targetpower, int style, float length, bool isBlocking)
		{
			iTween.Stop(base.gameObject);
			Shader shader = null;
			bool flag = false;
			switch (effecttype)
			{
			case 0:
			case 1:
				shader = Shader.Find("MGShader/EffectColorMix");
				flag = true;
				break;
			case 2:
				shader = Shader.Find("MGShader/DrainColor");
				flag = true;
				break;
			case 3:
				shader = Shader.Find("MGShader/Negative");
				break;
			case 10:
				shader = Shader.Find("MGShader/HorizontalBlur2");
				break;
			case 12:
				shader = Shader.Find("MGShader/GaussianBlur");
				break;
			default:
				Logger.LogError("FilmEffector created without a valid effecttype! Effect type specified: " + effecttype);
				break;
			}
			float num = 0f;
			if (SceneMaterial != null)
			{
				num = targetPower;
			}
			targetPower = (float)targetpower / 256f;
			SceneMaterial = new Material(shader);
			if (flag)
			{
				SceneMaterial.SetColor("_Color", targetColor);
			}
			if (length > 0f)
			{
				SceneMaterial.SetFloat("_Power", num);
				if (targetpower == 0)
				{
					iTween.ValueTo(base.gameObject, iTween.Hash("time", length, "from", num, "to", targetPower, "onupdate", "UpdatePower", "oncomplete", "RemoveEffector"));
					if (isBlocking)
					{
						GameSystem.Instance.AddWait(new Wait(length, WaitTypes.WaitForScene, RemoveEffector));
					}
				}
				else
				{
					iTween.ValueTo(base.gameObject, iTween.Hash("time", length, "from", num, "to", targetPower, "onupdate", "UpdatePower", "oncomplete", "SetFinalPower"));
					if (isBlocking)
					{
						GameSystem.Instance.AddWait(new Wait(length, WaitTypes.WaitForScene, SetFinalPower));
					}
				}
			}
			else
			{
				Power = targetPower;
				SceneMaterial.SetFloat("_Power", Power);
			}
			isReady = true;
		}

		private void OnRenderImage(RenderTexture source, RenderTexture dest)
		{
			if (isReady)
			{
				Graphics.Blit(source, dest, SceneMaterial);
				RenderTexture.active = null;
			}
		}
	}
}
