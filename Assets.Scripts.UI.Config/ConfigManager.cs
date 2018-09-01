using Assets.Scripts.Core;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI.Config
{
	public class ConfigManager : MonoBehaviour
	{
		public delegate void LeaveConfigDelegate();

		public GameObject Panel;

		public Material TextMaterial;

		private int screennum;

		private bool needShowMessageWindow;

		private UIPanel panel;

		private IEnumerator LeaveScreen(ConfigManager.LeaveConfigDelegate callback)
		{
			this.panel = this.Panel.GetComponent<UIPanel>();
			LeanTween.value(this.Panel, new Action<float>(this.UpdateAlpha), 1f, 0f, 0.3f);
			if (this.needShowMessageWindow)
			{
				GameSystem.Instance.MainUIController.FadeIn(0.3f);
				GameSystem.Instance.SceneController.RevealFace(0.3f);
				GameSystem.Instance.ExecuteActions();
			}
			yield return new WaitForSeconds(0.5f);
			if (callback != null)
			{
				callback();
			}
			UnityEngine.Object.Destroy(base.gameObject);
			yield break;
		}

		private void UpdateAlpha(float f)
		{
			panel.alpha = f;
			TextMaterial.SetColor("_FaceColor", new Color(1f, 1f, 1f, f));
			TextMaterial.SetColor("_OutlineColor", new Color(0f, 0f, 0f, f));
		}

		public void Leave(LeaveConfigDelegate callback)
		{
			StartCoroutine(LeaveScreen(callback));
		}

		private IEnumerator DoOpen(int offscreen)
		{
			this.Panel.SetActive(true);
			this.panel = this.Panel.GetComponent<UIPanel>();
			this.UpdateAlpha(0f);
			yield return null;
			yield return null;
			LeanTween.value(this.Panel, new Action<float>(this.UpdateAlpha), 0f, 1f, 0.3f);
			GameSystem.Instance.MainUIController.FadeOut(0.3f, false);
			GameSystem.Instance.SceneController.HideFace(0.3f);
			GameSystem.Instance.ExecuteActions();
			yield break;
		}

		public void Open(int screen, bool msgWindow)
		{
			needShowMessageWindow = msgWindow;
			screennum = screen;
			int offscreen = 0;
			if (screennum == 0)
			{
				offscreen = 1;
			}
			StartCoroutine(DoOpen(offscreen));
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
