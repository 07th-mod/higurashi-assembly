using Assets.Scripts.Core;
using MOD.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

namespace MOD.Scripts.Core
{
	class MODWindowManager
	{
		public static bool IsFullscreen;
		private static Resolution fullscreenResolution;
		private static int screenModeSet = -1;

		private static int lastSetWidth = 640;
		private static int lastSetHeight = 480;

		private static string[] prefsToPrint = {
			"width",
			"height",
			"is_fullscreen",
			"fullscreen_width",
			"fullscreen_height",
			"Screenmanager Resolution Width",
			"Screenmanager Resolution Height",
			"Screenmanager Is Fullscreen mode"
		};

		private static void PrintPlayerPrefs(string caption)
		{
			string PrintPref(string key)
			{
				return $"{key}: {(PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key).ToString() : "<MISSING>")}";
			}

			// Print out certain playerprefs values for debugging
			Debug.Log($"PlayerPrefs [{caption}]:\n{string.Join("\n", prefsToPrint.Select(key => PrintPref(key)).ToArray())}");
		}

		// Toggle between windowed and fullscreen.
		// Windowed mode will use the last windowed resolution.
		// Fullscreen mode will use the detected fullscreen resolution.
		public static void FullscreenToggle()
		{
			SetResolution(maybe_width: null, maybe_height: null, maybe_fullscreen: !IsFullscreen);
		}

		// Set the screen resolution, where the width will be set according to the current AspectRatio
		// The windowed/fullscreen state won't be changed.
		public static void SetWindowed(int height)
		{
			SetResolution(maybe_width: null, maybe_height: height, maybe_fullscreen: false);
		}

		// Go fullscreen. The new resolution will be detected automatically.
		public static void GoFullscreen()
		{
			SetResolution(maybe_width: null, maybe_height: null, maybe_fullscreen: true);
		}

		// This function does the following:
		// - If full screen is enabled, the resolution will set according to the monitor resolution
		// - If windowed, the window width will be set according to the height and AspectRatio
		public static void RefreshWindowAspect()
		{
			SetResolution(maybe_width: null, maybe_height: null, maybe_fullscreen: null);
		}

		private static void SetResolution(int? maybe_width, int? maybe_height, bool? maybe_fullscreen)
		{
			int height = 480;
			int width = 640;

			// Default to keeping current fullscreen state if fullscreen not specified
			IsFullscreen = maybe_fullscreen ?? IsFullscreen;

			if (maybe_width == null && maybe_height == null)
			{
				if (IsFullscreen)
				{
					// If going fullscreen, and width and height wasn't specified, use detected fullscreen resolution
					Resolution resolution = GetFullscreenResolution();
					height = resolution.height;
					width = resolution.width;
				}
				else if(PlayerPrefs.HasKey("height") && PlayerPrefs.HasKey("width"))
				{
					// If width and height both not specified, use saved player prefs width and height
					int player_prefs_height = PlayerPrefs.GetInt("height");
					int player_prefs_width = PlayerPrefs.GetInt("width");

					if (player_prefs_width != 0 && player_prefs_height != 0)
					{
						height = player_prefs_height;
						width = player_prefs_width;
					}
				}
			}
			else if (maybe_width == null && maybe_height != null)
			{
				// If only height specified, use aspect ratio to set width
				height = maybe_height.Value;
				width = Mathf.RoundToInt(height * GameSystem.Instance.AspectRatio);
			}
			else if (maybe_width != null && maybe_height != null)
			{
				// If both specified, just use directly (ignore current aspect ratio)
				height = maybe_height.Value;
				width = maybe_width.Value;
			}

			// Do some sanity checks on the width and height
			if (height < 480)
			{
				MODToaster.Show("Height too small - must be at least 480 pixels");
				Debug.Log("Height too small - must be at least 480 pixels");
				height = 480;
			}
			else if (height > 15360)
			{
				MODToaster.Show("Height too big - must be less than 15360 pixels");
				Debug.Log("Height too big - must be less than 15360 pixels");
				height = 15360;
			}

			if (width < 640)
			{
				MODToaster.Show("Width too small - must be at least 640 pixels");
				Debug.Log("Width too small - must be at least 640 pixels");
				width = 640;
			}
			else if (width > 15360)
			{
				MODToaster.Show("Width too big - must be less than 15360 pixels");
				Debug.Log("Width too big - must be less than 15360 pixels");
				width = 15360;
			}

			Screen.SetResolution(width, height, IsFullscreen);

			lastSetWidth = width;
			lastSetHeight = height;

			// Update playerprefs (won't be saved until game exits or PlayerPrefs.Save() is called
			SetPlayerPrefs();
		}

		// NOTE: this function does not save playerprefs
		// playerprefs are saved when the game exits cleanly, or on manual calls to PlayerPrefs.Save()
		private static void SetPlayerPrefs()
		{
			PlayerPrefs.SetInt(IsFullscreen ? "fullscreen_width" : "width", lastSetWidth);
			PlayerPrefs.SetInt(IsFullscreen ? "fullscreen_height" : "height", lastSetHeight);
			PlayerPrefs.SetInt("is_fullscreen", IsFullscreen ? 1 : 0);

			PlayerPrefs.SetInt("Screenmanager Resolution Width", lastSetWidth);
			PlayerPrefs.SetInt("Screenmanager Resolution Height", lastSetHeight);

			// This used to be always set false, but on Linux Gnome this caused
			// TODO: decide whether to set this to IsFullscreen, or to just always set true.
			// On Windows this doesn't seem to make any difference
			PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", 1);
		}

		public static void GameSystemInitSetResolution()
		{
			PrintPlayerPrefs("On Startup");

			// Restore IsFullscreen variable from playerprefs
			IsFullscreen = PlayerPrefs.GetInt("is_fullscreen", 1) == 1;

			// Restore fullscreenResolution variable using GetFullscreenResolution()
			fullscreenResolution.width = 0;
			fullscreenResolution.height = 0;
			fullscreenResolution = GetFullscreenResolution();

			// TODO: fix this when restoring from fullscreen on startup
			// Now that variables restored set the actual resolution
			SetResolution(null, null, null);

			// If the playerprefs is corrupted, the game will crash shortly after starting up
			// This prevents us from repairing the playerprefs, because the game normally only saves when the game exits cleanly
			// To fix this, save the playerprefs immediately as soon as the game starts up
			PlayerPrefs.Save();

			PrintPlayerPrefs("After Init Resolution");
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
			SetPlayerPrefs();

			PrintPlayerPrefs("On Shutdown");
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
