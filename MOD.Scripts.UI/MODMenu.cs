﻿using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using MOD.Scripts.Core;
using MOD.Scripts.Core.Audio;
using MOD.Scripts.Core.Localization;
using MOD.Scripts.Core.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;

namespace MOD.Scripts.UI
{
	public enum ModSubMenu
	{
		Normal,
		AudioSetup,
	}

	public class MODMenu
	{
		private const int DEBUG_WINDOW_ID = 1;

		private readonly GameSystem gameSystem;
		public bool visible;
		public bool debug;
		private bool lastDebug;
		private bool lastMenuVisibleStatus;
		private MODSimpleTimer defaultToolTipTimer;
		private MODSimpleTimer startupWatchdogTimer;
		private bool startupFailed;
		Vector2 scrollPosition;
		Vector2 leftDebugColumnScrollPosition;
		private Rect debugWindowRect;
		GUISound buttonClickSound;

		private MODMenuNormal normalMenu;
		private MODMenuAudioOptions audioOptionsMenu;
		private MODMenuAudioSetup audioSetupMenu;
		private MODMenuModuleInterface currentMenu; // The menu that is currently visible

		private MODMenuFontConfig fontMenuFragment;

		string lastToolTip = String.Empty;

		string startupFailureToolTip = Loc.MODMenu_16; //It looks like there was a problem starting up\n\nPlease send the developers your log file (output_log.txt or Player.log).\n\nYou can try the following yourself to fix the issue.\n  1. Try waiting 30 seconds for the game to progress. If nothing happens, try restarting the game\n\n  2. Use the buttons under 'Troubleshooting' on the bottom left to show your save files, log files, and compiled scripts.\n\n  3. If the log indicates you have corrupt save files, you may wish to delete the offending save file (or all of them).\n\n  4. You can try to clear your compiled script files, then restart the game.\n\n  5. If the above do not fix the problem, please click the 'Open Support Page' button, which has extra troubleshooting info and links to join our Discord server for direct support.

		bool showBGMButtonPressed;
		Vector2 bgmInfoScrollPosition;

		public MODMenu(GameSystem gameSystem)
		{
			this.gameSystem = gameSystem;
			this.visible = false;
			this.debug = false;
			this.lastMenuVisibleStatus = false;
			this.defaultToolTipTimer = new MODSimpleTimer();
			this.startupWatchdogTimer = new MODSimpleTimer();
			this.startupFailed = false;

			// Start the watchdog timer as soon as possible, so it starts from "when the game started"
			this.startupWatchdogTimer.Start(5.0f);

			this.audioOptionsMenu = new MODMenuAudioOptions(this);
			this.normalMenu = new MODMenuNormal(this, this.audioOptionsMenu);
			this.audioSetupMenu = new MODMenuAudioSetup(this, this.audioOptionsMenu);
			this.fontMenuFragment = new MODMenuFontConfig();
			this.currentMenu = this.normalMenu;

			this.debugWindowRect = new Rect(0, 0, Screen.width / 3, Screen.height - 50);

			this.showBGMButtonPressed = false;
			this.bgmInfoScrollPosition = new Vector2();
		}

		public void Update()
		{
			defaultToolTipTimer.Update();
			startupWatchdogTimer.Update();
		}

		public void LateUpdate()
		{
			// Hide the menu on right-click
			// This must be done in LateUpdate() rather than Update(),
			// otherwise the right-click event will also open the in-game right-click menu
			if (Input.GetMouseButtonDown(1))
			{
				this.UserHide();
			}
		}

		private void OnBeforeDebugMenuVisible()
		{
			fontMenuFragment.OnBeforeMenuVisible();
		}

