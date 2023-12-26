using System;
using System.Collections.Generic;
using System.Text;

namespace MOD.Scripts.Core.Localization
{
	public partial class Loc
	{
		// Note: The default strings here are only used if the localization.json file is missing from the game folder (As of 2023-10-15, our mod does not ship with a localization.json file.
		// For an example of this file, see the tools/localization.json file.

		// The below entries are for the F10 mod menu, in the file MOD.Scripts.UI/MODMenu.cs. NOTE: some of these are the debug menu strings.
		public static string MODMenu_0 => Get("MODMenu_0", "[Audio Tracking] - indicates what would play on each BGM flow");
		public static string MODMenu_1 => Get("MODMenu_1", "[Audio Flags and last played audio]");
		public static string MODMenu_2 => Get("MODMenu_2", "Reset GAudioSet");
		public static string MODMenu_3 => Get("MODMenu_3", "Set GAudioSet to 0, to force the game to do audio setup on next startup");
		public static string MODMenu_4 => Get("MODMenu_4", "Close");
		public static string MODMenu_5 => Get("MODMenu_5", "Close the debug menu");
		public static string MODMenu_6 => Get("MODMenu_6", "Developer Debug Window (click to drag)");
		public static string MODMenu_7 => Get("MODMenu_7", "Mod Menu\n(Hotkey: F10)");
		public static string MODMenu_8 => Get("MODMenu_8", "BGM Info");
		public static string MODMenu_9 => Get("MODMenu_9", "Copy BGM Name");
		public static string MODMenu_10 => Get("MODMenu_10", "Show File");
		public static string MODMenu_11 => Get("MODMenu_11", "Open In Youtube");
		public static string MODMenu_12 => Get("MODMenu_12", "Note: If explorer freezes\nuninstall Web Media Extensions");
		public static string MODMenu_13 => Get("MODMenu_13", "X");
		public static string MODMenu_14 => Get("MODMenu_14", "Close the Mod menu");
		public static string MODMenu_15 => Get("MODMenu_15", "Please close the current menu and try again");
		public static string MODMenu_16 => Get("MODMenu_16", "It looks like there was a problem starting up\n\nPlease send the developers your log file (output_log.txt or Player.log).\n\nYou can try the following yourself to fix the issue.\n  1. Try waiting 30 seconds for the game to progress. If nothing happens, try restarting the game\n\n  2. Use the buttons under 'Troubleshooting' on the bottom left to show your save files, log files, and compiled scripts.\n\n  3. If the log indicates you have corrupt save files, you may wish to delete the offending save file (or all of them).\n\n  4. You can try to clear your compiled script files, then restart the game.\n\n  5. If the above do not fix the problem, please click the 'Open Support Page' button, which has extra troubleshooting info and links to join our Discord server for direct support.");

