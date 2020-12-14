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

		// NOTE: Returning "false" from this function prevents the game from advancing!
		// See GameSystem.Update() in  for details.
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

			// Right click or ESC key toggles the menu
			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
			{
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

			// Quicksave
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

			// Quickload
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

			// Auto-Mode
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

			// Skip
			if (Input.GetKeyDown(KeyCode.S))
			{
				gameSystem.IsSkipping = !gameSystem.IsSkipping;
			}

			// Fullscreen
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

			// Toggle Language
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

			// Returning true here allows the game to continue
			return true;
		}

		public GameState GetStateType()
		{
			return GameState.Normal;
		}
	}
}