		// This is a separate, smaller draggable mod menu, mainly for developer use.
		private void OnGUIDebugWindow(int windowID)
		{
			MODStyleManager styleManager = MODStyleManager.OnGUIInstance;
			GUI.depth = 1;

			bool bgmFlagOK = MODAudioSet.Instance.GetBGMCascade(GetGlobal("GAltBGM"), out PathCascadeList BGMCascade);
			bool seFlagOK = MODAudioSet.Instance.GetSECascade(GetGlobal("GAltSE"), out PathCascadeList SECascade);

			// ============================= Begin Sroll View =============================
			if (!visible)
			{
				leftDebugColumnScrollPosition = GUILayout.BeginScrollView(leftDebugColumnScrollPosition, GUILayout.Width(Screen.width / 3), GUILayout.Height(Screen.height*9/10));
			}
			HeadingLabel(Loc.MODMenu_0); //[Audio Tracking] - indicates what would play on each BGM flow
			Label($"{MODAudioTracking.Instance}");

			HeadingLabel(Loc.MODMenu_1); //[Audio Flags and last played audio]
			Label($"Audio Set: {GetGlobal("GAudioSet")} ({MODAudioSet.Instance.GetCurrentAudioSetDisplayName()})\n\n" +
				$"AltBGM: {GetGlobal("GAltBGM")}\n" +
				$"AltBGMFlow: {GetGlobal("GAltBGMflow")} ({MODAudioSet.Instance.GetBGMFlowName(GetGlobal("GAltBGMflow"))})\n" +
				$"Last Played BGM: {AssetManager.Instance.debugLastBGM}\n" +
				$"BGM Cascade: [{string.Join(":", BGMCascade.paths)}] ({BGMCascade.nameEN}) {(bgmFlagOK ? "" : "9Warning: Using default due to unknown flag)")}\n\n" +
				$"AltSE:  {GetGlobal("GAltSE")}\n" +
				$"AltSEFlow: {GetGlobal("GAltSEflow")}\n" +
				$"Last Played SE Path: {AssetManager.Instance.debugLastSE}\n" +
				$"SE Cascade: [{string.Join(":", SECascade.paths)}] ({SECascade.nameEN}) {(seFlagOK ? "" : "(Warning: Using default due to unknown flag)")}\n" +
				$"Voice: {GetGlobal("GAltVoice")}\n" +
				$"Priority: {GetGlobal("GAltVoicePriority")}\n\n" +
				$"Last Played Voice Path: {AssetManager.Instance.debugLastVoice}\n" +
				$"Other Last Played Path: {AssetManager.Instance.debugLastOtherAudio}");

			Label(Core.Scene.MODLipsyncCache.DebugInfo());

			// Button to reset GAudio Set
			if(Button(new GUIContent(Loc.MODMenu_2, Loc.MODMenu_3))) //Reset GAudioSet | Set GAudioSet to 0, to force the game to do audio setup on next startup
			{
				SetGlobal("GAudioSet", 0);
			}

			// Font Adjustment Debug Menu
			fontMenuFragment.OnGUIFontDebug();

			// Button to close the debug menu
			if (Button(new GUIContent(Loc.MODMenu_4, Loc.MODMenu_5))) //Close | Close the debug menu
			{
				ToggleDebugMenu();
			}

			if (!visible)
			{
				GUILayout.EndScrollView();
			}
			// ============================= End Scroll View =============================

			if(GameSystem.Instance.MODIgnoreInputs())
			{
				Label("NOTE: Game Paused while Mouse On Debug Menu!");
			}

			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}