		// Most of the localization for the F10 menu
		public static string MODMenuNormal_0 => Get("MODMenuNormal_0", "Lip Sync for Console Sprites (Hotkey: 7)");
		public static string MODMenuNormal_1 => Get("MODMenuNormal_1", "Lip Sync (NOTE: Select 'Console' sprites or preset to enable)");
		public static string MODMenuNormal_2 => Get("MODMenuNormal_2", "Console");
		public static string MODMenuNormal_3 => Get("MODMenuNormal_3", "Use the Console sprites");
		public static string MODMenuNormal_4 => Get("MODMenuNormal_4", "MangaGamer");
		public static string MODMenuNormal_5 => Get("MODMenuNormal_5", "Use Mangagamer's remake sprites");
		public static string MODMenuNormal_6 => Get("MODMenuNormal_6", "Original");
		public static string MODMenuNormal_7 => Get("MODMenuNormal_7", "Use Original/Ryukishi sprites");
		public static string MODMenuNormal_8 => Get("MODMenuNormal_8", "Voice Matching Level (Hotkey: F2)");
		public static string MODMenuNormal_9 => Get("MODMenuNormal_9", "Censorship level 0 - Equivalent to PC");
		public static string MODMenuNormal_10 => Get("MODMenuNormal_10", "Censorship level 1");
		public static string MODMenuNormal_11 => Get("MODMenuNormal_11", "Censorship level 2 (this is the default/recommended value)");
		public static string MODMenuNormal_12 => Get("MODMenuNormal_12", "Censorship level 3");
		public static string MODMenuNormal_13 => Get("MODMenuNormal_13", "Censorship level 4");
		public static string MODMenuNormal_14 => Get("MODMenuNormal_14", "Censorship level 5 - Equivalent to Console");
		public static string MODMenuNormal_15 => Get("MODMenuNormal_15", "Lip Sync Off");
		public static string MODMenuNormal_16 => Get("MODMenuNormal_16", "Disables Lip Sync for Console Sprites");
		public static string MODMenuNormal_17 => Get("MODMenuNormal_17", "Lip Sync On");
		public static string MODMenuNormal_18 => Get("MODMenuNormal_18", "Enables Lip Sync for Console Sprites");
		public static string MODMenuNormal_19 => Get("MODMenuNormal_19", "Lip Sync Off (Inactive)");
		public static string MODMenuNormal_20 => Get("MODMenuNormal_20", "Disables Lip Sync for Console Sprites\n\nNOTE: Lip Sync only works with Console sprites - please select 'Console' preset or sprites");
		public static string MODMenuNormal_21 => Get("MODMenuNormal_21", "Lip Sync On (Inactive)");
		public static string MODMenuNormal_22 => Get("MODMenuNormal_22", "Enables Lip Sync for Console Sprites\n\nNOTE: Lip Sync only works with Console sprites - please select 'Console' preset or sprites");
		public static string MODMenuNormal_23 => Get("MODMenuNormal_23", "Opening Movies (Hotkey: Shift-F12)");
		public static string MODMenuNormal_24 => Get("MODMenuNormal_24", "Disabled");
		public static string MODMenuNormal_25 => Get("MODMenuNormal_25", "Disables all opening videos");
		public static string MODMenuNormal_26 => Get("MODMenuNormal_26", "Enabled");
		public static string MODMenuNormal_27 => Get("MODMenuNormal_27", "Enables opening videos\n\nNOTE: Once the opening video plays the first time, will automatically switch to 'Launch + In-Game'\n\nWe have setup openings this way to avoid spoilers.");
		public static string MODMenuNormal_28 => Get("MODMenuNormal_28", "Launch + In-Game");
		public static string MODMenuNormal_29 => Get("MODMenuNormal_29", "WARNING: There is usually no need to set this manually.\n\nIf openings are enabled, the first time you reach an opening while playing the game, this flag will be set automatically\n\nThat is, after the opening is played the first time, from then on openings will play every time the game launches");
		public static string MODMenuNormal_30 => Get("MODMenuNormal_30", "Show/Hide CGs");
		public static string MODMenuNormal_31 => Get("MODMenuNormal_31", "Show CGs");
		public static string MODMenuNormal_32 => Get("MODMenuNormal_32", "Shows CGs (You probably want this enabled for Console ADV/NVL mode)");
		public static string MODMenuNormal_33 => Get("MODMenuNormal_33", "Hide CGs");
		public static string MODMenuNormal_34 => Get("MODMenuNormal_34", "Disables all CGs (mainly for use with the Original/Ryukishi preset)");
		public static string MODMenuNormal_35 => Get("MODMenuNormal_35", "Background Style");
		public static string MODMenuNormal_36 => Get("MODMenuNormal_36", "Console BGs");
		public static string MODMenuNormal_37 => Get("MODMenuNormal_37", "Use Console backgrounds");
		public static string MODMenuNormal_38 => Get("MODMenuNormal_38", "Original BGs");
		public static string MODMenuNormal_39 => Get("MODMenuNormal_39", "Use Original/Ryukishi backgrounds.");
		public static string MODMenuNormal_40 => Get("MODMenuNormal_40", "Background Stretching");
		public static string MODMenuNormal_41 => Get("MODMenuNormal_41", "BG Stretching Off");
		public static string MODMenuNormal_42 => Get("MODMenuNormal_42", "Makes backgrounds as big as possible without any stretching (Keep Aspect Ratio)");
		public static string MODMenuNormal_43 => Get("MODMenuNormal_43", "BG Stretching On");
		public static string MODMenuNormal_44 => Get("MODMenuNormal_44", "Stretches backgrounds to fit the screen (Ignore Aspect Ratio)\n\nMainly for use with the Original BGs, which are in 4:3 aspect ratio.");
		public static string MODMenuNormal_45 => Get("MODMenuNormal_45", "Sprite Style");
		public static string MODMenuNormal_46 => Get("MODMenuNormal_46", "Text Window Appearance");
		public static string MODMenuNormal_47 => Get("MODMenuNormal_47", "ADV Mode");
		public static string MODMenuNormal_48 => Get("MODMenuNormal_48", "This option:\n- Makes text show at the bottom of the screen in a textbox\n- Shows the name of the current character on the textbox\n");
		public static string MODMenuNormal_49 => Get("MODMenuNormal_49", "NVL Mode");
		public static string MODMenuNormal_50 => Get("MODMenuNormal_50", "This option:\n- Makes text show across the whole screen\n");
		public static string MODMenuNormal_51 => Get("MODMenuNormal_51", "Original");
		public static string MODMenuNormal_52 => Get("MODMenuNormal_52", "This option:\n- Darkens the whole screen to emulate the original game\n- Makes text show only in a 4:3 section of the screen (narrower than NVL mode)\n");
		public static string MODMenuNormal_53 => Get("MODMenuNormal_53", "Force Computed Lipsync");
		public static string MODMenuNormal_54 => Get("MODMenuNormal_54", "As Fallback");
		public static string MODMenuNormal_55 => Get("MODMenuNormal_55", "Only use computed lipsync if there is no baked 'spectrum' file for a given .ogg file");
		public static string MODMenuNormal_56 => Get("MODMenuNormal_56", "Computed Always");
		public static string MODMenuNormal_57 => Get("MODMenuNormal_57", "Always use computed lipsync for all voices. Any 'spectrum' files will be ignored.");
		public static string MODMenuNormal_58 => Get("MODMenuNormal_58", "Gameplay");
		public static string MODMenuNormal_59 => Get("MODMenuNormal_59", "Voice Matching and Opening Videos");
		public static string MODMenuNormal_60 => Get("MODMenuNormal_60", "Graphics");
		public static string MODMenuNormal_61 => Get("MODMenuNormal_61", "Sprites, Backgrounds, CGs, Resolution");
		public static string MODMenuNormal_62 => Get("MODMenuNormal_62", "Audio");
		public static string MODMenuNormal_63 => Get("MODMenuNormal_63", "BGM and SE options");
		public static string MODMenuNormal_64 => Get("MODMenuNormal_64", "Troubleshooting");
		public static string MODMenuNormal_65 => Get("MODMenuNormal_65", "Tools to help you if something goes wrong");
		public static string MODMenuNormal_66 => Get("MODMenuNormal_66", "Original/Ryukishi Experimental 4:3 Aspect Ratio");
		public static string MODMenuNormal_67 => Get("MODMenuNormal_67", "16:9 (default)");
		public static string MODMenuNormal_68 => Get("MODMenuNormal_68", "The game's aspect ratio will be 16:9.\n\nWhen playing in OG mode, the left and right of the screen will be padded to 4:3 with black bars.");
		public static string MODMenuNormal_69 => Get("MODMenuNormal_69", "The game's aspect ratio will be 4:3, however this may cause some issues when playing our mod.\nOnly use this option if your monitor is 4:3 aspect ratio, like an old CRT monitor.\n\nPlease note the following:\n\n - You should enable the Original/Ryukishi preset before enabling this option. Using other settings should all work, but are not well tested.\n\n - 16:9 images will be squished to fit in 4:3 so they don't get cut off. This includes text images, and PS3 CG images if enabled.\n\n - You may see graphical artifacts just after this is enabled - they should fix after the next character transition or text clear\n\n");
		public static string MODMenuNormal_70 => Get("MODMenuNormal_70", "Graphics Presets (Hotkey: F1)");
		public static string MODMenuNormal_71 => Get("MODMenuNormal_71", "Console");
		public static string MODMenuNormal_72 => Get("MODMenuNormal_72", "This preset:\n- Makes text show at the bottom of the screen in a textbox\n- Shows the name of the current character on the textbox\n- Uses the console sprites (with lipsync) and console backgrounds\n- Displays in 16:9 widescreen\n\nNote that sprites and backgrounds can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available");
		public static string MODMenuNormal_73 => Get("MODMenuNormal_73", "MangaGamer");
		public static string MODMenuNormal_74 => Get("MODMenuNormal_74", "This preset:\n- Makes text show across the whole screen\n- Uses the Mangagamer remake sprites and Console backgrounds\n- Displays in 16:9 widescreen\n\nNote that sprites and backgrounds can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available");
		public static string MODMenuNormal_75 => Get("MODMenuNormal_75", "Original/Ryukishi");
		public static string MODMenuNormal_76 => Get("MODMenuNormal_76", "This preset makes the game behave similarly to the unmodded game:\n- Displays backgrounds in 4:3 'standard' aspect\n- CGs are disabled (Can be re-enabled, see 'Show/Hide CGs')\n- Switches to original sprites and backgrounds\n\nNote that sprites, backgrounds, and CG hiding can be overridden by setting the 'Choose Art Set' & 'Override Art Set Backgrounds' options under 'Advanced Options', if available");
		public static string MODMenuNormal_77 => Get("MODMenuNormal_77", "Custom");
		public static string MODMenuNormal_78 => Get("MODMenuNormal_78", "Your own custom preset, using the options below.\n\nThis custom preset will be saved, even when you switch to the other presets.");
		public static string MODMenuNormal_79 => Get("MODMenuNormal_79", "Advanced Options");
		public static string MODMenuNormal_80 => Get("MODMenuNormal_80", "WARNING: You have ADV mode enabled, but you are in a forced-NVL section, so the game will display in NVL mode temporarily!");
		public static string MODMenuNormal_81 => Get("MODMenuNormal_81", "Resolution");
		public static string MODMenuNormal_82 => Get("MODMenuNormal_82", "Advanced Options");
		public static string MODMenuNormal_83 => Get("MODMenuNormal_83", "Experimental Gameplay Tools");
		public static string MODMenuNormal_84 => Get("MODMenuNormal_84", "Game Clear Forced!");
		public static string MODMenuNormal_85 => Get("MODMenuNormal_85", "All Progress Reset!");
		public static string MODMenuNormal_86 => Get("MODMenuNormal_86", "Please reload menu/game!");
		public static string MODMenuNormal_87 => Get("MODMenuNormal_87", "Reset All Progress");
		public static string MODMenuNormal_88 => Get("MODMenuNormal_88", "Force Game Clear");
		public static string MODMenuNormal_89 => Get("MODMenuNormal_89", "Click");
		public static string MODMenuNormal_90 => Get("MODMenuNormal_90", "more times");
		public static string MODMenuNormal_91 => Get("MODMenuNormal_91", "WARNING: This option will toggle your game clear status:\n - If you haven't finished the game, it will unlock everything.\n - If you have already finished the game, it will reset all your progress!\n\nThis toggles the EXTRAS menu, including all TIPS, and all chapter jump chapters.\n\nYou may need to reload the current menu or restart the game before you can see the changes.\n\nSaves shouldn't be affected.");
		public static string MODMenuNormal_92 => Get("MODMenuNormal_92", "No");
		public static string MODMenuNormal_93 => Get("MODMenuNormal_93", "Yes");
		public static string MODMenuNormal_94 => Get("MODMenuNormal_94", "Game Cleared?: ");
		public static string MODMenuNormal_95 => Get("MODMenuNormal_95", "Highest Chapter: ");
		public static string MODMenuNormal_96 => Get("MODMenuNormal_96", "Developer Tools");
		public static string MODMenuNormal_97 => Get("MODMenuNormal_97", "Developer Debug Menu");
		public static string MODMenuNormal_98 => Get("MODMenuNormal_98", "Toggle debug menu (Shift-F9)");
		public static string MODMenuNormal_99 => Get("MODMenuNormal_99", "Toggle the debug menu");
		public static string MODMenuNormal_100 => Get("MODMenuNormal_100", "Toggle flag menu (Shift-F10)");
		public static string MODMenuNormal_101 => Get("MODMenuNormal_101", "Toggle the flag menu. Toggle Multiple times for more options.\n\nNote: 3rd and 4th panels are only shown if GMOD_DEBUG_MODE is true.");
		public static string MODMenuNormal_102 => Get("MODMenuNormal_102", "Show Developer Tools: Only click if asked by developers");
		public static string MODMenuNormal_103 => Get("MODMenuNormal_103", "Show the Developer Tools.\n\nOnly click this button if you're asked to by the developers.");
		public static string MODMenuNormal_104 => Get("MODMenuNormal_104", "Flag Unlocks");
		public static string MODMenuNormal_105 => Get("MODMenuNormal_105", "Toggle GFlag_GameClear is ");
		public static string MODMenuNormal_106 => Get("MODMenuNormal_106", "Toggle the 'GFlag_GameClear' flag which is normally activated when you complete the game. Unlocks things like the All-cast review.");
		public static string MODMenuNormal_107 => Get("MODMenuNormal_107", "Toggle GHighestChapter 0 <-> 999 | Is ");
		public static string MODMenuNormal_108 => Get("MODMenuNormal_108", "Toggle the 'GHighestChapter' flag which indicates the highest chapter you have completed.\n\nWhen >= 1, unlocks the extras menu.\nAlso unlocks other things like which chapters are shown on the chapter jump menu, etc.");
		public static string MODMenuNormal_109 => Get("MODMenuNormal_109", "Restart Pending");
		public static string MODMenuNormal_110 => Get("MODMenuNormal_110", "Restore Settings");
		public static string MODMenuNormal_111 => Get("MODMenuNormal_111", "ADV defaults");
		public static string MODMenuNormal_112 => Get("MODMenuNormal_112", "This restores flags to the defaults for NVL mode\nNOTE: Requires you to relaunch the game 2 times to come into effect");
		public static string MODMenuNormal_113 => Get("MODMenuNormal_113", "NVL defaults");
		public static string MODMenuNormal_114 => Get("MODMenuNormal_114", "This restores flags to the defaults for NVL mode\nNOTE: Requires you to relaunch the game 2 times to come into effect");
		public static string MODMenuNormal_115 => Get("MODMenuNormal_115", "Vanilla Defaults");
		public static string MODMenuNormal_116 => Get("MODMenuNormal_116", "This restores flags to the same settings as the unmodded game.\nNOTE: Requires a relaunch to come into effect. After this, uninstall the mod.");
		public static string MODMenuNormal_117 => Get("MODMenuNormal_117", "Cancel Pending Restore");
		public static string MODMenuNormal_118 => Get("MODMenuNormal_118", "Click this to stop restoring defaults on next game launch");
		public static string MODMenuNormal_119 => Get("MODMenuNormal_119", "Computed Lipsync Options");
		public static string MODMenuNormal_120 => Get("MODMenuNormal_120", "LipSync Thresh 1: ");
		public static string MODMenuNormal_121 => Get("MODMenuNormal_121", "Above this threshold, expression 1 will be used.\n\nBelow or equal to this threshold, expression 0 will be used.\n\nOnly saved until the game restarts");
		public static string MODMenuNormal_122 => Get("MODMenuNormal_122", "LipSync Thresh 2: ");
		public static string MODMenuNormal_123 => Get("MODMenuNormal_123", "Above this thireshold, expression 2 will be used\n\nOnly saved until the game restarts");
		public static string MODMenuNormal_124 => Get("MODMenuNormal_124", "Set");
		public static string MODMenuNormal_125 => Get("MODMenuNormal_125", "Tries to set the given lipsync thresholds\n\n");
		public static string MODMenuNormal_126 => Get("MODMenuNormal_126", "Invalid thresholds - each threshold should be a value between 0 and 1");
		public static string MODMenuNormal_127 => Get("MODMenuNormal_127", "Mod Options Menu");
		public static string MODMenuNormal_128 => Get("MODMenuNormal_128", "\n\nSets the script censorship level\n- This setting exists because the voices are taken from the censored, Console versions of the game, so no voices exist for the PC uncensored dialogue.\n- We recommend the default level (2), the most balanced option. Using this option, only copyright changes, innuendos, and a few words will be changed.\n  - 5: Use voiced lines from PS3 script as much as possible (censored at parts)\n  - 2: Default - most balanced option\n  - 0: Original PC Script with voices where it fits (fully uncensored), but uncensored scenes may be missing voices");
		public static string MODMenuNormal_129 => Get("MODMenuNormal_129", "Hover over a button on the left panel for its description.\n\n[Vanilla Hotkeys]\nEnter,Return,RightArrow,PageDown : Advance Text\nLeftArrow,Pageup : See Backlog\nESC : Open Menu\nCtrl : Hold Skip Mode\nA : Auto Mode\nS : Toggle Skip Mode\nF, Alt-Enter : FullScreen\nSpace : Hide Text\nL : Swap Language\n\n[MOD Hotkeys]\nF1 : ADV-NVL MODE\nF2 : Voice Matching Level\nF3 : Effect Level (Not Used)\nF5 : QuickSave\nF7 : QuickLoad\nF10 : Mod Menu\nM : Increase Voice Volume\nN : Decrease Voice Volume\nP : Cycle through art styles\n2 : Cycle through BGM/SE\n7 : Enable/Disable Lip-Sync\nLShift + M : Voice Volume MAX\nLShift + N : Voice Volume MINN");

