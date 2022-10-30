using Assets.Scripts.Core;
using MOD.Scripts.Core;
using MOD.Scripts.Core.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;

namespace MOD.Scripts.UI
{
	class MODMenuNormal : MODMenuModuleInterface
	{
		private readonly MODMenu modMenu;
		private readonly MODMenuResolution resolutionMenu;
		private readonly MODMenuAudioOptions audioOptionsMenu;

		private GUIContent[] defaultArtsetDescriptions;
		private readonly bool hasOGBackgrounds;
		private bool hasMangaGamerSprites;

		private readonly MODRadio radioCensorshipLevel;
		private readonly MODRadio radioLipSync;
		private readonly string radioLipSyncLabelActive = "Lip Sync for Console Sprites (Hotkey: 7)";
		private readonly string radioLipSyncLabelInactive = "Lip Sync (NOTE: Select 'Console' sprites or preset to enable)";
		private readonly GUIContent[] radioLipSyncActive;
		private readonly GUIContent[] radioLipSyncInactive;
		private readonly MODRadio radioOpenings;
		private readonly MODRadio radioHideCG;
		private readonly MODRadio radioBackgrounds;
		private readonly MODRadio radioArtSet;
		private readonly MODRadio radioStretchBackgrounds;
		private readonly MODRadio radioTextWindowModeAndCrop;
		private readonly MODRadio radioForceComputedLipsync;
		private readonly MODRadio radioFullscreenLock;

		private readonly MODTabControl tabControl;

		private readonly MODCustomFlagPreset customFlagPreset;

		private bool showDeveloperSubmenu;

		string TextField_ComputedLipSyncThreshold1;
		string TextField_ComputedLipSyncThreshold2;

		private static int gameClearClickCount = 3;

