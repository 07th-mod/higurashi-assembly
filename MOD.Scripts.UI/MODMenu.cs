using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using MOD.Scripts.Core.Audio;
using MOD.Scripts.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	public enum ModMenuMode
	{
		Normal,
		AudioSetup,
	}

	public class MODMenu
	{
		private readonly MODStyleManager styleManager;
		private readonly MODMenuCommon c;
		private readonly GameSystem gameSystem;
		public bool visible;
		private bool showDebugInfo;
		private bool lastMenuVisibleStatus;
		private MODSimpleTimer defaultToolTipTimer;
		private MODSimpleTimer startupWatchdogTimer;
		private bool startupFailed;
		private bool anyButtonPressed;
		Vector2 scrollPosition;
		Vector2 leftDebugColumnScrollPosition;

		private ModMenuMode menuMode;

		private MODMenuNormal normalMenu;

		string lastToolTip = String.Empty;
		string defaultTooltip = @"Hover over a button on the left panel for its description.

[Vanilla Hotkeys]
Enter,Return,RightArrow,PageDown : Advance Text
LeftArrow,Pageup : See Backlog
ESC : Open Menu
Ctrl : Hold Skip Mode
A : Auto Mode
S : Toggle Skip Mode
F, Alt-Enter : FullScreen
Space : Hide Text
L : Swap Language

[MOD Hotkeys]
F1 : ADV-NVL MODE
F2 : Voice Matching Level
F3 : Effect Level (Not Used)
F5 : QuickSave
F7 : QuickLoad
F10 : Mod Menu
M : Increase Voice Volume
N : Decrease Voice Volume
P : Cycle through art styles
2 : Cycle through BGM/SE
7 : Enable/Disable Lip-Sync
LShift + M : Voice Volume MAX
LShift + N : Voice Volume MIN";

		string startupFailureToolTip = @"It looks like there was a problem starting up

Please send the developers your log file (output_log.txt or Player.log).

You can try the following yourself to fix the issue.

  1. Use the buttons under 'Troubleshooting' on the bottom left to show your save files, log files, and compiled scripts.

  2. If the log indicates you have corrupt save files, you may wish to delete the offending save file (or all of them).

  3. You can try to clear your compiled script files, then restart the game.

  4. If the above do not fix the problem, please click the 'Open Support Page' button, which has extra troubleshooting info and links to join our Discord server for direct support.";

		public MODMenu(GameSystem gameSystem, MODStyleManager styleManager)
		{
			this.gameSystem = gameSystem;
			this.styleManager = styleManager;
			this.visible = false;
			this.showDebugInfo = false;
			this.lastMenuVisibleStatus = false;
			this.defaultToolTipTimer = new MODSimpleTimer();
			this.startupWatchdogTimer = new MODSimpleTimer();
			this.startupFailed = false;
			this.c = new MODMenuCommon(styleManager);

			// Start the watchdog timer as soon as possible, so it starts from "when the game started"
			this.startupWatchdogTimer.Start(5.0f);

			this.normalMenu = new MODMenuNormal(this, this.c, styleManager);
		}

		public void Update()
		{
			defaultToolTipTimer.Update();
			startupWatchdogTimer.Update();
		}

		public void LateUpdate()
		{
			if (Input.GetMouseButtonDown(1))
			{
				this.Hide();
			}
		}

		/// <summary>
		/// Must be called from an OnGUI()
		/// </summary>
		public void OnGUIFragment()
		{
			if (showDebugInfo && AssetManager.Instance != null)
			{
				bool bgmFlagOK = MODAudioSet.Instance.GetBGMCascade(c.GetGlobal("GAltBGM"), out PathCascadeList BGMCascade);
				bool seFlagOK = MODAudioSet.Instance.GetSECascade(c.GetGlobal("GAltSE"), out PathCascadeList SECascade);

				GUILayout.BeginArea(new Rect(0, 0, Screen.width/3, Screen.height), styleManager.modMenuAreaStyleLight);
				leftDebugColumnScrollPosition = GUILayout.BeginScrollView(leftDebugColumnScrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height - 10));
				GUILayout.Label($"[Audio Tracking] - indicates what would play on each BGM flow", styleManager.Group.upperLeftHeadingLabel);
				GUILayout.Label($"{MODAudioTracking.Instance}", styleManager.Group.upperLeftHeadingLabel);

				GUILayout.Label($"[Audio Flags and last played audio]", styleManager.Group.upperLeftHeadingLabel);
				GUILayout.Label($"Audio Set: {c.GetGlobal("GAudioSet")} ({MODAudioSet.Instance.GetCurrentAudioSetName()})\n" +
					"\n" +
					$"AltBGM: {c.GetGlobal("GAltBGM")}\n" +
					$"AltBGMFlow: {c.GetGlobal("GAltBGMflow")} ({MODAudioSet.Instance.GetBGMFlowName(c.GetGlobal("GAltBGMflow"))})\n" +
					$"Last Played BGM: {AssetManager.Instance.debugLastBGM}\n" +
					$"BGM Cascade: [{string.Join(":", BGMCascade.paths)}] ({BGMCascade.nameEN}) {(bgmFlagOK ? "" : "9Warning: Using default due to unknown flag)")}\n" +
					"\n" +
					$"AltSE:  {c.GetGlobal("GAltSE")}\n" +
					$"AltSEFlow: {c.GetGlobal("GAltSEflow")}\n" +
					$"Last Played SE Path: {AssetManager.Instance.debugLastSE}\n" +
					$"SE Cascade: [{string.Join(":", SECascade.paths)}] ({SECascade.nameEN}) {(seFlagOK ? "" : "(Warning: Using default due to unknown flag)")}\n" +
					$"Voice: {c.GetGlobal("GAltVoice")}\n" +
					$"Priority: {c.GetGlobal("GAltVoicePriority")}\n" +
					"\n" +
					$"Last Played Voice Path: {AssetManager.Instance.debugLastVoice}\n" +
					$"Other Last Played Path: {AssetManager.Instance.debugLastOtherAudio}");
				GUILayout.EndScrollView();
				GUILayout.EndArea();
			}

			if (this.startupWatchdogTimer.Finished())
			{
				this.startupWatchdogTimer.Cancel();
				if (!BurikoScriptSystem.Instance.FlowWasReached)
				{
					this.startupFailed = true;
					this.Show();
				}
			}

			// Button to open the Mod Menu on the Config Screen
			if (gameSystem.GameState == GameState.ConfigScreen)
			{
				if (gameSystem.ConfigManager() != null)
				{
					// Temporarily override the global tint color to get a fade in effect which matches the config screen fade-in
					// This value is not saved by Unity (it resets to the default value each frame)
					GUI.color = new Color(1.0f, 1.0f, 1.0f, gameSystem.ConfigManager().PanelAlpha());
				}
				string text = "Mod Menu\n(Hotkey: F10)";
				float areaWidth = Screen.width / 8;
				float areaHeight = Mathf.Round(styleManager.Group.button.CalcHeight(new GUIContent(text, ""), areaWidth)) + 10;
				float xOffset = 0;
				float yOffset = Screen.height - areaHeight;
				GUILayout.BeginArea(new Rect(xOffset, yOffset, areaWidth, areaHeight), styleManager.modMenuAreaStyle);
				if (GUILayout.Button(text, styleManager.Group.button))
				{
					this.Show();
				}

				GUILayout.EndArea();
			}

			if (visible && !lastMenuVisibleStatus)
			{
				// Executes just before menu becomes visible
				normalMenu.OnBeforeMenuVisible();
			}
			lastMenuVisibleStatus = visible;


			if (visible)
			{
				float totalAreaWidth = styleManager.Group.menuWidth; // areaWidth + toolTipWidth;

				float areaWidth = Mathf.Round(totalAreaWidth * 9/16);
				float toolTipWidth = Mathf.Round(totalAreaWidth * 7/16);

				float areaHeight = styleManager.Group.menuHeight;

				float areaPosX = Screen.width / 2 - totalAreaWidth / 2;
				float areaPosY = Screen.height / 2 - areaHeight / 2;

				float toolTipPosX = areaPosX + areaWidth;

				float exitButtonWidth = toolTipWidth * .1f;
				float exitButtonHeight = areaHeight * .05f;

				// Radio buttons
				{
					GUILayout.BeginArea(new Rect(areaPosX, areaPosY, areaWidth, areaHeight), styleManager.modMenuAreaStyle);
					// Note: GUILayout.Height is adjusted to be slightly smaller, otherwise not all content is visible/scroll bar is slightly cut off.
					scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(areaWidth), GUILayout.Height(areaHeight-10));

					switch(menuMode)
					{
						case ModMenuMode.AudioSetup:
							OnGUIFirstTimeAudioSetup();
							break;

						case ModMenuMode.Normal:
						default:
							normalMenu.OnGUI();
							break;
					}

					GUILayout.EndScrollView();
					GUILayout.EndArea();
				}

				// Descriptions for each button are shown on hover, like a tooltip
				GUILayout.BeginArea(new Rect(toolTipPosX, areaPosY, toolTipWidth, areaHeight), styleManager.modMenuAreaStyle);
				c.HeadingLabel("Mod Options Menu");
				GUILayout.Space(10);

				GUIStyle toolTipStyle = styleManager.Group.label;
				string displayedToolTip;
				if (GUI.tooltip == String.Empty)
				{
					if (defaultToolTipTimer.timeLeft == 0)
					{
						if(this.startupFailed)
						{
							displayedToolTip = startupFailureToolTip;
							toolTipStyle = styleManager.Group.errorLabel;
						}
						else
						{
							displayedToolTip = defaultTooltip;
						}
					}
					else
					{
						displayedToolTip = lastToolTip;
					}
				}
				else
				{
					lastToolTip = GUI.tooltip;
					displayedToolTip = GUI.tooltip;
					defaultToolTipTimer.Start(.2f);
				}
				// MUST pass in MinHeight option, otherwise Unity will get confused and assume
				// label is one line high on first draw, and subsequent changes will truncate
				// label to one line even if it is multiple lines tall.
				GUILayout.Label(displayedToolTip, toolTipStyle, GUILayout.MinHeight(areaHeight));
				GUILayout.EndArea();

				// Exit button
				GUILayout.BeginArea(new Rect(toolTipPosX + toolTipWidth - exitButtonWidth, areaPosY, exitButtonWidth, exitButtonHeight));
				if (c.Button(new GUIContent("X", "Close the Mod menu")))
				{
					this.Hide();
				}
				GUILayout.EndArea();

				if(MODRadio.anyRadioPressed || anyButtonPressed)
				{
					GameSystem.Instance.AudioController.PlaySystemSound(MODSound.GetSoundPathFromEnum(GUISound.Click));
					MODRadio.anyRadioPressed = false;
					anyButtonPressed = false;
				}
			}
		}

		public void Show(ModMenuMode menuMode = ModMenuMode.Normal)
		{
			this.menuMode = menuMode;

			void ForceShow()
			{
				gameSystem.MODIgnoreInputs = true;
				gameSystem.HideUIControls();
				this.visible = true;
			}

			if (gameSystem.GameState == GameState.SaveLoadScreen)
			{
				MODToaster.Show("Please close the current menu and try again");
			}
			else if (gameSystem.GameState == GameState.ConfigScreen)
			{
				gameSystem.LeaveConfigScreen(delegate
				{
					ForceShow();
				});
			}
			else
			{
				ForceShow();
			}

		}

		public void Hide()
		{
			this.visible = false;
			gameSystem.MODIgnoreInputs = false;
			gameSystem.ShowUIControls();
		}

		public void ToggleVisibility()
		{
			if (this.visible)
			{
				this.Hide();
			}
			else
			{
				this.Show();
			}
		}

		private void OnGUIFirstTimeAudioSetup()
		{
			c.Label("The patch supports different BGM/SE types, they can vary what you will hear and when. Choose the one that feels most appropriate for your experience.");
			if(normalMenu.OnGUIFragmentChooseAudioSet(c))
			{
				menuMode = ModMenuMode.Normal;
				Hide();
			}
		}

		public void ToggleDebugMenu()
		{
			showDebugInfo = !showDebugInfo;
			MODAudioTracking.Instance.LoggingEnabled = showDebugInfo;
		}
	}
}
