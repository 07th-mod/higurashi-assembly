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
	}
}
