using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using Assets.Scripts.UI.Prompt;
using UnityEngine;

namespace Assets.Scripts.UI.Menu
{
	internal class MenuUIButton : MonoBehaviour
	{
		private float time = 0.45f;

		public bool isEnabled = true;

		private GameSystem gameSystem;

		private UIButton button;

		private void OnClick()
		{
			if (gameSystem == null)
			{
				gameSystem = GameSystem.Instance;
			}
			if(gameSystem.MODIgnoreInputs)
			{
				return;
			}
			if (gameSystem.GameState == GameState.RightClickMenu && !(time > 0f) && UICamera.currentTouchID == -1 && isEnabled)
			{
				switch (base.name)
				{
				case "0-SaveLoad":
					gameSystem.LeaveMenu(delegate
					{
						gameSystem.PushStateObject(new StateSaveLoad(restoreUI: true));
					}, doPop: true);
					break;
				case "1-Config":
					gameSystem.LeaveMenu(delegate
					{
						gameSystem.SwitchToConfig(0, showMessageWindow: true);
					}, doPop: true);
					break;
				case "2-HideWindow":
					gameSystem.SwitchToHiddenWindow();
					break;
				case "3-MainMenu":
					gameSystem.LeaveMenu(null, doPop: false);
					gameSystem.PopStateStack();
					GameSystem.Instance.PushStateObject(new StateDialogPrompt(PromptType.DialogTitle, delegate
					{
						gameSystem.ClearActions();
						gameSystem.ClearAllWaits();
						gameSystem.TextController.ClearText();
						gameSystem.MainUIController.FadeOut(0f, isBlocking: false);
						gameSystem.SceneController.HideFace(0f);
						gameSystem.SceneController.HideAllLayers(0f);
						gameSystem.SceneController.HideFilmEffector(0f, isBlocking: false);
						gameSystem.MainUIController.HideMessageBox();
						gameSystem.ExecuteActions();
						BurikoScriptSystem.Instance.JumpToScript("flow");
						BurikoScriptSystem.Instance.JumpToBlock("Title");
						gameSystem.AudioController.StopAllAudio();
						BurikoMemory.Instance.ResetScope();
					}, delegate
					{
						gameSystem.RevealMessageBox();
					}));
					break;
				case "4-QuitGame":
					gameSystem.LeaveMenu(null, doPop: false);
					gameSystem.PopStateStack();
					GameSystem.Instance.PushStateObject(new StateDialogPrompt(PromptType.DialogExit, delegate
					{
						gameSystem.CanExit = true;
						Application.Quit();
					}, delegate
					{
						gameSystem.RevealMessageBox();
					}));
					break;
				default:
					Debug.Log("Button ID not found: " + base.name);
					break;
				}
				AudioController.Instance.PlaySystemSound("wa_038.ogg", 1);
				isEnabled = false;
			}
		}

		private void OnHover(bool hover)
		{
		}

		private void Update()
		{
			time -= Time.deltaTime;
		}

		private void Awake()
		{
			button = GetComponent<UIButton>();
		}

		private void LateUpdate()
		{
			button.isEnabled = !GameSystem.Instance.MODIgnoreInputs;
		}
	}
}
