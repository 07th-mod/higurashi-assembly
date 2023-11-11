using Assets.Scripts.Core.AssetManagement;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	[RequireComponent(typeof(Camera))]
	[RequireComponent(typeof(UICamera))]
	public class Scene : MonoBehaviour
	{
		public GameObject Panel;

		public GameObject OtherPanel;

		public LayerMask LayerMask;

		public string LayerName;

		public bool HasBackground;

		[HideInInspector]
		public Layer BackgroundLayer;

		private SceneController sceneController;

		private UICamera uiCamera;

		public Material sceneMaterial;

		private Shader defaultShader;

		private Shader maskShader;

		private Shader fadeShader;

		private Shader scrollShader;

		private string maskTextureName;

		private Texture2D maskTexture;

		public float Depth
		{
			get
			{
				return GetComponent<Camera>().depth;
			}
			set
			{
				GetComponent<Camera>().depth = value;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return GetComponent<Camera>().enabled;
			}
			set
			{
				GetComponent<Camera>().enabled = value;
				uiCamera.enabled = value;
			}
		}

		public void SetTransitionMask(string maskname, int style)
		{
			sceneMaterial.shader = maskShader;
			if (maskTexture != null)
			{
				AssetManager.Instance.ReleaseTexture(maskTextureName, maskTexture);
			}
			maskTextureName = maskname;
			maskTexture = AssetManager.Instance.LoadTexture(maskname);
			sceneMaterial.SetTexture("_Mask", maskTexture);
			sceneMaterial.SetFloat("_Fuzzyness", (style == 1) ? 0.01f : 0.75f);
			UpdateRange(0f);
		}

		public void SetScrollX(float x)
		{
			sceneMaterial.SetFloat("_ScrollX", x);
		}

		public void SetScrollY(float y)
		{
			sceneMaterial.SetFloat("_ScrollY", y);
		}

		public void StartTransition(float time)
		{
			base.gameObject.SetActive(value: true);
			UpdateRange(0f);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", time, "onupdate", "UpdateRange", "oncomplete", "UpdateRange", "oncompleteparams", 1f));
		}

		public void FadeSceneIn(float time)
		{
			base.gameObject.SetActive(value: true);
			base.transform.localPosition = Vector3.zero;
			sceneMaterial.shader = fadeShader;
			StartTransition(time);
		}

		public void ScrollSceneIn(float time, int direction)
		{
			base.gameObject.SetActive(value: true);
			if (direction < 0 || direction > 3)
			{
				throw new Exception("Invalid scene scroll direction: " + direction);
			}
			sceneMaterial.shader = scrollShader;
			SetScrollY(0f);
			SetScrollX(0f);
			if (direction == 0)
			{
				SetScrollX(-1f);
				iTween.ValueTo(base.gameObject, iTween.Hash("from", -1f, "to", 0f, "time", time, "onupdate", "SetScrollX", "oncomplete", "StopFadeIn"));
			}
			if (direction == 1)
			{
				SetScrollX(1f);
				iTween.ValueTo(base.gameObject, iTween.Hash("from", 1f, "to", 0f, "time", time, "onupdate", "SetScrollX", "oncomplete", "StopFadeIn"));
			}
			if (direction == 2)
			{
				SetScrollY(-1f);
				iTween.ValueTo(base.gameObject, iTween.Hash("from", -1f, "to", 0f, "time", time, "onupdate", "SetScrollY", "oncomplete", "StopFadeIn"));
			}
			if (direction == 3)
			{
				SetScrollY(1f);
				iTween.ValueTo(base.gameObject, iTween.Hash("from", 1f, "to", 0f, "time", time, "onupdate", "SetScrollY", "oncomplete", "StopFadeIn"));
			}
		}

		public void ScrollSceneOut(float time, int direction)
		{
			base.gameObject.SetActive(value: true);
			if (direction < 0 || direction > 3)
			{
				throw new Exception("Invalid scene scroll direction: " + direction);
			}
			sceneMaterial.shader = scrollShader;
			SetScrollY(0f);
			SetScrollX(0f);
			if (direction == 0)
			{
				iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", time, "onupdate", "SetScrollX", "oncomplete", "StopFadeIn"));
			}
			if (direction == 1)
			{
				iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", -1f, "time", time, "onupdate", "SetScrollX", "oncomplete", "StopFadeIn"));
			}
			if (direction == 2)
			{
				iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", time, "onupdate", "SetScrollY", "oncomplete", "StopFadeIn"));
			}
			if (direction == 3)
			{
				iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", -1f, "time", time, "onupdate", "SetScrollY", "oncomplete", "StopFadeIn"));
			}
		}

		public void StopFadeIn()
		{
			iTween.Stop(base.gameObject);
			UpdateRange(1f);
			Layer[] componentsInChildren = OtherPanel.GetComponentsInChildren<Layer>();
			foreach (Layer layer in componentsInChildren)
			{
				if (layer.gameObject.layer != sceneController.GetActiveLayerMask())
				{
					if (layer.gameObject.layer == LayerMask.NameToLayer("RenderBoth") && !layer.IsPersistent)
					{
						layer.gameObject.layer = sceneController.GetActiveLayerMask();
					}
					else if (layer.tag != "BackgroundLayer" && !layer.IsPersistent)
					{
						layer.HideLayer();
					}
				}
			}
			sceneController.HideBackgroundSceneObject();
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(ChangeShader(defaultShader));
			}
			else
			{
				sceneMaterial.shader = defaultShader;
			}
		}

		public void UpdateRange(float r)
		{
			if (sceneMaterial != null)
			{
				sceneMaterial.SetFloat("_Range", r);
			}
		}

		private IEnumerator ChangeShader(Shader target)
		{
			yield return new WaitForEndOfFrame();
			sceneMaterial.shader = target;
		}

		private void Awake()
		{
			sceneController = base.transform.parent.parent.GetComponent<SceneController>();
			uiCamera = GetComponent<UICamera>();
			GetComponent<Camera>().cullingMask = LayerMask;
			if (HasBackground)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Layer", typeof(GameObject))) as GameObject;
				if (gameObject == null)
				{
					throw new Exception("Failed to instantiate Layer prefab!");
				}
				gameObject.transform.parent = Panel.transform;
				gameObject.layer = LayerMask.NameToLayer(LayerName);
				gameObject.transform.localPosition = new Vector3(0f, 0f, 9.9f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject.tag = "BackgroundLayer";
				gameObject.name = base.name + " Background";
				BackgroundLayer = gameObject.GetComponent<Layer>();
			}
			defaultShader = Shader.Find("MGShader/SceneOpaque");
			maskShader = Shader.Find("MGShader/SceneFadeWithMask");
			fadeShader = Shader.Find("MGShader/SceneFade");
			scrollShader = Shader.Find("MGShader/SceneScroll");
			sceneMaterial = new Material(defaultShader);
		}

		private void Update()
		{
			GetComponent<Camera>().cullingMask = LayerMask;
			uiCamera.eventReceiverMask = LayerMask;
		}

		private void OnRenderImage(RenderTexture source, RenderTexture dest)
		{
			if (!(sceneMaterial == null))
			{
				Graphics.Blit(source, dest, sceneMaterial);
			}
		}
	}
}
