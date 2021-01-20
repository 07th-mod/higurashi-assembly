using Assets.Scripts.Core;
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

		public float PanelAlpha() => panel.alpha;

		private IEnumerator LeaveScreen(LeaveConfigDelegate callback)
		{
			panel = Panel.GetComponent<UIPanel>();
			LeanTween.value(Panel, UpdateAlpha, 1f, 0f, 0.3f);
			if (needShowMessageWindow)
			{
				GameSystem.Instance.MainUIController.FadeIn(0.3f);
				GameSystem.Instance.SceneController.RevealFace(0.3f);
				GameSystem.Instance.ExecuteActions();
			}
			yield return (object)new WaitForSeconds(0.5f);
			callback?.Invoke();
			Object.Destroy(base.gameObject);
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
			Panel.SetActive(value: true);
			panel = Panel.GetComponent<UIPanel>();
			UpdateAlpha(0f);
			yield return (object)null;
			yield return (object)null;
			LeanTween.value(Panel, UpdateAlpha, 0f, 1f, 0.3f);
			GameSystem.Instance.MainUIController.FadeOut(0.3f, isBlocking: false);
			GameSystem.Instance.SceneController.HideFace(0.3f);
			GameSystem.Instance.ExecuteActions();
			if (GameSystem.Instance.ConfigMenuFontSize > 0)
			{
				foreach (TextRefresher text in Panel.GetComponentsInChildren<TextRefresher>())
				{
					text.SetFontSize(GameSystem.Instance.ConfigMenuFontSize);
				}
			}
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
