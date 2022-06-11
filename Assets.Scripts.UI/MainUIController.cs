using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.Buriko.Util;
using Assets.Scripts.Core.Scene;
using Assets.Scripts.Core.TextWindow;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
	[RequireComponent(typeof(UIPanel))]
	public class MainUIController : MonoBehaviour
	{
		public LayerPool LayerPool;

		public GameObject QuickSaveIcon;

		public GameObject CarretPageSprite;

		public GameObject CarretLineSprite;

		public GameObject AutoMarker;

		public GameObject SkipMarker;

		public TextMeshPro TextWindow;

		public string[] FontList;

		public BlackBars[] Bars;

		private UIPanel panel;

		private GameObject mainuiPanel;

		private Layer bgLayer;

		private bool carretVisible;

		private bool language;

		private int altFontId;

		private Animator carretAnimator;

		private GameSystem gameSystem;

		public void UpdateGuiPosition(int x, int y)
		{
			mainuiPanel.transform.localPosition = new Vector3(x, y, 0f);
		}

		public void UpdateBlackBars()
		{
			for (int i = 0; i < Bars.Length; i++)
			{
				Bars[i].UpdatePosition();
			}
		}

		private void UpdateAlpha(float a)
		{
			panel.alpha = a;
			Color color = TextController.TextColor;
			gameSystem.TextController.TextArea.color = new Color(color.r, color.g, color.b, a);
		}

		public void StopShake()
		{
			Shaker component = mainuiPanel.gameObject.GetComponent<Shaker>();
			if (component != null)
			{
				component.StopShake();
			}
			mainuiPanel.transform.localPosition = Vector3.zero;
		}

		public void ShakeScene(float speed, int level, int attenuation, int vector, int loopcount, bool isblocking)
		{
			Shaker.ShakeObject(mainuiPanel.gameObject, speed, level, attenuation, vector, loopcount, isblocking);
		}

		public void SetWindowOpacity(float alpha)
		{
			if (gameSystem.MessageBoxVisible)
			{
				bgLayer.SetRange(alpha);
			}
		}

		private void ShowLayerBackground(float time)
		{
			if (carretVisible)
			{
				ShowCarret();
			}
			string text = "windo_filter";
			if (BurikoMemory.Instance.IsMemory("WindowBackground"))
			{
				text = BurikoMemory.Instance.GetMemory("WindowBackground").StringValue(new BurikoReference("bg", 0));
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "windo_filter";
			}
			if (bgLayer != null)
			{
				string primaryName = bgLayer.PrimaryName;
				if (text == primaryName)
				{
					bgLayer.FadeTo(gameSystem.MessageWindowOpacity, time);
					return;
				}
				bgLayer.IsStatic = false;
				bgLayer.HideLayer();
				bgLayer = null;
			}
			if (bgLayer == null)
			{
				bgLayer = LayerPool.ActivateLayer();
			}
			bgLayer.gameObject.layer = LayerMask.NameToLayer("Scene3");
			bgLayer.transform.parent = GameSystem.Instance.SceneController.FacePanel.transform;
			bgLayer.SetPriority(62);
			bgLayer.name = "Window Background";
			bgLayer.IsStatic = true;
			bgLayer.DrawLayer(text, 0, 0, 0, null, null, gameSystem.MessageWindowOpacity, isBustshot: false, 0, time, isBlocking: false);
		}

		private void HideLayerBackground(float time)
		{
			if (bgLayer == null || !bgLayer.IsInUse)
			{
				if (bgLayer != null)
				{
					bgLayer.FadeTo(0f, time);
				}
				return;
			}
			if (Mathf.Approximately(time, 0f))
			{
				if (bgLayer != null)
				{
					bgLayer.FadeTo(0f, time);
				}
			}
			else if (bgLayer != null)
			{
				bgLayer.FadeTo(0f, time);
			}
			HideCarret();
		}

		public void ShowMessageBox()
		{
			ShowLayerBackground(0f);
			bgLayer.FinishAll();
			iTween.Stop(base.gameObject);
			UpdateAlpha(1f);
			GameSystem.Instance.MessageBoxVisible = true;
			if (carretVisible)
			{
				ShowCarret();
			}
		}

		public void HideMessageBox()
		{
			if (bgLayer != null)
			{
				bgLayer.FinishAll();
			}
			iTween.Stop(base.gameObject);
			GameSystem.Instance.MessageBoxVisible = false;
			UpdateAlpha(0f);
			QuickSaveIcon.SetActive(value: false);
			HideCarret();
		}

		public void FadeOut(float time, bool isBlocking)
		{
			HideLayerBackground(time);
			HideCarret();
			GameSystem.Instance.MessageBoxVisible = false;
			if (isBlocking || !Mathf.Approximately(time, 0f))
			{
				iTween.Stop(base.gameObject);
				iTween.ValueTo(base.gameObject, iTween.Hash("name", "FadeOut", "from", panel.alpha, "to", 0, "time", time, "onupdate", "UpdateAlpha", "oncomplete", "HideMessageBox"));
				if (isBlocking)
				{
					GameSystem.Instance.AddWait(new Wait(time, WaitTypes.WaitForScene, HideMessageBox));
				}
			}
			else
			{
				UpdateAlpha(0f);
			}
		}

		public void FadeIn(float time)
		{
			GameSystem.Instance.MessageBoxVisible = true;
			ShowLayerBackground(time);
			if (!Mathf.Approximately(time, 0f))
			{
				iTween.Stop(base.gameObject);
				iTween.ValueTo(base.gameObject, iTween.Hash("from", 0, "to", 1, "time", time, "onupdate", "UpdateAlpha", "oncomplete", "ShowMessageBox"));
				GameSystem.Instance.AddWait(new Wait(time, WaitTypes.WaitForScene, ShowMessageBox));
			}
			else
			{
				UpdateAlpha(1f);
			}
		}

		private IEnumerator QuickSaveAnimation()
		{
			QuickSaveIcon.SetActive(value: true);
			TweenAlpha t = QuickSaveIcon.GetComponent<TweenAlpha>();
			t.PlayForward();
			yield return new WaitForSeconds(3f);
			t.PlayReverse();
			yield return new WaitForSeconds(0.5f);
			QuickSaveIcon.SetActive(value: false);
		}

		public void ShowQuickSaveIcon()
		{
			StartCoroutine(QuickSaveAnimation());
		}

		public void SetCharSpacing(int spacing)
		{
			TextWindow.characterSpacing = spacing;
		}

		public void SetLineSpacing(int spacing)
		{
			TextWindow.lineSpacing = spacing;
		}

		public void SetFontSize(int size)
		{
			TextWindow.fontSize = size;
		}

		public void SetWindowPos(int x, int y)
		{
			TextWindow.gameObject.transform.localPosition = new Vector3(x, y, 0f);
		}

		public void SetWindowSize(int x, int y)
		{
			TextWindow.rectTransform.sizeDelta = new Vector2(x, y);
		}

		public void SetWindowMargins(int left, int top, int right, int bottom)
		{
			TextWindow.margin = new Vector4(left, top, right, bottom);
		}

		public TMP_FontAsset GetEnglishFont()
		{
			return Resources.Load<TMP_FontAsset>(FontList[altFontId]);
		}

		public TMP_FontAsset GetJapaneseFont()
		{
			return Resources.Load<TMP_FontAsset>(FontList[0]);
		}

		public TMP_FontAsset GetCurrentFont()
		{
			return TextWindow.font;
		}

		public void ChangeFontId(int id)
		{
			altFontId = id;
			if (language)
			{
				TextWindow.font = Resources.Load<TMP_FontAsset>(FontList[altFontId]);
			}
			else
			{
				TextWindow.font = Resources.Load<TMP_FontAsset>(FontList[0]);
			}
		}

		private void Awake()
		{
			language = GameSystem.Instance.UseEnglishText;
			if (language)
			{
				TextWindow.font = Resources.Load<TMP_FontAsset>(FontList[altFontId]);
			}
			else
			{
				TextWindow.font = Resources.Load<TMP_FontAsset>(FontList[0]);
			}
		}

		public void ShowCarret()
		{
			carretVisible = true;
			if (!gameSystem.TextController.GetAppendState())
			{
				CarretPageSprite.transform.localPosition = gameSystem.TextController.GetCarretPosition();
				if (!CarretPageSprite.activeSelf)
				{
					CarretPageSprite.SetActive(value: true);
				}
				CarretLineSprite.SetActive(value: false);
			}
			else
			{
				CarretLineSprite.transform.localPosition = gameSystem.TextController.GetCarretPosition();
				if (!CarretLineSprite.activeSelf)
				{
					CarretLineSprite.SetActive(value: true);
				}
				CarretPageSprite.SetActive(value: false);
			}
		}

		public void HideCarret()
		{
			carretVisible = false;
			CarretPageSprite.SetActive(value: false);
			CarretLineSprite.SetActive(value: false);
		}

		private void Start()
		{
			panel = GetComponent<UIPanel>();
			panel.enabled = false;
			panel.alpha = 0f;
			mainuiPanel = GameObject.FindGameObjectWithTag("PrimaryUIPanel");
		}

		private void Update()
		{
			if (gameSystem == null)
			{
				gameSystem = GameSystem.Instance;
			}
			int num = 402;
			int num2 = 402;
			if (gameSystem.IsSkipping && !gameSystem.IsForceSkip)
			{
				num = 334;
			}
			if (gameSystem.IsAuto)
			{
				num2 = 334;
			}
			Vector3 localPosition = new Vector3(Mathf.Lerp(SkipMarker.transform.localPosition.x, num, Time.deltaTime * 10f), SkipMarker.transform.localPosition.y, 0f);
			Vector3 localPosition2 = new Vector3(Mathf.Lerp(AutoMarker.transform.localPosition.x, num2, Time.deltaTime * 10f), AutoMarker.transform.localPosition.y, 0f);
			SkipMarker.transform.localPosition = localPosition;
			AutoMarker.transform.localPosition = localPosition2;
			SkipMarker.SetActive(!(localPosition.x > 400f));
			AutoMarker.SetActive(!(localPosition2.x > 400f));
			if (carretVisible)
			{
				ShowCarret();
			}
			if (language != GameSystem.Instance.UseEnglishText)
			{
				language = GameSystem.Instance.UseEnglishText;
				if (language)
				{
					TextWindow.font = Resources.Load<TMP_FontAsset>(FontList[altFontId]);
				}
				else
				{
					TextWindow.font = Resources.Load<TMP_FontAsset>(FontList[0]);
				}
			}
		}
	}
}
