using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.Audio;
using MOD.Scripts.Core;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Core.State
{
	internal class StateNormal : IGameState
	{
		private GameSystem gameSystem;

		public StateNormal()
		{
			gameSystem = GameSystem.Instance;
		}

		public void RequestLeaveImmediate()
		{
		}

		public void RequestLeave()
		{
		}

		public void OnLeaveState()
		{
		}

		public void OnRestoreState()
		{
		}

		public bool InputHandler()
		{
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				if (!gameSystem.CanSkip)
				{
					return true;
				}
				gameSystem.IsSkipping = true;
				gameSystem.IsForceSkip = true;
				if (gameSystem.WaitList.Count > 0)
				{
					return true;
				}
				return true;
			}
			if (gameSystem.IsForceSkip)
			{
				gameSystem.IsSkipping = false;
				gameSystem.IsForceSkip = false;
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				var voices = gameSystem.TextHistory.LatestVoice;
				AudioController.Instance.PlayVoices(voices);
				return false;
			}
			if (Input.GetKeyDown(KeyCode.Space))
			{
				if (!gameSystem.MessageBoxVisible && gameSystem.GameState == GameState.Normal)
				{
					return false;
				}
				gameSystem.IsSkipping = false;
				gameSystem.IsForceSkip = false;
				gameSystem.IsAuto = false;
				gameSystem.SwitchToViewMode();
				return false;
			}
			if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.PageUp))
			{
				if (!gameSystem.MessageBoxVisible && gameSystem.GameState == GameState.Normal)
				{
					return false;
				}
				gameSystem.SwitchToHistoryScreen();
				return false;
			}
			if (Input.GetMouseButtonDown(0) || Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.PageDown) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				if (gameSystem.IsSkipping)
				{
					gameSystem.IsSkipping = false;
				}
				if (gameSystem.IsAuto && !gameSystem.ClickDuringAuto)
				{
					gameSystem.IsAuto = false;
					if (gameSystem.WaitList.Exists((Wait a) => a.Type == WaitTypes.WaitForAuto))
					{
						gameSystem.AddWait(new Wait(0f, WaitTypes.WaitForInput, null));
					}
					return false;
				}
				if (UICamera.hoveredObject == gameSystem.SceneController.SceneCameras || UICamera.hoveredObject == null)
				{
					gameSystem.ClearWait();
				}
				return false;
			}
			if (!Input.GetMouseButtonDown(1) && !Input.GetKeyDown(KeyCode.Escape))
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					if (Input.GetKeyDown(KeyCode.F10))
					{
						if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
						{
							return false;
						}
						if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
						{
							return false;
						}
						if (BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 0)
						{
							return false;
						}
						if (BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 1)
						{
							BurikoMemory.Instance.SetGlobalFlag("GMOD_DEBUG_MODE", 2);
							GameSystem.Instance.AudioController.PlaySystemSound("switchsound/enable.ogg");
							return true;
						}
						BurikoMemory.Instance.SetGlobalFlag("GMOD_DEBUG_MODE", 1);
						GameSystem.Instance.AudioController.PlaySystemSound("switchsound/disable.ogg");
						return true;
					}
					if (Input.GetKeyDown(KeyCode.F9))
					{
						if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
						{
							return false;
						}
						if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
						{
							return false;
						}
						int num = BurikoMemory.Instance.GetGlobalFlag("GMOD_SETTING_LOADER").IntValue();
						if (num < 3 && num >= 0)
						{
							num++;
							string str = num.ToString();
							string str2 = ".ogg";
							string filename = "switchsound/" + str + str2;
							GameSystem.Instance.AudioController.PlaySystemSound(filename);
							BurikoMemory.Instance.SetGlobalFlag("GMOD_SETTING_LOADER", num);
							return true;
						}
						num = 0;
						BurikoMemory.Instance.SetGlobalFlag("GMOD_SETTING_LOADER", num);
						GameSystem.Instance.AudioController.PlaySystemSound("switchsound/0.ogg");
					}
					if (Input.GetKeyDown(KeyCode.M))
					{
						if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
						{
							return false;
						}
						if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
						{
							return false;
						}
						AdjustVoiceVolumeAbsolute(100);
						return true;
					}
					if (Input.GetKeyDown(KeyCode.N))
					{
						if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
						{
							return false;
						}
						if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
						{
							return false;
						}
						if (BurikoMemory.Instance.GetGlobalFlag("GVoiceVolume").IntValue() == 0)
						{
							return true;
						}
						AdjustVoiceVolumeAbsolute(0);
						return true;
					}
				}
				if (Input.GetKeyDown(KeyCode.F1))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetFlag("NVL_in_ADV").IntValue() == 1)
					{
						GameSystem.Instance.MainUIController.ShowToast($"Can't toggle now - try later", maybeSound: null);
						return false;
					}
					GameSystem.Instance.MainUIController.MODResetLayerBackground();
				}
				if (Input.GetKeyDown(KeyCode.F2))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetFlag("DisableModHotkey").IntValue() == 1)
					{
						return false;
					}
					int censorNum = BurikoMemory.Instance.GetGlobalFlag("GCensor").IntValue();
					int maxCensorNum = BurikoMemory.Instance.GetGlobalFlag("GCensorMaxNum").IntValue();
					censorNum = (censorNum + 1) % (maxCensorNum + 1); //cycle from 0 to maxCensorNum
					BurikoMemory.Instance.SetGlobalFlag("GCensor", censorNum);
					GameSystem.Instance.MainUIController.ShowToast(
						$"Censorship Level: {censorNum}{(censorNum == 2 ? " (default)" : "")}",
						numberedSound: censorNum
					);
				}
				if (Input.GetKeyDown(KeyCode.F3))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetFlag("DisableModHotkey").IntValue() == 1)
					{
						return false;
					}
					int num6 = BurikoMemory.Instance.GetGlobalFlag("GEffectExtend").IntValue();
					int num7 = BurikoMemory.Instance.GetGlobalFlag("GEffectExtendMaxNum").IntValue();
					if (num6 < num7 && num6 >= 0)
					{
						num6++;
						string str5 = num6.ToString();
						string str6 = ".ogg";
						string filename3 = "switchsound/" + str5 + str6;
						GameSystem.Instance.AudioController.PlaySystemSound(filename3);
						BurikoMemory.Instance.SetGlobalFlag("GEffectExtend", num6);
						return true;
					}
					num6 = 0;
					BurikoMemory.Instance.SetGlobalFlag("GEffectExtend", num6);
					GameSystem.Instance.AudioController.PlaySystemSound("switchsound/0.ogg");
				}
				if (Input.GetKeyDown(KeyCode.F5))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (!gameSystem.CanSave)
					{
						return false;
					}
					BurikoScriptSystem.Instance.SaveQuickSave();
					GameSystem.Instance.AudioController.PlaySystemSound("switchsound/enable.ogg");
				}
				if (Input.GetKeyDown(KeyCode.F7))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					BurikoScriptSystem.Instance.LoadQuickSave();
				}
				if (Input.GetKeyDown(KeyCode.F10))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() != 1 && BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() != 2)
					{
						if (BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() == 0)
						{
							BurikoMemory.Instance.SetFlag("LFlagMonitor", 1);
							return true;
						}
						if (BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue() == 1)
						{
							BurikoMemory.Instance.SetFlag("LFlagMonitor", 2);
							return true;
						}
						BurikoMemory.Instance.SetFlag("LFlagMonitor", 0);
						return true;
					}
					int num8 = BurikoMemory.Instance.GetFlag("LFlagMonitor").IntValue();
					if (num8 < 4)
					{
						num8++;
						BurikoMemory.Instance.SetFlag("LFlagMonitor", num8);
						return true;
					}
					if (num8 >= 4 || num8 < 0)
					{
						BurikoMemory.Instance.SetFlag("LFlagMonitor", 0);
						return true;
					}
				}
				if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 1 || BurikoMemory.Instance.GetGlobalFlag("GMOD_DEBUG_MODE").IntValue() == 2)
					{
						GameSystem.Instance.MainUIController.MODDebugFontSizeChanger();
					}
				}
				if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetFlag("DisableModHotkey").IntValue() == 1)
					{
						return false;
					}
					if (BurikoMemory.Instance.GetGlobalFlag("GAltBGM").IntValue() == 1)
					{
						BurikoMemory.Instance.SetGlobalFlag("GAltBGM", 0);
						GameSystem.Instance.AudioController.PlaySystemSound("switchsound/disable.ogg");
						return true;
					}
					BurikoMemory.Instance.SetGlobalFlag("GAltBGM", 1);
					GameSystem.Instance.AudioController.PlaySystemSound("switchsound/enable.ogg");
				}
				if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetFlag("DisableModHotkey").IntValue() == 1)
					{
						return false;
					}
					int num9 = BurikoMemory.Instance.GetGlobalFlag("GAltBGMflow").IntValue();
					int num10 = BurikoMemory.Instance.GetGlobalFlag("GAltBGMflowMaxNum").IntValue();
					if (num9 < num10 && num9 >= 0)
					{
						num9++;
						string str7 = num9.ToString();
						string str8 = ".ogg";
						string filename4 = "switchsound/" + str7 + str8;
						GameSystem.Instance.AudioController.PlaySystemSound(filename4);
						BurikoMemory.Instance.SetGlobalFlag("GAltBGMflow", num9);
						return true;
					}
					num9 = 0;
					BurikoMemory.Instance.SetGlobalFlag("GAltBGMflow", num9);
					GameSystem.Instance.AudioController.PlaySystemSound("switchsound/0.ogg");
				}
				if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetFlag("DisableModHotkey").IntValue() == 1)
					{
						return false;
					}
					if (BurikoMemory.Instance.GetGlobalFlag("GAltSE").IntValue() == 1)
					{
						BurikoMemory.Instance.SetGlobalFlag("GAltSE", 0);
						GameSystem.Instance.AudioController.PlaySystemSound("switchsound/disable.ogg");
						return true;
					}
					BurikoMemory.Instance.SetGlobalFlag("GAltSE", 1);
					GameSystem.Instance.AudioController.PlaySystemSound("switchsound/enable.ogg");
				}
				if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetFlag("DisableModHotkey").IntValue() == 1)
					{
						return false;
					}
					int num11 = BurikoMemory.Instance.GetGlobalFlag("GAltSEflow").IntValue();
					int num12 = BurikoMemory.Instance.GetGlobalFlag("GAltSEflowMaxNum").IntValue();
					if (num11 < num12 && num11 >= 0)
					{
						num11++;
						string str9 = num11.ToString();
						string str10 = ".ogg";
						string filename5 = "switchsound/" + str9 + str10;
						GameSystem.Instance.AudioController.PlaySystemSound(filename5);
						BurikoMemory.Instance.SetGlobalFlag("GAltSEflow", num11);
						return true;
					}
					num11 = 0;
					BurikoMemory.Instance.SetGlobalFlag("GAltSEflow", num11);
					GameSystem.Instance.AudioController.PlaySystemSound("switchsound/0.ogg");
				}
				if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetFlag("DisableModHotkey").IntValue() == 1)
					{
						return false;
					}
					if (BurikoMemory.Instance.GetGlobalFlag("GAltVoice").IntValue() == 1)
					{
						BurikoMemory.Instance.SetGlobalFlag("GAltVoice", 0);
						GameSystem.Instance.AudioController.PlaySystemSound("switchsound/disable.ogg");
						return true;
					}
					BurikoMemory.Instance.SetGlobalFlag("GAltVoice", 1);
					GameSystem.Instance.AudioController.PlaySystemSound("switchsound/enable.ogg");
				}
				if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetFlag("DisableModHotkey").IntValue() == 1)
					{
						return false;
					}
					if (BurikoMemory.Instance.GetGlobalFlag("GAltVoicePriority").IntValue() == 1)
					{
						BurikoMemory.Instance.SetGlobalFlag("GAltVoicePriority", 0);
						GameSystem.Instance.AudioController.PlaySystemSound("switchsound/disable.ogg");
						return true;
					}
					BurikoMemory.Instance.SetGlobalFlag("GAltVoicePriority", 1);
					GameSystem.Instance.AudioController.PlaySystemSound("switchsound/enable.ogg");
				}
				if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}
					if (BurikoMemory.Instance.GetFlag("DisableModHotkey").IntValue() == 1)
					{
						return false;
					}

					// Toggle lipsync between 0 and 1
					int newLipSync = (BurikoMemory.Instance.GetGlobalFlag("GLipSync").IntValue() + 1) % 2;
					BurikoMemory.Instance.SetGlobalFlag("GLipSync", newLipSync);
					GameSystem.Instance.MainUIController.ShowToast($"Lip Sync: {(newLipSync == 1 ? "ON": "OFF")}");
				}
				if (Input.GetKeyDown(KeyCode.M))
				{
					if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
					{
						return false;
					}
					if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
					{
						return false;
					}

					AdjustVoiceVolumeRelative(5);

					return true;
				}
				if (!Input.GetKeyDown(KeyCode.N))
				{
					if (Input.GetKeyDown(KeyCode.A))
					{
						gameSystem.IsAuto = !gameSystem.IsAuto;
						if (gameSystem.IsAuto)
						{
							return true;
						}
						if (gameSystem.WaitList.Exists((Wait a) => a.Type == WaitTypes.WaitForAuto))
						{
							gameSystem.AddWait(new Wait(0f, WaitTypes.WaitForInput, null));
						}
					}
					if (Input.GetKeyDown(KeyCode.S))
					{
						gameSystem.IsSkipping = !gameSystem.IsSkipping;
					}
					if (Input.GetKeyDown(KeyCode.F))
					{
						if (GameSystem.Instance.IsFullscreen)
						{
							int num14 = PlayerPrefs.GetInt("width");
							int num15 = PlayerPrefs.GetInt("height");
							if (num14 == 0 || num15 == 0)
							{
								num14 = 640;
								num15 = 480;
							}
							GameSystem.Instance.DeFullscreen(width: num14, height: num15);
						}
						else
						{
							GameSystem.Instance.GoFullscreen();
						}
					}
					if (Input.GetKeyDown(KeyCode.L))
					{
						if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
						{
							return false;
						}
						if (!gameSystem.HasWaitOfType(WaitTypes.WaitForText))
						{
							GameSystem.Instance.UseEnglishText = !GameSystem.Instance.UseEnglishText;
							int val = 0;
							if (gameSystem.UseEnglishText)
							{
								val = 1;
							}
							gameSystem.TextController.SwapLanguages();
							BurikoMemory.Instance.SetGlobalFlag("GLanguage", val);
						}
					}
					if (Input.GetKeyDown(KeyCode.P))
					{
						if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
						{
							return false;
						}
						if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
						{
							return false;
						}
						MODSystem.instance.modTextureController.ToggleArtStyle();
					}
					return true;
				}

				// The below section of code is executed if the N key is pressed.
				// TODO: lift the body of the `if (!Input.GetKeyDown(KeyCode.N))` statement outside so it is less confusing.
				if (!gameSystem.MessageBoxVisible || gameSystem.IsAuto || gameSystem.IsSkipping || gameSystem.IsForceSkip)
				{
					return false;
				}
				if (!gameSystem.HasWaitOfType(WaitTypes.WaitForInput))
				{
					return false;
				}
				int num16 = BurikoMemory.Instance.GetGlobalFlag("GVoiceVolume").IntValue();
				if (num16 == 0)
				{
					return true;
				}

				AdjustVoiceVolumeRelative(-5);

				return true;
			}
			if (!gameSystem.MessageBoxVisible && gameSystem.GameState == GameState.Normal)
			{
				return false;
			}
			if (gameSystem.IsAuto && gameSystem.ClickDuringAuto)
			{
				gameSystem.IsAuto = false;
				if (gameSystem.WaitList.Exists((Wait a) => a.Type == WaitTypes.WaitForAuto))
				{
					gameSystem.AddWait(new Wait(0f, WaitTypes.WaitForInput, null));
				}
				return false;
			}
			gameSystem.IsSkipping = false;
			gameSystem.IsForceSkip = false;
			if (gameSystem.RightClickMenu)
			{
				gameSystem.SwitchToRightClickMenu();
			}
			else
			{
				gameSystem.SwitchToHiddenWindow2();
			}
			return false;
		}

		public GameState GetStateType()
		{
			return GameState.Normal;
		}

		private void AdjustVoiceVolumeRelative(int difference)
		{
			// Maintaining volume within limits is done in AdjustVoiceVolumeAbsolute()
			AdjustVoiceVolumeAbsolute(BurikoMemory.Instance.GetGlobalFlag("GVoiceVolume").IntValue() + difference);
		}

		private void AdjustVoiceVolumeAbsolute(int uncheckedNewVolume)
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
	}
}
