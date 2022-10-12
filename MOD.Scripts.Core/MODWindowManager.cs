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
		static string playerPrefsCrashDetectorKey = "crash_detector";
		const string FULLSCREEN_LOCK_KEY = "fullscreen_lock";

		private static bool _IsFullscreen;
		public static bool IsFullscreen { get { return _IsFullscreen; } }
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
			"Screenmanager Is Fullscreen mode",
			playerPrefsCrashDetectorKey
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
		public static void FullscreenToggle(bool showToast=false)
		{
			SetResolution(maybe_width: null, maybe_height: null, maybe_fullscreen: !IsFullscreen, showToast: showToast);
		}

		// Set the screen resolution, where the width will be set according to the current AspectRatio
		// The windowed/fullscreen state won't be changed.
		public static void SetResolution(int height, bool showToast = false)
		{
			SetResolution(maybe_width: null, maybe_height: height, maybe_fullscreen: null, showToast: showToast);
		}

		// Go fullscreen. The new resolution will be detected automatically.
		public static void GoFullscreen(bool showToast = false)
		{
			SetResolution(maybe_width: null, maybe_height: null, maybe_fullscreen: true, showToast: showToast);
		}

		// This function does the following:
		// - If full screen is enabled, the resolution will set according to the monitor resolution
		// - If windowed, the window width will be set according to the height and AspectRatio
		public static void RefreshWindowAspect(bool showToast = false)
		{
			SetResolution(maybe_width: null, maybe_height: null, maybe_fullscreen: null, showToast: showToast);
		}

		public static void SetResolution(int? maybe_width, int? maybe_height, bool? maybe_fullscreen, bool showToast=false)
		{
			int height = 480;
			int width = 640;

			// Default to keeping current fullscreen state if fullscreen not specified
			TrySetIsFullscreen(maybe_fullscreen ?? IsFullscreen, "SetResolution");

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

			if (showToast)
			{
				string prefix = "Set Res";
				if(maybe_fullscreen == false && FullscreenLocked())
				{
					prefix = "Fullscreen Locked";
				}
				MODToaster.Show($"{prefix}: {width}x{height}");
			}
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

		private static bool PlayerPrefsNeedsReset()
		{
			// If there was a crash, then assume playerprefs needs reset to prevent further crashes
			if (PlayerPrefs.HasKey(playerPrefsCrashDetectorKey))
			{
				return true;
			}

			// TODO: not sure if should enable this, as if Unity resets 'Screenmanager Is Fullscreen mode' to windowed
			// because the player was playing in windowed, and our mod wasn't able to override it,
			// some users will have their settings reset unexpectedly.
			//
			// Additionally, the above crash detection seems sufficient to fix the Gnome crash bug.
			// On Linux, if 'Screenmanager Is Fullscreen mode' is set to 0, assume game needs playerprefs reset?
			//if (Application.platform == RuntimePlatform.LinuxPlayer)
			//{
			//	if (PlayerPrefs.HasKey("Screenmanager Is Fullscreen mode"))
			//	{
			//		return PlayerPrefs.GetInt("Screenmanager Is Fullscreen mode") == 0;
			//	}
			//	else
			//	{
			//		return true;
			//	}
			//}

			return false;
		}

		public static void GameSystemInitSetResolution()
		{
			PrintPlayerPrefs("On Startup");

			// On OS other than Linux, default to disabling fullscreen lock rather than asking the user,
			// as it doesn't help with any known bugs on Windows/MacOS
			if (Application.platform != RuntimePlatform.LinuxPlayer && !FullscreenLockConfigured())
			{
				SetFullScreenLock(false);
			}

			// If crash detected on linux, force fullscreen mode, in case crash was caused by gnome windowed mode bug.
			// Also, show the window setup menu again so user can configure fullscreen lock.
			if (Application.platform == RuntimePlatform.LinuxPlayer && PlayerPrefsNeedsReset())
			{
				ForceUnconfigureFullscreenLock();
				// TODO: could fully reset playerprefs by calling PlayerPrefs.DeleteAll(), but not sure if such drastic measures are necessary?
				GoFullscreen();
				Debug.Log("WARNING: Crash or corrupted playerprefs detected. Reverting to fullscreen mode!");
				PrintPlayerPrefs("After Fixing due to Crash or Corrupted PlayerPrefs");
			}

			PlayerPrefs.SetInt(playerPrefsCrashDetectorKey, 1);

			// Restore IsFullscreen variable from playerprefs
			TrySetIsFullscreen(PlayerPrefs.GetInt("is_fullscreen", 1) == 1, "On Startup");

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

		// This is inserted into both:
		//  - ~GameSystem(), which DOES NOT execute on Windows when the program exits (only Linux?)
		//  - OnApplicationQuit, to be called only if the application is really quitting, which runs on Windows
		//  - Calling this function more than once shouldn't cause any problems.
		//
		// Also note that on Windows:
		//
		//  - if DISPLAY CONFIRMATION is OFF, and the 'task' is ended via normal task manager (not via the process list)
		//    this function is still called, because Windows will first try to close the program normally.
		//
		//  - if DISPLAY CONFIRMATION is ON, since the program can't close on its own, windows will then
		//    force close the program which is detected as a crash
		//
		//  - Additionally, if you go to the process instead of the program and kill it that way, it will always
		//    be detected as a crash
		//
		// On Linux, closing via task manager generally is treated as a crash.
		public static void OnApplicationReallyQuit(string context)
		{
			// Fixes an issue where Unity would write garbage values to its saved state on Linux
			// If we do this while the game is running, Unity will overwrite the values
			// So do it in the finalizer, which will run as the game quits and the GameSystem is deallocated
			//
			// This also allows us to override the Screenmanager playerprefs variables on Windows,
			// which are normally overwritten when the game exits
			SetPlayerPrefs();

			// Clear the crash detector key if program closed normally
			PlayerPrefs.DeleteKey(playerPrefsCrashDetectorKey);

			PrintPlayerPrefs($"OnApplicationReallyQuit() called from {context}");
		}

		public static void SetFullScreenLock(bool enableLock)
		{
			PlayerPrefs.SetInt(FULLSCREEN_LOCK_KEY, enableLock ? 1 : 0);
			if(FullscreenLocked())
			{
				GoFullscreen();
			}
		}

		public static bool FullscreenLocked()
		{
			return PlayerPrefs.GetInt(FULLSCREEN_LOCK_KEY, 0) != 0;
		}

		public static bool FullscreenLockConfigured()
		{
			return PlayerPrefs.HasKey(FULLSCREEN_LOCK_KEY);
		}
		public static void ForceUnconfigureFullscreenLock()
		{
			PlayerPrefs.DeleteKey(FULLSCREEN_LOCK_KEY);
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
		private static void TrySetIsFullscreen(bool isFullscreen, string context)
		{
			if (!isFullscreen && FullscreenLocked())
			{
				Debug.Log($"WARNING [{context}]: Attempted to change to windowed mode, but 'fullscreen lock' enabled, so staying in fullscreen mode!");
				return;
			}

			_IsFullscreen = isFullscreen;
		}
	}
}
