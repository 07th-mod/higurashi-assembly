using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	static class MODActions
	{
		private enum WindowFilterType
		{
			Normal,
			ADV,
			NVLInADV,
			OG,
		}

		public enum ModPreset
		{
			ADV = 0,
			NVL = 1,
			OG = 2,
		}

		private static void TryRedrawTextWindowBackground(WindowFilterType filterType)
		{
			string windowFilterTextureName = "windo_filter";
			if (filterType == WindowFilterType.ADV)
			{
				windowFilterTextureName = "windo_filter_adv";
			}
			else if (filterType == WindowFilterType.NVLInADV)
			{
				windowFilterTextureName = "windo_filter_nvladv";
			}
			else if (filterType == WindowFilterType.OG)
			{
				windowFilterTextureName = "windo_filter_nvladv";
			}

			GameSystem.Instance.MainUIController.TryRedrawTextWindowBackground(windowFilterTextureName);
		}

		/// <summary>
		/// Cycles and saves ADV->NVL->OG->ADV...
		/// </summary>
		/// <returns>True if set and displayed, false if in a NVL_in_ADV region and value might not be applied immediately</returns>
		public static void ToggleAndSaveADVMode()
		{
			if (BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode").IntValue() == 1)
			{
				SetAndSaveADV(ModPreset.ADV);
			}
			else if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1)
			{
				SetAndSaveADV(ModPreset.NVL);
			}
			else
			{
				if(HasOGBackgrounds())
				{
					SetAndSaveADV(ModPreset.OG);
				}
				else
				{
					SetAndSaveADV(ModPreset.ADV);
				}
			}
		}

		/// <summary>
		/// Sets and saves NVL/ADV mode
		/// </summary>
		/// <param name="setADVMode">If True, sets and saves ADV mode. If False, sets and saves NVL mode</param>
		/// <param name="showInfoToast">If True, always shows a toast indicting the NVL/ADV status.
		/// If False, will only show toast if there is a warning (like you are in a nvl_in_adv region)</param>
		/// <returns>True if set and displayed, false if in a NVL_in_ADV region and value might not be applied immediately</returns>
		///
		/// NOTE: if this function is updated, you should update the corresponding "GetModeFromFlags()" function immediately below
		public static void SetAndSaveADV(ModPreset setting, bool showInfoToast = true)
		{
			MODMainUIController mODMainUIController = new MODMainUIController();
			if (setting == ModPreset.ADV)
			{
				BurikoMemory.Instance.SetGlobalFlag("GADVMode", 1);
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 0);
				BurikoMemory.Instance.SetGlobalFlag("GRyukishiMode", 0);
				BurikoMemory.Instance.SetGlobalFlag("GHideCG", 0);
				BurikoMemory.Instance.SetGlobalFlag("GStretchBackgrounds", 0);
				TryRedrawTextWindowBackground(WindowFilterType.ADV);
				mODMainUIController.WideGuiPositionStore();
				mODMainUIController.ADVModeSettingStore();
				string feedbackString = $"Set ADV Mode";
				int toastDuration = 3;
				Core.MODSystem.instance.modTextureController.SetArtStyle(0, showInfoToast);
				bool is_nvl_in_adv_region = BurikoMemory.Instance.GetFlag("NVL_in_ADV").IntValue() == 1;
				if (is_nvl_in_adv_region)
				{
					feedbackString += "\nIn NVL region - changes won't be displayed until later";
					toastDuration = 5;
				}
				if (is_nvl_in_adv_region || showInfoToast) { MODToaster.Show(feedbackString, isEnable: true, toastDuration: toastDuration); }
			}
			else if (setting == ModPreset.NVL)
			{
				BurikoMemory.Instance.SetGlobalFlag("GADVMode", 0);
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 2);
				BurikoMemory.Instance.SetGlobalFlag("GRyukishiMode", 0);
				BurikoMemory.Instance.SetGlobalFlag("GHideCG", 0);
				BurikoMemory.Instance.SetGlobalFlag("GStretchBackgrounds", 0);
				TryRedrawTextWindowBackground(WindowFilterType.Normal);
				mODMainUIController.WideGuiPositionStore();
				mODMainUIController.NVLModeSettingStore();
				Core.MODSystem.instance.modTextureController.SetArtStyle(0, showInfoToast);
				if (showInfoToast) { MODToaster.Show($"Set NVL Mode", isEnable: false); }
			}
			else if (setting == ModPreset.OG)
			{
				BurikoMemory.Instance.SetGlobalFlag("GADVMode", 0);
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 2);
				BurikoMemory.Instance.SetGlobalFlag("GRyukishiMode", 1);
				BurikoMemory.Instance.SetGlobalFlag("GHideCG", 1);
				BurikoMemory.Instance.SetGlobalFlag("GStretchBackgrounds", 0);
				TryRedrawTextWindowBackground(WindowFilterType.OG);
				mODMainUIController.RyukishiGuiPositionStore();
				mODMainUIController.RyukishiModeSettingStore();
				Core.MODSystem.instance.modTextureController.SetArtStyle(2, showInfoToast);
				if (showInfoToast) { MODToaster.Show($"Set OG Mode", isEnable: false); }
			}
		}

		// This expressions for 'presetModified' should be updated each time SetAndSaveADV() above is changed,
		// so that the player knows when the flags have changed from their default values for the current preset
		public static int GetADVNVLRyukishiModeFromFlags(out bool presetModified)
		{
			// If background override is enabled on any preset, the preset has been modified
			presetModified = BurikoMemory.Instance.GetGlobalFlag("GBackgroundSet").IntValue() != 0;

			if (BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode").IntValue() == 1)
			{
				presetModified = presetModified ||
					BurikoMemory.Instance.GetGlobalFlag("GArtStyle").IntValue() != 2 ||
					BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() != 0 ||
					BurikoMemory.Instance.GetGlobalFlag("GLinemodeSp").IntValue() != 2 ||
					BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode").IntValue() != 1 ||
					BurikoMemory.Instance.GetGlobalFlag("GHideCG").IntValue() != 1 ||
					BurikoMemory.Instance.GetGlobalFlag("GStretchBackgrounds").IntValue() != 0;

				return 2;
			}
			else if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1)
			{
				presetModified = presetModified ||
					BurikoMemory.Instance.GetGlobalFlag("GArtStyle").IntValue() != 0 ||
					BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() != 1 ||
					BurikoMemory.Instance.GetGlobalFlag("GLinemodeSp").IntValue() != 0 ||
					BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode").IntValue() != 0 ||
					BurikoMemory.Instance.GetGlobalFlag("GHideCG").IntValue() != 0 ||
					BurikoMemory.Instance.GetGlobalFlag("GStretchBackgrounds").IntValue() != 0;

				return 0;
			}
			else
			{
				presetModified = presetModified ||
					BurikoMemory.Instance.GetGlobalFlag("GArtStyle").IntValue() != 0 ||
					BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() != 0 ||
					BurikoMemory.Instance.GetGlobalFlag("GLinemodeSp").IntValue() != 2 ||
					BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode").IntValue() != 0 ||
					BurikoMemory.Instance.GetGlobalFlag("GHideCG").IntValue() != 0 ||
					BurikoMemory.Instance.GetGlobalFlag("GStretchBackgrounds").IntValue() != 0;

				return 1;
			}
		}

		public static void EnableNVLModeINADVMode()
		{
			BurikoMemory.Instance.SetFlag("NVL_in_ADV", 1);
			if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1)
			{
				MODMainUIController mODMainUIController = new MODMainUIController();
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 2);
				TryRedrawTextWindowBackground(WindowFilterType.NVLInADV);
				mODMainUIController.NVLADVModeSettingStore();
			}
		}

		public static void DisableNVLModeINADVMode()
		{
			BurikoMemory.Instance.SetFlag("NVL_in_ADV", 0);
			if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1)
			{
				MODMainUIController mODMainUIController = new MODMainUIController();
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 0);
				TryRedrawTextWindowBackground(WindowFilterType.ADV);
				mODMainUIController.ADVModeSettingStore();
			}
		}

		public static void DebugFontSizeChanger()
		{
			new MODMainUIController().DebugFontChangerSettingStore();
		}

		public static void AdjustVoiceVolumeRelative(int difference)
		{
			// Maintaining volume within limits is done in AdjustVoiceVolumeAbsolute()
			AdjustVoiceVolumeAbsolute(BurikoMemory.Instance.GetGlobalFlag("GVoiceVolume").IntValue() + difference);
		}

		public static void AdjustVoiceVolumeAbsolute(int uncheckedNewVolume)
		{
			int newVolume = Mathf.Clamp(uncheckedNewVolume, 0, 100);

			BurikoMemory.Instance.SetGlobalFlag("GVoiceVolume", newVolume);
			GameSystem.Instance.AudioController.VoiceVolume = (float)newVolume / 100f;
			GameSystem.Instance.AudioController.RefreshLayerVolumes();

			// Play a sample voice file so the user can get feedback on the set volume
			// For some reason the script uses "256" as the default volume, which gets divided by 128 to become 2.0f,
			// so to keep in line with the script, the test volume is set to "2.0f"
			GameSystem.Instance.AudioController.PlayVoice("voice_test.ogg", 3, 2.0f);
		}

		// Variant for global flags, using another variable as max limit
		public static int IncrementGlobalFlagWithRollover(string flagName, string maxFlagName)
		{
			return _IncrementFlagWithRollover(flagName, 0, BurikoMemory.Instance.GetGlobalFlag(maxFlagName).IntValue(), isLocalFlag: false);
		}

		// Variant for global flags, using literal limits
		public static int IncrementGlobalFlagWithRollover(string flagName, int minValueInclusive, int maxValueInclusive)
		{
			return _IncrementFlagWithRollover(flagName, minValueInclusive, maxValueInclusive, isLocalFlag: false);
		}

		// Variant for local flags
		public static int IncrementLocalFlagWithRollover(string flagName, int minValueInclusive, int maxValueInclusive)
		{
			return _IncrementFlagWithRollover(flagName, minValueInclusive, maxValueInclusive, isLocalFlag: true);
		}

		/// <summary>
		/// Increment a flag with rollover (from GetGlobalFlag())
		/// If min/max set to (3,6), it will loop over the values 3,4,5,6
		/// </summary>
		/// <param name="flagName">the name of the global flag, eg. "GVoiceVolume"</param>
		/// <param name="minValueInclusive">This is the min value the flag can be allowed to have before it rolls over, inclusive.</param>
		/// <param name="maxValueInclusive">This is the max value the flag can be allowed to have before it rolls over, inclusive.</param>
		/// <returns></returns>
		static int _IncrementFlagWithRollover(string flagName, int minValueInclusive, int maxValueInclusive, bool isLocalFlag)
		{
			int initialValue = isLocalFlag ? BurikoMemory.Instance.GetFlag(flagName).IntValue() : BurikoMemory.Instance.GetGlobalFlag(flagName).IntValue();

			int newValue = initialValue + 1;
			if (newValue > maxValueInclusive)
			{
				newValue = minValueInclusive;
			}

			if (isLocalFlag)
			{
				BurikoMemory.Instance.SetFlag(flagName, newValue);
			}
			else
			{
				BurikoMemory.Instance.SetGlobalFlag(flagName, newValue);
			}

			return newValue;
		}

		public static bool ToggleFlagAndSave(string flagName)
		{
			int newValue = (BurikoMemory.Instance.GetGlobalFlag(flagName).IntValue() + 1) % 2;
			BurikoMemory.Instance.SetGlobalFlag(flagName, newValue);

			return newValue == 1;
		}
		public static string VideoOpeningDescription(int videoOpeningValue)
		{
			switch (videoOpeningValue)
			{
				case 0:
					return "Unset";
				case 1:
					return "Disabled";
				case 2:
					return "In-game";
				case 3:
					return "At launch + in-game";
			}

			return "Unknown";
		}

		/// <summary>
		/// Returns the log folder where logs are kept - does not include the log filename
		/// </summary>
		/// <returns></returns>
		public static string GetLogFolder()
		{
			switch (MODUtility.GetPlatform())
			{
				case MODUtility.Platform.Windows:
				default:
					if(MODUtility.IsUnity2000())
					{
						// Higurashi Ep8 uses Unity 2017, which uses a folder in Appdata (similar to linux)
						// eg. C:\Users\[YOUR_USERNAME]\AppData\LocalLow\MangaGamer\Higurashi When They Cry - Ch.8 Matsuribayashi, where log file would be output_log.txt
						return MODUtility.CombinePaths(Environment.GetEnvironmentVariable("AppData"), "..", "LocalLow", Application.companyName, Application.productName);
					}
					else
					{
						// Higurashi 1-7 use the "HigurashiEp01_Data", which is one folder above the streamingAssets folder
						// eg. C:\games\Steam\steamapps\common\Higurashi When They Cry\HigurashiEp01_Data, where log file would be output_log.txt
						return MODUtility.CombinePaths(Application.streamingAssetsPath, "..");
					}

				//eg. ~/Library/Logs/Unity, where log file would be Player.log
				case MODUtility.Platform.MacOS:
					return "~/Library/Logs/Unity";

				//eg. ~/.config/unity3d/MangaGamer/GameName, where log file would be Player.log
				case MODUtility.Platform.Linux:
					return MODUtility.CombinePaths("~/.config/unity3d", Application.companyName, Application.productName);
			}
		}

		// Shows the folder containing the log files in the native file browser
		// The log file will either be called "output_log.txt" or "Player.log"
		public static void ShowLogFolder()
		{
			ShowFile(GetLogFolder());
		}

		public static void ShowSaveFolder()
		{
			ShowFile(MGHelper.GetSavePath());
		}

		//NOTE: paths might not open properly on windows if they contain backslashes
		public static void ShowFile(string path)
		{
			Assets.Scripts.Core.Logger.Log($"MOD ShowFile(): Showing [{path}]");
			try
			{
				switch (MODUtility.GetPlatform())
				{
					case MODUtility.Platform.Windows:
					default:
						Process.Start("explorer", path.Replace('/', '\\'));
						break;

					case MODUtility.Platform.MacOS:
						Process.Start("open", path.Replace('\\', '/'));
						break;

					case MODUtility.Platform.Linux:
						Process.Start("xdg-open", path.Replace('\\', '/'));
						break;
				}
			}
			catch(Exception e)
			{
				Assets.Scripts.Core.Logger.Log($"Failed to open {path}:\n{e}");
			}
		}

		public static bool HasOGBackgrounds()
		{
			return Directory.Exists(Path.Combine(Application.streamingAssetsPath, "OGBackgrounds"));
		}
		public static bool HasMusicToggle()
		{
			return Directory.Exists(Path.Combine(Application.streamingAssetsPath, "OGBGM"));
		}

		public static void ToggleFlagMenu()
		{
			int maxFlagMonitorValue = BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 0 ? 2 : 4;
			IncrementLocalFlagWithRollover("LFlagMonitor", 0, maxFlagMonitorValue);
		}
	}
}
