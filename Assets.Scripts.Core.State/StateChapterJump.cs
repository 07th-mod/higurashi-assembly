using Assets.Scripts.UI.ChapterJump;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateChapterJump : IGameState
	{
		private GameSystem gameSystem;

		private ChapterJumpManager jumpScreen;

		private bool isLeaving;

		public StateChapterJump()
		{
			gameSystem = GameSystem.Instance;
			GameObject gameObject = Object.Instantiate(gameSystem.ChapterJumpPrefab);
			jumpScreen = gameObject.GetComponent<ChapterJumpManager>();
			jumpScreen.Show();
		}

		public void RequestLeaveImmediate()
		{
			Object.Destroy(jumpScreen.gameObject);
			gameSystem.PopStateStack();
			isLeaving = true;
		}

		public void RequestLeave()
		{
			if (!isLeaving)
			{
				jumpScreen.Hide(delegate
				{
					gameSystem.PopStateStack();
				});
				isLeaving = true;
			}
		}

		public void OnLeaveState()
		{
			Debug.Log("OnLeave StateExtraScreen " + isLeaving.ToString());
			if (!isLeaving)
			{
				jumpScreen.Hide(delegate
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
			return GameState.ChapterJumpScreen;
		}
	}
}
