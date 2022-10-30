using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	class MODMenuSupport
	{
		private static Vector2 emergencyMenuScrollPosition;

		/// <summary>
		/// NOTE: You must call this from within a OnGUI() call
		///
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

		/// <summary>
		/// NOTE: You must call this from within a OnGUI() call
		/// </summary>
		public static void ShowSupportButtons(Func<GUIContent, bool> buttonRenderer)
		{
			MODMenuCommon.Label("Save Files and Log Files");
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

				if (buttonRenderer(new GUIContent("Show Saves", "Click to open the save folder, which includes saves, quicksaves, and global save data\n\n" +
					"Clearing your save files can fix some issues with game startup, and resets all mod flags.\n\n" +
					"- WARNING: Steam cloud will restore your saves if you manually delete them! Therefore, remember to disable steam cloud, otherwise your saves will magically reappear!\n" +
					"- The 'global.dat' file stores your global unlock process and mod flags\n" +
					"- The 'qsaveX.dat' and 'saveXXX.dat' files contain individual save files. Note that these becoming corrupted can break your game\n" +
					"- It's recommended to take a backup of all your saves before you modify them")))
				{
					MODActions.ShowSaveFolder();
				}

				GUILayout.EndHorizontal();
			}

			MODMenuCommon.Label("Quick Access to Game Folders");
			{
				GUILayout.BeginHorizontal();
				if (buttonRenderer(new GUIContent("Show .assets Folder", "Click to open the game's main data folder, which contains the .assets files.\n\n" +
					"Fonts, certain textures, and other game data is kept in the sharedassets0.assets file.")))
				{
					Application.OpenURL(Application.dataPath);
				}

				if (buttonRenderer(new GUIContent("Show StreamingAssets Folder", "Click to open the StreamingAssets folder, which contains most of the game assets.\n\n" +
					"- Most Images/Textures, including Sprites, Backgrounds, Text Images, and Filter effects\n" +
					"- Voices, Background Music, and Sound Effects\n" +
					"- Game Scripts and Compiled Game Scripts\n" +
					"- Movies\n" +
					"- Other Misc Game Data")))
				{
					Application.OpenURL(Application.streamingAssetsPath);
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				if (buttonRenderer(new GUIContent("Show DLL Folder", "Click to open the 'Managed' folder, which contains our modded 'Assembly-CSharp.dll'")))
				{
					Application.OpenURL(System.IO.Path.Combine(Application.dataPath, "Managed"));
				}

				if (buttonRenderer(new GUIContent("Show Compiled Scripts", "Sometimes out-of-date scripts can cause the game to fail to start up (stuck on black screen).\n\n" +
					"You can manually clear the *.mg files (compiled scripts) in this folder to force the game to regenerate them the next time the game starts.\n\n" +
					"Please be aware that the game will freeze for a couple minutes on a white screen, while scripts are being compiled.")))
				{
					MODActions.ShowCompiledScripts();
				}

				GUILayout.EndHorizontal();

				if(Application.platform == RuntimePlatform.WindowsPlayer)
				{
					string playerPrefsPath = $"Computer\\HKEY_CURRENT_USER\\SOFTWARE\\{Application.companyName}\\{Application.productName}";
					if (buttonRenderer(new GUIContent(
							"Copy PlayerPrefs Registry Path",
							$"Click to copy the playerprefs registry folder to clipboard. This registry folder mainly contains Unity resolution settings.\n\n" +
							$"Paste the path into the Windows Registry Editor (regedit) to view playerprefs.\n\n" +
							$"Registry Path: {playerPrefsPath}"
						)))
					{
						GUIUtility.systemCopyBuffer = playerPrefsPath;
					}
				}
				else
				{
					string playerPrefsPath = Application.platform == RuntimePlatform.LinuxPlayer ?
							$"~/.config/unity3d/{Application.companyName}/{Application.productName}" :
							"~/Library/Preferences";

					if (buttonRenderer(new GUIContent("Show PlayerPrefs Folder", $"Click to show the folder containing the playerprefs config file. This config file mainly contains Unity resolution settings.\n\nPath: {playerPrefsPath}")))
					{
						MODActions.ShowFile(playerPrefsPath);
					}
				}
			}

			MODMenuCommon.Label("Support Pages");
			if (buttonRenderer(new GUIContent("Open Support Page: 07th-mod.com/wiki/Higurashi/support", "If you have problems with the game, the information on this site may help.\n\n" +
				"There are also instructions on reporting bugs, as well as a link to our Discord server to contact us directly")))
			{
				Application.OpenURL("https://07th-mod.com/wiki/Higurashi/support/");
			}
		}
	}
}
