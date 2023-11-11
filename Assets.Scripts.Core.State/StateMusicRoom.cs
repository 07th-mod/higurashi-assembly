using Assets.Scripts.UI.Extra;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateMusicRoom : IGameState
	{
		private GameSystem gameSystem;

		private MusicRoomManager musicRoom;

		private bool isLeaving;

		public StateMusicRoom()
		{
			gameSystem = GameSystem.Instance;
			GameObject gameObject = Object.Instantiate(gameSystem.MusicRoomPrefab);
			musicRoom = gameObject.GetComponent<MusicRoomManager>();
			GameSystem.Instance.IsForceSkip = false;
			GameSystem.Instance.IsSkipping = false;
			GameSystem.Instance.IsAuto = false;
			musicRoom.Show();
		}

		public void RequestLeaveImmediate()
		{
			Object.Destroy(musicRoom.gameObject);
			gameSystem.PopStateStack();
			isLeaving = true;
		}

		public void RequestLeave()
		{
			if (!isLeaving)
			{
				musicRoom.Hide(delegate
				{
					gameSystem.PopStateStack();
				});
				isLeaving = true;
			}
		}

		public void OnLeaveState()
		{
			Debug.Log(string.Format("OnLeave {0} {1}", "StateMusicRoom", isLeaving));
			if (!isLeaving)
			{
				musicRoom.Hide(delegate
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
			return GameState.MusicRoom;
		}
	}
}
