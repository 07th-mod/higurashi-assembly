using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using MOD.Scripts.Core.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	static class MODKeyboardShortcuts
	{

		public static bool ModInputHandlingAllowed()
		{
			GameSystem gameSystem = GameSystem.Instance;

			switch (gameSystem.GameState)
			{
				case GameState.RightClickMenu:
				case GameState.Normal:
				case GameState.TitleScreen:
				case GameState.MODDisableInput:
					break;

				default:
					return false;
			}


			if (!gameSystem.IsInitialized || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
			{
				return false;
			}

			// Don't allow mod options on any wait, except "WaitForInput"
			// Note: if this is removed, it mostly still works, but if changing art style during
			// an animation, some things may bug out until the next scene.
			foreach (Wait w in gameSystem.WaitList)
			{
				if (w.Type != WaitTypes.WaitForInput)
				{
					return false;
				}
			}

			return true;
		}

		enum Action
		{
			ToggleADV,
			CensorshipLevel,
			EffectLevel,
			FlagMonitor,
			ModMenu,
			OpeningVideo,
			DebugFontSize,
			AltBGM,
			AltBGMFlow,
			AltSE,
			AltSEFlow,
			AltVoice,
			AltVoicePriority,
			LipSync,
			VoiceVolumeUp,
			VoiceVolumeDown,
			VoiceVolumeMax,
			VoiceVolumeMin,
			ToggleArtStyle,
			DebugMode,
			RestoreSettings,
		}

		private static Action? GetUserAction()
		{
			// These take priority over the non-shift key buttons
			if (Input.GetKey(KeyCode.LeftShift))
			{
				if (Input.GetKeyDown(KeyCode.F10))
				{
					return Action.FlagMonitor;
				}
				else if (Input.GetKeyDown(KeyCode.F11))
				{
					return Action.OpeningVideo;
				}
				else if (Input.GetKeyDown(KeyCode.F12))
				{
					return Action.DebugMode;
				}
				else if (Input.GetKeyDown(KeyCode.M))
				{
					return Action.VoiceVolumeMax;
				}
				else if (Input.GetKeyDown(KeyCode.N))
				{
					return Action.VoiceVolumeMin;
				}
			}

			if (Input.GetKeyDown(KeyCode.F1))
			{
				return Action.ToggleADV;
			}
			else if (Input.GetKeyDown(KeyCode.F2))
			{
				return Action.CensorshipLevel;
			}
			else if (Input.GetKeyDown(KeyCode.F3))
			{
				return Action.EffectLevel;
			}
			else if (Input.GetKeyDown(KeyCode.F10))
			{
				return Action.ModMenu;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
			{
				return Action.DebugFontSize;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
			{
				return Action.AltBGM;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
			{
				return Action.AltBGMFlow;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
			{
				return Action.AltSE;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
			{
				return Action.AltSEFlow;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
			{
				return Action.AltVoice;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
			{
				return Action.AltVoicePriority;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
			{
				return Action.LipSync;
			}
			else if (Input.GetKeyDown(KeyCode.M))
			{
				return Action.VoiceVolumeUp;
			}
			else if (Input.GetKeyDown(KeyCode.P))
			{
				return Action.ToggleArtStyle;
			}
			else if (Input.GetKeyDown(KeyCode.N))
			{
				return Action.VoiceVolumeDown;
			}

			return null;
		}

		private static void ModHandleUserAction(Action action)
		{
			switch (action)
			{
				case Action.ToggleADV:
					MODActions.ToggleAndSaveADVMode();
					break;

				case Action.CensorshipLevel:
					{
						int newCensorNum = MODActions.IncrementGlobalFlagWithRollover("GCensor", "GCensorMaxNum");
						MODToaster.Show(
							$"Censorship Level: {newCensorNum}{(newCensorNum == 2 ? " (default)" : "")}",
							numberedSound: newCensorNum
						);
					}
					break;

				case Action.EffectLevel:
					{
						int effectLevel = MODActions.IncrementGlobalFlagWithRollover("GEffectExtend", "GEffectExtendMaxNum");
						MODToaster.Show($"Effect Level: {effectLevel} (Not Used)", numberedSound: effectLevel);
					}
					break;

				case Action.FlagMonitor:
					{
						int maxFlagMonitorValue = BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 0 ? 2 : 4;
						MODActions.IncrementLocalFlagWithRollover("LFlagMonitor", 0, maxFlagMonitorValue);
					}
					break;

				case Action.OpeningVideo:
					{
						// Loop "GVideoOpening" over the values 1-3.
						// 0 is skipped as it represents "value not set"
						int newVideoOpening = MODActions.IncrementGlobalFlagWithRollover("GVideoOpening", 1, 3);
						MODToaster.Show($"OP Video: {MODActions.VideoOpeningDescription(newVideoOpening)} ({newVideoOpening})");
					}
					break;

				case Action.DebugFontSize when BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 1 || BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 2:
					MODActions.DebugFontSizeChanger();
					break;

				case Action.AltBGM:
					{
						bool altBGMEnabled = MODActions.ToggleFlagAndSave("GAltBGM");
						MODToaster.Show($"Alt BGM: {(altBGMEnabled ? "ON" : "OFF")} (Not Used)", isEnable: altBGMEnabled);
					}
					break;

				case Action.AltBGMFlow:
					{
						int newAltBGMFlow = MODActions.IncrementGlobalFlagWithRollover("GAltBGMflow", "GAltBGMflowMaxNum");
						MODAudioTracking.Instance.SetAndSaveBGMSE(newAltBGMFlow);
						MODToaster.Show($"{MODAudioTracking.GetBGMNameFromAltBGMFlag(newAltBGMFlow)} ({newAltBGMFlow})");
					}
					break;

				case Action.AltSE:
					{
						bool seIsEnabled = MODActions.ToggleFlagAndSave("GAltSE");
						MODToaster.Show($"Alt SE: {(seIsEnabled ? "ON" : "OFF")} (Not Used)", isEnable: seIsEnabled);
					}
					break;

				case Action.AltSEFlow:
					{
						int newAltSEFlow = MODActions.IncrementGlobalFlagWithRollover("GAltSEflow", "GAltSEflowMaxNum");
						MODToaster.Show($"Alt SE Flow: {newAltSEFlow} (Not Used)", numberedSound: newAltSEFlow);
					}
					break;

				case Action.AltVoice:
					{
						bool altVoiceIsEnabled = MODActions.ToggleFlagAndSave("GAltVoice");
						MODToaster.Show($"Alt Voice: {(altVoiceIsEnabled ? "ON" : "OFF")} (Not Used)", isEnable: altVoiceIsEnabled);
					}
					break;

				case Action.AltVoicePriority:
					{
						bool altVoicePriorityIsEnabled = MODActions.ToggleFlagAndSave("GAltVoicePriority");
						MODToaster.Show($"Alt Priority: {(altVoicePriorityIsEnabled ? "ON" : "OFF")} (Not Used)", isEnable: altVoicePriorityIsEnabled);
					}
					break;

				case Action.LipSync:
					{
						bool lipSyncIsEnabled = MODActions.ToggleFlagAndSave("GLipSync");
						MODToaster.Show($"Lip Sync: {(lipSyncIsEnabled ? "ON" : "OFF")}", isEnable: lipSyncIsEnabled);
					}
					break;

				case Action.VoiceVolumeUp:
					MODActions.AdjustVoiceVolumeRelative(5);
					break;

				case Action.VoiceVolumeDown:
					MODActions.AdjustVoiceVolumeRelative(-5);
					break;

				case Action.VoiceVolumeMax:
					MODActions.AdjustVoiceVolumeAbsolute(100);
					break;

				case Action.VoiceVolumeMin:
					MODActions.AdjustVoiceVolumeAbsolute(0);
					break;

				case Action.ToggleArtStyle:
					MOD.Scripts.Core.MODSystem.instance.modTextureController.ToggleArtStyle();
					break;

				// Enable debug mode, which shows an extra 2 panels of info in the flag menu
				// Note that if your debug mode is 0, there is no way to enable debug mode unless you manually set the flag value in the game script
				case Action.DebugMode when BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() != 0:
					{
						int debugMode = MODActions.IncrementGlobalFlagWithRollover("GMOD_DEBUG_MODE", 1, 2);
						MODToaster.Show($"Debug Mode: {debugMode}", numberedSound: debugMode);
					}
					break;

				// Restore game settings
				case Action.RestoreSettings:
					{
						int restoreGameSettingsNum = MODActions.IncrementGlobalFlagWithRollover("GMOD_SETTING_LOADER", 0, 3);
						MODToaster.Show($"Reset Settings: {restoreGameSettingsNum} (see F10 menu)", numberedSound: restoreGameSettingsNum);
					}
					break;

				default:
					Assets.Scripts.Core.Logger.Log($"Warning: Unknown mod action {action} was requested to be executed");
					break;
			}
		}

		/// <summary>
		/// Handles mod inputs. Call from Update function.
		/// </summary>
		/// <returns>Currently the return value is not used for anything</returns>
		public static bool ModInputHandler()
		{
			if (GetUserAction() is Action action)
			{
				if (action == Action.ModMenu)
				{
					GameSystem.Instance.MainUIController.modMenu.ToggleVisibility();
				}
				else
				{
					if (!ModInputHandlingAllowed())
					{
						MODToaster.Show($"Please let animation finish first and/or close the menu");
					}
					else
					{
						ModHandleUserAction(action);
					}
				}
			}

			return true;
		}
	}
}
