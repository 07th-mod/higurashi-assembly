using Assets.Scripts.UI.SaveLoad;
using System;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateSaveLoad : IGameState
	{
		private GameSystem gameSystem;

		private SaveLoadManager saveLoadManager;

		private bool isActive = true;

		public StateSaveLoad(bool restoreUI)
		{
			gameSystem = GameSystem.Instance;
			GameObject gameObject = UnityEngine.Object.Instantiate(gameSystem.SaveLoadPrefab);
			saveLoadManager = gameObject.GetComponent<SaveLoadManager>();
			if (saveLoadManager == null)
			{
				throw new Exception("Failed to instantiate configManager!");
			}
			saveLoadManager.Open(restoreUI);
			gameSystem.ExecuteActions();
		}

		public void Leave(Action onLeave)
		{
			saveLoadManager.Leave(delegate
			{
				gameSystem.PopStateStack();
				gameSystem.ExecuteActions();
				if (onLeave != null)
				{
					onLeave();
				}
			});
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
			gameSystem.RegisterAction(delegate
			{
				saveLoadManager.RefreshList();
			});
			gameSystem.ExecuteActions();
		}

		public bool InputHandler()
		{
			if (!isActive)
			{
				return false;
			}
			if (Input.GetMouseButtonDown(1))
			{
				Leave(null);
				isActive = false;
			}
			return false;
		}

		public GameState GetStateType()
		{
			return GameState.SaveLoadScreen;
		}
	}
}
