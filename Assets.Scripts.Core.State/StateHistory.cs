using Assets.Scripts.Core.History;
using System;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateHistory : IGameState
	{
		private GameSystem gameSystem;

		private HistoryWindow historyWindow;

		public StateHistory()
		{
			gameSystem = GameSystem.Instance;
			GameObject gameObject = UnityEngine.Object.Instantiate(gameSystem.HistoryPrefab);
			historyWindow = gameObject.GetComponent<HistoryWindow>();
			if (historyWindow == null)
			{
				throw new Exception("Failed to instantiate historyWindow!");
			}
			gameSystem.MainUIController.FadeOut(0.2f, isBlocking: false);
			gameSystem.SceneController.HideFace(0.2f);
			gameSystem.ExecuteActions();
		}

		public void RequestLeaveImmediate()
		{
		}

		public void RequestLeave()
		{
			historyWindow.Leave(delegate
			{
				gameSystem.PopStateStack();
				gameSystem.UpdateWaits();
				gameSystem.MessageBoxVisible = true;
				gameSystem.MainUIController.ShowMessageBox();
				gameSystem.ExecuteActions();
			});
		}

		public void OnLeaveState()
		{
			if (historyWindow != null)
			{
				UnityEngine.Object.Destroy(historyWindow);
			}
		}

		public void OnRestoreState()
		{
		}

		public bool InputHandler()
		{
			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				RequestLeave();
			}
			if (Input.GetKeyDown(KeyCode.PageUp))
			{
				historyWindow.Step(5f);
			}
			if (Input.GetKeyDown(KeyCode.PageDown))
			{
				historyWindow.Step(-5f);
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				historyWindow.Step(1f);
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				historyWindow.Step(-1f);
			}
			return false;
		}

		public GameState GetStateType()
		{
			return GameState.History;
		}
	}
}