		// The resolution part of the F10 menu
		public static string MODMenuResolution_0 => Get("MODMenuResolution_0", "Set a custom fullscreen resolution\n\nUse this option only if the fullscreen resolution is detected incorrectly (such as on some Linux systems)\nYou can manually type in a resolution to use below.\n\nClick 'Clear Override' to let the game automatically determine the fullscreen resolution");
		public static string MODMenuResolution_1 => Get("MODMenuResolution_1", "Windowed Resolution Settings");
		public static string MODMenuResolution_2 => Get("MODMenuResolution_2", "480p");
		public static string MODMenuResolution_3 => Get("MODMenuResolution_3", "Set resolution to 853 x 480");
		public static string MODMenuResolution_4 => Get("MODMenuResolution_4", "720p");
		public static string MODMenuResolution_5 => Get("MODMenuResolution_5", "Set resolution to 1280 x 720");
		public static string MODMenuResolution_6 => Get("MODMenuResolution_6", "1080p");
		public static string MODMenuResolution_7 => Get("MODMenuResolution_7", "Set resolution to 1920 x 1080");
		public static string MODMenuResolution_8 => Get("MODMenuResolution_8", "1440p");
		public static string MODMenuResolution_9 => Get("MODMenuResolution_9", "Set resolution to 2560 x 1440");
		public static string MODMenuResolution_10 => Get("MODMenuResolution_10", "Set");
		public static string MODMenuResolution_11 => Get("MODMenuResolution_11", "Sets a custom resolution - mainly for windowed mode.\n\nHeight set automatically to maintain 16:9 aspect ratio.");
		public static string MODMenuResolution_12 => Get("MODMenuResolution_12", "Height too small - must be at least 480 pixels");
		public static string MODMenuResolution_13 => Get("MODMenuResolution_13", "Height too big - must be less than 15360 pixels");
		public static string MODMenuResolution_14 => Get("MODMenuResolution_14", "Click here to go Windowed to change these settings");
		public static string MODMenuResolution_15 => Get("MODMenuResolution_15", "Toggle Fullscreen");
		public static string MODMenuResolution_16 => Get("MODMenuResolution_16", "Go Fullscreen");
		public static string MODMenuResolution_17 => Get("MODMenuResolution_17", "Toggle Fullscreen");
		public static string MODMenuResolution_18 => Get("MODMenuResolution_18", "Off");
		public static string MODMenuResolution_19 => Get("MODMenuResolution_19", "Fullscreen Resolution Override (Detected: ");
		public static string MODMenuResolution_20 => Get("MODMenuResolution_20", " Override: ");
		public static string MODMenuResolution_21 => Get("MODMenuResolution_21", "Width:");
		public static string MODMenuResolution_22 => Get("MODMenuResolution_22", "Height:");
		public static string MODMenuResolution_23 => Get("MODMenuResolution_23", "Click repeatedly to override");
		public static string MODMenuResolution_24 => Get("MODMenuResolution_24", "Override Fullscreen Resolution");
		public static string MODMenuResolution_25 => Get("MODMenuResolution_25", "Invalid Height");
		public static string MODMenuResolution_26 => Get("MODMenuResolution_26", "Invalid Width");
		public static string MODMenuResolution_27 => Get("MODMenuResolution_27", "Clear Override");
		public static string MODMenuResolution_28 => Get("MODMenuResolution_28", "Height too small - must be at least 480 pixels");
		public static string MODMenuResolution_29 => Get("MODMenuResolution_29", "Height too big - must be less than 15360 pixels");

