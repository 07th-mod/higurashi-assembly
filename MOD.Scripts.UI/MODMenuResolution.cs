﻿using Assets.Scripts.Core;
using MOD.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;

namespace MOD.Scripts.UI
{
	class MODMenuResolution
	{
		private string screenHeightString;

		public MODMenuResolution()
		{
			screenHeightString = "";
		}

		public void OnBeforeMenuVisible()
		{
			screenHeightString = $"{Screen.height}";
		}

		public void OnGUI()
		{
			Label($"Resolution Settings (Current: {Screen.width}x{Screen.height})");

			// I noticed a bug on Linux where going Windowed sometimes doesn't let you change window size,
			// probably due res settings Playerprefs that Unity reads on startup.
			// Restarting the game after changing settings fixes this.
			if(Application.platform == RuntimePlatform.LinuxPlayer)
			{
				Label($"NOTE: You may need to restart the game after changing to Windowed!");
			}

			{
				GUILayout.BeginHorizontal();
				if (Button(new GUIContent("480p", "Set resolution to 853 x 480"))) { SetAndSaveResolution(480); }
				if (Button(new GUIContent("720p", "Set resolution to 1280 x 720"))) { SetAndSaveResolution(720); }
				if (Button(new GUIContent("1080p", "Set resolution to 1920 x 1080"))) { SetAndSaveResolution(1080); }
				if (Button(new GUIContent("1440p", "Set resolution to 2560 x 1440"))) { SetAndSaveResolution(1440); }

				GUIContent buttonContent = MODWindowManager.IsFullscreen ?
					new GUIContent("Go Windowed", "Toggle Fullscreen") :
					new GUIContent("Go Fullscreen", "Toggle Fullscreen");

				if(Button(buttonContent))
				{
					MODWindowManager.FullscreenToggle(showToast: true);
				}

				screenHeightString = GUILayout.TextField(screenHeightString);
				if (Button(new GUIContent("Set", "Sets a custom resolution - mainly for windowed mode.\n\n" +
					"Height set automatically to maintain 16:9 aspect ratio.")))
				{
					if (int.TryParse(screenHeightString, out int new_height))
					{
						SetAndSaveResolution(new_height);
					}
				}
				GUILayout.EndHorizontal();

				if (Button(new GUIContent(
					"Reset Fullscreen Resolution",
					"Force re-detection of the fullscreen resolution (matching your monitor's max resolution).\n\n" +
					"You may want to do this if you change to a monitor with a different fullscreen resolution, and the game detects the wrong resolution.")
					))
				{
					MODWindowManager.ClearPlayerPrefsFullscreenResolution();
					MODWindowManager.GoFullscreen();
				}
			}
		}

		private void SetAndSaveResolution(int height)
		{
			screenHeightString = $"{height}";
			MODWindowManager.SetResolution(height, showToast: true);
		}
	}
}
