using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	class MODMenuResolution : MODMenuModuleInterface
	{
		private string screenHeightString;
		MODMenuCommon c;

		public MODMenuResolution(MODMenuCommon c)
		{
			this.c = c;
			screenHeightString = "";
		}

		public void OnBeforeMenuVisible()
		{
			screenHeightString = $"{Screen.height}";
		}

		public void OnGUI()
		{
			c.Label("Resolution Settings");
			{
				GUILayout.BeginHorizontal();
				if (c.Button(new GUIContent("480p", "Set resolution to 853 x 480"))) { SetAndSaveResolution(480); }
				if (c.Button(new GUIContent("720p", "Set resolution to 1280 x 720"))) { SetAndSaveResolution(720); }
				if (c.Button(new GUIContent("1080p", "Set resolution to 1920 x 1080"))) { SetAndSaveResolution(1080); }
				if (c.Button(new GUIContent("1440p", "Set resolution to 2560 x 1440"))) { SetAndSaveResolution(1440); }
				if (GameSystem.Instance.IsFullscreen)
				{
					if (c.Button(new GUIContent("Windowed", "Toggle Fullscreen")))
					{
						GameSystem.Instance.DeFullscreen(PlayerPrefs.GetInt("width"), PlayerPrefs.GetInt("height"));
					}
				}
				else
				{
					if (c.Button(new GUIContent("Fullscreen", "Toggle Fullscreen")))
					{
						GameSystem.Instance.GoFullscreen();
					}
				}

				screenHeightString = GUILayout.TextField(screenHeightString);
				if (c.Button(new GUIContent("Set", "Sets a custom resolution - mainly for windowed mode.\n\n" +
					"Height set automatically to maintain 16:9 aspect ratio.")))
				{
					if (int.TryParse(screenHeightString, out int new_height))
					{
						if (new_height < 480)
						{
							MODToaster.Show("Height too small - must be at least 480 pixels");
							new_height = 480;
						}
						else if (new_height > 15360)
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
		}

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
	}
}
