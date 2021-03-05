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
		private const int DEBUG_WINDOW_ID = 1;

		private readonly MODStyleManager styleManager;
		private readonly MODMenuCommon c;
		private readonly GameSystem gameSystem;
		public bool visible;
		public bool debug;
		private bool lastMenuVisibleStatus;
		private MODSimpleTimer defaultToolTipTimer;
		private MODSimpleTimer startupWatchdogTimer;
		private bool startupFailed;
		private bool anyButtonPressed;
		Vector2 scrollPosition;
		Vector2 leftDebugColumnScrollPosition;
		private Rect debugWindowRect;
		GUISound buttonClickSound;

		private MODMenuNormal normalMenu;
		private MODMenuAudioOptions audioOptionsMenu;
		private MODMenuAudioSetup audioSetupMenu;
		private MODMenuModuleInterface currentMenu; // The menu that is currently visible

		string lastToolTip = String.Empty;

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
			this.debug = false;
			this.lastMenuVisibleStatus = false;
			this.defaultToolTipTimer = new MODSimpleTimer();
			this.startupWatchdogTimer = new MODSimpleTimer();
			this.startupFailed = false;
			this.c = new MODMenuCommon(styleManager);

			// Start the watchdog timer as soon as possible, so it starts from "when the game started"
			this.startupWatchdogTimer.Start(5.0f);

			this.audioOptionsMenu = new MODMenuAudioOptions(this, this.c, styleManager);
			this.normalMenu = new MODMenuNormal(this, this.c, styleManager, this.audioOptionsMenu);
			this.audioSetupMenu = new MODMenuAudioSetup(this, this.c, this.audioOptionsMenu);
			this.currentMenu = this.normalMenu;

			this.debugWindowRect = new Rect(0, 0, Screen.width / 3, Screen.height - 50);
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
				this.UserHide();
			}
		}

		private void OnGUIDebugWindow(int windowID)
		{
			GUI.depth = 1;

			bool bgmFlagOK = MODAudioSet.Instance.GetBGMCascade(c.GetGlobal("GAltBGM"), out PathCascadeList BGMCascade);
			bool seFlagOK = MODAudioSet.Instance.GetSECascade(c.GetGlobal("GAltSE"), out PathCascadeList SECascade);

			if (!visible)
			{
				leftDebugColumnScrollPosition = GUILayout.BeginScrollView(leftDebugColumnScrollPosition, GUILayout.Width(Screen.width / 3), GUILayout.Height(Screen.height*9/10));
			}
			GUILayout.Label($"[Audio Tracking] - indicates what would play on each BGM flow", styleManager.Group.upperLeftHeadingLabel);
			GUILayout.Label($"{MODAudioTracking.Instance}", styleManager.Group.upperLeftHeadingLabel);

			GUILayout.Label($"[Audio Flags and last played audio]", styleManager.Group.upperLeftHeadingLabel);
			GUILayout.Label($"Audio Set: {c.GetGlobal("GAudioSet")} ({MODAudioSet.Instance.GetCurrentAudioSetDisplayName()})\n" +
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

			if (debug)
			{
				if(c.Button(new GUIContent("Reset GAudioSet", "Set GAudioSet to 0, to force the game to do audio setup on next startup")))
				{
					c.SetGlobal("GAudioSet", 0);
				}
			}

			if (c.Button(new GUIContent("Close", "Close the debug menu")))
			{
				ToggleDebugMenu();
			}

			if (!visible)
			{
				GUILayout.EndScrollView();
			}

			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}

		/// <summary>
		/// Must be called from an OnGUI()
		/// </summary>
		public void OnGUIFragment()
		{
			buttonClickSound = GUISound.Click;

			if (debug && AssetManager.Instance != null)
			{
				debugWindowRect = GUILayout.Window(DEBUG_WINDOW_ID, debugWindowRect, OnGUIDebugWindow, "Developer Debug Window (click to drag)", styleManager.modMenuAreaStyleLight);
			}

			GUI.depth = 0;

			// Show a troubleshooting message if game failed to start-up
			if (this.startupWatchdogTimer.Finished())
			{
				this.startupWatchdogTimer.Cancel();
				if (!BurikoScriptSystem.Instance.FlowWasReached)
				{
					this.startupFailed = true;
					this.Show();
				}
			}

			if(BurikoScriptSystem.Instance.FlowWasReached)
			{
				this.startupFailed = false;
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
				currentMenu.OnBeforeMenuVisible();
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

				float toolTipPosX = areaWidth;

				float exitButtonWidth = toolTipWidth * .1f;
				float exitButtonHeight = areaHeight * .05f;

				float innerMargin = 4f;

				// Radio buttons
				GUILayout.BeginArea(new Rect(areaPosX, areaPosY, areaWidth + toolTipWidth, areaHeight), styleManager.modMenuAreaStyle);
				{
					GUILayout.BeginArea(new Rect(innerMargin, innerMargin, areaWidth-innerMargin, areaHeight-innerMargin), styleManager.modGUIBackgroundTextureTransparent);
					// Note: GUILayout.Height is adjusted to be slightly smaller, otherwise not all content is visible/scroll bar is slightly cut off.
					scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(areaWidth), GUILayout.Height(areaHeight-10));

					currentMenu.OnGUI();

					GUILayout.EndScrollView();
					GUILayout.EndArea();
				}

				// Descriptions for each button are shown on hover, like a tooltip
				{
					GUILayout.BeginArea(new Rect(toolTipPosX, innerMargin, toolTipWidth- innerMargin, areaHeight-innerMargin), styleManager.modGUIBackgroundTextureTransparent);
					c.HeadingLabel(currentMenu.Heading());
					GUILayout.Space(10);

					GUIStyle toolTipStyle = styleManager.Group.label;
					string displayedToolTip;
					if (GUI.tooltip == String.Empty)
					{
						if (defaultToolTipTimer.timeLeft == 0)
						{
							if (this.startupFailed)
							{
								displayedToolTip = startupFailureToolTip;
								toolTipStyle = styleManager.Group.errorLabel;
							}
							else
							{
								displayedToolTip = currentMenu.DefaultTooltip();
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
					if (currentMenu.UserCanClose())
					{
						GUILayout.BeginArea(new Rect(toolTipPosX + toolTipWidth - exitButtonWidth - innerMargin, innerMargin, exitButtonWidth, exitButtonHeight));
						if (c.Button(new GUIContent("X", "Close the Mod menu")))
						{
							this.UserHide();
						}
						GUILayout.EndArea();
					}
				}
				GUILayout.EndArea();

				if(MODRadio.anyRadioPressed || anyButtonPressed)
				{
					GameSystem.Instance.AudioController.PlaySystemSound(MODSound.GetSoundPathFromEnum(buttonClickSound));
					MODRadio.anyRadioPressed = false;
					anyButtonPressed = false;
				}
			}
		}

		public void OverrideClickSound(GUISound sound)
		{
			buttonClickSound = sound;
		}

		public void SetMode(ModMenuMode menuMode)
		{
			switch (menuMode)
			{
				case ModMenuMode.AudioSetup:
					currentMenu = audioSetupMenu;
					break;

				case ModMenuMode.Normal:
				default:
					currentMenu = normalMenu;
					break;
			}
		}

		public void Show()
		{
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

		/// <summary>
		/// This function should be called when the user has initiated the hiding of the menu.
		/// This function call might be ignored if the menu disallows closing - call
		/// </summary>
		public void UserHide()
		{
			if (currentMenu.UserCanClose())
			{
				ForceHide();
			}
		}

		public void UserToggleVisibility()
		{
			if (currentMenu.UserCanClose())
			{
				ForceToggleVisibility();
			}
		}

		public void ForceHide()
		{
			this.visible = false;
			gameSystem.MODIgnoreInputs = false;
			gameSystem.ShowUIControls();
		}

		public void ForceToggleVisibility()
		{
			if (this.visible)
			{
				this.ForceHide();
			}
			else
			{
				this.Show();
			}
		}

		public void ToggleDebugMenu()
		{
			debug = !debug;
			MODAudioTracking.Instance.LoggingEnabled = debug;
		}
	}
}
