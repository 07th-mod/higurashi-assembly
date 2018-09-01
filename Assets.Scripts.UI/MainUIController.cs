using Assets.Scripts.Core;
using Assets.Scripts.Core.Scene;
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

		private Layer bgLayer2;

		private bool carretVisible;

		private bool language;

		private int altFontId;

		private Animator carretAnimator;

		private GameSystem gameSystem;

		public void UpdateGuiPosition(int x, int y)
		{
			mainuiPanel.transform.localPosition = new Vector3((float)x, (float)y, 0f);
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
			gameSystem.TextController.TextArea.color = new Color(1f, 1f, 1f, a);
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
				bgLayer2.SetRange(alpha);
			}
		}

		private void ShowLayerBackground(float time)
		{
			if (carretVisible)
			{
				ShowCarret();
			}
			if (bgLayer != null && bgLayer2 != null)
			{
				bgLayer.FadeTo(gameSystem.MessageWindowOpacity, time);
				bgLayer2.FadeTo(gameSystem.MessageWindowOpacity, time);
			}
			else
			{
				if (bgLayer == null)
				{
					bgLayer = LayerPool.ActivateLayer();
				}
				bgLayer.gameObject.layer = LayerMask.NameToLayer("Scene1");
				bgLayer.SetPriority(62);
				bgLayer.name = "Window Background 1";
				bgLayer.IsStatic = true;
				bgLayer.DrawLayer("windo_filter", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, time, /*isBlocking:*/ false);
				if (bgLayer2 == null)
				{
					bgLayer2 = LayerPool.ActivateLayer();
				}
				bgLayer2.gameObject.layer = LayerMask.NameToLayer("Scene2");
				bgLayer2.SetPriority(62);
				bgLayer2.name = "Window Background 2";
				bgLayer2.IsStatic = true;
				bgLayer2.DrawLayer("windo_filter", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, time, /*isBlocking:*/ false);
			}
		}

		private void HideLayerBackground(float time)
		{
			if (bgLayer == null || !bgLayer.IsInUse)
			{
				if (bgLayer != null)
				{
					bgLayer.FadeTo(0f, time);
				}
				if (bgLayer2 != null)
				{
					bgLayer2.FadeTo(0f, time);
				}
			}
			else
			{
				if (Mathf.Approximately(time, 0f))
				{
					if (bgLayer != null)
					{
						bgLayer.FadeTo(0f, time);
					}
					if (bgLayer2 != null)
					{
						bgLayer2.FadeTo(0f, time);
					}
				}
				else
				{
					if (bgLayer != null)
					{
						bgLayer.FadeTo(0f, time);
					}
					if (bgLayer2 != null)
					{
						bgLayer2.FadeTo(0f, time);
					}
				}
				HideCarret();
			}
		}

		public void ShowMessageBox()
		{
			ShowLayerBackground(0f);
			bgLayer.FinishAll();
			bgLayer2.FinishAll();
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
			if (bgLayer2 != null)
			{
				bgLayer2.FinishAll();
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
			gameSystem.ExecuteActions();
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
			gameSystem.ExecuteActions();
		}

		private IEnumerator QuickSaveAnimation()
		{
			QuickSaveIcon.SetActive(value: true);
			TweenAlpha t = QuickSaveIcon.GetComponent<TweenAlpha>();
			t.PlayForward();
			yield return (object)new WaitForSeconds(3f);
			t.PlayReverse();
			yield return (object)new WaitForSeconds(0.5f);
			QuickSaveIcon.SetActive(value: false);
		}

		public void ShowQuickSaveIcon()
		{
			StartCoroutine(QuickSaveAnimation());
		}

		public void SetCharSpacing(int spacing)
		{
			TextWindow.characterSpacing = (float)spacing;
		}

		public void SetLineSpacing(int spacing)
		{
			TextWindow.lineSpacing = (float)spacing;
		}

		public void SetFontSize(int size)
		{
			TextWindow.fontSize = (float)size;
		}

		public void SetWindowPos(int x, int y)
		{
			TextWindow.gameObject.transform.localPosition = new Vector3((float)x, (float)y, 0f);
		}

		public void SetWindowSize(int x, int y)
		{
			TextContainer textContainer = TextWindow.textContainer;
			textContainer.width = (float)x;
			textContainer.height = (float)y;
		}

		public void SetWindowMargins(int left, int top, int right, int bottom)
		{
			TextContainer textContainer = TextWindow.textContainer;
			textContainer.margins.Set((float)left, (float)top, (float)right, (float)bottom);
		}

		public TextMeshProFont GetEnglishFont()
		{
			return Resources.Load<TextMeshProFont>(FontList[altFontId]);
		}

		public TextMeshProFont GetJapaneseFont()
		{
			return Resources.Load<TextMeshProFont>(FontList[0]);
		}

		public TextMeshProFont GetCurrentFont()
		{
			return TextWindow.font;
		}

		public void ChangeFontId(int id)
		{
			altFontId = id;
			if (language)
			{
				TextWindow.font = Resources.Load<TextMeshProFont>(FontList[altFontId]);
			}
			else
			{
				TextWindow.font = Resources.Load<TextMeshProFont>(FontList[0]);
			}
		}

		private void Awake()
		{
			language = GameSystem.Instance.UseEnglishText;
			if (language)
			{
				TextWindow.font = Resources.Load<TextMeshProFont>(FontList[altFontId]);
			}
			else
			{
				TextWindow.font = Resources.Load<TextMeshProFont>(FontList[0]);
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
			Vector3 localPosition = SkipMarker.transform.localPosition;
			float x = Mathf.Lerp(localPosition.x, (float)num, Time.deltaTime * 10f);
			Vector3 localPosition2 = SkipMarker.transform.localPosition;
			Vector3 localPosition3 = new Vector3(x, localPosition2.y, 0f);
			Vector3 localPosition4 = AutoMarker.transform.localPosition;
			float x2 = Mathf.Lerp(localPosition4.x, (float)num2, Time.deltaTime * 10f);
			Vector3 localPosition5 = AutoMarker.transform.localPosition;
			Vector3 localPosition6 = new Vector3(x2, localPosition5.y, 0f);
			SkipMarker.transform.localPosition = localPosition3;
			AutoMarker.transform.localPosition = localPosition6;
			SkipMarker.SetActive(!(localPosition3.x > 400f));
			AutoMarker.SetActive(!(localPosition6.x > 400f));
			if (carretVisible)
			{
				ShowCarret();
			}
			if (language != GameSystem.Instance.UseEnglishText)
			{
				language = GameSystem.Instance.UseEnglishText;
				if (language)
				{
					TextWindow.font = Resources.Load<TextMeshProFont>(FontList[altFontId]);
				}
				else
				{
					TextWindow.font = Resources.Load<TextMeshProFont>(FontList[0]);
				}
			}
		}
	}
}
