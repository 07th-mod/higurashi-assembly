using Assets.Scripts.UI.Extra;
using System;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateOmakeSection : IGameState
	{
		private GameSystem gameSystem;

		private ExtraManager extraScreen;

		private bool isLeaving;

		private bool clearBgOnLeave;

		public StateOmakeSection()
		{
			gameSystem = GameSystem.Instance;
			GameObject gameObject;
			switch (gameSystem.ScriptSystem.GetFlag("OmakeState"))
			{
			case 1:
				gameObject = UnityEngine.Object.Instantiate(gameSystem.ScenarioLockSectionPrefab);
				clearBgOnLeave = true;
				break;
			case 2:
				gameObject = UnityEngine.Object.Instantiate(gameSystem.StaffRoomSectionPrefab);
				clearBgOnLeave = true;
				break;
			default:
				throw new Exception("Invalid omake section state!");
			}
			extraScreen = gameObject.GetComponent<ExtraManager>();
			GameSystem.Instance.IsForceSkip = false;
			GameSystem.Instance.IsSkipping = false;
			GameSystem.Instance.IsAuto = false;
			extraScreen.Show();
		}

		public void RequestLeaveImmediate()
		{
			UnityEngine.Object.Destroy(extraScreen.gameObject);
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
				if (clearBgOnLeave)
				{
					gameSystem.SceneController.DrawScene("black", 0.8f);
				}
				gameSystem.ExecuteActions();
				isLeaving = true;
			}
		}

		public void OnLeaveState()
		{
			Debug.Log("OnLeave StateExtraScreen " + isLeaving.ToString());
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
			return GameState.OmakeSection;
		}
	}
}
