using Assets.Scripts.Core.AssetManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	public class SceneOld : MonoBehaviour
	{
		private Camera sceneCamera;

		private UICamera uiCamera;

		private int layerNum;

		public GameObject PanelGameObject;

		public GameObject LayerPrefab;

		public Material SceneMaterial;

		public Shader SceneShader;

		public Shader SceneFadeShader;

		public Shader SceneTransitionShader;

		public int SceneNum;

		public int LayerCount;

		public string SceneName;

		private Vector3 targetPosition;

		private Vector3 targetScale;

		private Vector3 defaultOffset = Vector3.zero;

		private List<LayerOld> layerList = new List<LayerOld>();

		private const float Fuzziness = 0.32f;

		public void ResetLayer()
		{
			ResetViewportSize();
			SceneMaterial.shader = SceneShader;
			SceneMaterial.SetFloat("_Range", 0f);
			sceneCamera.enabled = true;
		}

		public LayerOld GetLayer(int layer)
		{
			return layerList[layer];
		}

		public void SetLayerTextureImmediate(int layer, string textureName)
		{
			layerList[layer].SetTextureImmediate(textureName, LayerAlignment.AlignCenter);
		}

		public void StopShake()
		{
			iTween.Stop(PanelGameObject);
			PanelGameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		}

		public void FinalizeViewportChange()
		{
			iTween.Stop(PanelGameObject);
			PanelGameObject.transform.localPosition = targetPosition;
			PanelGameObject.transform.localScale = targetScale;
		}

		public void ResetViewportSize()
		{
			iTween.Stop(PanelGameObject);
			PanelGameObject.transform.localPosition = (targetPosition = defaultOffset);
			PanelGameObject.transform.localScale = (targetScale = new Vector3(1f, 1f, 1f));
		}

		public void EnlargeScene(float x, float y, float sx, float sy, float time, bool isblocking)
		{
			targetPosition = new Vector3(x, 0f - y, 0f) + defaultOffset;
			targetScale = new Vector3(800f / sx, 600f / sy, 1f);
			iTween.ScaleTo(PanelGameObject, iTween.Hash("scale", targetScale, "time", time, "islocal", true, "easetype", iTween.EaseType.linear));
			iTween.MoveTo(PanelGameObject, iTween.Hash("position", targetPosition, "time", time, "islocal", true, "easetype", iTween.EaseType.linear));
			if (isblocking)
			{
				GameSystem.Instance.AddWait(new Wait(time, WaitTypes.WaitForMove, FinalizeViewportChange));
			}
		}

		public void ShakeScene(int speed, int level, int attenuation, int vector, int loopcount, bool isblocking)
		{
			StopShake();
			float num = (float)(speed * loopcount);
			if (loopcount == 0)
			{
				num = 2.14748365E+09f;
			}
			Hashtable hashtable = iTween.Hash("time", num, "islocal", true);
			switch (vector)
			{
			case 0:
				hashtable.Add("x", level);
				break;
			case 2:
				hashtable.Add("y", level);
				break;
			default:
				Debug.LogWarning("Unhandled vector type for SHakeScreen! Type: " + vector);
				hashtable.Add("amount", level);
				break;
			}
			if (attenuation > 0)
			{
				Debug.LogWarning("attenuation set on ShakeScreen, but no handler set!");
			}
			iTween.ShakePosition(PanelGameObject, hashtable);
			if (isblocking && loopcount != 0)
			{
				GameSystem.Instance.AddWait(new Wait(num, WaitTypes.WaitForMove, StopShake));
			}
		}

		public void FadeScene(float time)
		{
			SceneMaterial.shader = SceneFadeShader;
			SceneMaterial.SetFloat("_Range", 0f);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", 0, "to", 1, "time", time / 1000f, "onupdate", "UpdateFadeProgress", "oncomplete", "HideScene"));
			GameSystem.Instance.AddWait(new Wait(time / 1000f, WaitTypes.WaitForScene, HideScene));
		}

		public void FadeSceneWithMask(string mask, float time)
		{
			Texture2D texture2D = AssetManager.Instance.LoadTexture(mask);
			if (texture2D == null)
			{
				Logger.LogWarning("Failed to load mask '" + mask + "' for scene transition! Falling back to fade.");
				FadeScene(time);
			}
			else
			{
				SceneMaterial.shader = SceneTransitionShader;
				SceneMaterial.SetTexture("_Mask", texture2D);
				SceneMaterial.SetFloat("_Range", 0f);
				SceneMaterial.SetFloat("_Fuzzyness", 0.32f);
				iTween.ValueTo(base.gameObject, iTween.Hash("from", 0, "to", 1, "time", time / 1000f, "onupdate", "UpdateFadeProgress", "oncomplete", "HideScene"));
				GameSystem.Instance.AddWait(new Wait(time / 1000f, WaitTypes.WaitForScene, HideScene));
			}
		}

		public void ChangeSceneDepth(int depth)
		{
			sceneCamera.depth = (float)depth;
		}

		public void UpdateFadeProgress(float progress)
		{
			SceneMaterial.SetFloat("_Range", progress);
		}

		public void ShowScene()
		{
			ResetLayer();
		}

		public void HideScene()
		{
			iTween.Stop(base.gameObject);
			sceneCamera.enabled = false;
			foreach (LayerOld layer in layerList)
			{
				layer.HideLayer();
			}
		}

		public void Initialize(int scenenum, int layercount)
		{
			sceneCamera = base.gameObject.GetComponent<Camera>();
			uiCamera = base.gameObject.GetComponent<UICamera>();
			SceneNum = scenenum;
			SceneName = "Scene" + (scenenum + 1);
			base.gameObject.name = SceneName;
			layerNum = LayerMask.NameToLayer(SceneName);
			base.gameObject.layer = layerNum;
			PanelGameObject.layer = layerNum;
			int num = 1 << layerNum;
			sceneCamera.cullingMask = num;
			uiCamera.eventReceiverMask = num;
			sceneCamera.depth = (float)scenenum;
			LayerCount = layercount;
			for (int i = 0; i <= LayerCount; i++)
			{
				GameObject gameObject = Object.Instantiate(LayerPrefab);
				gameObject.transform.parent = PanelGameObject.transform;
				gameObject.name = "Layer " + i;
				gameObject.transform.localPosition = new Vector3(0f, 0f, (float)i * 0.1f);
				gameObject.layer = base.gameObject.layer;
				LayerOld component = gameObject.GetComponent<LayerOld>();
				component.LayerNum = i;
				layerList.Add(component);
			}
			if (SceneMaterial == null)
			{
				PrepareMaterial();
			}
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
			{
				defaultOffset = new Vector3(-0.5f, -0.5f, 0f);
				PanelGameObject.transform.localPosition = defaultOffset;
			}
		}

		private void PrepareMaterial()
		{
			SceneMaterial = new Material(SceneShader);
		}

		private void Start()
		{
			PrepareMaterial();
		}

		private void OnRenderImage(RenderTexture source, RenderTexture dest)
		{
			if (SceneMaterial == null)
			{
				PrepareMaterial();
			}
			Graphics.Blit(source, dest, SceneMaterial);
		}
	}
}
