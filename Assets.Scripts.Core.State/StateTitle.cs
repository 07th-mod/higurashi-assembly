using Assets.Scripts.Core.Buriko;
using Assets.Scripts.UI.TitleScreen;
using MOD.Scripts.UI;
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

			// If you were playing in ADV mode, encountered a ModEnableNVLModeInADVMode() call, then quit to the menu, the
			// "GLinemodeSp" (controlling if the text is drawn in NVL or ADV mode) would still be set to NVL mode.
			//
			// If you then started a new game, this would result in NVL mode text overlaid on the ADV mode textbox.
			//
			// The below call resets GLinemodeSp to 0 if in ADV mode.
			// 'redraw' must be set to false, otherwise the textbox will appear over the title screen.
			//
			// Also, on older chapters which do not have a "BurikoMemory.Instance.ResetFlags()" call above,
			// the "NVL_in_ADV" non-global flag would still be set to true. The following function also resets that flag.
			MODActions.DisableNVLModeINADVMode(redraw: false);

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