		public MODMenuNormal(MODMenu modMenu, MODMenuAudioOptions audioOptionsMenu)
		{
			this.modMenu = modMenu;
			this.audioOptionsMenu = audioOptionsMenu;
			this.resolutionMenu = new MODMenuResolution();

			hasOGBackgrounds = MODActions.HasOGBackgrounds();

			defaultArtsetDescriptions = new GUIContent[] {
				new GUIContent("Console", "Use the Console sprites"),
				new GUIContent("MangaGamer", "Use Mangagamer's remake sprites"),
				new GUIContent("Original", "Use Original/Ryukishi sprites"),
			};

			string baseCensorshipDescription = @"

Sets the script censorship level
- This setting exists because the voices are taken from the censored, Console versions of the game, so no voices exist for the PC uncensored dialogue.
- We recommend the default level (2), the most balanced option. Using this option, only copyright changes, innuendos, and a few words will be changed.
  - 5: Full PS3 script fully voiced (most censored)
  - 2: Default - most balanced option
  - 0: Original PC Script with voices where it fits (fully uncensored), but uncensored scenes may be missing voices";

			radioCensorshipLevel = new MODRadio("Voice Matching Level (Hotkey: F2)", new GUIContent[] {
				new GUIContent("0", "Censorship level 0 - Equivalent to PC" + baseCensorshipDescription),
				new GUIContent("1", "Censorship level 1" + baseCensorshipDescription),
				new GUIContent("2*", "Censorship level 2 (this is the default/recommended value)" + baseCensorshipDescription),
				new GUIContent("3", "Censorship level 3" + baseCensorshipDescription),
				new GUIContent("4", "Censorship level 4" + baseCensorshipDescription),
				new GUIContent("5", "Censorship level 5 - Equivalent to Console" + baseCensorshipDescription),
				});

			radioLipSyncActive = new GUIContent[]
			{
				new GUIContent("Lip Sync Off", "Disables Lip Sync for Console Sprites"),
				new GUIContent("Lip Sync On", "Enables Lip Sync for Console Sprites"),
			};

			radioLipSyncInactive = new GUIContent[]
			{
				new GUIContent("Lip Sync Off (Inactive)", "Disables Lip Sync for Console Sprites\n\nNOTE: Lip Sync only works with Console sprites - please select 'Console' preset or sprites"),
				new GUIContent("Lip Sync On (Inactive)", "Enables Lip Sync for Console Sprites\n\nNOTE: Lip Sync only works with Console sprites - please select 'Console' preset or sprites"),
			};

			radioLipSync = new MODRadio(radioLipSyncLabelActive, radioLipSyncActive);

			radioOpenings = new MODRadio("Opening Movies (Hotkey: Shift-F12)", new GUIContent[]
			{
				new GUIContent("Disabled", "Disables all opening videos"),
				new GUIContent("Enabled", "Enables opening videos\n\n" +
				"NOTE: Once the opening video plays the first time, will automatically switch to 'Launch + In-Game'\n\n" +
				"We have setup openings this way to avoid spoilers."),
				new GUIContent("Launch + In-Game", "WARNING: There is usually no need to set this manually.\n\n" +
				"If openings are enabled, the first time you reach an opening while playing the game, this flag will be set automatically\n\n" +
				"That is, after the opening is played the first time, from then on openings will play every time the game launches"),
			});

			radioHideCG = new MODRadio("Show/Hide CGs", new GUIContent[]
			{
				new GUIContent("Show CGs", "Shows CGs (You probably want this enabled for Console ADV/NVL mode)"),
				new GUIContent("Hide CGs", "Disables all CGs (mainly for use with the Original/Ryukishi preset)"),
			});

			radioBackgrounds = new MODRadio("Background Style", new GUIContent[]{
				new GUIContent("Console BGs", "Use Console backgrounds"),
				new GUIContent("Original BGs", "Use Original/Ryukishi backgrounds."),
			}, itemsPerRow: 2);

			radioStretchBackgrounds = new MODRadio("Background Stretching", new GUIContent[]
			{
				new GUIContent("BG Stretching Off", "Makes backgrounds as big as possible without any stretching (Keep Aspect Ratio)"),
				new GUIContent("BG Stretching On", "Stretches backgrounds to fit the screen (Ignore Aspect Ratio)\n\n" +
				"Mainly for use with the Original BGs, which are in 4:3 aspect ratio."),
			});

			radioArtSet = new MODRadio("Sprite Style", defaultArtsetDescriptions, itemsPerRow: 3);

			radioTextWindowModeAndCrop = new MODRadio("Text Window Appearance", new GUIContent[]{
				new GUIContent("ADV Mode", "This option:\n" +
				"- Makes text show at the bottom of the screen in a textbox\n" +
				"- Shows the name of the current character on the textbox\n"),
				new GUIContent("NVL Mode", "This option:\n" +
				"- Makes text show across the whole screen\n"),
				new GUIContent("Original", "This option:\n" +
				"- Darkens the whole screen to emulate the original game\n" +
				"- Makes text show only in a 4:3 section of the screen (narrower than NVL mode)\n"),
			}, itemsPerRow: 2);

			radioForceComputedLipsync = new MODRadio("Force Computed Lipsync", new GUIContent[] {
				new GUIContent("As Fallback", "Only use computed lipsync if there is no baked 'spectrum' file for a given .ogg file"),
				new GUIContent("Computed Always", "Always use computed lipsync for all voices. Any 'spectrum' files will be ignored.")
			});

			radioFullscreenLock = new MODRadio("Fullscreen Lock", new GUIContent[] {
				new GUIContent("No Lock", "Allow switching to Windowed mode at any time"),
				new GUIContent("Force Fullscreen Always", "Force fullscreen mode always - do not allow switching to windowed mode.")
			});

			tabControl = new MODTabControl(new List<MODTabControl.TabProperties>
			{
				new MODTabControl.TabProperties("Gameplay", "Voice Matching and Opening Videos", GameplayTabOnGUI),
				new MODTabControl.TabProperties("Graphics", "Sprites, Backgrounds, CGs, Resolution", GraphicsTabOnGUI),
				new MODTabControl.TabProperties("Audio", "BGM and SE options", AudioTabOnGUI),
				new MODTabControl.TabProperties("Troubleshooting", "Tools to help you if something goes wrong", TroubleShootingTabOnGUI),
			});

			customFlagPreset = Assets.Scripts.Core.Buriko.BurikoMemory.Instance.GetCustomFlagPresetInstance();
		}

		public void OnGUI()
		{
			tabControl.OnGUI();
		}

