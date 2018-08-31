using Assets.Scripts.UI.ChapterScreen;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateChapterScreen : IGameState
	{
		private GameSystem gameSystem;

		private ChapterScreen chapterScreen;

		private bool isLeaving;

		public StateChapterScreen()
		{
			gameSystem = GameSystem.Instance;
			GameObject gameObject = Object.Instantiate(gameSystem.ChapterScreenPrefab);
			chapterScreen = gameObject.GetComponent<ChapterScreen>();
			chapterScreen.Show();
		}

		public void RequestLeaveImmediate()
		{
			Object.Destroy(chapterScreen.gameObject);
			gameSystem.PopStateStack();
			isLeaving = true;
		}

		public void RequestLeave()
		{
			if (!isLeaving)
			{
				chapterScreen.Hide(delegate
				{
					gameSystem.PopStateStack();
				});
				isLeaving = true;
			}
		}

		public void OnLeaveState()
		{
			Debug.Log("OnLeave StateChapterScreen " + isLeaving);
			if (!isLeaving)
			{
				chapterScreen.Hide(delegate
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
			return GameState.ChapterScreen;
		}
	}
}