		// The below entries are for the audio options section of the F10 mod menu, in the file MOD.Scripts.UI/MODMenuAudioOptions.cs
		public static string MODMenuAudioOptions_0 => Get("MODMenuAudioOptions_0", "Audio Presets (Hotkey: 2)");
		public static string MODMenuAudioOptions_1 => Get("MODMenuAudioOptions_1", "Override SE");
		public static string MODMenuAudioOptions_2 => Get("MODMenuAudioOptions_2", "Invalid BGM cascade");
		public static string MODMenuAudioOptions_3 => Get("MODMenuAudioOptions_3", "Invalid SE cascade");
		public static string MODMenuAudioOptions_4 => Get("MODMenuAudioOptions_4", " (NOT INSTALLED)");
		public static string MODMenuAudioOptions_5 => Get("MODMenuAudioOptions_5", "\n\nWARNING: This audio set is not installed! You can try to run the installer again to update your mod with this option.\nYou're missing the BGM or SE folder in the StreamingAssets folder:");
		public static string MODMenuAudioOptions_6 => Get("MODMenuAudioOptions_6", "Force enable the following sound effects (ignore preset SE): ");
		public static string MODMenuAudioOptions_7 => Get("MODMenuAudioOptions_7", "The patch supports different Background Music (BGM) and Sound Effects(SE). Please click the button below for more information.");
		public static string MODMenuAudioOptions_8 => Get("MODMenuAudioOptions_8", "Open BGM/SE FAQ: 07th-mod.com/wiki/Higurashi/BGM-SE-FAQ/");
		public static string MODMenuAudioOptions_9 => Get("MODMenuAudioOptions_9", "Click here to open the  Background Music (BGM) and Sound Effects (SE) FAQ in your browser.\n\nThe BGM/SE FAQ contains information on the settings below.");
		public static string MODMenuAudioOptions_10 => Get("MODMenuAudioOptions_10", "https://07th-mod.com/wiki/Higurashi/BGM-SE-FAQ/");
		public static string MODMenuAudioOptions_11 => Get("MODMenuAudioOptions_11", "To continue, please choose a BGM/SE option below (hover button for info).\nYou can change this option later via the mod menu.");
		public static string MODMenuAudioOptions_12 => Get("MODMenuAudioOptions_12", "Option Not Installed!");

