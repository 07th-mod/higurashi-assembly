using Assets.Scripts.UI.Extra;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateExtraScreen : IGameState
	{
		private GameSystem gameSystem;

		private ExtraManager extraScreen;

		private bool isLeaving;

		public StateExtraScreen()
		{
			gameSystem = GameSystem.Instance;
			GameObject gameObject = Object.Instantiate(gameSystem.ExtraScreenPrefab);
			extraScreen = gameObject.GetComponent<ExtraManager>();
			GameSystem.Instance.IsForceSkip = false;
			GameSystem.Instance.IsSkipping = false;
			GameSystem.Instance.IsAuto = false;
			extraScreen.Show();
		}

		public void RequestLeaveImmediate()
		{
			Object.Destroy(extraScreen.gameObject);
			gameSystem.PopStateStack();
			isLeaving = true;
		}

		public void RequestLeave()
		{
			if (!isLeaving)
			{
				extraScreen.Hide(delegate
				{
					gameSystem.PopStateStack();
				});
				isLeaving = true;
			}
		}

		public void OnLeaveState()
		{
			Debug.Log("OnLeave StateExtraScreen " + isLeaving);
			if (!isLeaving)
			{
				extraScreen.Hide(delegate
				{
				});
			}
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
			return GameState.ExtraScreen;
		}
	}
}
