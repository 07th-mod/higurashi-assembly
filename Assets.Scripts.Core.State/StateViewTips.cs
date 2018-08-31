using Assets.Scripts.Core.Buriko;
using Assets.Scripts.UI.Tips;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateViewTips : IGameState
	{
		private GameSystem gameSystem;

		private TipsManager tipsManager;

		private int mode;

		public StateViewTips(int tipsMode)
		{
			mode = tipsMode;
			gameSystem = GameSystem.Instance;
			GameObject gameObject = Object.Instantiate(gameSystem.TipsPrefab);
			tipsManager = gameObject.GetComponent<TipsManager>();
			GameSystem.Instance.IsForceSkip = false;
			GameSystem.Instance.IsSkipping = false;
			GameSystem.Instance.IsAuto = false;
			tipsManager.Show(tipsMode);
		}

		public void OpenTips(string script)
		{
			BurikoScriptSystem.Instance.CallScript(script);
			gameSystem.AudioController.FadeOutBGM(0, 500, waitForFade: false);
			gameSystem.TextController.ClearText();
			gameSystem.CanSave = false;
			tipsManager.Hide(delegate
			{
				gameSystem.PopStateStack();
			});
		}

		public void RequestLeave()
		{
			tipsManager.Hide(delegate
			{
				gameSystem.PopStateStack();
			});
			gameSystem.AudioController.FadeOutBGM(0, 500, waitForFade: false);
			switch (mode)
			{
			case 0:
			case 1:
				BurikoMemory.Instance.SetFlag("TipsMode", 2);
				BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
				break;
			case 2:
				BurikoMemory.Instance.SetFlag("TipsMode", 1);
				BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
				break;
			}
		}

		public void RequestLeaveImmediate()
		{
			Object.Destroy(tipsManager.gameObject);
		}

		public void OnLeaveState()
		{
			if (tipsManager != null)
			{
				Object.Destroy(tipsManager.gameObject);
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
			return GameState.TipsScreen;
		}
	}
}
