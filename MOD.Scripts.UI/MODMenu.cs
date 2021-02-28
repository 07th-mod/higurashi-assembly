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
	class MODRadio
	{
		public static bool anyRadioPressed;
		string label;
		private GUIContent[] radioContents;
		MODStyleManager styleManager;
		int itemsPerRow;

		public MODRadio(string label, GUIContent[] radioContents, MODStyleManager styleManager, int itemsPerRow=0) //Action<int> onRadioChange, 
		{
			this.label = label;
			this.radioContents = radioContents;
			this.itemsPerRow = radioContents.Length == 0 ? 1 : radioContents.Length;
			if(itemsPerRow != 0)
			{
				this.itemsPerRow = itemsPerRow;
			}
			this.styleManager = styleManager;
		}

		/// <summary>
		/// NOTE: only call this function within OnGUI()
		/// Displays the radio, calling onRadioChange when the user clicks on a different radio value.
		/// </summary>
		/// <param name="displayedRadio">Sets the currently displayed radio. Use "-1" for "None selected"</param>
		/// <returns>If radio did not change value, null is returned, otherwise the new value is returned.</returns>
		public int? OnGUIFragment(int displayedRadio)
		{
			GUILayout.Label(this.label, styleManager.Group.label);
			int i = GUILayout.SelectionGrid(displayedRadio, radioContents, itemsPerRow, styleManager.Group.modMenuSelectionGrid);
			if (i != displayedRadio)
			{
				MODRadio.anyRadioPressed = true;
				return i;
			}

			return null;
		}

		public void SetContents(GUIContent[] content)
		{
			this.radioContents = content;
		}
	}

	public class MODMenu
	{
		private readonly MODStyleManager styleManager;
		private readonly MODRadio radioCensorshipLevel;
		private readonly MODRadio radioLipSync;
		private readonly MODRadio radioOpenings;
		private readonly MODRadio radioHideCG;
		private readonly MODRadio radioBackgrounds;
		private readonly MODRadio radioArtSet;
		private readonly MODRadio radioBGMSESet;
		private readonly GameSystem gameSystem;
		public bool visible;
		private bool lastMenuVisibleStatus;
		private MODSimpleTimer defaultToolTipTimer;
		private MODSimpleTimer startupWatchdogTimer;
		private bool startupFailed;
		private string screenHeightString;
		private bool anyButtonPressed;
		Vector2 scrollPosition;
		Vector2 leftDebugColumnScrollPosition;
		private static Vector2 emergencyMenuScrollPosition;
		private bool hasOGBackgrounds;
		private bool hasBGMSEOptions;
		private bool showDebugInfo;

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

		GUIContent[] defaultArtsetDescriptions = new GUIContent[] {
			new GUIContent("Console", "Use the Console sprites and backgrounds"),
			new GUIContent("Remake", "Use Mangagamer's remake sprites with Console backgrounds"),
			new GUIContent("Original", "Use Original/Ryukishi sprites and backgrounds (if available - OG backgrounds not available for Console Arcs)\n\n" +
			"Warning: Most users should use the Original/Ryukishi preset at the top of this menu!"),
		};

		public MODMenu(GameSystem gameSystem, MODStyleManager styleManager)
		{
			this.gameSystem = gameSystem;
			this.styleManager = styleManager;
			this.visible = false;
			this.lastMenuVisibleStatus = false;
			this.defaultToolTipTimer = new MODSimpleTimer();
			this.startupWatchdogTimer = new MODSimpleTimer();
			this.startupFailed = false;
			this.screenHeightString = String.Empty;
			this.hasOGBackgrounds = MODActions.HasOGBackgrounds();
			this.hasBGMSEOptions = MODActions.HasMusicToggle();

			string baseCensorshipDescription = @"

Sets the script censorship level
- This setting exists because the voices are taken from the censored, Console versions of the game, so no voices exist for the PC uncensored dialogue.
- We recommend the default level (2), the most balanced option. Using this option, only copyright changes, innuendos, and a few words will be changed.
  - 5: Full PS3 script fully voiced (most censored)
  - 2: Default - most balanced option
  - 0: Original PC Script with voices where it fits (least uncensored), but uncensored scenes may be missing voices";

			this.radioCensorshipLevel = new MODRadio("Voice Matching Level (Hotkey: F2)", new GUIContent[] {
				new GUIContent("0", "Censorship level 0 - Equivalent to PC" + baseCensorshipDescription),
				new GUIContent("1", "Censorship level 1" + baseCensorshipDescription),
				new GUIContent("2*", "Censorship level 2 (this is the default/recommended value)" + baseCensorshipDescription),
				new GUIContent("3", "Censorship level 3" + baseCensorshipDescription),
				new GUIContent("4", "Censorship level 4" + baseCensorshipDescription),
				new GUIContent("5", "Censorship level 5 - Equivalent to Console" + baseCensorshipDescription),
				}, styleManager);

			this.radioLipSync = new MODRadio("Lip Sync for Console Sprites (Hotkey: 7)", new GUIContent[]
			{
				new GUIContent("Lip Sync Off", "Disables Lip Sync for Console Sprites"),
				new GUIContent("Lip Sync On", "Enables Lip Sync for Console Sprites"),
			}, styleManager);

			this.radioOpenings = new MODRadio("Opening Movies (Hotkey: Shift-F12)", new GUIContent[]
			{
				new GUIContent("Disabled", "Disables all opening videos"),
				new GUIContent("Enabled", "Enables opening videos\n\n" +
				"NOTE: Once the opening video plays the first time, will automatically switch to 'Launch + In-Game'\n\n" +
				"We have setup openings this way to avoid spoilers."),
				new GUIContent("Launch + In-Game", "WARNING: There is usually no need to set this manually.\n\n" +
				"If openings are enabled, the first time you reach an opening while playing the game, this flag will be set automatically\n\n" +
				"That is, after the opening is played the first time, from then on openings will play every time the game launches"),
			}, styleManager);

			this.radioHideCG = new MODRadio("Show/Hide CGs", new GUIContent[]
			{
				new GUIContent("Show CGs", "Shows CGs (You probably want this enabled for Console ADV/NVL mode)"),
				new GUIContent("Hide CGs", "Disables all CGs (mainly for use with the Original/Ryukishi preset)"),
			}, styleManager);

			this.radioArtSet = new MODRadio("Choose Art Set", defaultArtsetDescriptions, styleManager, itemsPerRow: 3);

			this.radioBackgrounds = new MODRadio("Override Art Set Backgrounds", new GUIContent[]{
				new GUIContent("Default BGs", "Use the default backgrounds for the current artset"),
				new GUIContent("Console BGs", "Force Console backgrounds, regardless of the artset"),
				new GUIContent("Original BGs", "Force Original/Ryukishi backgrounds, regardless of the artset"),
				new GUIContent("Original Stretched", "Force Original/Ryukishi backgrounds, stretched to fit, regardless of the artset\n\n" +
				"WARNING: When using this option, you should have ADV/NVL mode selected, otherwise sprites will be cut off, and UI will appear in the wrong place"),
			}, styleManager, itemsPerRow: 2);

			this.radioBGMSESet = new MODRadio("Choose BGM/SE (Hotkey: 2)", new GUIContent[]
			{
				new GUIContent("New BGM/SE", "Use the new BGM/SE introduced by MangaGamer in the April 2019 update."),
				new GUIContent("Original BGM/SE", "Use the original BGM/SE from the Japanese version of the game. This option was previously known as 'BGM/SE fix'.\n\n" +
				"Note that this not only changes which audio files are played, but also when BGM starts to play/stops playing, in certain cases."),
			}, styleManager);

			// Start the watchdog timer as soon as possible, so it starts from "when the game started"
			this.startupWatchdogTimer.Start(5.0f);
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

		private void OnGUIRestoreSettings()
		{
			Label($"Restore Settings {(GetGlobal("GMOD_SETTING_LOADER") == 3 ? "" : ": <Restart Pending>")}");

			GUILayout.BeginHorizontal();
			if (GetGlobal("GMOD_SETTING_LOADER") == 3)
			{
				if (Button(new GUIContent("ADV defaults", "This restores flags to the defaults for NVL mode\n" +
					"NOTE: Requires you to relaunch the game 2 times to come into effect")))
				{
					SetGlobal("GMOD_SETTING_LOADER", 0);
				}
				else if (Button(new GUIContent("NVL defaults", "This restores flags to the defaults for NVL mode\n" +
					"NOTE: Requires you to relaunch the game 2 times to come into effect")))
				{
					SetGlobal("GMOD_SETTING_LOADER", 1);
				}
				else if (Button(new GUIContent("Vanilla Defaults", "This restores flags to the same settings as the unmodded game.\n" +
					"NOTE: Requires a relaunch to come into effect. After this, uninstall the mod.")))
				{
					SetGlobal("GMOD_SETTING_LOADER", 2);
				}
			}
			else
			{
				if (Button(new GUIContent("Cancel Pending Restore", "Click this to stop restoring defaults on next game launch")))
				{
					SetGlobal("GMOD_SETTING_LOADER", 3);
				}
			}
			GUILayout.EndHorizontal();
		}

		/// <summary>
		/// Must be called from an OnGUI()
		/// </summary>
		public void OnGUIFragment()
		{
			if (showDebugInfo && AssetManager.Instance != null)
			{
				bool bgmFlagOK = MODAudioSet.Instance.GetBGMCascade(GetGlobal("GAltBGM"), out PathCascadeList BGMCascade);
				bool seFlagOK = MODAudioSet.Instance.GetSECascade(GetGlobal("GAltSE"), out PathCascadeList SECascade);

				GUILayout.BeginArea(new Rect(0, 0, Screen.width/3, Screen.height), styleManager.modMenuAreaStyleLight);
				leftDebugColumnScrollPosition = GUILayout.BeginScrollView(leftDebugColumnScrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height - 10));
				GUILayout.Label($"[Audio Tracking] - indicates what would play on each BGM flow", styleManager.Group.upperLeftHeadingLabel);
				GUILayout.Label($"{MODAudioTracking.Instance}", styleManager.Group.upperLeftHeadingLabel);

				GUILayout.Label($"[Audio Flags and last played audio]", styleManager.Group.upperLeftHeadingLabel);
				GUILayout.Label($"Audio Set: {GetGlobal("GAudioSet")} ({MODAudioSet.Instance.GetCurrentAudioSetName()})\n" +
					"\n" +
					$"AltBGM: {GetGlobal("GAltBGM")}\n" +
					$"AltBGMFlow: {GetGlobal("GAltBGMflow")} ({MODAudioSet.Instance.GetBGMFlowName(GetGlobal("GAltBGMflow"))})\n" +
					$"Last Played BGM: {AssetManager.Instance.debugLastBGM}\n" +
					$"BGM Cascade: [{string.Join(":", BGMCascade.paths)}] ({BGMCascade.nameEN}) {(bgmFlagOK ? "" : "9Warning: Using default due to unknown flag)")}\n" +
					"\n" +
					$"AltSE:  {GetGlobal("GAltSE")}\n" +
					$"AltSEFlow: {GetGlobal("GAltSEflow")}\n" +
					$"Last Played SE Path: {AssetManager.Instance.debugLastSE}\n" +
					$"SE Cascade: [{string.Join(":", SECascade.paths)}] ({SECascade.nameEN}) {(seFlagOK ? "" : "(Warning: Using default due to unknown flag)")}\n" +
					$"Voice: {GetGlobal("GAltVoice")}\n" +
					$"Priority: {GetGlobal("GAltVoicePriority")}\n" +
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
				// Update the artset radio buttons/descriptions, as these are set by ModAddArtset() calls in init.txt at runtime
				// Technically only need to do this once after init.txt has been called, but it's easier to just do it each time menu is opened
				GUIContent[] descriptions = Core.MODSystem.instance.modTextureController.GetArtStyleDescriptions();
				for (int i = 0; i < descriptions.Count(); i++)
				{
					if (i < this.defaultArtsetDescriptions.Count())
					{
						descriptions[i] = this.defaultArtsetDescriptions[i];
					}
				}
				this.radioArtSet.SetContents(descriptions);

				this.screenHeightString = $"{Screen.width}";
				this.screenHeightString = $"{Screen.height}";
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

					HeadingLabel("Basic Options");

					Label("Graphics Presets (Hotkey: F1)");
					{
						GUILayout.BeginHorizontal();

						int advNVLRyukishiMode = MODActions.GetADVNVLRyukishiModeFromFlags(out bool presetModified);

						if (this.Button(new GUIContent(advNVLRyukishiMode == 0 && presetModified ? "ADV (custom)" : "ADV", "This preset:\n" +
						"- Makes text show at the bottom of the screen in a textbox\n" +
						"- Shows the name of the current character on the textbox\n" +
						"- Uses the console sprites and backgrounds\n" +
						"- Displays in 16:9 widescreen\n\n" +
						"Note that sprites and backgrounds can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available"), selected: advNVLRyukishiMode == 0))
						{
							MODActions.SetAndSaveADV(MODActions.ModPreset.ADV, showInfoToast: false);
						}

						if (this.Button(new GUIContent(advNVLRyukishiMode == 1 && presetModified ? "NVL (custom)" : "NVL", "This preset:\n" +
							"- Makes text show across the whole screen\n" +
							"- Uses the console sprites and backgrounds\n" +
							"- Displays in 16:9 widescreen\n\n" +
							"Note that sprites and backgrounds can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available"), selected: advNVLRyukishiMode == 1))
						{
							MODActions.SetAndSaveADV(MODActions.ModPreset.NVL, showInfoToast: false);
						}

						if (this.hasOGBackgrounds &&
							this.Button(new GUIContent(advNVLRyukishiMode == 2 && presetModified ? "Original/Ryukishi (custom)" : "Original/Ryukishi", "This preset makes the game behave similarly to the unmodded game:\n" +
							"- Displays backgrounds in 4:3 'standard' aspect\n" +
							"- CGs are disabled (Can be re-enabled, see 'Show/Hide CGs')\n" +
							"- Switches to original sprites and backgrounds\n\n" +
							"Note that sprites, backgrounds, and CG hiding can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available"), selected: advNVLRyukishiMode == 2))
						{
							MODActions.SetAndSaveADV(MODActions.ModPreset.OG, showInfoToast: false);
						}

						GUILayout.EndHorizontal();
					}

					if (this.radioCensorshipLevel.OnGUIFragment(GetGlobal("GCensor")) is int censorLevel)
					{
						SetGlobal("GCensor", censorLevel);
					};

					if (this.radioLipSync.OnGUIFragment(GetGlobal("GLipSync")) is int lipSyncEnabled)
					{
						SetGlobal("GLipSync", lipSyncEnabled);
					};

					if (this.radioOpenings.OnGUIFragment(GetGlobal("GVideoOpening") - 1) is int openingVideoLevelZeroIndexed)
					{
						SetGlobal("GVideoOpening", openingVideoLevelZeroIndexed + 1);
					};

					if (this.hasBGMSEOptions)
					{
						// Set GAltBGM, GAltSE, GAltBGMFlow, GAltSEFlow to the same value. In the future we may set them to different values.
						if (this.radioBGMSESet.OnGUIFragment(GetGlobal("GAltBGM")) is int newBGMSEValue)
						{
							MODAudioSet.Instance.SetFromZeroBasedIndex(newBGMSEValue);
						}
					}

					HeadingLabel("Advanced Options");

					if (this.radioHideCG.OnGUIFragment(GetGlobal("GHideCG")) is int hideCG)
					{
						SetGlobal("GHideCG", hideCG);
					};

					if (this.radioArtSet.OnGUIFragment(Core.MODSystem.instance.modTextureController.GetArtStyle()) is int artStyle)
					{
						SetGlobal("GStretchBackgrounds", 0);
						Core.MODSystem.instance.modTextureController.SetArtStyle(artStyle, showInfoToast: false);
					}

					if(this.hasOGBackgrounds)
					{
						int currentBackground = GetGlobal("GBackgroundSet");
						if(currentBackground == 2)
						{
							if (GetGlobal("GStretchBackgrounds") == 1)
							{
								currentBackground = 3;
							}
						}
						if(this.radioBackgrounds.OnGUIFragment(currentBackground) is int background)
						{
							if(background == 3)
							{
								SetGlobal("GStretchBackgrounds", 1);
								SetGlobal("GBackgroundSet", 2);
							}
							else
							{
								SetGlobal("GStretchBackgrounds", 0);
								SetGlobal("GBackgroundSet", background);
							}
							GameSystem.Instance.SceneController.ReloadAllImages();
						}
					}

					GUILayout.Space(10);
					OnGUIRestoreSettings();

					Label("Resolution Settings");
					{
						GUILayout.BeginHorizontal();
						if (Button(new GUIContent("480p", "Set resolution to 853 x 480"))) { SetAndSaveResolution(480); }
						if (Button(new GUIContent("720p", "Set resolution to 1280 x 720"))) { SetAndSaveResolution(720); }
						if (Button(new GUIContent("1080p", "Set resolution to 1920 x 1080"))) { SetAndSaveResolution(1080); }
						if (Button(new GUIContent("1440p", "Set resolution to 2560 x 1440"))) { SetAndSaveResolution(1440); }
						if (gameSystem.IsFullscreen)
						{
							if (Button(new GUIContent("Windowed", "Toggle Fullscreen")))
							{
								GameSystem.Instance.DeFullscreen(PlayerPrefs.GetInt("width"), PlayerPrefs.GetInt("height"));
							}
						}
						else
						{
							if (Button(new GUIContent("Fullscreen", "Toggle Fullscreen")))
							{
								gameSystem.GoFullscreen();
							}
						}

						screenHeightString = GUILayout.TextField(screenHeightString);
						if(Button(new GUIContent("Set", "Sets a custom resolution - mainly for windowed mode.\n\n" +
							"Height set automatically to maintain 16:9 aspect ratio.")))
						{
							if(int.TryParse(screenHeightString, out int new_height))
							{
								if(new_height < 480)
								{
									MODToaster.Show("Height too small - must be at least 480 pixels");
									new_height = 480;
								}
								else if(new_height > 15360)
								{
									MODToaster.Show("Height too big - must be less than 15360 pixels");
									new_height = 15360;
								}
								screenHeightString = $"{new_height}";
								int new_width = Mathf.RoundToInt(new_height * 16f / 9f);
								Screen.SetResolution(new_width, new_height, Screen.fullScreen);
								PlayerPrefs.SetInt("width", new_width);
								PlayerPrefs.SetInt("height", new_height);
							}
						}
						GUILayout.EndHorizontal();
					}

					HeadingLabel("Troubleshooting");
					Label("Save Files and Log Files");
					ShowSupportButtons(content => Button(content));

					Label("Developer");
					GUILayout.BeginHorizontal();
					if (Button(new GUIContent("Toggle debug menu (Shift-F9)", "Toggle the debug menu")))
					{
						ToggleDebugMenu();
					}
					if (Button(new GUIContent("Toggle flag menu (Shift-F10)", "Toggle the flag menu. Toggle Multiple times for more options.\n\nNote: 3rd and 4th panels are only shown if GMOD_DEBUG_MODE is true.")))
					{
						MODActions.ToggleFlagMenu();
					}
					GUILayout.EndHorizontal();

					GUILayout.EndScrollView();
					GUILayout.EndArea();
				}

				// Descriptions for each button are shown on hover, like a tooltip
				GUILayout.BeginArea(new Rect(toolTipPosX, areaPosY, toolTipWidth, areaHeight), styleManager.modMenuAreaStyle);
				HeadingLabel("Mod Options Menu");
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
					if(Button(new GUIContent("X", "Close the Mod menu")))
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

		private void Show()
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

		private bool Button(GUIContent guiContent, bool selected=false)
		{
			if(GUILayout.Button(guiContent, selected ? styleManager.Group.selectedButton : styleManager.Group.button))
			{
				anyButtonPressed = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		private void Label(string label)
		{
			GUILayout.Label(label, styleManager.Group.label);
		}

		private void HeadingLabel(string label)
		{
			GUILayout.Label(label, styleManager.Group.headingLabel);
		}

		private int GetGlobal(string flagName) => BurikoMemory.Instance.GetGlobalFlag(flagName).IntValue();
		private void SetGlobal(string flagName, int flagValue) => BurikoMemory.Instance.SetGlobalFlag(flagName, flagValue);

		private void SetAndSaveResolution(int height)
		{
			if (height < 480)
			{
				MODToaster.Show("Height too small - must be at least 480 pixels");
				height = 480;
			}
			else if (height > 15360)
			{
				MODToaster.Show("Height too big - must be less than 15360 pixels");
				height = 15360;
			}
			screenHeightString = $"{height}";
			int width = Mathf.RoundToInt(height * 16f / 9f);
			Screen.SetResolution(width, height, Screen.fullScreen);
			PlayerPrefs.SetInt("width", width);
			PlayerPrefs.SetInt("height", height);
		}

		/// <summary>
		/// This function draws an emergency mod menu in case of a critical game error
		/// (for example, corrupted game save)
		///
		/// Please do null checks/try catch for gamesystem etc. if used in this function as it may be called
		/// during an error condition where gamesystem and other core system parts are not initialized yet
		/// </summary>
		/// <param name="errorMessage"></param>
		public static void EmergencyModMenu(string shortErrorMessage, string longErrorMessage)
		{
			emergencyMenuScrollPosition = GUILayout.BeginScrollView(emergencyMenuScrollPosition);
			GUILayout.Label(shortErrorMessage);
			GUILayout.Label(longErrorMessage);

			ShowSupportButtons(content => GUILayout.Button(content));

			GUILayout.Label(GUI.tooltip == "" ? "Please hover over a button for more information" : GUI.tooltip, GUI.skin.textArea, GUILayout.MinHeight(210));

			GUILayout.EndScrollView();
		}

		private static void ShowSupportButtons(Func<GUIContent, bool> buttonRenderer)
		{
			{
				GUILayout.BeginHorizontal();
				if (buttonRenderer(new GUIContent("Show output_log.txt / Player.log",
					"This button shows the location of the 'ouput_log.txt' or 'Player.log' files\n\n" +
					"- This file is called 'output_log.txt' on Windows and 'Player.log' on MacOS/Linux\n" +
					"- This file records errors that occur during gameplay, and during game startup\n" +
					"- This file helps when the game fails start, for example\n" +
					"  - a corrupted save file\n" +
					"  - the wrong UI (sharedassets0.assets) file\n" +
					"- Note that each time the game starts up, the current log file is replaced")))
				{
					MODActions.ShowLogFolder();
				}

				if (buttonRenderer(new GUIContent("Show Saves", "Clearing your save files can fix some issues with game startup, and resets all mod flags.\n\n" +
					"- WARNING: Steam sync will restore your saves if you manually delete them! Therefore, remember to disable steam sync, otherwise your saves will magically reappear!\n" +
					"- The 'global.dat' file stores your global unlock process and mod flags\n" +
					"- The 'qsaveX.dat' and 'saveXXX.dat' files contain individual save files. Note that these becoming corrupted can break your game\n" +
					"- It's recommended to take a backup of all your saves before you modify them")))
				{
					MODActions.ShowSaveFolder();
				}

				if (buttonRenderer(new GUIContent("Show Compiled Scripts", "Sometimes out-of-date scripts can cause the game to fail to start up (stuck on black screen).\n\n" +
					"You can manually clear the *.mg files (compiled scripts) in this folder to force the game to regenerate them the next time the game starts.\n\n" +
					"Please be aware that the game will freeze for a couple minutes on a white screen, while scripts are being compiled.")))
				{
					Application.OpenURL(System.IO.Path.Combine(Application.streamingAssetsPath, "CompiledUpdateScripts"));
				}


				GUILayout.EndHorizontal();
			}

			if (buttonRenderer(new GUIContent("Open Support Page: 07th-mod.com/wiki/Higurashi/support", "If you have problems with the game, the information on this site may help.\n\n" +
				"There are also instructions on reporting bugs, as well as a link to our Discord server to contact us directly")))
			{
				Application.OpenURL("https://07th-mod.com/wiki/Higurashi/support/");
			}
		}

		public void ToggleDebugMenu()
		{
			showDebugInfo = !showDebugInfo;
			MODAudioTracking.Instance.LoggingEnabled = showDebugInfo;
		}

	}
}
