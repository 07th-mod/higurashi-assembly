using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.Scene;
using MOD.Scripts.UI;
using System;
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

		private Vector3 unscaledPosition;

		public void UpdateGuiPosition(int x, int y)
		{
			unscaledPosition = new Vector3((float)x, (float)y, 0f);
			UpdateGuiPosition();
		}

		public void UpdateGuiScale(float x, float y)
		{
			mainuiPanel.transform.localScale = new Vector3(x, y, 1f);
			UpdateGuiPosition();
		}

		private void UpdateGuiPosition()
		{
			Vector3 scaledPosition = new Vector3(unscaledPosition.x, unscaledPosition.y, unscaledPosition.z);
			scaledPosition.x *= mainuiPanel.transform.localScale.x;
			scaledPosition.y *= mainuiPanel.transform.localScale.y;
			scaledPosition.z *= mainuiPanel.transform.localScale.z;
			mainuiPanel.transform.localPosition = scaledPosition;
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
			if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1)
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
					if (BurikoMemory.Instance.GetGlobalFlag("GADVTextbox").IntValue() == 0)
					{
						bgLayer.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, time, /*isBlocking:*/ false);
					}
					if (BurikoMemory.Instance.GetGlobalFlag("GADVTextbox").IntValue() == 1)
					{
						bgLayer.DrawLayer("windo_filter_adv2", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, time, /*isBlocking:*/ false);
					}
					if (bgLayer2 == null)
					{
						bgLayer2 = LayerPool.ActivateLayer();
					}
					bgLayer2.gameObject.layer = LayerMask.NameToLayer("Scene2");
					bgLayer2.SetPriority(62);
					bgLayer2.name = "Window Background 2";
					bgLayer2.IsStatic = true;
					if (BurikoMemory.Instance.GetGlobalFlag("GADVTextbox").IntValue() == 0)
					{
						bgLayer2.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, time, /*isBlocking:*/ false);
					}
					if (BurikoMemory.Instance.GetGlobalFlag("GADVTextbox").IntValue() == 1)
					{
						bgLayer2.DrawLayer("windo_filter_adv2", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, time, /*isBlocking:*/ false);
					}
				}
			}
			else
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
			TextWindow.outlineWidth = GameSystem.Instance.OutlineWidth;
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
			TextWindow.outlineWidth = GameSystem.Instance.OutlineWidth;
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
				TextWindow.outlineWidth = GameSystem.Instance.OutlineWidth;
			}
		}

		public void MODResetLayerBackground()
		{
			MODMainUIController mODMainUIController = new MODMainUIController();
			if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1)
			{
				BurikoMemory.Instance.SetGlobalFlag("GADVMode", 0);
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 2);
				GameSystem.Instance.MainUIController.bgLayer.ReleaseTextures();
				GameSystem.Instance.MainUIController.bgLayer2.ReleaseTextures();
				GameSystem.Instance.MainUIController.bgLayer.DrawLayer("windo_filter", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				GameSystem.Instance.MainUIController.bgLayer2.DrawLayer("windo_filter", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				mODMainUIController.NVLModeSettingStore();
			}
			else
			{
				BurikoMemory.Instance.SetGlobalFlag("GADVMode", 1);
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 0);
				GameSystem.Instance.MainUIController.bgLayer.ReleaseTextures();
				GameSystem.Instance.MainUIController.bgLayer2.ReleaseTextures();
				if (BurikoMemory.Instance.GetGlobalFlag("GADVTextbox").IntValue() == 0)
				{
					GameSystem.Instance.MainUIController.bgLayer.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
					GameSystem.Instance.MainUIController.bgLayer2.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);

					mODMainUIController.ADVModeSettingStore();
				}
				if (BurikoMemory.Instance.GetGlobalFlag("GADVTextbox").IntValue() == 1)
				{
					GameSystem.Instance.MainUIController.bgLayer.DrawLayer("windo_filter_adv2", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
					GameSystem.Instance.MainUIController.bgLayer2.DrawLayer("windo_filter_adv2", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
					mODMainUIController.ADVModeTextbox2SettingStore();
				}
			}
		}

		public void MODADVModeTextbox2SettingLoad()
		{
			MODMainUIController mODMainUIController = new MODMainUIController();
			BurikoMemory.Instance.SetGlobalFlag("GADVMode", 1);
			BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 0);
			GameSystem.Instance.MainUIController.bgLayer.ReleaseTextures();
			GameSystem.Instance.MainUIController.bgLayer2.ReleaseTextures();

			if (BurikoMemory.Instance.GetGlobalFlag("GADVTextbox").IntValue() == 1)
			{
				BurikoMemory.Instance.SetGlobalFlag("GADVTextbox", 1);
				GameSystem.Instance.MainUIController.bgLayer.DrawLayer("windo_filter_adv2", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				GameSystem.Instance.MainUIController.bgLayer2.DrawLayer("windo_filter_adv2", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				mODMainUIController.ADVModeTextbox2SettingStore();
			}
			if (BurikoMemory.Instance.GetGlobalFlag("GADVTextbox").IntValue() == 0)
			{
				BurikoMemory.Instance.SetGlobalFlag("GADVTextbox", 0);
				GameSystem.Instance.MainUIController.bgLayer.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				GameSystem.Instance.MainUIController.bgLayer2.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				mODMainUIController.ADVModeSettingStore();
			}
			if (BurikoMemory.Instance.GetGlobalFlag("GADVTextbox").IntValue() > 1)
			{
				BurikoMemory.Instance.SetGlobalFlag("GADVTextbox", 0);
				GameSystem.Instance.MainUIController.bgLayer.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				GameSystem.Instance.MainUIController.bgLayer2.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				mODMainUIController.ADVModeSettingStore();
			}
		}

		public void MODenableNVLModeINADVMode()
		{
			BurikoMemory.Instance.SetFlag("NVL_in_ADV", 1);
			if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1)
			{
				MODMainUIController mODMainUIController = new MODMainUIController();
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 2);
				GameSystem.Instance.MainUIController.bgLayer.ReleaseTextures();
				GameSystem.Instance.MainUIController.bgLayer2.ReleaseTextures();
				GameSystem.Instance.MainUIController.bgLayer.DrawLayer("windo_filter_nvladv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				GameSystem.Instance.MainUIController.bgLayer2.DrawLayer("windo_filter_nvladv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				mODMainUIController.NVLADVModeSettingStore();
			}
		}

		public void MODdisableNVLModeINADVMode()
		{
			BurikoMemory.Instance.SetFlag("NVL_in_ADV", 0);
			if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1)
			{
				MODMainUIController mODMainUIController = new MODMainUIController();
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 0);
				GameSystem.Instance.MainUIController.bgLayer.ReleaseTextures();
				GameSystem.Instance.MainUIController.bgLayer2.ReleaseTextures();
				GameSystem.Instance.MainUIController.bgLayer.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				GameSystem.Instance.MainUIController.bgLayer2.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
				mODMainUIController.ADVModeSettingStore();
			}
		}

		public void OnGUI()
		{
			if (BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 2)
			{
				gameSystem.CanSkip = true;
				gameSystem.CanInput = true;
				gameSystem.ShowUIControls();
			}
			if ((BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() == 1) | (BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() == 2))
			{
				string[] array = new string[6]
				{
					"GADVMode",
					"GAltBGM",
					"GAltSE",
					"GAltVoice",
					"GAltVoicePriority",
					"GLipSync"
				};
				string[] array2 = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					if (BurikoMemory.Instance.GetGlobalFlag(array[i]).IntValue() == 0)
					{
						array2[i] = "OFF";
					}
					else if (BurikoMemory.Instance.GetGlobalFlag(array[i]).IntValue() == 1)
					{
						array2[i] = "ON";
					}
					else
					{
						array2[i] = "ERROR";
					}
				}
				string[] array3 = new string[4]
				{
					"GCensor",
					"GEffectExtend",
					"GAltBGMflow",
					"GAltSEflow"
				};
				string[] array4 = new string[4]
				{
					"GCensorMaxNum",
					"GEffectExtendMaxNum",
					"GAltBGMflowMaxNum",
					"GAltSEflowMaxNum"
				};
				string[] array5 = new string[array4.Length];
				for (int j = 0; j < array4.Length; j++)
				{
					array5[j] = " (MAX:" + BurikoMemory.Instance.GetGlobalFlag(array4[j]).IntValue().ToString() + ")";
				}
				string[] array6 = new string[array3.Length];
				for (int k = 0; k < array3.Length; k++)
				{
					if (BurikoMemory.Instance.GetGlobalFlag(array3[k]).IntValue() > BurikoMemory.Instance.GetGlobalFlag(array4[k]).IntValue())
					{
						array6[k] = "ERROR";
					}
					else
					{
						array6[k] = BurikoMemory.Instance.GetGlobalFlag(array3[k]).IntValue().ToString();
					}
				}
				string text = (BurikoMemory.Instance.GetGlobalFlag("GMOD_SETTING_LOADER").IntValue() == 0) ? "\nRestore ADV-MODE Settings\nRelaunch Game 2 Times" : ((BurikoMemory.Instance.GetGlobalFlag("GMOD_SETTING_LOADER").IntValue() == 1) ? "\nRestore NVL-MODE Settings\nRelaunch Game 2 Times" : ((BurikoMemory.Instance.GetGlobalFlag("GMOD_SETTING_LOADER").IntValue() == 2) ? "\nRestore Vanilla Settings\nRelaunch Game and Delete MOD" : ((BurikoMemory.Instance.GetGlobalFlag("GMOD_SETTING_LOADER").IntValue() != 3) ? "\nERROR" : "\nDisable")));
				string text2 = BurikoMemory.Instance.GetGlobalFlag("GVoiceVolume").IntValue().ToString();
				string text3;
				try
				{
					text3 = ((BurikoMemory.Instance.GetFlag("NVL_in_ADV").IntValue() != 1) ? "" : "You can not swap NVL-ADV now\n");
				}
				catch (Exception)
				{
					text3 = "";
				}
				string text4;
				try
				{
					text4 = ((BurikoMemory.Instance.GetFlag("DisableModHotkey").IntValue() != 1) ? "" : "You can not use Hotkey\n1,2,3,4,5,6,F2,F3 for avoid bug\n");
				}
				catch (Exception)
				{
					text4 = "";
				}
				string text5 = "";
				if (!gameSystem.CanSave)
				{
					text5 = "You can't save now\n";
				}
				string text6 = "";
				if (!gameSystem.CanInput)
				{
					text6 = "Game avoid any input now\n";
				}
				var videoOpeningValue = BurikoMemory.Instance.GetGlobalFlag("GVideoOpening").IntValue();
				var videoOpeningDescription = videoOpeningValue == 0 ? "Unset" : videoOpeningValue == 1 ? "Disabled" : videoOpeningValue == 2 ? "In-game" : videoOpeningValue == 3 ? "At launch + in-game" : "Unknown";
				var ADVTextboxValue = BurikoMemory.Instance.GetGlobalFlag("GADVTextbox").IntValue();
				string text7 = "[MOD SETTINGS]\nADV-MODE = " + array2[0] + $"\nADV Textbox = {ADVTextboxValue}" + "\nLip-Sync = " + array2[5] + "\nAlternative BGM = " + array2[1] + "\nAlternative BGM Flow = " + array6[2] + array5[2] + "\nAlternative SE = " + array2[2] + "\nAlternative SE Flow = " + array6[3] + array5[3] + "\nAlternative Voice = " + array2[3] + "\nAlternative Voice Priority = " + array2[4] + "\nVoice Matching Level = " + array6[0] + array5[0] + "\nEffect Level = " + array6[1] + array5[1] + "\nVoice Volume = " + text2 + $"\nOP movies = {videoOpeningDescription} ({videoOpeningValue})" + "\n\n[Restore Game Settings]" + text + "\n\n[Status]\n" + text4 + text3 + text5 + text6;
				GUI.TextArea(new Rect(0f, 0f, 320f, 1080f), text7, 900);
			}
			if (BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() == 2)
			{
				string text8 = "[Vanilla Hotkey]\nEnter,Return,RightArrow,PageDown : Advance Text\nLeftArrow,Pageup : See Backlog\nESC : Open Menu\nCtrl : Hold Skip Mode\nA : Auto Mode\nS : Toggle Skip Mode\nF : FullScreen\nSpace : Hide Text\nL : Swap Language\nP : Swap Sprites\n\n[MOD Hotkey]\nF1 : ADV-NVL MODE\nF2 : Voice Matching Level\nF3 : Effect Level\nF5 : QuickSave\nF7 : QuickLoad\nF10 : Setting Monitor\nT : ADV Textbox Style\nM : Increase Voice Volume\nN : Decrease Voice Volume\n1 : Alternative BGM\n2 : Alternative BGM Flow\n3 : Alternative SE\n4 : Alternative SE Flow\n5 : Alternative Voice\n6 : Alternative Voice Priority\n7 : Lip-Sync\nLShift + F9 : Restore Settings\nLShift + M : Voice Volume MAX\nLShift + N : Voice Volume MIN";
				GUI.TextArea(new Rect(320f, 0f, 320f, 1080f), text8, 900);
			}
			if (BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() >= 3)
			{
				string[] array7 = new string[]
				{
					"GADVMode",
					"GLinemodeSp",
					"GCensor",
					"GEffectExtend",
					"GAltBGM",
					"GAltSE",
					"GAltBGMflow",
					"GAltSEflow",
					"GAltVoice",
					"GAltVoicePriority",
					"GCensorMaxNum",
					"GEffectExtendMaxNum",
					"GAltBGMflowMaxNum",
					"GAltSEflowMaxNum",
					"GMOD_SETTING_LOADER",
					"GFlagForTest1",
					"GFlagForTest2",
					"GFlagForTest3",
					"GMOD_DEBUG_MODE",
					"GLipSync",
					"GVideoOpening",
					"GADVTextbox"
				};
				string[] array8 = new string[array7.Length];
				for (int l = 0; l < array7.Length; l++)
				{
					array8[l] = BurikoMemory.Instance.GetGlobalFlag(array7[l]).IntValue().ToString();
				}
				string[] array9 = new string[array7.Length];
				string str = "[MOD Global Flags]\n";
				for (int m = 0; m < array7.Length; m++)
				{
					array9[m] = array7[m] + " = " + array8[m] + "\n";
					str += array9[m];
				}
				str += "\n[MOD Local Flags]\n";
				string[] array10 = new string[]
				{
					"NVL_in_ADV",
					"DisableModHotkey",
					"LFlagMonitor"
				};
				string[] array11 = new string[array10.Length];
				for (int n = 0; n < array10.Length; n++)
				{
					try
					{
						array11[n] = BurikoMemory.Instance.GetFlag(array10[n]).IntValue().ToString();
					}
					catch (Exception)
					{
						array11[n] = "disable";
					}
				}
				string[] array12 = new string[array10.Length];
				for (int num = 0; num < array10.Length; num++)
				{
					array12[num] = array10[num] + " = " + array11[num] + "\n";
					str += array12[num];
				}
				string text9 = "\n\n[GameStatus]";
				string text10 = "";
				text10 = ((!gameSystem.CanInput) ? "false" : "true");
				string text11 = "";
				text11 = ((!gameSystem.CanSave) ? "false" : "true");
				text9 = text9 + "\nCanInput = " + text10 + "\nCanSave = " + text11;
				str += text9;
				GUI.TextArea(new Rect(0f, 0f, 320f, 1080f), str, 900);
			}
			if (BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() >= 4)
			{
				string[] array13 = new string[33]
				{
					"GFlag_FirstPlay",
					"GFlag_GameClear",
					"GQsaveNum",
					"GOnikakushiDay",
					"GMessageSpeed",
					"GAutoSpeed",
					"GAutoAdvSpeed",
					"GUsePrompts",
					"GSlowSkip",
					"GSkipUnread",
					"GClickDuringAuto",
					"GRightClickMenu",
					"GWindowOpacity",
					"GVoiceVolume",
					"GBGMVolume",
					"GSEVolume",
					"GCutVoiceOnClick",
					"GUseSystemSound",
					"GLanguage",
					"GVChie",
					"GVEiji",
					"GVKana",
					"GVKira",
					"GVMast",
					"GVMura",
					"GVRiho",
					"GVRmn_",
					"GVSari",
					"GVTika",
					"GVYayo",
					"GVOther",
					"GArtStyle",
					"GHideButtons"
				};
				string[] array14 = new string[array13.Length];
				for (int num2 = 0; num2 < array13.Length; num2++)
				{
					array14[num2] = BurikoMemory.Instance.GetGlobalFlag(array13[num2]).IntValue().ToString();
				}
				string[] array15 = new string[array13.Length];
				string str2 = "[Vanilla Global Flags]\n";
				for (int num3 = 0; num3 < array13.Length; num3++)
				{
					array15[num3] = array13[num3] + " = " + array14[num3] + "\n";
					str2 += array15[num3];
				}
				str2 += "\n[Vanilla Local Flags]\n";
				string[] array16 = new string[5]
				{
					"LOCALWORK_NO_RESULT",
					"TipsMode",
					"ChapterNumber",
					"LOnikakushiDay",
					"LTextFade"
				};
				string[] array17 = new string[array16.Length];
				for (int num4 = 0; num4 < array16.Length; num4++)
				{
					try
					{
						array17[num4] = BurikoMemory.Instance.GetFlag(array16[num4]).IntValue().ToString();
					}
					catch (Exception)
					{
						array17[num4] = "disable";
					}
				}
				string[] array18 = new string[array16.Length];
				for (int num5 = 0; num5 < array16.Length; num5++)
				{
					array18[num5] = array16[num5] + " = " + array17[num5] + "\n";
					str2 += array18[num5];
				}
				GUI.TextArea(new Rect(320f, 0f, 320f, 1080f), str2, 900);
			}
		}

		public void MODDebugFontSizeChanger()
		{
			new MODMainUIController().DebugFontChangerSettingStore();
		}
	}
}
