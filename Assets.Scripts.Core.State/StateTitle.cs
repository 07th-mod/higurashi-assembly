using Assets.Scripts.Core.Buriko;
using Assets.Scripts.UI.TitleScreen;
using System;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateTitle : IGameState
	{
		private GameSystem gameSystem;

		private TitleScreen titleScreen;

		public StateTitle()
		{
			gameSystem = GameSystem.Instance;
			GameObject gameObject = UnityEngine.Object.Instantiate(gameSystem.TitlePrefab);
			titleScreen = gameObject.GetComponent<TitleScreen>();
			BurikoMemory.Instance.ResetFlags();
			gameSystem.IsSkipping = false;
			gameSystem.IsForceSkip = false;
			gameSystem.IsAuto = false;
			gameSystem.TextHistory.ClearHistory();
			if (titleScreen == null)
			{
				throw new Exception("Failed to instantiate titleScreen!");
			}
			titleScreen.Enter();
		}

		public void RequestLeave()
		{
			titleScreen.Leave(gameSystem.PopStateStack);
		}

		public void RequestLeaveImmediate()
		{
			UnityEngine.Object.Destroy(titleScreen.gameObject);
			gameSystem.PopStateStack();
		}

		public void OnLeaveState()
		{
		}

		public void OnRestoreState()
		{
		}

		public bool InputHandler()
		{
			return false;
		}

		public GameState GetStateType()
		{
			return GameState.TitleScreen;
		}
	}
}
