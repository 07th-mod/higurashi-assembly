using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko;
using UnityEngine;

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
				_ = gameSystem.WaitList.Count;
				_ = 0;
				return true;
			}
			if (gameSystem.IsForceSkip)
			{
				gameSystem.IsSkipping = false;
				gameSystem.IsForceSkip = false;
			}
			if (Input.GetKeyDown(KeyCode.Space))
			{
				if (!gameSystem.MessageBoxVisible && gameSystem.GameState == GameState.Normal)
				{
					return false;
				}
				gameSystem.SwitchToViewMode();
				return false;
			}
			if ((Input.GetAxis("Mouse ScrollWheel") > 0f && gameSystem.IsInWindowBounds) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.PageUp))
			{
				if (!gameSystem.MessageBoxVisible && gameSystem.GameState == GameState.Normal)
				{
					return false;
				}
				gameSystem.SwitchToHistoryScreen();
				return false;
			}
			if (((Input.GetMouseButtonDown(0) || Input.GetAxis("Mouse ScrollWheel") < 0f) && gameSystem.IsInWindowBounds) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.PageDown) || Input.GetKeyDown(KeyCode.KeypadEnter))
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
			if ((Input.GetMouseButtonDown(1) && gameSystem.IsInWindowBounds) || Input.GetKeyDown(KeyCode.Escape))
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
				if (Screen.fullScreen)
				{
					int num = PlayerPrefs.GetInt("width");
					int num2 = PlayerPrefs.GetInt("height");
					if (num == 0 || num2 == 0)
					{
						num = 640;
						num2 = 480;
					}
					Screen.SetResolution(num, num2, fullscreen: false);
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
				AssetManager.Instance.UseNewArt = !AssetManager.Instance.UseNewArt;
				BurikoMemory.Instance.SetGlobalFlag("GArtStyle", AssetManager.Instance.UseNewArt ? 1 : 0);
				GameSystem.Instance.SceneController.ReloadAllImages();
			}
			return true;
		}

		public GameState GetStateType()
		{
			return GameState.Normal;
		}
	}
}
