using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using MOD.Scripts.Core.Audio;
using MOD.Scripts.Core.Localization;
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
		private readonly string radioLipSyncLabelActive = Loc.MODMenuNormal_0; //Lip Sync for Console Sprites (Hotkey: 7)
		private readonly string radioLipSyncLabelInactive = Loc.MODMenuNormal_1; //Lip Sync (NOTE: Select 'Console' sprites or preset to enable)
		private readonly GUIContent[] radioLipSyncActive;
		private readonly GUIContent[] radioLipSyncInactive;
		private readonly MODRadio radioOpenings;
		private readonly MODRadio radioHideCG;
		private readonly MODRadio radioBackgrounds;
		private readonly MODRadio radioArtSet;
		private readonly MODRadio radioStretchBackgrounds;
		private readonly MODRadio radioTextWindowModeAndCrop;
		private readonly MODRadio radioForceComputedLipsync;
		private readonly MODRadio radioRyukishiExperimentalAspect;

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
				new GUIContent(Loc.MODMenuNormal_2, Loc.MODMenuNormal_3), //Console | Use the Console sprites
				new GUIContent(Loc.MODMenuNormal_4, Loc.MODMenuNormal_5), //MangaGamer | Use Mangagamer's remake sprites
				new GUIContent(Loc.MODMenuNormal_6, Loc.MODMenuNormal_7), //Original | Use Original/Ryukishi sprites
			};

			string baseCensorshipDescription = Loc.MODMenuNormal_128; //\n\nSets the script censorship level\n- This setting exists because the voices are taken from the censored, Console versions of the game, so no voices exist for the PC uncensored dialogue.\n- We recommend the default level (2), the most balanced option. Using this option, only copyright changes, innuendos, and a few words will be changed.\n  - 5: Use voiced lines from PS3 script as much as possible (censored at parts)\n  - 2: Default - most balanced option\n  - 0: Original PC Script with voices where it fits (fully uncensored), but uncensored scenes may be missing voices

			radioCensorshipLevel = new MODRadio(Loc.MODMenuNormal_8, new GUIContent[] { //Voice Matching Level (Hotkey: F2)
				new GUIContent("0", Loc.MODMenuNormal_9 + baseCensorshipDescription), //Censorship level 0 - Equivalent to PC
				new GUIContent("1", Loc.MODMenuNormal_10 + baseCensorshipDescription), //Censorship level 1
				new GUIContent("2*", Loc.MODMenuNormal_11 + baseCensorshipDescription), //Censorship level 2 (this is the default/recommended value)
				new GUIContent("3", Loc.MODMenuNormal_12 + baseCensorshipDescription), //Censorship level 3
				new GUIContent("4", Loc.MODMenuNormal_13 + baseCensorshipDescription), //Censorship level 4
				new GUIContent("5", Loc.MODMenuNormal_14 + baseCensorshipDescription), //Censorship level 5 - Equivalent to Console
				});

			radioLipSyncActive = new GUIContent[]
			{
				new GUIContent(Loc.MODMenuNormal_15, Loc.MODMenuNormal_16), //Lip Sync Off | Disables Lip Sync for Console Sprites
				new GUIContent(Loc.MODMenuNormal_17, Loc.MODMenuNormal_18), //Lip Sync On | Enables Lip Sync for Console Sprites
			};

			radioLipSyncInactive = new GUIContent[]
			{
				new GUIContent(Loc.MODMenuNormal_19, Loc.MODMenuNormal_20), //Lip Sync Off (Inactive) | Disables Lip Sync for Console Sprites\n\nNOTE: Lip Sync only works with Console sprites - please select 'Console' preset or sprites
				new GUIContent(Loc.MODMenuNormal_21, Loc.MODMenuNormal_22), //Lip Sync On (Inactive) | Enables Lip Sync for Console Sprites\n\nNOTE: Lip Sync only works with Console sprites - please select 'Console' preset or sprites
			};

			radioLipSync = new MODRadio(radioLipSyncLabelActive, radioLipSyncActive);

			radioOpenings = new MODRadio(Loc.MODMenuNormal_23, new GUIContent[] //Opening Movies (Hotkey: Shift-F12)
			{
				new GUIContent(Loc.MODMenuNormal_24, Loc.MODMenuNormal_25), //Disabled | Disables all opening videos
				new GUIContent(Loc.MODMenuNormal_26, Loc.MODMenuNormal_27), //Enabled | Enables opening videos\n\nNOTE: Once the opening video plays the first time, will automatically switch to 'Launch + In-Game'\n\nWe have setup openings this way to avoid spoilers.
				new GUIContent(Loc.MODMenuNormal_28, Loc.MODMenuNormal_29), //Launch + In-Game | WARNING: There is usually no need to set this manually.\n\nIf openings are enabled, the first time you reach an opening while playing the game, this flag will be set automatically\n\nThat is, after the opening is played the first time, from then on openings will play every time the game launches
			});

			radioHideCG = new MODRadio(Loc.MODMenuNormal_30, new GUIContent[] //Show/Hide CGs
			{
				new GUIContent(Loc.MODMenuNormal_31, Loc.MODMenuNormal_32), //Show CGs | Shows CGs (You probably want this enabled for Console ADV/NVL mode)
				new GUIContent(Loc.MODMenuNormal_33, Loc.MODMenuNormal_34), //Hide CGs | Disables all CGs (mainly for use with the Original/Ryukishi preset)
			});

			radioBackgrounds = new MODRadio(Loc.MODMenuNormal_35, new GUIContent[]{ //Background Style
				new GUIContent(Loc.MODMenuNormal_36, Loc.MODMenuNormal_37), //Console BGs | Use Console backgrounds
				new GUIContent(Loc.MODMenuNormal_38, Loc.MODMenuNormal_39), //Original BGs | Use Original/Ryukishi backgrounds.
			}, itemsPerRow: 2);

			radioStretchBackgrounds = new MODRadio(Loc.MODMenuNormal_40, new GUIContent[] //Background Stretching
			{
				new GUIContent(Loc.MODMenuNormal_41, Loc.MODMenuNormal_42), //BG Stretching Off | Makes backgrounds as big as possible without any stretching (Keep Aspect Ratio)
				new GUIContent(Loc.MODMenuNormal_43, Loc.MODMenuNormal_44), //BG Stretching On | Stretches backgrounds to fit the screen (Ignore Aspect Ratio)\n\nMainly for use with the Original BGs, which are in 4:3 aspect ratio.
			});

			radioArtSet = new MODRadio(Loc.MODMenuNormal_45, defaultArtsetDescriptions, itemsPerRow: 3); //Sprite Style

			radioTextWindowModeAndCrop = new MODRadio(Loc.MODMenuNormal_46, new GUIContent[]{ //Text Window Appearance
				new GUIContent(Loc.MODMenuNormal_47, Loc.MODMenuNormal_48), //ADV Mode | This option:\n- Makes text show at the bottom of the screen in a textbox\n- Shows the name of the current character on the textbox\n
				new GUIContent(Loc.MODMenuNormal_49, Loc.MODMenuNormal_50), //NVL Mode | This option:\n- Makes text show across the whole screen\n
				new GUIContent(Loc.MODMenuNormal_51, Loc.MODMenuNormal_52), //Original | This option:\n- Darkens the whole screen to emulate the original game\n- Makes text show only in a 4:3 section of the screen (narrower than NVL mode)\n
			}, itemsPerRow: 2);

			radioForceComputedLipsync = new MODRadio(Loc.MODMenuNormal_53, new GUIContent[] { //Force Computed Lipsync
				new GUIContent(Loc.MODMenuNormal_54, Loc.MODMenuNormal_55), //As Fallback | Only use computed lipsync if there is no baked 'spectrum' file for a given .ogg file
				new GUIContent(Loc.MODMenuNormal_56, Loc.MODMenuNormal_57) //Computed Always | Always use computed lipsync for all voices. Any 'spectrum' files will be ignored.
			});

			tabControl = new MODTabControl(new List<MODTabControl.TabProperties>
			{
				new MODTabControl.TabProperties(Loc.MODMenuNormal_58, Loc.MODMenuNormal_59, GameplayTabOnGUI), //Gameplay | Voice Matching and Opening Videos
				new MODTabControl.TabProperties(Loc.MODMenuNormal_60, Loc.MODMenuNormal_61, GraphicsTabOnGUI), //Graphics | Sprites, Backgrounds, CGs, Resolution
				new MODTabControl.TabProperties(Loc.MODMenuNormal_62, Loc.MODMenuNormal_63, AudioTabOnGUI), //Audio | BGM and SE options
				new MODTabControl.TabProperties(Loc.MODMenuNormal_64, Loc.MODMenuNormal_65, TroubleShootingTabOnGUI), //Troubleshooting | Tools to help you if something goes wrong
			});

			radioRyukishiExperimentalAspect = new MODRadio(Loc.MODMenuNormal_66, new GUIContent[]{ //Original/Ryukishi Experimental 4:3 Aspect Ratio
				new GUIContent(Loc.MODMenuNormal_67, Loc.MODMenuNormal_68), //16:9 (default) | The game's aspect ratio will be 16:9.\n\nWhen playing in OG mode, the left and right of the screen will be padded to 4:3 with black bars.
				new GUIContent("4:3", Loc.MODMenuNormal_69 //The game's aspect ratio will be 4:3, however this may cause some issues when playing our mod.\nOnly use this option if your monitor is 4:3 aspect ratio, like an old CRT monitor.\n\nPlease note the following:\n\n - You should enable the Original/Ryukishi preset before enabling this option. Using other settings should all work, but are not well tested.\n\n - 16:9 images will be squished to fit in 4:3 so they don't get cut off. This includes text images, and PS3 CG images if enabled.\n\n - You may see graphical artifacts just after this is enabled - they should fix after the next character transition or text clear\n\n
				),
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
			Label(Loc.MODMenuNormal_70); //Graphics Presets (Hotkey: F1)
			{
				GUILayout.BeginHorizontal();

				int advNVLRyukishiMode = MODActions.GetADVNVLRyukishiModeFromFlags(out bool presetModified);

				if (Button(new GUIContent(Loc.MODMenuNormal_71, Loc.MODMenuNormal_72), selected: !customFlagPreset.Enabled && !presetModified && advNVLRyukishiMode == 0)) //Console | This preset:\n- Makes text show at the bottom of the screen in a textbox\n- Shows the name of the current character on the textbox\n- Uses the console sprites (with lipsync) and console backgrounds\n- Displays in 16:9 widescreen\n\nNote that sprites and backgrounds can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available
				{
					MODActions.SetGraphicsPreset(MODActions.ModPreset.Console, showInfoToast: false);
				}

				if (this.hasMangaGamerSprites && Button(new GUIContent(Loc.MODMenuNormal_73, Loc.MODMenuNormal_74), selected: !customFlagPreset.Enabled && !presetModified && advNVLRyukishiMode == 1)) //MangaGamer | This preset:\n- Makes text show across the whole screen\n- Uses the Mangagamer remake sprites and Console backgrounds\n- Displays in 16:9 widescreen\n\nNote that sprites and backgrounds can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available
				{
					MODActions.SetGraphicsPreset(MODActions.ModPreset.MangaGamer, showInfoToast: false);
				}

				if (this.hasOGBackgrounds &&
					Button(new GUIContent(Loc.MODMenuNormal_75, Loc.MODMenuNormal_76), selected: !customFlagPreset.Enabled && !presetModified && advNVLRyukishiMode == 2)) //Original/Ryukishi | This preset makes the game behave similarly to the unmodded game:\n- Displays backgrounds in 4:3 'standard' aspect\n- CGs are disabled (Can be re-enabled, see 'Show/Hide CGs')\n- Switches to original sprites and backgrounds\n\nNote that sprites, backgrounds, and CG hiding can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available
				{
					MODActions.SetGraphicsPreset(MODActions.ModPreset.OG, showInfoToast: false);
				}

				if (Button(new GUIContent(Loc.MODMenuNormal_77, Loc.MODMenuNormal_78), selected: customFlagPreset.Enabled)) //Custom | Your own custom preset, using the options below.\n\nThis custom preset will be saved, even when you switch to the other presets.
				{
					MODActions.LoadCustomGraphicsPreset(showInfoToast: false);
				}

				GUILayout.EndHorizontal();
			}

			HeadingLabel(Loc.MODMenuNormal_79); //Advanced Options


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
					GameSystem.Instance.UpdateAspectRatio();
					GameSystem.Instance.SceneController.ReloadAllImages();
				}

				if (this.radioStretchBackgrounds.OnGUIFragment(GetGlobal("GStretchBackgrounds")) is int stretchBackgrounds)
				{
					MODActions.SetFlagFromUserInput("GStretchBackgrounds", stretchBackgrounds, showInfoToast: false);
					GameSystem.Instance.UpdateAspectRatio();
					GameSystem.Instance.SceneController.ReloadAllImages();
				}
			}

			if (this.radioTextWindowModeAndCrop.OnGUIFragment(MODActions.GetADVNVLRyukishiModeFromFlags()) is int windowMode)
			{
				MODActions.SetTextWindowAppearance((MODActions.ModPreset) windowMode, showInfoToast: false);
				GameSystem.Instance.SceneController.ReloadAllImages();
			}

			if (this.radioRyukishiExperimentalAspect.OnGUIFragment(GetGlobal("GRyukishiMode43Aspect")) is int experimentalAspect)
			{
				SetGlobal("GRyukishiMode43Aspect", experimentalAspect);
				GameSystem.Instance.UpdateAspectRatio();
				GameSystem.Instance.SceneController.ReloadAllImages();
			}

			if (Assets.Scripts.Core.Buriko.BurikoMemory.Instance.GetFlag("NVL_in_ADV").IntValue() == 1)
			{
				Label(Loc.MODMenuNormal_80); //WARNING: You have ADV mode enabled, but you are in a forced-NVL section, so the game will display in NVL mode temporarily!
			}

			HeadingLabel(Loc.MODMenuNormal_81); //Resolution

			resolutionMenu.OnGUI();
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

			HeadingLabel(Loc.MODMenuNormal_82); //Advanced Options

			this.audioOptionsMenu.AdvancedOnGUI();
		}

		private static void ShowExperimentalGameplayTools()
		{
			Label(Loc.MODMenuNormal_83); //Experimental Gameplay Tools
			{
				GUILayout.BeginHorizontal();

				bool gameClear = GetGlobal("GFlag_GameClear") != 0;

				string gameClearButtonText;
				if (gameClearClickCount == 0)
				{
					string clearStateIndicator = gameClear ? Loc.MODMenuNormal_84 : Loc.MODMenuNormal_85; //Game Clear Forced! | All Progress Reset!
					gameClearButtonText = clearStateIndicator + " " + Loc.MODMenuNormal_86; //Please reload menu/game!
				}
				else
				{
					string clearStateIndicator = gameClear ? Loc.MODMenuNormal_87 : Loc.MODMenuNormal_88; //Reset All Progress | Force Game Clear
					gameClearButtonText = clearStateIndicator + "(" + Loc.MODMenuNormal_89 + $"{gameClearClickCount}" + Loc.MODMenuNormal_90 + ")"; //Click | more times
				}

				string gameClearButtonDescription =
					Loc.MODMenuNormal_91; //WARNING: This option will toggle your game clear status:\n - If you haven't finished the game, it will unlock everything.\n - If you have already finished the game, it will reset all your progress!\n\nThis toggles the EXTRAS menu, including all TIPS, and all chapter jump chapters.\n\nYou may need to reload the current menu or restart the game before you can see the changes.\n\nSaves shouldn't be affected.

				if (Button(new GUIContent(gameClearButtonText, gameClearButtonDescription)))
				{
					if (gameClearClickCount == 1)
					{
						// Some flags might not be available on all chapters, but try to set them anyway
						if (gameClear)
						{
							TrySetGlobal("GFlag_GameClear", 0);
							TrySetGlobal("GHighestChapter", 0);

							// Rei
							TrySetGlobal("GOmakeUnlock", 0);
							TrySetGlobal("GCastReview", 0);
							TrySetGlobal("GSaikoroshi", 0);
							TrySetGlobal("GHirukowashi", 0);
							TrySetGlobal("GBatsukoishi", 0);

							// Hou+
							TrySetGlobal("TEIEND", 1);
							TrySetGlobal("HIGUEND", 1);
							TrySetGlobal("MEHEND", 1);
							// Hou+: Get an achievement once you've read all 9 staff rooms
							for (int i = 1; i <= 9; i++)
							{
								TrySetGlobal($"ReadStaffRoom{i}", 0);
							}
						}
						else
						{
							TrySetGlobal("GFlag_GameClear", 1);
							TrySetGlobal("GHighestChapter", 999);

							// Rei
							TrySetGlobal("GOmakeUnlock", 1);
							TrySetGlobal("GCastReview", 1);
							TrySetGlobal("GSaikoroshi", 1);
							TrySetGlobal("GHirukowashi", 1);
							TrySetGlobal("GBatsukoishi", 1);

							// Hou+
							TrySetGlobal("TEIEND", 1);
							TrySetGlobal("HIGUEND", 1);
							TrySetGlobal("MEHEND", 1);
							// Hou+: Get an achievement once you've read all 9 staff rooms
							for (int i = 1; i <= 9; i++)
							{
								TrySetGlobal($"ReadStaffRoom{i}", 1);
							}



						}
					}

					if (gameClearClickCount > 0)
					{
						gameClearClickCount--;
					}
				}

				string gameClearedYesNo = GetGlobal("GFlag_GameClear") == 0 ? Loc.MODMenuNormal_92 : Loc.MODMenuNormal_93; //No | Yes

				Label(Loc.MODMenuNormal_94 + gameClearedYesNo + "\n" + Loc.MODMenuNormal_95 + $"{GetGlobal("GHighestChapter")}"); //Game Cleared?:  | Highest Chapter:

				GUILayout.EndHorizontal();
			}
		}

		private void TroubleShootingTabOnGUI()
		{
			ShowExperimentalGameplayTools();

			MODMenuSupport.ShowSupportButtons(content => Button(content));

			HeadingLabel(Loc.MODMenuNormal_96); //Developer Tools

			if(showDeveloperSubmenu)
			{
				OnGUIRestoreSettings();

				Label(Loc.MODMenuNormal_97); //Developer Debug Menu
				GUILayout.BeginHorizontal();
				if (Button(new GUIContent(Loc.MODMenuNormal_98, Loc.MODMenuNormal_99))) //Toggle debug menu (Shift-F9) | Toggle the debug menu
				{
					modMenu.ToggleDebugMenu();
				}
				if (Button(new GUIContent(Loc.MODMenuNormal_100, Loc.MODMenuNormal_101))) //Toggle flag menu (Shift-F10) | Toggle the flag menu. Toggle Multiple times for more options.\n\nNote: 3rd and 4th panels are only shown if GMOD_DEBUG_MODE is true.
				{
					MODActions.ToggleFlagMenu();
				}
				GUILayout.EndHorizontal();

				OnGUIComputedLipsync();

				Label("Font Debugging");
				if (Button(new GUIContent("Font Debug is on the Draggable Debug Menu", "The font debug settings are on the draggable debug menu, to avoid it blocking the screen while you see the fonts change.")))
				{
					modMenu.ToggleDebugMenu();
				}
			}
			else
			{
				if (Button(new GUIContent(Loc.MODMenuNormal_102, Loc.MODMenuNormal_103))) //Show Developer Tools: Only click if asked by developers | Show the Developer Tools.\n\nOnly click this button if you're asked to by the developers.
				{
					showDeveloperSubmenu = true;
				}
			}
		}

		private void OnGUIRestoreSettings()
		{
			Label(Loc.MODMenuNormal_104); //Flag Unlocks
			GUILayout.BeginHorizontal();
			{
				if (Button(new GUIContent(Loc.MODMenuNormal_105 + $"{GetGlobal("GFlag_GameClear")}", Loc.MODMenuNormal_106))) //Toggle GFlag_GameClear is  | Toggle the 'GFlag_GameClear' flag which is normally activated when you complete the game. Unlocks things like the All-cast review.
				{
					SetGlobal("GFlag_GameClear", GetGlobal("GFlag_GameClear") == 0 ? 1 : 0);
				}

				if (Button(new GUIContent(Loc.MODMenuNormal_107 + $"{GetGlobal("GHighestChapter")}", Loc.MODMenuNormal_108))) //Toggle GHighestChapter 0 <-> 999 | Is  | Toggle the 'GHighestChapter' flag which indicates the highest chapter you have completed.\n\nWhen >= 1, unlocks the extras menu.\nAlso unlocks other things like which chapters are shown on the chapter jump menu, etc.
				{
					bool isZero = GetGlobal("GHighestChapter") == 0;
					SetGlobal("GHighestChapter", isZero ? 999 : 0);
				}
			}
			GUILayout.EndHorizontal();

			string restoreSettingsRestartPending = GetGlobal("GMOD_SETTING_LOADER") == 3 ? "" : ": <" + Loc.MODMenuNormal_109 + ">"; //Restart Pending
			Label(Loc.MODMenuNormal_110 + restoreSettingsRestartPending); //Restore Settings

			GUILayout.BeginHorizontal();
			if (GetGlobal("GMOD_SETTING_LOADER") == 3)
			{
				if (Button(new GUIContent(Loc.MODMenuNormal_111, Loc.MODMenuNormal_112))) //ADV defaults | This restores flags to the defaults for NVL mode\nNOTE: Requires you to relaunch the game 2 times to come into effect
				{
					SetGlobal("GMOD_SETTING_LOADER", 0);
				}
				else if (Button(new GUIContent(Loc.MODMenuNormal_113, Loc.MODMenuNormal_114))) //NVL defaults | This restores flags to the defaults for NVL mode\nNOTE: Requires you to relaunch the game 2 times to come into effect
				{
					SetGlobal("GMOD_SETTING_LOADER", 1);
				}
				else if (Button(new GUIContent(Loc.MODMenuNormal_115, Loc.MODMenuNormal_116))) //Vanilla Defaults | This restores flags to the same settings as the unmodded game.\nNOTE: Requires a relaunch to come into effect. After this, uninstall the mod.
				{
					SetGlobal("GMOD_SETTING_LOADER", 2);
				}
			}
			else
			{
				if (Button(new GUIContent(Loc.MODMenuNormal_117, Loc.MODMenuNormal_118))) //Cancel Pending Restore | Click this to stop restoring defaults on next game launch
				{
					SetGlobal("GMOD_SETTING_LOADER", 3);
				}
			}
			GUILayout.EndHorizontal();
		}

		private void OnGUIComputedLipsync()
		{
			Label(Loc.MODMenuNormal_119); //Computed Lipsync Options
			GUILayout.BeginHorizontal();
			Label(new GUIContent(Loc.MODMenuNormal_120, Loc.MODMenuNormal_121)); //LipSync Thresh 1:  | Above this threshold, expression 1 will be used.\n\nBelow or equal to this threshold, expression 0 will be used.\n\nOnly saved until the game restarts
			TextField_ComputedLipSyncThreshold1 = GUILayout.TextField(TextField_ComputedLipSyncThreshold1);

			Label(new GUIContent(Loc.MODMenuNormal_122, Loc.MODMenuNormal_123)); //LipSync Thresh 2:  | Above this thireshold, expression 2 will be used\n\nOnly saved until the game restarts
			TextField_ComputedLipSyncThreshold2 = GUILayout.TextField(TextField_ComputedLipSyncThreshold2);

			if (Button(new GUIContent(Loc.MODMenuNormal_124, Loc.MODMenuNormal_125))) //Set | Tries to set the given lipsync thresholds\n\n
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
					MODToaster.Show(Loc.MODMenuNormal_126); //Invalid thresholds - each threshold should be a value between 0 and 1
				}
			}
			GUILayout.EndHorizontal();

			if(this.radioForceComputedLipsync.OnGUIFragment(GameSystem.Instance.SceneController.MODGetForceComputedLipsync()) is bool newForceComputedLipsync)
			{
				GameSystem.Instance.SceneController.MODSetForceComputedLipsync(newForceComputedLipsync);
			}
		}

		public bool UserCanClose() => true;
		public string Heading() => Loc.MODMenuNormal_127; //Mod Options Menu

		public string DefaultTooltip()
		{
			return Loc.MODMenuNormal_129; //Hover over a button on the left panel for its description.\n\n[Vanilla Hotkeys]\nEnter,Return,RightArrow,PageDown : Advance Text\nLeftArrow,Pageup : See Backlog\nESC : Open Menu\nCtrl : Hold Skip Mode\nA : Auto Mode\nS : Toggle Skip Mode\nF, Alt-Enter : FullScreen\nSpace : Hide Text\nL : Swap Language\n\n[MOD Hotkeys]\nF1 : ADV-NVL MODE\nF2 : Voice Matching Level\nF3 : Effect Level (Not Used)\nF5 : QuickSave\nF7 : QuickLoad\nF10 : Mod Menu\nM : Increase Voice Volume\nN : Decrease Voice Volume\nP : Cycle through art styles\n2 : Cycle through BGM/SE\n7 : Enable/Disable Lip-Sync\nLShift + M : Voice Volume MAX\nLShift + N : Voice Volume MINN
		}
	}
}
