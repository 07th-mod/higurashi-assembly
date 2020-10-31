using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.Scene;
using MOD.Scripts.UI;
using System;
using System.Collections;
using System.Linq;
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

		// While this timer is > 0, the current toast will be displayed (in seconds)
		private float toastNotificationTimer;
		private string toastText = "Example Toast Notification";
		private GUIStyle labelStyle;

		public enum Sound
		{
			Click,
			LoudBang,
			Disable,
			Enable,
			Pluck0,
			Pluck1,
			Pluck2,
			Pluck3,
			Pluck4,
			Pluck5,
		}

		private enum WindowFilterType
		{
			Normal,
			ADV,
			NVLInADV,
		}

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
					bgLayer.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, time, /*isBlocking:*/ false);
					if (bgLayer2 == null)
					{
						bgLayer2 = LayerPool.ActivateLayer();
					}
					bgLayer2.gameObject.layer = LayerMask.NameToLayer("Scene2");
					bgLayer2.SetPriority(62);
					bgLayer2.name = "Window Background 2";
					bgLayer2.IsStatic = true;
					bgLayer2.DrawLayer("windo_filter_adv", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, time, /*isBlocking:*/ false);
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

			// Update the toast countdown timer, making sure it doesn't go below 0
			toastNotificationTimer = Math.Max(0, toastNotificationTimer - Time.deltaTime);

			// Handle mod keyboard shortcuts
			ModInputHandler();

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

		private void TryRedrawTextWindowBackground(WindowFilterType filterType)
		{
			MainUIController ui = GameSystem.Instance.MainUIController;

			// If this function is called from the main menu, the bgLayers might be null
			if (ui.bgLayer == null || ui.bgLayer2 == null)
			{
				return;
			}

			string windowFilterTextureName = "windo_filter";
			if (filterType == WindowFilterType.ADV)
			{
				windowFilterTextureName = "windo_filter_adv";
			}
			else if (filterType == WindowFilterType.NVLInADV)
			{
				windowFilterTextureName = "windo_filter_nvladv";
			}

			ui.bgLayer.ReleaseTextures();
			ui.bgLayer2.ReleaseTextures();
			ui.bgLayer.DrawLayer(windowFilterTextureName, 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
			ui.bgLayer2.DrawLayer(windowFilterTextureName, 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
		}

		/// <summary>
		/// Toggles, then saves ADV/NVL mode.
		/// </summary>
		public void MODToggleAndSaveADVMode()
		{
			bool isADVMode = BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1;
			MODSetAndSaveADV(setADVMode: !isADVMode);
		}

		/// <summary>
		/// Sets and saves NVL/ADV mode
		/// </summary>
		/// <param name="setADVMode">If True, sets and saves ADV mode. If False, sets and saves NVL mode</param>
		public void MODSetAndSaveADV(bool setADVMode)
		{
			MODMainUIController mODMainUIController = new MODMainUIController();
			if (setADVMode)
			{
				BurikoMemory.Instance.SetGlobalFlag("GADVMode", 1);
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 0);
				TryRedrawTextWindowBackground(WindowFilterType.ADV);
				mODMainUIController.ADVModeSettingStore();
				GameSystem.Instance.MainUIController.ShowToast($"Textbox: ADV Mode", isEnable: true);
			}
			else
			{
				BurikoMemory.Instance.SetGlobalFlag("GADVMode", 0);
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 2);
				TryRedrawTextWindowBackground(WindowFilterType.Normal);
				mODMainUIController.NVLModeSettingStore();
				GameSystem.Instance.MainUIController.ShowToast($"Textbox: NVL Mode", isEnable: false);
			}
		}

		public void MODenableNVLModeINADVMode()
		{
			BurikoMemory.Instance.SetFlag("NVL_in_ADV", 1);
			if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1)
			{
				MODMainUIController mODMainUIController = new MODMainUIController();
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 2);
				TryRedrawTextWindowBackground(WindowFilterType.NVLInADV);
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
				TryRedrawTextWindowBackground(WindowFilterType.ADV);
				mODMainUIController.ADVModeSettingStore();
			}
		}

		// TODO: An empty OnGUI costs .03ms per frame and produces a little garbage, even if empty/not doing anything
		// https://forum.unity.com/threads/gui-that-hidden-bastard.257383/
		// https://answers.unity.com/questions/259870/performance-of-an-empty-ongui-fixedupdate.html
		// Consider moving this to its own class, then disabling it if there is nothing to be drawn.
		public void OnGUI()
		{
			// This sets up the style of the toast notification (mostly to make the font bigger and the text anchor location)
			// From what I've read, The GUIStyle must be initialized in OnGUI(), otherwise
			// GUI.skin.label will not be defined, so please do not move this part elsewhere without testing it.
			if(labelStyle == null)
			{
				labelStyle = new GUIStyle(GUI.skin.box) //Copy the default style for 'box' as a base
				{
					alignment = TextAnchor.UpperCenter,
					fontSize = 40,
					fontStyle = FontStyle.Bold,
				};
				int width = 1;
				int height = 1;
				Color[] pix = new Color[width * height];
				for(int i = 0; i < pix.Length; i++)
				{
					pix[i] = new Color(0.0f, 0.0f, 0.0f, 0.8f);
				}
				Texture2D result = new Texture2D(width, height);
				result.SetPixels(pix);
				result.Apply();
				labelStyle.normal.background = result;

				labelStyle.normal.textColor = Color.white;
			}

			// Helper Functions for processing flags
			string boolDesc(string flag, string name)
			{
				switch (BurikoMemory.Instance.GetGlobalFlag(flag).IntValue())
				{
				case 0:
					return name + " = OFF";
				case 1:
					return name + " = ON";
				default:
					return name + " = ERROR";
				}
			}
			string intDesc(string flag, string maxFlag, string name)
			{
				int max = BurikoMemory.Instance.GetGlobalFlag(maxFlag).IntValue();
				int val = BurikoMemory.Instance.GetGlobalFlag(flag).IntValue();
				return val > max ? name + " = ERROR" : name + " = " + val;
			}
			BurikoVariable getOptionalLocalFlag(string flag)
			{
				try
				{
					return BurikoMemory.Instance.GetFlag(flag);
				}
				catch
				{
					return null;
				}
			}


			if (BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 2)
			{
				gameSystem.CanSkip = true;
				gameSystem.CanInput = true;
				gameSystem.ShowUIControls();
			}
			if ((BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() == 1) || (BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() == 2))
			{
				string settingLoaderDesc;
				switch (BurikoMemory.Instance.GetGlobalFlag("GMOD_SETTING_LOADER").IntValue())
				{
				case 0:
					settingLoaderDesc = "Restore ADV-MODE Settings\nRelaunch Game 2 Times";
					break;
				case 1:
					settingLoaderDesc = "Restore NVL-MODE Settings\nRelaunch Game 2 Times";
					break;
				case 2:
					settingLoaderDesc = "Restore Vanilla Settings\nRelaunch Game and Delete MOD";
					break;
				case 3:
					settingLoaderDesc = "Disable";
					break;
				default:
					settingLoaderDesc = "ERROR";
					break;
				}
				string nvlAdvDesc = getOptionalLocalFlag("NVL_in_ADV")?.IntValue() != 1 ? "" : "You can not swap NVL-ADV now\n";
				string hotkeyDesc = getOptionalLocalFlag("DisableModHotkey")?.IntValue() != 1 ? "" : "You can not use Hotkey\n1,2,3,4,5,6,F2,F3 for avoid bug\n";
				string canSaveDesc = gameSystem.CanSave ? "" : "You can't save now\n";
				string canInputDesc = gameSystem.CanInput ? "" : "Game avoid any input now\n";
				var videoOpeningValue = BurikoMemory.Instance.GetGlobalFlag("GVideoOpening").IntValue();
				var artsetDescription = "Art = " + GameSystem.Instance.ChooseJapaneseEnglish(
					japanese: Core.AssetManagement.AssetManager.Instance.CurrentArtset.nameJP,
					english: Core.AssetManagement.AssetManager.Instance.CurrentArtset.nameEN
				);
				string textToDraw = string.Join("\n", new string[] {
					"[MOD SETTINGS]",
					boolDesc("GADVMode",                             "ADV-MODE"),
					boolDesc("GLipSync",                             "Lip-Sync"),
					boolDesc("GAltBGM",                              "Alternative BGM"),
					intDesc ("GAltBGMflow",   "GAltBGMflowMaxNum",   "Alternative BGM Flow"),
					boolDesc("GAltSE",                               "Alternative SE"),
					intDesc ("GAltSEflow",    "GAltSEflowMaxNum",    "Alternative SE Flow"),
					boolDesc("GAltVoice",                            "Alternative Voice"),
					boolDesc("GAltVoicePriority",                    "Alternative Voice Priority"),
					intDesc ("GCensor",       "GCensorMaxNum",       "Voice Matching Level"),
					intDesc ("GEffectExtend", "GEffectExtendMaxNum", "Effect Level"),
					"Voice Volume = " + BurikoMemory.Instance.GetGlobalFlag("GVoiceVolume").IntValue().ToString(),
					$"OP Movies = {VideoOpeningDescription(videoOpeningValue)} ({videoOpeningValue})",
					artsetDescription,
					"\n[Restore Game Settings]",
					settingLoaderDesc,
					"\n[Status]",
					hotkeyDesc + nvlAdvDesc + canSaveDesc + canInputDesc
				});
				GUI.TextArea(new Rect(0f, 0f, 320f, 1080f), textToDraw, 900);
			}
			if (BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() == 2)
			{
				string textToDraw = string.Join("\n", new string[] {
					"[Vanilla Hotkey]",
					"Enter,Return,RightArrow,PageDown : Advance Text",
					"LeftArrow,Pageup : See Backlog",
					"ESC : Open Menu",
					"Ctrl : Hold Skip Mode",
					"A : Auto Mode",
					"S : Toggle Skip Mode",
					"F : FullScreen",
					"Space : Hide Text",
					"L : Swap Language",
					"P : Swap Sprites",
					"\n[MOD Hotkey]",
					"F1 : ADV-NVL MODE",
					"F2 : Voice Matching Level",
					"F3 : Effect Level (Not Used)",
					"F5 : QuickSave",
					"F7 : QuickLoad",
					"F10 : Setting Monitor",
					"F11 : OP Movies",
					"M : Increase Voice Volume",
					"N : Decrease Voice Volume",
					"1 : Alternative BGM (Not Used)",
					"2 : Alternative BGM Flow (Not Used)",
					"3 : Alternative SE (Not Used)",
					"4 : Alternative SE Flow (Not Used)",
					"5 : Alternative Voice (Not Used)",
					"6 : Alternative Voice Priority (Not Used)",
					"7 : Lip-Sync",
					"LShift + F9 : Restore Settings",
					"LShift + M : Voice Volume MAX",
					"LShift + N : Voice Volume MIN"
				});
				GUI.TextArea(new Rect(320f, 0f, 320f, 1080f), textToDraw, 900);
			}
			if (BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() >= 3)
			{
				string textToDraw = "[MOD Global Flags]\n";
				textToDraw += string.Join("\n",
					new string[]
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
						"GVideoOpening"
					}
					.Select(flag => flag + " = " + BurikoMemory.Instance.GetGlobalFlag(flag).IntValue().ToString())
					.ToArray()
				);
				textToDraw += "\n\n[MOD Local Flags]\n";
				textToDraw += string.Join("\n",
					new string[]
					{
						"NVL_in_ADV",
						"DisableModHotkey",
						"LFlagMonitor"
					}
					.Select(flag => flag + " = " + (getOptionalLocalFlag(flag)?.IntValue().ToString() ?? "disable"))
					.ToArray()
				);
				textToDraw += "\n\n[GameStatus]\n";
				textToDraw += string.Join("\n", new string[] {
					"CanInput = " + (gameSystem.CanInput ? "true" : "false"),
					"CanSave = " + (gameSystem.CanSave ? "true" : "false"),
				});
				GUI.TextArea(new Rect(0f, 0f, 320f, 1080f), textToDraw, 900);
			}
			if (BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() >= 4)
			{
				string textToDraw = "[Vanilla Global Flags]\n";
				textToDraw += string.Join("\n",
					new string[]
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
					}
					.Select(flag => flag + " = " + BurikoMemory.Instance.GetGlobalFlag(flag).IntValue().ToString())
					.ToArray()
				);
				textToDraw += "\n\n[Vanilla Local Flags]\n";
				textToDraw += string.Join("\n",
					new string[]
					{
						"LOCALWORK_NO_RESULT",
						"TipsMode",
						"ChapterNumber",
						"LOnikakushiDay",
						"LTextFade"
					}
					.Select(flag => flag + " = " + (getOptionalLocalFlag(flag)?.IntValue().ToString() ?? "disable"))
					.ToArray()
				);
				GUI.TextArea(new Rect(320f, 0f, 320f, 1080f), textToDraw, 900);
			}

			if(toastNotificationTimer > 0)
			{
				// This scrolls the toast notification off the window when it's nearly finished
				float toastYPosition = Math.Min(50f, 200f * toastNotificationTimer - 50f);
				float toastWidth = 700f;
				float toastXPosition = (Screen.width - toastWidth) / 2.0f;
				GUILayout.BeginArea(new Rect(toastXPosition, toastYPosition, 700f, 200f));
				GUILayout.Box(toastText, labelStyle);
				GUILayout.EndArea();
			}
		}

		public void MODDebugFontSizeChanger()
		{
			new MODMainUIController().DebugFontChangerSettingStore();
		}

		private string GetSoundPathFromEnum(Sound sound)
		{
			switch (sound)
			{
				case Sound.Click:
					return "wa_038.ogg";
				case Sound.LoudBang:
					return "wa_040.ogg";
				case Sound.Disable:
					return "switchsound/disable.ogg";
				case Sound.Enable:
					return "switchsound/enable.ogg";
				case Sound.Pluck0:
					return "switchsound/0.ogg";
				case Sound.Pluck1:
					return "switchsound/1.ogg";
				case Sound.Pluck2:
					return "switchsound/2.ogg";
				case Sound.Pluck3:
					return "switchsound/3.ogg";
				case Sound.Pluck4:
					return "switchsound/4.ogg";
				case Sound.Pluck5:
					return "switchsound/5.ogg";
			}

			return "wa_038.ogg";
		}

		/// <summary>
		/// Displays a toast notification. It will appear ontop of everything else on the screen.
		/// </summary>
		/// <param name="toastText">The text to display in the toast</param>
		/// <param name="toastDuration">The duration the toast will be shown for.
		/// The toast will slide off the screen for the last part of this duration.</param>
		public void ShowToast(string toastText, Sound? maybeSound = Sound.Click, float toastDuration = 3)
		{
			this.toastText = toastText;
			this.toastNotificationTimer = toastDuration;
			if (maybeSound is Sound sound)
			{
				GameSystem.Instance.AudioController.PlaySystemSound(GetSoundPathFromEnum(sound));
			}
		}

		public void ShowToast(string toastText, bool isEnable, float toastDuration = 3)
		{
			ShowToast(toastText, isEnable ? Sound.Enable : Sound.Disable, toastDuration);
		}

		public void ShowToast(string toastText, int numberedSound, float toastDuration = 3)
		{
			Sound sound = Sound.Click;
			switch (numberedSound)
			{
				case 0:
					sound = Sound.Pluck0;
					break;
				case 1:
					sound = Sound.Pluck1;
					break;
				case 2:
					sound = Sound.Pluck2;
					break;
				case 3:
					sound = Sound.Pluck3;
					break;
				case 4:
					sound = Sound.Pluck4;
					break;
				case 5:
					sound = Sound.Pluck5;
					break;
			}

			ShowToast(toastText, sound, toastDuration);
		}

		public static string VideoOpeningDescription(int videoOpeningValue)
		{
			switch(videoOpeningValue)
			{
				case 0:
					return "Unset";
				case 1:
					return "Disabled";
				case 2:
					return "In-game";
				case 3:
					return "At launch + in-game";
			}

			return "Unknown";
		}

		private void AdjustVoiceVolumeRelative(int difference)
		{
			// Maintaining volume within limits is done in AdjustVoiceVolumeAbsolute()
			AdjustVoiceVolumeAbsolute(BurikoMemory.Instance.GetGlobalFlag("GVoiceVolume").IntValue() + difference);
		}

		private void AdjustVoiceVolumeAbsolute(int uncheckedNewVolume)
		{
			int newVolume = Mathf.Clamp(uncheckedNewVolume, 0, 100);

			BurikoMemory.Instance.SetGlobalFlag("GVoiceVolume", newVolume);
			GameSystem.Instance.AudioController.VoiceVolume = (float)newVolume / 100f;
			GameSystem.Instance.AudioController.RefreshLayerVolumes();

			// Play a sample voice file so the user can get feedback on the set volume
			// For some reason the script uses "256" as the default volume, which gets divided by 128 to become 2.0f,
			// so to keep in line with the script, the test volume is set to "2.0f"
			GameSystem.Instance.AudioController.PlayVoice("voice_test.ogg", 3, 2.0f);
		}

		// Variant for global flags, using another variable as max limit
		private int IncrementGlobalFlagWithRollover(string flagName, string maxFlagName)
		{
			return _IncrementFlagWithRollover(flagName, 0, BurikoMemory.Instance.GetGlobalFlag(maxFlagName).IntValue(), isLocalFlag: false);
		}

		// Variant for global flags, using literal limits
		private int IncrementGlobalFlagWithRollover(string flagName, int minValueInclusive, int maxValueInclusive)
		{
			return _IncrementFlagWithRollover(flagName, minValueInclusive, maxValueInclusive, isLocalFlag: false);
		}

		// Variant for local flags
		private int IncrementLocalFlagWithRollover(string flagName, int minValueInclusive, int maxValueInclusive)
		{
			return _IncrementFlagWithRollover(flagName, minValueInclusive, maxValueInclusive, isLocalFlag: true);
		}

		/// <summary>
		/// Increment a flag with rollover (from GetGlobalFlag())
		/// If min/max set to (3,6), it will loop over the values 3,4,5,6
		/// </summary>
		/// <param name="flagName">the name of the global flag, eg. "GVoiceVolume"</param>
		/// <param name="minValueInclusive">This is the minvalue the flag can be allowed to have before it rolls over, inclusive.</param>
		/// <param name="maxValueInclusive">This is the max value the flag can be allowed to have before it rolls over, inclusive.</param>
		/// <returns></returns>
		private int _IncrementFlagWithRollover(string flagName, int minValueInclusive, int maxValueInclusive, bool isLocalFlag)
		{
			int initialValue = isLocalFlag ? BurikoMemory.Instance.GetFlag(flagName).IntValue() : BurikoMemory.Instance.GetGlobalFlag(flagName).IntValue();

			int newValue = initialValue + 1;
			if (newValue > maxValueInclusive)
			{
				newValue = minValueInclusive;
			}

			if (isLocalFlag)
			{
				BurikoMemory.Instance.SetFlag(flagName, newValue);
			}
			else
			{
				BurikoMemory.Instance.SetGlobalFlag(flagName, newValue);
			}

			return newValue;
		}

		private bool ToggleFlagAndSave(string flagName)
		{
			int newValue = (BurikoMemory.Instance.GetGlobalFlag(flagName).IntValue() + 1) % 2;
			BurikoMemory.Instance.SetGlobalFlag(flagName, newValue);

			return newValue == 1;
		}

		private bool ModInputHandlingAllowed()
		{
			if (!gameSystem.IsInitialized || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
			{
				return false;
			}

			// Don't allow mod options on any wait, except "WaitForInput"
			// Note: if this is removed, it mostly still works, but if changing art style during
			// an animation, some things may bug out until the next scene.
			foreach (Wait w in gameSystem.WaitList)
			{
				if (w.Type != WaitTypes.WaitForInput)
				{
					return false;
				}
			}

			return true;
		}

		enum Action
		{
			ToggleADV,
			CensorshipLevel,
			EffectLevel,
			FlagMonitor,
			OpeningVideo,
			RyukishiMode,
			DebugFontSize,
			AltBGM,
			AltBGMFlow,
			AltSE,
			AltSEFlow,
			AltVoice,
			AltVoicePriority,
			LipSync,
			VoiceVolumeUp,
			VoiceVolumeDown,
			VoiceVolumeMax,
			VoiceVolumeMin,
			ToggleArtStyle,
			DebugMode,
			RestoreSettings,
		}

		private Action? GetUserAction()
		{
			// These take priority over the non-shift key buttons
			if (Input.GetKey(KeyCode.LeftShift))
			{
				if (Input.GetKeyDown(KeyCode.F10))
				{
					return Action.DebugMode;
				}
				else if (Input.GetKeyDown(KeyCode.F9))
				{
					return Action.RestoreSettings;
				}
				else if (Input.GetKeyDown(KeyCode.M))
				{
					return Action.VoiceVolumeMax;
				}
				else if (Input.GetKeyDown(KeyCode.N))
				{
					return Action.VoiceVolumeMin;
				}
			}

			if (Input.GetKeyDown(KeyCode.F1))
			{
				return Action.ToggleADV;
			}
			else if (Input.GetKeyDown(KeyCode.F2))
			{
				return Action.CensorshipLevel;
			}
			else if (Input.GetKeyDown(KeyCode.F3))
			{
				return Action.EffectLevel;
			}
			else if (Input.GetKeyDown(KeyCode.F10))
			{
				return Action.FlagMonitor;
			}
			else if (Input.GetKeyDown(KeyCode.F11))
			{
				return Action.OpeningVideo;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
			{
				return Action.DebugFontSize;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
			{
				return Action.AltBGM;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
			{
				return Action.AltBGMFlow;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
			{
				return Action.AltSE;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
			{
				return Action.AltSEFlow;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
			{
				return Action.AltVoice;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
			{
				return Action.AltVoicePriority;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
			{
				return Action.LipSync;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
			{
				return Action.RyukishiMode;
			}
			else if (Input.GetKeyDown(KeyCode.M))
			{
				return Action.VoiceVolumeUp;
			}
			else if (Input.GetKeyDown(KeyCode.P))
			{
				return Action.ToggleArtStyle;
			}
			else if (Input.GetKeyDown(KeyCode.N))
			{
				return Action.VoiceVolumeDown;
			}

			return null;
		}

		private void ModHandleUserAction(Action action)
		{
			switch (action)
			{
				case Action.ToggleADV:
					if (BurikoMemory.Instance.GetFlag("NVL_in_ADV").IntValue() == 1)
					{
						GameSystem.Instance.MainUIController.ShowToast($"Can't toggle now - try later", maybeSound: null);
					}
					else
					{
						GameSystem.Instance.MainUIController.MODToggleAndSaveADVMode();
					}
					break;

				case Action.CensorshipLevel:
					{
						int newCensorNum = IncrementGlobalFlagWithRollover("GCensor", "GCensorMaxNum");
						GameSystem.Instance.MainUIController.ShowToast(
							$"Censorship Level: {newCensorNum}{(newCensorNum == 2 ? " (default)" : "")}",
							numberedSound: newCensorNum
						);
					}
					break;

				case Action.EffectLevel:
					{
						int effectLevel = IncrementGlobalFlagWithRollover("GEffectExtend", "GEffectExtendMaxNum");
						GameSystem.Instance.MainUIController.ShowToast($"Effect Level: {effectLevel} (Not Used)", numberedSound: effectLevel);
					}
					break;

				case Action.FlagMonitor:
					IncrementLocalFlagWithRollover("LFlagMonitor", 0, BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 0 ? 2 : 4);
					break;

				case Action.OpeningVideo:
					{
						// Loop "GVideoOpening" over the values 1-3.
						// 0 is skipped as it represents "value not set"
						int newVideoOpening = IncrementGlobalFlagWithRollover("GVideoOpening", 1, 3);
						GameSystem.Instance.MainUIController.ShowToast($"OP Video: {VideoOpeningDescription(newVideoOpening)} ({newVideoOpening})");
					}
					break;

				case Action.RyukishiMode:
					{
						// TODO: need to consider how these settings will be restored on startup
						// as may interfere with other settings!
						// Instead of just one setting, might be better to have multiple flags toggled when you press this one button.
						if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 0)
						{
							////// Switch to Console / 16:9 Mode

							// Disable clamping sprites to 4:3
							BurikoMemory.Instance.SetGlobalFlag("GClampSprite43", 0);

							// Enable CGs
							BurikoMemory.Instance.SetGlobalFlag("GHideCG", 0);

							//TODO: Change game aspect ratio to 16:9
							//gameSystem.UpdateAspectRatio(16.0f / 9.0f);

							//TODO: Restore UI for 16:9, Save setting (see note about GUI position in 'else' statement below)
							gameSystem.MainUIController.UpdateGuiPosition(170, 0);

							//TODO: Restore textbox size for 16:9
							MODMainUIController mODMainUIController = new MODMainUIController();
							mODMainUIController.NVLModeSettingLoad(
								"",   //name
								-170,    //posx
								-10,  //posy
								1240, //sizex
								720,  //sizey
								60,   //mleft
								30,   //mtop
								50,   //mright
								30,   //mbottom
								1,    //font
								0,    //cspace
								8,    //lspace
								34);  //fsize

							// Set ADV mode (take settings from init file), Save setting
							MODSetAndSaveADV(setADVMode: true);

							//TODO: Optional - disable image stretching 16:9
							GameSystem.Instance.MainUIController.ShowToast($"Enabled Console Mode");
						}
						else
						{
							////// Switch to Ryukishi / 4:3 Mode

							// Enable clamping sprites to 4:3
							BurikoMemory.Instance.SetGlobalFlag("GClampSprite43", 1);

							// Disable CGs (displayed CGs would be cut off). Can press Shift-9 to forcibly show CGs
							BurikoMemory.Instance.SetGlobalFlag("GHideCG", 1);

							//TODO: Force NVL mode for 4:3 (may need to add another option in the init.txt file), save setting
							MODSetAndSaveADV(setADVMode: false);

							//TODO: Shift UI for 4:3, save setting
							// NOTE: The textbox location seems tied to the Gui position (as in, the text/textbox is parented to the GUI)
							// This means that when we change the Gui position from (170, 0) to (0, 0), you don't need the -170 offset that we use in our mod
							gameSystem.MainUIController.UpdateGuiPosition(0, 0);

							//TODO: Squish textbox
							MODMainUIController mODMainUIController = new MODMainUIController();
							mODMainUIController.NVLModeSettingLoad(
								"",   //name
								0,    //posx
								-10,  //posy
								1024, //sizex
								768,  //sizey
								60,   //mleft
								30,   //mtop
								50,   //mright
								30,   //mbottom
								1,    //font
								0,    //cspace
								8,    //lspace
								34);  //fsize

							// Change game aspect ration to 4:3
							//gameSystem.UpdateAspectRatio(4.0f / 3.0f);

							//TODO: Optional - stretch backgrounds to 16:9 if wrong resolution? entirely optional though, maybe do later, Save setting
							GameSystem.Instance.MainUIController.ShowToast($"Enabled Ryukishi Mode");
						}
					}
					break;

				case Action.DebugFontSize when BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 1 || BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 2:
					GameSystem.Instance.MainUIController.MODDebugFontSizeChanger();
					break;

				case Action.AltBGM:
					{
						bool altBGMEnabled = ToggleFlagAndSave("GAltBGM");
						GameSystem.Instance.MainUIController.ShowToast($"Alt BGM: {(altBGMEnabled ? "ON" : "OFF")} (Not Used)", isEnable: altBGMEnabled);
					}
					break;

				case Action.AltBGMFlow:
					{
						int newAltBGMFlow = IncrementGlobalFlagWithRollover("GAltBGMflow", "GAltBGMflowMaxNum");
						GameSystem.Instance.MainUIController.ShowToast($"Alt BGM Flow: {newAltBGMFlow} (Not Used)", numberedSound: newAltBGMFlow);
					}
					break;

				case Action.AltSE:
					{
						bool seIsEnabled = ToggleFlagAndSave("GAltSE");
						GameSystem.Instance.MainUIController.ShowToast($"Alt SE: {(seIsEnabled ? "ON" : "OFF")} (Not Used)", isEnable: seIsEnabled);
					}
					break;

				case Action.AltSEFlow:
					{
						int newAltSEFlow = IncrementGlobalFlagWithRollover("GAltSEflow", "GAltSEflowMaxNum");
						GameSystem.Instance.MainUIController.ShowToast($"Alt SE Flow: {newAltSEFlow} (Not Used)", numberedSound: newAltSEFlow);
					}
					break;

				case Action.AltVoice:
					{
						bool altVoiceIsEnabled = ToggleFlagAndSave("GAltVoice");
						GameSystem.Instance.MainUIController.ShowToast($"Alt Voice: {(altVoiceIsEnabled ? "ON" : "OFF")} (Not Used)", isEnable: altVoiceIsEnabled);
					}
					break;

				case Action.AltVoicePriority:
					{
						bool altVoicePriorityIsEnabled = ToggleFlagAndSave("GAltVoicePriority");
						GameSystem.Instance.MainUIController.ShowToast($"Alt Priority: {(altVoicePriorityIsEnabled ? "ON" : "OFF")} (Not Used)", isEnable: altVoicePriorityIsEnabled);
					}
					break;

				case Action.LipSync:
					{
						bool lipSyncIsEnabled = ToggleFlagAndSave("GLipSync");
						GameSystem.Instance.MainUIController.ShowToast($"Lip Sync: {(lipSyncIsEnabled ? "ON" : "OFF")}", isEnable: lipSyncIsEnabled);
					}
					break;

				case Action.VoiceVolumeUp:
					AdjustVoiceVolumeRelative(5);
					break;

				case Action.VoiceVolumeDown:
					AdjustVoiceVolumeRelative(-5);
					break;

				case Action.VoiceVolumeMax:
					AdjustVoiceVolumeAbsolute(100);
					break;

				case Action.VoiceVolumeMin:
					AdjustVoiceVolumeAbsolute(0);
					break;

				case Action.ToggleArtStyle:
					MOD.Scripts.Core.MODSystem.instance.modTextureController.ToggleArtStyle();
					break;

				// Enable debug mode, which shows an extra 2 panels of info in the flag menu
				// Note that if your debug mode is 0, there is no way to enable debug mode unless you manually set the flag value in the game script
				case Action.DebugMode when BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() != 0:
					{
						int debugMode = IncrementGlobalFlagWithRollover("GMOD_DEBUG_MODE", 1, 2);
						GameSystem.Instance.MainUIController.ShowToast($"Debug Mode: {debugMode}", numberedSound: debugMode);
					}
					break;

				// Restore game settings
				case Action.RestoreSettings:
					{
						int restoreGameSettingsNum = IncrementGlobalFlagWithRollover("GMOD_SETTING_LOADER", 0, 3);
						GameSystem.Instance.MainUIController.ShowToast($"Reset Settings: {restoreGameSettingsNum} (see F10 menu)", numberedSound: restoreGameSettingsNum);
					}
					break;

				default:
					Logger.Log($"Warning: Unknown mod action {action} was requested to be executed");
					break;
			}
		}

		/// <summary>
		/// Handles mod inputs. Call from Update function.
		/// </summary>
		/// <returns>Currently the return value is not used for anything</returns>
		public bool ModInputHandler()
		{
			if(GetUserAction() is Action action)
			{
				if (!ModInputHandlingAllowed())
				{
					GameSystem.Instance.MainUIController.ShowToast($"Please let animation finish first");
				}
				else
				{
					ModHandleUserAction(action);
				}
			}

			return true;
		}

	}
}
