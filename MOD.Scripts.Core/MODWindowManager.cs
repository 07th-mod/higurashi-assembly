using Assets.Scripts.Core;
using MOD.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.Core
{
	class MODWindowManager
	{
		public static bool IsFullscreen;
		private static Resolution fullscreenResolution;
		private static int screenModeSet = -1;

		public static void FullscreenToggleAltEnter()
		{
			if (IsFullscreen)
			{
				DeFullscreen(PlayerPrefs.GetInt("width"), PlayerPrefs.GetInt("height"));
			}
			else
			{
				GoFullscreen();
			}
		}

		public static void FullscreenTogglePressF()
		{
			if (IsFullscreen)
			{
				int num14 = PlayerPrefs.GetInt("width");
				int num15 = PlayerPrefs.GetInt("height");
				if (num14 == 0 || num15 == 0)
				{
					num14 = 640;
					num15 = 480;
				}
				DeFullscreen(width: num14, height: num15);
			}
			else
			{
				GoFullscreen();
			}
		}

		public static void ConfigMenuButtonSetResolution(bool IsFullscreenArgument, int height)
		{
			if (IsFullscreenArgument)
			{
				GoFullscreen();
			}
			else
			{
				int width = Mathf.RoundToInt(height * GameSystem.Instance.AspectRatio);
				DeFullscreen(width: width, height: height);
				PlayerPrefs.SetInt("width", width);
				PlayerPrefs.SetInt("height", height);
			}
		}

		public static void ConfigMenuButtonGoFullscreen()
		{
			GoFullscreen();
		}

		public static void FullscreenToggleMODMenu()
		{
			if (IsFullscreen)
			{
				DeFullscreen(PlayerPrefs.GetInt("width"), PlayerPrefs.GetInt("height"));
			}
			else
			{
				GoFullscreen();
			}
		}

		public static void UpdateWindowAspect(float AspectRatio)
		{
			if (!IsFullscreen)
			{
				int width = Mathf.RoundToInt((float)Screen.height * AspectRatio);
				Screen.SetResolution(width, Screen.height, fullscreen: false);
			}
			PlayerPrefs.SetInt("width", Mathf.RoundToInt(PlayerPrefs.GetInt("height") * AspectRatio));
		}

		public static void GameSystemInitSetResolution()
		{
			IsFullscreen = PlayerPrefs.GetInt("is_fullscreen", 0) == 1;
			fullscreenResolution.width = 0;
			fullscreenResolution.height = 0;

			fullscreenResolution = GetFullscreenResolution();
			if (IsFullscreen)
			{
				Screen.SetResolution(fullscreenResolution.width, fullscreenResolution.height, fullscreen: true);
			}
			else if (PlayerPrefs.HasKey("height") && PlayerPrefs.HasKey("width"))
			{
				int width = PlayerPrefs.GetInt("width");
				int height = PlayerPrefs.GetInt("height");
				Debug.Log("Requesting window size " + width + "x" + height + " based on config file");
				Screen.SetResolution(width, height, fullscreen: false);
			}
			if ((Screen.width < 640 || Screen.height < 480) && !IsFullscreen)
			{
				Screen.SetResolution(640, 480, fullscreen: false);
			}
		}

		public static void GetFullScreenResolutionLateUpdate()
		{
			if (screenModeSet == -1)
			{
				screenModeSet = 0;
				fullscreenResolution = Screen.currentResolution;
				if (PlayerPrefs.HasKey("fullscreen_width") && PlayerPrefs.HasKey("fullscreen_height") && Screen.fullScreen)
				{
					fullscreenResolution.width = PlayerPrefs.GetInt("fullscreen_width");
					fullscreenResolution.height = PlayerPrefs.GetInt("fullscreen_height");
				}
				Debug.Log("Fullscreen Resolution: " + fullscreenResolution.width + ", " + fullscreenResolution.height);
			}
		}

		public static void ScreenManagerFix()
		{
			// Fixes an issue where Unity would write garbage values to its saved state on Linux
			// If we do this while the game is running, Unity will overwrite the values
			// So do it in the finalizer, which will run as the game quits and the GameSystem is deallocated
			if (PlayerPrefs.HasKey("width") && PlayerPrefs.HasKey("height"))
			{
				int width = PlayerPrefs.GetInt("width");
				int height = PlayerPrefs.GetInt("height");
				PlayerPrefs.SetInt("Screenmanager Resolution Width", width);
				PlayerPrefs.SetInt("Screenmanager Resolution Height", height);
				PlayerPrefs.SetInt("is_fullscreen", IsFullscreen ? 1 : 0);
				PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", 0);
			}
		}
		public static void MODMenuSetAndSaveResolution(int height)
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

			int width = Mathf.RoundToInt(height * 16f / 9f);
			Screen.SetResolution(width, height, Screen.fullScreen);
			PlayerPrefs.SetInt("width", width);
			PlayerPrefs.SetInt("height", height);
		}

		private static void GoFullscreen()
		{
			IsFullscreen = true;
			PlayerPrefs.SetInt("is_fullscreen", 1);
			Resolution resolution = GetFullscreenResolution();
			Screen.SetResolution(resolution.width, resolution.height, fullscreen: true);
			Debug.Log(resolution.width + " , " + resolution.height);
			PlayerPrefs.SetInt("fullscreen_width", resolution.width);
			PlayerPrefs.SetInt("fullscreen_height", resolution.height);
		}

		private static void DeFullscreen(int width, int height)
		{
			IsFullscreen = false;
			PlayerPrefs.SetInt("is_fullscreen", 0);
			Screen.SetResolution(width, height, fullscreen: false);
		}

		private static Resolution GetFullscreenResolution()
		{
			Resolution resolution = new Resolution();
			string source = "";
			// Try to guess resolution from Screen.currentResolution
			if (!Screen.fullScreen || Application.platform == RuntimePlatform.OSXPlayer)
			{
				resolution.width = fullscreenResolution.width = Screen.currentResolution.width;
				resolution.height = fullscreenResolution.height = Screen.currentResolution.height;
				source = "Screen.currentResolution";
			}
			else if (fullscreenResolution.width > 0 && fullscreenResolution.height > 0)
			{
				resolution.width = fullscreenResolution.width;
				resolution.height = fullscreenResolution.height;
				source = "Stored fullscreenResolution";
			}
			else if (PlayerPrefs.HasKey("fullscreen_width") && PlayerPrefs.HasKey("fullscreen_height"))
			{
				resolution.width = PlayerPrefs.GetInt("fullscreen_width");
				resolution.height = PlayerPrefs.GetInt("fullscreen_height");
				source = "PlayerPrefs";
			}
			else
			{
				resolution.width = Screen.currentResolution.width;
				resolution.height = Screen.currentResolution.height;
				source = "Screen.currentResolution as Fallback";
			}

			// Above can be glitchy on Linux, so also check the maximum resolution of a single monitor
			// If it's bigger than that, then switch over
			// Note that this (from what I can tell) gives you the biggest resolution of any of your monitors,
			// not just the one the game is running under, so it could *also* be wrong, which is why we check both methods
			if (Screen.resolutions.Length > 0)
			{
				int index = 0;
				Resolution best = Screen.resolutions[0];
				for (int i = 1; i < Screen.resolutions.Length; i++)
				{
					if (Screen.resolutions[i].height * Screen.resolutions[i].width > best.height * best.width)
					{
						best = Screen.resolutions[i];
						index = i;
					}
				}
				if (best.width <= resolution.width && best.height <= resolution.height)
				{
					resolution = best;
					source = "Screen.resolutions #" + index;
				}
			}
			if (!PlayerPrefs.HasKey("fullscreen_width_override"))
			{
				PlayerPrefs.SetInt("fullscreen_width_override", 0);
			}
			if (!PlayerPrefs.HasKey("fullscreen_height_override"))
			{
				PlayerPrefs.SetInt("fullscreen_height_override", 0);
			}

			if (PlayerPrefs.GetInt("fullscreen_width_override") > 0)
			{
				resolution.width = PlayerPrefs.GetInt("fullscreen_width_override");
				source += " + Width Override";
			}
			if (PlayerPrefs.GetInt("fullscreen_height_override") > 0)
			{
				resolution.height = PlayerPrefs.GetInt("fullscreen_height_override");
				source += " + Height Override";
			}
			Debug.Log("Using resolution " + resolution.width + "x" + resolution.height + " as the fullscreen resolution based on " + source + ".");
			return resolution;
		}
	}
}