		// The below entries are for the audio setup optiosn which appear the first time you launch the game, in the file MOD.Scripts.UI/MODMenuAudioSetup.cs
		public static string MODMenuAudioSetup_0 => Get("MODMenuAudioSetup_0", "Click here when you're finished.");
		public static string MODMenuAudioSetup_1 => Get("MODMenuAudioSetup_1", "First-Time Setup Menu");
		public static string MODMenuAudioSetup_2 => Get("MODMenuAudioSetup_2", "Please choose the options on the left before continuing. You can hover over a button to view its description.");

		// The below entries are for the support section of the F10 mod menu, in the file MOD.Scripts.UI/MODMenuSupport.cs
		public static string MODMenuSupport_0 => Get("MODMenuSupport_0", "Please hover over a button for more information");
		public static string MODMenuSupport_1 => Get("MODMenuSupport_1", "Save Files and Log Files");
		public static string MODMenuSupport_2 => Get("MODMenuSupport_2", "Show output_log.txt / Player.log");
		public static string MODMenuSupport_3 => Get("MODMenuSupport_3", "This button shows the location of the 'ouput_log.txt' or 'Player.log' files\n\n- This file is called 'output_log.txt' on Windows and 'Player.log' on MacOS/Linux\n- This file records errors that occur during gameplay, and during game startup\n- This file helps when the game fails start, for example\n  - a corrupted save file\n  - the wrong UI (sharedassets0.assets) file\n- Note that each time the game starts up, the current log file is replaced");
		public static string MODMenuSupport_4 => Get("MODMenuSupport_4", "Show Saves");
		public static string MODMenuSupport_5 => Get("MODMenuSupport_5", "Click to open the save folder, which includes saves, quicksaves, and global save data\n\nClearing your save files can fix some issues with game startup, and resets all mod flags.\n\n- WARNING: Steam cloud will restore your saves if you manually delete them! Therefore, remember to disable steam cloud, otherwise your saves will magically reappear!\n- The 'global.dat' file stores your global unlock process and mod flags\n- The 'qsaveX.dat' and 'saveXXX.dat' files contain individual save files. Note that these becoming corrupted can break your game\n- It's recommended to take a backup of all your saves before you modify them");
		public static string MODMenuSupport_6 => Get("MODMenuSupport_6", "Quick Access to Game Folders");
		public static string MODMenuSupport_7 => Get("MODMenuSupport_7", "Show .assets Folder");
		public static string MODMenuSupport_8 => Get("MODMenuSupport_8", "Click to open the game's main data folder, which contains the .assets files.\n\nFonts, certain textures, and other game data is kept in the sharedassets0.assets file.");
		public static string MODMenuSupport_9 => Get("MODMenuSupport_9", "Show StreamingAssets Folder");
		public static string MODMenuSupport_10 => Get("MODMenuSupport_10", "Click to open the StreamingAssets folder, which contains most of the game assets.\n\n- Most Images/Textures, including Sprites, Backgrounds, Text Images, and Filter effects\n- Voices, Background Music, and Sound Effects\n- Game Scripts and Compiled Game Scripts\n- Movies\n- Other Misc Game Data");
		public static string MODMenuSupport_11 => Get("MODMenuSupport_11", "Show DLL Folder");
		public static string MODMenuSupport_12 => Get("MODMenuSupport_12", "Click to open the 'Managed' folder, which contains our modded 'Assembly-CSharp.dll'");
		public static string MODMenuSupport_13 => Get("MODMenuSupport_13", "Managed");
		public static string MODMenuSupport_14 => Get("MODMenuSupport_14", "Show Compiled Scripts");
		public static string MODMenuSupport_15 => Get("MODMenuSupport_15", "Sometimes out-of-date scripts can cause the game to fail to start up (stuck on black screen).\n\nYou can manually clear the *.mg files (compiled scripts) in this folder to force the game to regenerate them the next time the game starts.\n\nPlease be aware that the game will freeze for a couple minutes on a white screen, while scripts are being compiled.");
		public static string MODMenuSupport_16 => Get("MODMenuSupport_16", "Support Pages");
		public static string MODMenuSupport_17 => Get("MODMenuSupport_17", "Open Support Page: 07th-mod.com/wiki/Higurashi/support");
		public static string MODMenuSupport_18 => Get("MODMenuSupport_18", "If you have problems with the game, the information on this site may help.\n\nThere are also instructions on reporting bugs, as well as a link to our Discord server to contact us directly");
		public static string MODMenuSupport_19 => Get("MODMenuSupport_19", "https://07th-mod.com/wiki/Higurashi/support/");




	}
}
