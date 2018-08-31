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

		public void SetTransitionMask(string maskname)
		{
			sceneMaterial.shader = maskShader;
			Texture2D texture = AssetManager.Instance.LoadTexture(maskname);
			sceneMaterial.SetTexture("_Mask", texture);
			sceneMaterial.SetFloat("_Fuzzyness", 0.45f);
			UpdateRange(0f);
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
			sceneMaterial.shader = fadeShader;
			StartTransition(time);
		}

		public void StopFadeIn()
		{
			iTween.Stop(base.gameObject);
			UpdateRange(1f);
			Layer[] componentsInChildren = Panel.GetComponentsInChildren<Layer>();
			foreach (Layer layer in componentsInChildren)
			{
				if (layer.gameObject.layer != sceneController.GetActiveLayerMask())
				{
					if (layer.gameObject.layer == LayerMask.NameToLayer("RenderBoth"))
					{
						layer.gameObject.layer = sceneController.GetActiveLayerMask();
					}
					else if (layer.Priority < SceneController.UpperLayerRange && layer.tag != "BackgroundLayer")
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
			yield return (object)new WaitForEndOfFrame();
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
