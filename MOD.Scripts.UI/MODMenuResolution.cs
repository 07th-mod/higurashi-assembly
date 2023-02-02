using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;

namespace MOD.Scripts.UI
{
	class MODMenuResolution
	{
		private string overrideFullScreenResolutionDescription =
			"Set a custom fullscreen resolution\n\n" +
			"Use this option only if the fullscreen resolution is detected incorrectly (such as on some Linux systems)\n" +
			"You can manually type in a resolution to use below.\n\n" +
			"Click 'Clear Override' to let the game automatically determine the fullscreen resolution";

		private string screenHeightString;
		private string fullscreenWidthOverrideString;
		private string fullscreenHeightOverrideString;
		private int overrideFullResCountdown;

		public MODMenuResolution()
		{
			screenHeightString = "";
		}

		public void OnBeforeMenuVisible()
		{
			screenHeightString = $"{Screen.height}";

			if (PlayerPrefs.HasKey("fullscreen_width_override"))
			{
				fullscreenWidthOverrideString = $"{PlayerPrefs.GetInt("fullscreen_width_override")}";
			}

			if (PlayerPrefs.HasKey("fullscreen_height_override"))
			{
				fullscreenHeightOverrideString = $"{PlayerPrefs.GetInt("fullscreen_height_override")}";
			}

			overrideFullResCountdown = 3;
		}

		public void OnGUI()
		{
			Label("Windowed Resolution Settings");
			{
				GUILayout.BeginHorizontal();

				if (!GameSystem.Instance.IsFullscreen)
				{
					if (Button(new GUIContent("480p", "Set resolution to 853 x 480"))) { SetAndSaveResolution(480); }
					if (Button(new GUIContent("720p", "Set resolution to 1280 x 720"))) { SetAndSaveResolution(720); }
					if (Button(new GUIContent("1080p", "Set resolution to 1920 x 1080"))) { SetAndSaveResolution(1080); }
					if (Button(new GUIContent("1440p", "Set resolution to 2560 x 1440"))) { SetAndSaveResolution(1440); }

					screenHeightString = GUILayout.TextField(screenHeightString);
					if (Button(new GUIContent("Set", "Sets a custom resolution - mainly for windowed mode.\n\n" +
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
							int new_width = Mathf.RoundToInt(new_height * GameSystem.Instance.AspectRatio);
							GameSystem.Instance.SetResolution(new_width, new_height, Screen.fullScreen);
							PlayerPrefs.SetInt("width", new_width);
							PlayerPrefs.SetInt("height", new_height);
						}
					}
				}

				if (GameSystem.Instance.IsFullscreen)
				{
					if (Button(new GUIContent("Click here to go Windowed to change these settings", "Toggle Fullscreen")))
					{
						GameSystem.Instance.DeFullscreen(PlayerPrefs.GetInt("width"), PlayerPrefs.GetInt("height"));
					}
				}
				else
				{
					if (Button(new GUIContent("Go Fullscreen", "Toggle Fullscreen")))
					{
						GameSystem.Instance.GoFullscreen();
					}
				}

				GUILayout.EndHorizontal();
			}

			GUILayout.Space(20);

			Resolution detectedRes = GameSystem.Instance.GetFullscreenResolution(useOverride: false, doLogging: false);
			Resolution actualRes = GameSystem.Instance.GetFullscreenResolution(useOverride: true, doLogging: false);
			string overrideString = FullscreenOverrideEnabled() ? $"{actualRes.width} x {actualRes.height}" : "Off";
			Label($"Fullscreen Resolution Override (Detected: {detectedRes.width} x {detectedRes.height} Override: {overrideString})");
			{
				GUILayout.BeginHorizontal();

					GUILayout.BeginHorizontal();
					LabelRightAlign("Width:");
					fullscreenWidthOverrideString = GUILayout.TextField(fullscreenWidthOverrideString, 5);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					LabelRightAlign("Height:");
					fullscreenHeightOverrideString = GUILayout.TextField(fullscreenHeightOverrideString, 5);
					GUILayout.EndHorizontal();


				bool shouldUpdateFullscreenResolution = false;
				string gameClearButtonText = overrideFullResCountdown > 0 ? $"Click {overrideFullResCountdown} times to override resolution" : $"Override Fullscreen Resolution";
				if (Button(new GUIContent(gameClearButtonText, overrideFullScreenResolutionDescription)))
				{
					if (overrideFullResCountdown > 0)
					{
						overrideFullResCountdown--;
					}

					if (overrideFullResCountdown <= 0)
					{
						if(int.TryParse(fullscreenWidthOverrideString, out int widthOverride) && widthOverride >= 640)
						{
							if(int.TryParse(fullscreenHeightOverrideString, out int heightOverride) && heightOverride >= 480)
							{
								PlayerPrefs.SetInt("fullscreen_width_override", widthOverride);
								PlayerPrefs.SetInt("fullscreen_height_override", heightOverride);
								shouldUpdateFullscreenResolution = true;
								MODToaster.Show($"Fullscreen: {widthOverride} x {heightOverride}");
							}
							else
							{
								MODToaster.Show("Invalid Height");
							}
						}
						else
						{
							MODToaster.Show("Invalid Width");
						}
					}
				}

				if (Button(new GUIContent("Clear Override", overrideFullScreenResolutionDescription)))
				{
					PlayerPrefs.SetInt("fullscreen_width_override", 0);
					PlayerPrefs.SetInt("fullscreen_height_override", 0);
					fullscreenWidthOverrideString = "0";
					fullscreenHeightOverrideString = "0";
					shouldUpdateFullscreenResolution = true;
				}

				if (shouldUpdateFullscreenResolution && GameSystem.Instance.IsFullscreen)
				{
					GameSystem.Instance.GoFullscreen();
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
			int width = Mathf.RoundToInt(height * GameSystem.Instance.AspectRatio);
			GameSystem.Instance.SetResolution(width, height, Screen.fullScreen);
			PlayerPrefs.SetInt("width", width);
			PlayerPrefs.SetInt("height", height);
		}

		private bool FullscreenOverrideEnabled()
		{
			return	PlayerPrefs.GetInt("fullscreen_width_override", 0)  != 0 ||
					PlayerPrefs.GetInt("fullscreen_height_override", 0) != 0;
		}
	}
}
