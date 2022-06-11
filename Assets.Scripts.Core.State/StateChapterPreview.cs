using Assets.Scripts.UI.ChapterPreview;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateChapterPreview : IGameState
	{
		private GameSystem gameSystem;

		private ChapterPreviewManager previewScreen;

		private bool isLeaving;

		public StateChapterPreview()
		{
			gameSystem = GameSystem.Instance;
			GameObject gameObject = Object.Instantiate(gameSystem.ChapterPreviewPrefab);
			previewScreen = gameObject.GetComponent<ChapterPreviewManager>();
			previewScreen.Show();
		}

		public void RequestLeaveImmediate()
		{
			Object.Destroy(previewScreen.gameObject);
			gameSystem.PopStateStack();
			isLeaving = true;
		}

		public void RequestLeave()
		{
			if (!isLeaving)
			{
				previewScreen.Hide(delegate
				{
					gameSystem.PopStateStack();
				});
				isLeaving = true;
			}
		}

		public void OnLeaveState()
		{
			Debug.Log("OnLeave StateChapterPreview " + isLeaving.ToString());
			if (!isLeaving)
			{
				previewScreen.Hide(delegate
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
			return GameState.ChapterPreview;
		}
	}
}
