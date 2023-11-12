using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	public class LayerPool : MonoBehaviour
	{
		public int PoolSize;

		public SceneController SceneController;

		private Stack<GameObject> layerList = new Stack<GameObject>();

		private List<GameObject> layerObjList = new List<GameObject>();

		private bool isInitialized;

		public Layer ActivateLayer()
		{
			if (!isInitialized)
			{
				Initialize();
			}
			GameObject gameObject = layerList.Pop();
			GameObject activePanel = SceneController.GetActivePanel();
			gameObject.transform.parent = activePanel.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			Layer component = gameObject.GetComponent<Layer>();
			gameObject.SetActive(value: true);
			layerObjList.Remove(gameObject);
			return component;
		}

		public void ReturnLayer(Layer layer)
		{
			layer.LayerID = null;
			GameObject gameObject = layer.gameObject;
			if (!layerObjList.Contains(gameObject))
			{
				if (layer.IsInUse || !string.IsNullOrWhiteSpace(layer.PrimaryName))
				{
					Debug.LogWarning("Warning! Attempting to return layer " + layer.name + " to the pool while it still active!");
				}
				if (layer.IsPersistent)
				{
					layer.IsPersistent = false;
				}
				gameObject.transform.parent = base.transform;
				gameObject.layer = LayerMask.NameToLayer("NotRendered");
				gameObject.SetActive(value: false);
				layerList.Push(gameObject);
				layerObjList.Add(gameObject);
			}
			else
			{
				// NOTE: In Hou+, Doddler added the same fix that we previously had, (with some extra stuff),
				// so I have used his code exactly (above), but added the below log message if a layer gets returned twice
				MOD.Scripts.Core.MODLogger.Log("WARNING: Ignoring layer returned twice", true);
			}
		}

		private void Initialize()
		{
			GameObject original = Resources.Load<GameObject>("Layer");
			for (int i = 0; i < PoolSize; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(original);
				if (gameObject == null)
				{
					throw new Exception("Failed to instantiate Layer prefab!");
				}
				gameObject.transform.parent = base.transform;
				gameObject.SetActive(value: false);
				layerList.Push(gameObject);
				layerObjList.Add(gameObject);
			}
			isInitialized = true;
		}
	}
}
