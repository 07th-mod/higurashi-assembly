using Assets.Scripts.UI.Prompt;
using System;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateDialogPrompt : IGameState
	{
		private GameSystem gameSystem;

		private PromptController promptController;

		private bool isActive = true;

		public StateDialogPrompt(PromptType type, DialogCallback onYes, DialogCallback onNo)
		{
			gameSystem = GameSystem.Instance;
			if (!gameSystem.UsePrompts && type != 0)
			{
				Debug.Log("Immediately completing dialog, dialogs disabled in config.");
				gameSystem.RegisterAction(delegate
				{
					if (onYes != null)
					{
						onYes();
					}
					gameSystem.PopStateStack();
				});
				gameSystem.ExecuteActions();
			}
			else
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(gameSystem.PromptPrefab);
				promptController = gameObject.GetComponent<PromptController>();
				if (promptController == null)
				{
					throw new Exception("Failed to instantiate promptController!");
				}
				promptController.Open(type, onYes, onNo);
			}
		}

		public PromptController GetPromptController()
		{
			return promptController;
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
		}

		public bool InputHandler()
		{
			if (!isActive)
			{
				return false;
			}
			if (Input.GetMouseButtonDown(1))
			{
				promptController.Hide(affirmative: false);
			}
			return false;
		}

		public GameState GetStateType()
		{
			return GameState.DialogPrompt;
		}

		public void DisableInputActions()
		{
			isActive = false;
		}
	}
}