		public void OnBeforeMenuVisible() {
			// Update the artset radio buttons/descriptions, as these are set by ModAddArtset() calls in init.txt at runtime
			// Technically only need to do this once after init.txt has been called, but it's easier to just do it each time menu is opened
			GUIContent[] descriptions = Core.MODSystem.instance.modTextureController.GetArtStyleDescriptions();
			for (int i = 0; i < descriptions.Length; i++)
			{
				if (i < this.defaultArtsetDescriptions.Length)
				{
					descriptions[i] = this.defaultArtsetDescriptions[i];
				}
			}
			this.radioArtSet.SetContents(descriptions);

			hasMangaGamerSprites = descriptions.Length > 1;

			resolutionMenu.OnBeforeMenuVisible();
			audioOptionsMenu.OnBeforeMenuVisible();

			GameSystem.Instance.SceneController.MODGetExpressionThresholds(out float threshold1, out float threshold2);
			TextField_ComputedLipSyncThreshold1 = threshold1.ToString();
			TextField_ComputedLipSyncThreshold2 = threshold2.ToString();

			gameClearClickCount = 3;
		}

		private void GraphicsTabOnGUI()
		{
			Label("Graphics Presets (Hotkey: F1)");
			{
				GUILayout.BeginHorizontal();

				int advNVLRyukishiMode = MODActions.GetADVNVLRyukishiModeFromFlags(out bool presetModified);

				if (Button(new GUIContent("Console", "This preset:\n" +
				"- Makes text show at the bottom of the screen in a textbox\n" +
				"- Shows the name of the current character on the textbox\n" +
				"- Uses the console sprites (with lipsync) and console backgrounds\n" +
				"- Displays in 16:9 widescreen\n\n" +
				"Note that sprites and backgrounds can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available"), selected: !customFlagPreset.Enabled && !presetModified && advNVLRyukishiMode == 0))
				{
					MODActions.SetGraphicsPreset(MODActions.ModPreset.Console, showInfoToast: false);
				}

				if (this.hasMangaGamerSprites && Button(new GUIContent("MangaGamer", "This preset:\n" +
					"- Makes text show across the whole screen\n" +
					"- Uses the Mangagamer remake sprites and Console backgrounds\n" +
					"- Displays in 16:9 widescreen\n\n" +
					"Note that sprites and backgrounds can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available"), selected: !customFlagPreset.Enabled && !presetModified && advNVLRyukishiMode == 1))
				{
					MODActions.SetGraphicsPreset(MODActions.ModPreset.MangaGamer, showInfoToast: false);
				}

				if (this.hasOGBackgrounds &&
					Button(new GUIContent("Original/Ryukishi", "This preset makes the game behave similarly to the unmodded game:\n" +
					"- Displays backgrounds in 4:3 'standard' aspect\n" +
					"- CGs are disabled (Can be re-enabled, see 'Show/Hide CGs')\n" +
					"- Switches to original sprites and backgrounds\n\n" +
					"Note that sprites, backgrounds, and CG hiding can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available"), selected: !customFlagPreset.Enabled && !presetModified && advNVLRyukishiMode == 2))
				{
					MODActions.SetGraphicsPreset(MODActions.ModPreset.OG, showInfoToast: false);
				}

				if (Button(new GUIContent("Custom", "Your own custom preset, using the options below.\n\n" +
					"This custom preset will be saved, even when you switch to the other presets."), selected: customFlagPreset.Enabled))
				{
					MODActions.LoadCustomGraphicsPreset(showInfoToast: false);
				}

				GUILayout.EndHorizontal();
			}

			HeadingLabel("Advanced Options");


			// Show warning message if lip sync would have no effect
			if (Core.MODSystem.instance.modTextureController.GetArtStyle() == 0)
			{
				radioLipSync.SetLabel(radioLipSyncLabelActive);
				radioLipSync.SetContents(radioLipSyncActive);
			}
			else
			{
				radioLipSync.SetLabel(radioLipSyncLabelInactive);
				radioLipSync.SetContents(radioLipSyncInactive);
			}

			if (this.radioLipSync.OnGUIFragment(GetGlobal("GLipSync")) is int lipSyncEnabled)
			{
				MODActions.SetFlagFromUserInput("GLipSync", lipSyncEnabled, showInfoToast: false);
			};

			if (this.radioHideCG.OnGUIFragment(GetGlobal("GHideCG")) is int hideCG)
			{
				MODActions.SetFlagFromUserInput("GHideCG", hideCG, showInfoToast: false);
			};

			if (this.radioArtSet.OnGUIFragment(Core.MODSystem.instance.modTextureController.GetArtStyle()) is int artStyle)
			{
				MODActions.SetArtStyle(artStyle, showInfoToast: false);
			}

			if (this.hasOGBackgrounds)
			{
				if (this.radioBackgrounds.OnGUIFragment(GetGlobal("GBackgroundSet")) is int background)
				{
					MODActions.SetFlagFromUserInput("GBackgroundSet", background, showInfoToast: false);
					GameSystem.Instance.SceneController.ReloadAllImages();
				}

				if (this.radioStretchBackgrounds.OnGUIFragment(GetGlobal("GStretchBackgrounds")) is int stretchBackgrounds)
				{
					MODActions.SetFlagFromUserInput("GStretchBackgrounds", stretchBackgrounds, showInfoToast: false);
					GameSystem.Instance.SceneController.ReloadAllImages();
				}
			}

			if (this.radioTextWindowModeAndCrop.OnGUIFragment(MODActions.GetADVNVLRyukishiModeFromFlags()) is int windowMode)
			{
				MODActions.SetTextWindowAppearance((MODActions.ModPreset) windowMode, showInfoToast: false);
				GameSystem.Instance.SceneController.ReloadAllImages();
			}

			if (Assets.Scripts.Core.Buriko.BurikoMemory.Instance.GetFlag("NVL_in_ADV").IntValue() == 1)
			{
				Label("WARNING: You have ADV mode enabled, but you are in a forced-NVL section, so the game will display in NVL mode temporarily!");
			}

			HeadingLabel("Resolution");

			resolutionMenu.OnGUI();

			OnGUIFullscreenLock();
		}