		private void OnGUIConfigMenuButton(string text, float? alpha, Action action)
		{
			MODStyleManager styleManager = MODStyleManager.OnGUIInstance;

			if(alpha.HasValue)
			{
				// Temporarily override the global tint color to get a fade in effect which matches the existing UI fade in
				// This value is not saved by Unity (it resets to the default value each frame)
				GUI.color = new Color(1.0f, 1.0f, 1.0f, alpha.Value);
			}

			// Calculate the width and height of the button
			float areaWidth = Screen.width / 8 + Screen.width / 64;
			float areaHeight = Mathf.Max(
				Mathf.Round(styleManager.Group.button.CalcHeight(new GUIContent(text, ""), areaWidth)) + 10,
				3 * Screen.height / 32
			);

			// Position the top-left x so the button is aligned with the config menu background, with a small margin
			float xOffset = Screen.width / 8 + Screen.width / 64;

			// Reposition button in 4:3 mode otherwise it would overlap the settings text
			if(BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode43Aspect").IntValue() != 0)
			{
				xOffset = 7 * Screen.width / 128;
			}

			// Position the top-left y to vertically center the button
			float yOffset = Screen.height / 2 - areaHeight/2;

			GUILayout.BeginArea(new Rect(xOffset, yOffset, areaWidth, areaHeight), styleManager.modMenuAreaStyle);
			if (Button(text, options: GUILayout.ExpandHeight(true)))
			{
				action();
			}

			GUILayout.EndArea();
		}

		private void OnGUIRightClickMenuOverlay(float? alpha, Action onGUIInternal)
		{
			if (alpha.HasValue)
			{
				// Temporarily override the global tint color to get a fade in effect which matches the existing UI fade in
				// This value is not saved by Unity (it resets to the default value each frame)
				GUI.color = new Color(1.0f, 1.0f, 1.0f, alpha.Value);
			}

			// The width of the overlay should match the right-click menu width
			float areaWidth = Screen.width * 3 / 4;

			// The overlay should start where the right-click menu starts
			float xOffset = Screen.width / 8;

			// Set the y-offset so that the overlay appears under the list of controls
			float yOffset = Screen.height * 11 / 16;

			// Set the height to fill the rest of the screen, but with a little bit of margin at the bottom so it looks nicer
			float areaHeight = Screen.height - yOffset - Screen.height / 32;

			GUILayout.BeginArea(new Rect(xOffset, yOffset, areaWidth, areaHeight), MODStyleManager.OnGUIInstance.modMenuAreaStyleLight);
			bgmInfoScrollPosition = GUILayout.BeginScrollView(bgmInfoScrollPosition);

			onGUIInternal();

			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		/// <summary>
		/// This function MUST be called from an OnGUI(), otherwise Unity won't work
		/// properly when  the immediate mode GUI functions are called.
		/// </summary>
		public void OnGUIFragment()
		{
			MODStyleManager styleManager = MODStyleManager.OnGUIInstance;
			buttonClickSound = GUISound.Click;

			// If no menus are visible, allow mod inputs
			if(!visible && !debug)
			{
				gameSystem.SetMODIgnoreInputs(false);
			}

			if (debug && !lastDebug)
			{
				OnBeforeDebugMenuVisible();
			}
			if (debug && AssetManager.Instance != null)
			{
				debugWindowRect = GUILayout.Window(DEBUG_WINDOW_ID, debugWindowRect, OnGUIDebugWindow, Loc.MODMenu_6, styleManager.modMenuAreaStyleLight); //Developer Debug Window (click to drag)

				// Prevent mouse clicks being registered by the game when using debug menu
				gameSystem.SetMODIgnoreInputs(debugWindowRect.Contains(Input.mousePosition));
			}
			lastDebug = debug;

			GUI.depth = 0;

			// Show a troubleshooting message if game failed to start-up
			if (this.startupWatchdogTimer.Finished())
			{
				this.startupWatchdogTimer.Cancel();
				if (!BurikoScriptSystem.Instance.FlowWasReached)
				{
					this.startupFailed = true;
					// Normally opening the menu pauses the game, but in this case we want the game to keep running
					// in case this game-failed-startup is a false positive
					this.Show(menuStopsGameUpdate: false);
				}
			}

			// Assume that the game started up correctly if the flow.txt game script was reached
			if(BurikoScriptSystem.Instance.FlowWasReached)
			{
				this.startupFailed = false;
			}

			// This adds an "Open Mod Menu" button to the Config Screen
			// (the normal settings screen that comes with the stock game)
			if (gameSystem.GameState == GameState.ConfigScreen)
			{
				OnGUIConfigMenuButton(Loc.MODMenu_7, gameSystem.ConfigManager()?.PanelAlpha(), () => this.Show()); //Mod Menu\n(Hotkey: F10)
			}

			if (!visible && gameSystem.GameState == GameState.RightClickMenu)
			{
				OnGUIRightClickMenuOverlay(gameSystem.MenuUIController()?.PanelAlpha(), () =>
				{
					HeadingLabel(Loc.MODMenu_8, alignLeft: true); //BGM Info
					GUILayout.Space(10);

					// It is possible multiple BGM play at the same time (although secondary BGM are usually just background noises rather than actualBGM)
					List<KeyValuePair<int, AudioInfo>> currentBGM = AudioController.Instance.GetCurrrentBGM().ToList();
					currentBGM.Sort((x, y) => x.Key - y.Key);

					int bgmCount = 0;

					foreach (KeyValuePair<int, AudioInfo> kvp in currentBGM)
					{
						bgmCount++;

						AudioInfo audioInfo = kvp.Value;
						string audioPath = AssetManager.Instance._GetAudioFilePath(audioInfo.Filename, Assets.Scripts.Core.Audio.AudioType.BGM, out bool _, out bool _);
						BGMInfo bgmInfo = MODBGMInfo.GetBGMName(audioPath);

						// Display the name of the BGM on one line
						SelectableLabel($"♫ {bgmInfo.name.Trim()} ♫");

						// Below the BGM name, add utility buttons, all one one line
						GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
						{
							if (ButtonNoExpandWithPadding(Loc.MODMenu_9)) //Copy BGM Name
							{
								GUIUtility.systemCopyBuffer = bgmInfo.name.Trim();
							}

							if (ButtonNoExpandWithPadding(Loc.MODMenu_10 + $" ({audioPath})")) //Show File
							{
								string bgmFullPath = Path.Combine(Application.streamingAssetsPath, audioPath);
								showBGMButtonPressed = true;
								MODUtility.ShowInFolder(bgmFullPath);
							}

							if (!string.IsNullOrEmpty(bgmInfo.url))
							{
								if (ButtonNoExpandWithPadding(Loc.MODMenu_11)) //Open In Youtube
								{
									Application.OpenURL($"https://www.youtube.com/watch?v={bgmInfo.url}");
								}
							}
						}
						GUILayout.EndHorizontal();

						if (bgmCount < currentBGM.Count)
						{
							GUILayout.Space(10);
						}
					}

					// On Windows, add note about explorer .ogg file bug
					if (showBGMButtonPressed && Application.platform == RuntimePlatform.WindowsPlayer)
					{
						Label(Loc.MODMenu_12); //Note: If explorer freezes\nuninstall Web Media Extensions
					}
				});
			}
			else
			{
				showBGMButtonPressed = false;
			}

			// If you need to initialize things just once before the menu opens, rather than every frame
			// you can do it in the OnBeforeMenuVisible() function below.
			if (visible && !lastMenuVisibleStatus)
			{
				currentMenu.OnBeforeMenuVisible();
			}
			lastMenuVisibleStatus = visible;


			if (visible)
			{
				float totalAreaWidth = styleManager.Group.menuWidth;

				float toolTipSubtraction = totalAreaWidth * styleManager.Group.toolTipShrinkage;

				float areaWidth = Mathf.Round(totalAreaWidth * 9/16) + toolTipSubtraction;
				float toolTipWidth = Mathf.Round(totalAreaWidth * 7/16) - toolTipSubtraction;

				float areaHeight = styleManager.Group.menuHeight;

				float areaPosX = Screen.width / 2 - totalAreaWidth / 2;
				float areaPosY = Screen.height / 2 - areaHeight / 2;

				float toolTipPosX = areaWidth;

				float exitButtonWidth = toolTipWidth * .1f;
				float exitButtonHeight = areaHeight * .05f;

				float innerMargin = 4f;

				// This contains the the entire menu, and draws a white border around it
				GUILayout.BeginArea(new Rect(areaPosX, areaPosY, areaWidth + toolTipWidth, areaHeight), styleManager.modMenuAreaStyle);

				// This displays the left hand side of the menu which contains the option buttons and headings
				{
					GUILayout.BeginArea(new Rect(innerMargin, innerMargin, areaWidth-innerMargin, areaHeight-innerMargin), styleManager.modGUIBackgroundTextureTransparent);
					// Note: GUILayout.Height is adjusted to be slightly smaller, otherwise not all content is visible/scroll bar is slightly cut off.
					scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(areaWidth), GUILayout.Height(areaHeight-10));

					// 'currentMenu' is reassigned to switch to different sub-menus
					// please see the SetSubMenu() function
					currentMenu.OnGUI();

					GUILayout.EndScrollView();
					GUILayout.EndArea();
				}

				// This displays the right hand side of the menu which contains descriptions of each option.
				// Descriptions for each button are shown on hover, like a tooltip
				{
					GUILayout.BeginArea(new Rect(toolTipPosX, innerMargin, toolTipWidth- innerMargin, areaHeight-innerMargin), styleManager.modGUIBackgroundTextureTransparent);
					HeadingLabel(currentMenu.Heading());
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
						if (Button(new GUIContent(Loc.MODMenu_13, Loc.MODMenu_14))) //X | Close the Mod menu
						{
							this.UserHide();
						}
						GUILayout.EndArea();
					}
				}
				GUILayout.EndArea();

				if(MODRadio.anyRadioPressed || MODMenuCommon.anyButtonPressed)
				{
					GameSystem.Instance.AudioController.PlaySystemSound(MODSound.GetSoundPathFromEnum(buttonClickSound));
					MODRadio.anyRadioPressed = false;
					MODMenuCommon.anyButtonPressed = false;
				}
			}
		}

