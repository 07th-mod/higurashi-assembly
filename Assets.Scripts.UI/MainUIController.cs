using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.Scene;
using MOD.Scripts.Core.State;
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
		public MODMenu modMenu;
		private MODToaster toaster;

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
					bool isRyukishiMode = BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode").IntValue() == 1;
					if (bgLayer == null)
					{
						bgLayer = LayerPool.ActivateLayer();
					}
					bgLayer.gameObject.layer = LayerMask.NameToLayer("Scene1");
					bgLayer.SetPriority(62);
					bgLayer.name = "Window Background 1";
					bgLayer.IsStatic = true;
					bgLayer.DrawLayer(isRyukishiMode ? "windo_filter_nvladv" : "windo_filter", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, time, /*isBlocking:*/ false);
					if (bgLayer2 == null)
					{
						bgLayer2 = LayerPool.ActivateLayer();
					}
					bgLayer2.gameObject.layer = LayerMask.NameToLayer("Scene2");
					bgLayer2.SetPriority(62);
					bgLayer2.name = "Window Background 2";
					bgLayer2.IsStatic = true;
					bgLayer2.DrawLayer(isRyukishiMode ? "windo_filter_nvladv" : "windo_filter", 0, 0, 0, null, gameSystem.MessageWindowOpacity, /*isBustshot:*/ false, 0, time, /*isBlocking:*/ false);
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

			if (toaster != null)
			{
				toaster.Update();
			}

			if (modMenu != null)
			{
				modMenu.Update();
			}

			// Handle mod keyboard shortcuts
			MODKeyboardShortcuts.ModInputHandler();

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

		public void LateUpdate()
		{
			if (modMenu != null)
			{
				modMenu.LateUpdate();
			}
		}

		public void TryRedrawTextWindowBackground(string windowFilterTextureName)
		{
			MainUIController ui = GameSystem.Instance.MainUIController;

			// If this function is called from the main menu, the bgLayers might be null
			if (ui.bgLayer == null || ui.bgLayer2 == null)
			{
				return;
			}

			ui.bgLayer.ReleaseTextures();
			ui.bgLayer2.ReleaseTextures();
			ui.bgLayer.DrawLayer(windowFilterTextureName, 0, 0, 0, null, GameSystem.Instance.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
			ui.bgLayer2.DrawLayer(windowFilterTextureName, 0, 0, 0, null, GameSystem.Instance.MessageWindowOpacity, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
		}

		// TODO: An empty OnGUI costs .03ms per frame and produces a little garbage, even if empty/not doing anything
		// https://forum.unity.com/threads/gui-that-hidden-bastard.257383/
		// https://answers.unity.com/questions/259870/performance-of-an-empty-ongui-fixedupdate.html
		// Consider moving this to its own class, then disabling it if there is nothing to be drawn.
		// NOTE: this function can be called before Update() if CTRL (skip) down during game startup
		public void OnGUI()
		{
			if(BurikoSaveManager.lastSaveError != null)
			{
				MODMenuSupport.EmergencyModMenu("Error loading save file! Please backup your saves, DISABLE STEAM SYNC, then delete the following save file:", BurikoSaveManager.lastSaveError);
				return;
			}

			// This can happen if you hold CTRL (skip) during game startup, presumably because OnGUI() gets called before the first Update() call
			if(this.gameSystem == null)
			{
				return;
			}

			if (this.toaster == null)
			{
				this.toaster = new MODToaster();
			}

			if (this.modMenu == null)
			{
				this.modMenu = new MODMenu(this.gameSystem);
			}

			modMenu.OnGUIFragment();
			toaster.OnGUIFragment();

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
					"[MOD SETTINGS] (Press F10 to toggle)",
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
					$"OP Movies = {MODActions.VideoOpeningDescription(videoOpeningValue)} ({videoOpeningValue})",
					artsetDescription,
					"\n[Restore Game Settings]",
					settingLoaderDesc,
					"\n[Status]",
					hotkeyDesc + nvlAdvDesc + canSaveDesc + canInputDesc
				});
				GUIUnclickableTextArea(new Rect(0f, 0f, 320f, 1080f), textToDraw);
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
				GUIUnclickableTextArea(new Rect(320f, 0f, 320f, 1080f), textToDraw);
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
				GUIUnclickableTextArea(new Rect(0f, 0f, 320f, 1080f), textToDraw);
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
				GUIUnclickableTextArea(new Rect(320f, 0f, 320f, 1080f), textToDraw);
			}

		}

		void OnApplicationQuit()
		{
			this.modMenu.UserHide();
		}

		/// <summary>
		/// This looks the same as a TextArea, but you can't click on it
		/// WARNING: Only call this function from OnGUI()
		/// </summary>
		private static void GUIUnclickableTextArea(Rect rect, string text)
		{
			GUI.Label(rect, text, GUI.skin.textArea);
		}

	}
}