		private void GameplayTabOnGUI()
		{
			if (this.radioCensorshipLevel.OnGUIFragment(GetGlobal("GCensor")) is int censorLevel)
			{
				SetGlobal("GCensor", censorLevel);
			};

			if (this.radioOpenings.OnGUIFragment(GetGlobal("GVideoOpening") - 1) is int openingVideoLevelZeroIndexed)
			{
				SetGlobal("GVideoOpening", openingVideoLevelZeroIndexed + 1);
			};
		}


		private void AudioTabOnGUI()
		{
			this.audioOptionsMenu.OnGUI();

			HeadingLabel("Advanced Options");

			this.audioOptionsMenu.AdvancedOnGUI();
		}

		private static void ShowExperimentalGameplayTools()
		{
			Label("Experimental Gameplay Tools");
			{
				GUILayout.BeginHorizontal();

				bool gameClear = GetGlobal("GFlag_GameClear") != 0;

				string gameClearButtonText;
				if (gameClearClickCount == 0)
				{
					gameClearButtonText = $"{(gameClear ? "Game Clear Forced!" : "All Progress Reset!")} Please reload menu/game!";
				}
				else
				{
					gameClearButtonText = $"{(gameClear ? "Reset All Progress" : "Force Game Clear")} (Click {gameClearClickCount} times to confirm)";
				}

				string gameClearButtonDescription =
					"WARNING: This option will toggle your game clear status:" +
					"\n - If you haven't finished the game, it will unlock everything." +
					"\n - If you have already finished the game, it will reset all your progress!" +
					"\n\nThis toggles the EXTRAS menu, including all TIPS, and all chapter jump chapters." +
					"\n\nYou may need to reload the current menu or restart the game before you can see the changes." +
					"\n\nSaves shouldn't be affected.";

				if (Button(new GUIContent(gameClearButtonText, gameClearButtonDescription)))
				{
					if (gameClearClickCount == 1)
					{
						if (gameClear)
						{
							SetGlobal("GFlag_GameClear", 0);
							SetGlobal("GHighestChapter", 0);
						}
						else
						{
							SetGlobal("GFlag_GameClear", 1);
							SetGlobal("GHighestChapter", 999);
						}
					}

					if (gameClearClickCount > 0)
					{
						gameClearClickCount--;
					}
				}

				Label($"Game Cleared?: {(GetGlobal("GFlag_GameClear") == 0 ? "No" : "Yes")}" +
					$"\nHighest Chapter: {GetGlobal("GHighestChapter")}");

				GUILayout.EndHorizontal();
			}
		}

