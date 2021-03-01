using Assets.Scripts.Core;
using MOD.Scripts.Core.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	class MODMenuNormal : MODMenuModuleInterface
	{
		private readonly MODMenu modMenu;
		private readonly MODMenuCommon c;
		private readonly MODMenuResolution resolutionMenu;
		private readonly MODMenuAudioOptions audioOptionsMenu;

		private GUIContent[] defaultArtsetDescriptions;
		private readonly bool hasOGBackgrounds;

		private readonly MODRadio radioCensorshipLevel;
		private readonly MODRadio radioLipSync;
		private readonly MODRadio radioOpenings;
		private readonly MODRadio radioHideCG;
		private readonly MODRadio radioBackgrounds;
		private readonly MODRadio radioArtSet;

		public MODMenuNormal(MODMenu modMenu, MODMenuCommon c, MODStyleManager styleManager, MODMenuAudioOptions audioOptionsMenu)
		{
			this.modMenu = modMenu;
			this.c = c;
			this.audioOptionsMenu = audioOptionsMenu;
			this.resolutionMenu = new MODMenuResolution(c);

			hasOGBackgrounds = MODActions.HasOGBackgrounds();

			defaultArtsetDescriptions = new GUIContent[] {
				new GUIContent("Console", "Use the Console sprites and backgrounds"),
				new GUIContent("Remake", "Use Mangagamer's remake sprites with Console backgrounds"),
				new GUIContent("Original", "Use Original/Ryukishi sprites and backgrounds (if available - OG backgrounds not available for Console Arcs)\n\n" +
				"Warning: Most users should use the Original/Ryukishi preset at the top of this menu!"),
			};

			string baseCensorshipDescription = @"

Sets the script censorship level
- This setting exists because the voices are taken from the censored, Console versions of the game, so no voices exist for the PC uncensored dialogue.
- We recommend the default level (2), the most balanced option. Using this option, only copyright changes, innuendos, and a few words will be changed.
  - 5: Full PS3 script fully voiced (most censored)
  - 2: Default - most balanced option
  - 0: Original PC Script with voices where it fits (least uncensored), but uncensored scenes may be missing voices";

			radioCensorshipLevel = new MODRadio("Voice Matching Level (Hotkey: F2)", new GUIContent[] {
				new GUIContent("0", "Censorship level 0 - Equivalent to PC" + baseCensorshipDescription),
				new GUIContent("1", "Censorship level 1" + baseCensorshipDescription),
				new GUIContent("2*", "Censorship level 2 (this is the default/recommended value)" + baseCensorshipDescription),
				new GUIContent("3", "Censorship level 3" + baseCensorshipDescription),
				new GUIContent("4", "Censorship level 4" + baseCensorshipDescription),
				new GUIContent("5", "Censorship level 5 - Equivalent to Console" + baseCensorshipDescription),
				}, styleManager);

			radioLipSync = new MODRadio("Lip Sync for Console Sprites (Hotkey: 7)", new GUIContent[]
			{
				new GUIContent("Lip Sync Off", "Disables Lip Sync for Console Sprites"),
				new GUIContent("Lip Sync On", "Enables Lip Sync for Console Sprites"),
			}, styleManager);

			radioOpenings = new MODRadio("Opening Movies (Hotkey: Shift-F12)", new GUIContent[]
			{
				new GUIContent("Disabled", "Disables all opening videos"),
				new GUIContent("Enabled", "Enables opening videos\n\n" +
				"NOTE: Once the opening video plays the first time, will automatically switch to 'Launch + In-Game'\n\n" +
				"We have setup openings this way to avoid spoilers."),
				new GUIContent("Launch + In-Game", "WARNING: There is usually no need to set this manually.\n\n" +
				"If openings are enabled, the first time you reach an opening while playing the game, this flag will be set automatically\n\n" +
				"That is, after the opening is played the first time, from then on openings will play every time the game launches"),
			}, styleManager);

			radioHideCG = new MODRadio("Show/Hide CGs", new GUIContent[]
			{
				new GUIContent("Show CGs", "Shows CGs (You probably want this enabled for Console ADV/NVL mode)"),
				new GUIContent("Hide CGs", "Disables all CGs (mainly for use with the Original/Ryukishi preset)"),
			}, styleManager);

			radioBackgrounds = new MODRadio("Override Art Set Backgrounds", new GUIContent[]{
				new GUIContent("Default BGs", "Use the default backgrounds for the current artset"),
				new GUIContent("Console BGs", "Force Console backgrounds, regardless of the artset"),
				new GUIContent("Original BGs", "Force Original/Ryukishi backgrounds, regardless of the artset"),
				new GUIContent("Original Stretched", "Force Original/Ryukishi backgrounds, stretched to fit, regardless of the artset\n\n" +
				"WARNING: When using this option, you should have ADV/NVL mode selected, otherwise sprites will be cut off, and UI will appear in the wrong place"),
			}, styleManager, itemsPerRow: 2);

			radioArtSet = new MODRadio("Choose Art Set", defaultArtsetDescriptions, styleManager, itemsPerRow: 3);
		}

		public void OnGUI()
		{
			c.HeadingLabel("Basic Options");

			c.Label("Graphics Presets (Hotkey: F1)");
			{
				GUILayout.BeginHorizontal();

				int advNVLRyukishiMode = MODActions.GetADVNVLRyukishiModeFromFlags(out bool presetModified);

				if (c.Button(new GUIContent(advNVLRyukishiMode == 0 && presetModified ? "ADV (custom)" : "ADV", "This preset:\n" +
				"- Makes text show at the bottom of the screen in a textbox\n" +
				"- Shows the name of the current character on the textbox\n" +
				"- Uses the console sprites and backgrounds\n" +
				"- Displays in 16:9 widescreen\n\n" +
				"Note that sprites and backgrounds can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available"), selected: advNVLRyukishiMode == 0))
				{
					MODActions.SetAndSaveADV(MODActions.ModPreset.ADV, showInfoToast: false);
				}

				if (c.Button(new GUIContent(advNVLRyukishiMode == 1 && presetModified ? "NVL (custom)" : "NVL", "This preset:\n" +
					"- Makes text show across the whole screen\n" +
					"- Uses the console sprites and backgrounds\n" +
					"- Displays in 16:9 widescreen\n\n" +
					"Note that sprites and backgrounds can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available"), selected: advNVLRyukishiMode == 1))
				{
					MODActions.SetAndSaveADV(MODActions.ModPreset.NVL, showInfoToast: false);
				}

				if (this.hasOGBackgrounds &&
					c.Button(new GUIContent(advNVLRyukishiMode == 2 && presetModified ? "Original/Ryukishi (custom)" : "Original/Ryukishi", "This preset makes the game behave similarly to the unmodded game:\n" +
					"- Displays backgrounds in 4:3 'standard' aspect\n" +
					"- CGs are disabled (Can be re-enabled, see 'Show/Hide CGs')\n" +
					"- Switches to original sprites and backgrounds\n\n" +
					"Note that sprites, backgrounds, and CG hiding can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available"), selected: advNVLRyukishiMode == 2))
				{
					MODActions.SetAndSaveADV(MODActions.ModPreset.OG, showInfoToast: false);
				}

				GUILayout.EndHorizontal();
			}

			if (this.radioCensorshipLevel.OnGUIFragment(c.GetGlobal("GCensor")) is int censorLevel)
			{
				c.SetGlobal("GCensor", censorLevel);
			};

			if (this.radioLipSync.OnGUIFragment(c.GetGlobal("GLipSync")) is int lipSyncEnabled)
			{
				c.SetGlobal("GLipSync", lipSyncEnabled);
			};

			if (this.radioOpenings.OnGUIFragment(c.GetGlobal("GVideoOpening") - 1) is int openingVideoLevelZeroIndexed)
			{
				c.SetGlobal("GVideoOpening", openingVideoLevelZeroIndexed + 1);
			};

			this.audioOptionsMenu.OnGUI();

			c.HeadingLabel("Advanced Options");

			if (this.radioHideCG.OnGUIFragment(c.GetGlobal("GHideCG")) is int hideCG)
			{
				c.SetGlobal("GHideCG", hideCG);
			};

			if (this.radioArtSet.OnGUIFragment(Core.MODSystem.instance.modTextureController.GetArtStyle()) is int artStyle)
			{
				c.SetGlobal("GStretchBackgrounds", 0);
				Core.MODSystem.instance.modTextureController.SetArtStyle(artStyle, showInfoToast: false);
			}

			if (this.hasOGBackgrounds)
			{
				int currentBackground = c.GetGlobal("GBackgroundSet");
				if (currentBackground == 2)
				{
					if (c.GetGlobal("GStretchBackgrounds") == 1)
					{
						currentBackground = 3;
					}
				}
				if (this.radioBackgrounds.OnGUIFragment(currentBackground) is int background)
				{
					if (background == 3)
					{
						c.SetGlobal("GStretchBackgrounds", 1);
						c.SetGlobal("GBackgroundSet", 2);
					}
					else
					{
						c.SetGlobal("GStretchBackgrounds", 0);
						c.SetGlobal("GBackgroundSet", background);
					}
					GameSystem.Instance.SceneController.ReloadAllImages();
				}
			}

			resolutionMenu.OnGUI();

			GUILayout.Space(10);
			OnGUIRestoreSettings(c);


			c.HeadingLabel("Troubleshooting");
			c.Label("Save Files and Log Files");
			MODMenuSupport.ShowSupportButtons(content => c.Button(content));

			c.Label("Developer");
			GUILayout.BeginHorizontal();
			if (c.Button(new GUIContent("Toggle debug menu (Shift-F9)", "Toggle the debug menu")))
			{
				modMenu.ToggleDebugMenu();
			}
			if (c.Button(new GUIContent("Toggle flag menu (Shift-F10)", "Toggle the flag menu. Toggle Multiple times for more options.\n\nNote: 3rd and 4th panels are only shown if GMOD_DEBUG_MODE is true.")))
			{
				MODActions.ToggleFlagMenu();
			}
			GUILayout.EndHorizontal();
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

			resolutionMenu.OnBeforeMenuVisible();
			audioOptionsMenu.OnBeforeMenuVisible();
		}

		private void OnGUIRestoreSettings(MODMenuCommon w)
		{
			w.Label($"Restore Settings {(w.GetGlobal("GMOD_SETTING_LOADER") == 3 ? "" : ": <Restart Pending>")}");

			GUILayout.BeginHorizontal();
			if (w.GetGlobal("GMOD_SETTING_LOADER") == 3)
			{
				if (w.Button(new GUIContent("ADV defaults", "This restores flags to the defaults for NVL mode\n" +
					"NOTE: Requires you to relaunch the game 2 times to come into effect")))
				{
					w.SetGlobal("GMOD_SETTING_LOADER", 0);
				}
				else if (w.Button(new GUIContent("NVL defaults", "This restores flags to the defaults for NVL mode\n" +
					"NOTE: Requires you to relaunch the game 2 times to come into effect")))
				{
					w.SetGlobal("GMOD_SETTING_LOADER", 1);
				}
				else if (w.Button(new GUIContent("Vanilla Defaults", "This restores flags to the same settings as the unmodded game.\n" +
					"NOTE: Requires a relaunch to come into effect. After this, uninstall the mod.")))
				{
					w.SetGlobal("GMOD_SETTING_LOADER", 2);
				}
			}
			else
			{
				if (w.Button(new GUIContent("Cancel Pending Restore", "Click this to stop restoring defaults on next game launch")))
				{
					w.SetGlobal("GMOD_SETTING_LOADER", 3);
				}
			}
			GUILayout.EndHorizontal();
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