		// This temporarily changes button click sound for one frame.
		// The button click sound will revert to the default click sound on the next frame.
		public void OverrideClickSound(GUISound sound)
		{
			buttonClickSound = sound;
		}

		// The mod menu has different sub-menus, which can be switched between by calling this function.
		// If the sub-menus have any state, it will be retained during switching, and even if the menu is closed and reopened.
		public void SetSubMenu(ModSubMenu subMenu)
		{
			switch (subMenu)
			{
				case ModSubMenu.AudioSetup:
					currentMenu = audioSetupMenu;
					break;

				case ModSubMenu.Normal:
				default:
					currentMenu = normalMenu;
					break;
			}
		}

		// This function attempts to show the menu, but please note:
		// - on the Save / Load screen, it will instead tell you to close the Save/Load screen
		// - the menu might open after a short delay, due to using a delegate to close
		//   the currently open menu. Please keep this in mind if you're relying on the menu opening immediately.
		public void Show(bool menuStopsGameUpdate=true)
		{
			void ForceShow()
			{
				gameSystem.SetMODIgnoreInputs(menuStopsGameUpdate);
				gameSystem.HideUIControls();
				this.visible = true;
			}

			if (gameSystem.GameState == GameState.SaveLoadScreen)
			{
				MODToaster.Show(Loc.MODMenu_15); //Please close the current menu and try again
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
		/// This function hides the menu if the menu allows it.
		/// Use ForceHide() to forcibly hide the menu.
		/// </summary>
		public void UserHide()
		{
			if (currentMenu.UserCanClose())
			{
				ForceHide();
			}
		}

		/// <summary>
		/// This function toggles the menu on/off if the menu allows it.
		/// Use ForceToggleVisibility() to forcibly toggle the menu.
		/// </summary>
		public void UserToggleVisibility()
		{
			if (currentMenu.UserCanClose())
			{
				ForceToggleVisibility();
			}
		}

		/// <summary>
		/// This function hides the menu, even if the if the menu prefers not to be hidden.
		/// </summary>
		public void ForceHide()
		{
			this.visible = false;
			gameSystem.SetMODIgnoreInputs(false);
			gameSystem.ShowUIControls();
		}

		/// <summary>
		/// This function toggles the menu, even if the if the menu prefers not to be toggled.
		/// </summary>
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