		private void TroubleShootingTabOnGUI()
		{
			ShowExperimentalGameplayTools();

			MODMenuSupport.ShowSupportButtons(content => Button(content));

			HeadingLabel("Developer Tools");

			if(showDeveloperSubmenu)
			{
				OnGUIRestoreSettings();

				Label("Developer Debug Menu");
				GUILayout.BeginHorizontal();
				if (Button(new GUIContent("Toggle debug menu (Shift-F9)", "Toggle the debug menu")))
				{
					modMenu.ToggleDebugMenu();
				}
				if (Button(new GUIContent("Toggle flag menu (Shift-F10)", "Toggle the flag menu. Toggle Multiple times for more options.\n\nNote: 3rd and 4th panels are only shown if GMOD_DEBUG_MODE is true.")))
				{
					MODActions.ToggleFlagMenu();
				}
				GUILayout.EndHorizontal();

				OnGUIComputedLipsync();
			}
			else
			{
				if (Button(new GUIContent("Show Developer Tools: Only click if asked by developers", "Show the Developer Tools.\n\nOnly click this button if you're asked to by the developers.")))
				{
					showDeveloperSubmenu = true;
				}
			}
		}

		private void OnGUIRestoreSettings()
		{
			Label("Flag Unlocks");
			GUILayout.BeginHorizontal();
			{
				if (Button(new GUIContent($"Toggle GFlag_GameClear (is {GetGlobal("GFlag_GameClear")})", "Toggle the 'GFlag_GameClear' flag which is normally activated when you complete the game. Unlocks things like the All-cast review.")))
				{
					SetGlobal("GFlag_GameClear", GetGlobal("GFlag_GameClear") == 0 ? 1 : 0);
				}

				if (Button(new GUIContent($"Toggle GHighestChapter 0 <-> 999 (is {GetGlobal("GHighestChapter")})", "Toggle the 'GHighestChapter' flag which indicates the highest chapter you have completed.\n\n" +
					"When >= 1, unlocks the extras menu.\n" +
					"Also unlocks other things like which chapters are shown on the chapter jump menu, etc.")))
				{
					bool isZero = GetGlobal("GHighestChapter") == 0;
					SetGlobal("GHighestChapter", isZero ? 999 : 0);
				}
			}
			GUILayout.EndHorizontal();

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

		private void OnGUIComputedLipsync()
		{
			Label("Computed Lipsync Options");
			GUILayout.BeginHorizontal();
			Label(new GUIContent("LipSync Thresh 1: ", "Above this threshold, expression 1 will be used.\n\n" +
				"Below or equal to this threshold, expression 0 will be used.\n\n" +
				"Only saved until the game restarts"));
			TextField_ComputedLipSyncThreshold1 = GUILayout.TextField(TextField_ComputedLipSyncThreshold1);

			Label(new GUIContent("LipSync Thresh 2: ", "Above this thireshold, expression 2 will be used\n\n" +
				"Only saved until the game restarts"));
			TextField_ComputedLipSyncThreshold2 = GUILayout.TextField(TextField_ComputedLipSyncThreshold2);

			if (Button(new GUIContent("Set", "Tries to set the given lipsync thresholds\n\n")))
			{
				if (float.TryParse(TextField_ComputedLipSyncThreshold1, out float threshold1) &&
					float.TryParse(TextField_ComputedLipSyncThreshold2, out float threshold2) &&
					threshold1 >= 0 &&
					threshold1 <= 1 &&
					threshold2 >= 0 &&
					threshold2 <= 1)
				{
					GameSystem.Instance.SceneController.MODSetExpressionThresholds(threshold1, threshold2);
				}
				else
				{
					MODToaster.Show("Invalid thresholds - each threshold should be a value between 0 and 1");
				}
			}
			GUILayout.EndHorizontal();

			if(this.radioForceComputedLipsync.OnGUIFragment(GameSystem.Instance.SceneController.MODGetForceComputedLipsync()) is bool newForceComputedLipsync)
			{
				GameSystem.Instance.SceneController.MODSetForceComputedLipsync(newForceComputedLipsync);
			}
		}

		public void OnGUIFullscreenLock()
		{
			HeadingLabel("Fullscreen Lock");

			int currentValue = -1;
			if(MODWindowManager.FullscreenLockConfigured())
			{
				currentValue = MODWindowManager.FullscreenLocked() ? 1 : 0;
			}

			if (this.radioFullscreenLock.OnGUIFragment(currentValue) is int newFullscreenlock)
			{
				MODWindowManager.SetFullScreenLock(newFullscreenlock == 0 ? false : true);
				MODToaster.Show(MODWindowManager.FullscreenLocked() ? "Fullscreen lock enabled" : "Fullscreen lock disabled");
			}
		}

		public bool UserCanClose() => true;
		public string Heading() => "Mod Options Menu";

		public string DefaultTooltip()
		{
			return @"Hover over a button on the left panel for its description.

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
		}
	}
}
