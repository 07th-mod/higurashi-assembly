using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using Assets.Scripts.UI.Prompt;
using UnityEngine;

namespace Assets.Scripts.UI.TitleScreen
{
	public class TitleScreenButton : MonoBehaviour
	{
		private float time = 2.25f;

		private UIButton button;

		private bool isReady;

		private bool isHover;

		public bool IsEnabled = true;

		public bool IsLeaving;

		private string hoverSprite;

		private string pressedSprite;

		private void OnClick()
		{
			GameSystem gameSystem = GameSystem.Instance;

			if(gameSystem.MODIgnoreInputs) { return; }

			if (gameSystem.GameState == GameState.TitleScreen && !(time > 0f) && UICamera.currentTouchID == -1)
			{
				StateTitle stateTitle = gameSystem.GetStateObject() as StateTitle;
				if (stateTitle != null)
				{
					switch (base.name)
					{
					case "0-Start":
						BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 0);
						stateTitle.RequestLeave();
						gameSystem.AudioController.StopAllAudio();
						gameSystem.AudioController.PlaySystemSound("wa_040.ogg");
						IsEnabled = false;
						break;
					case "1-Load":
						gameSystem.PushStateObject(new StateSaveLoad(restoreUI: false));
						break;
					case "2-Config":
						gameSystem.SwitchToConfig(0, showMessageWindow: false);
						break;
					case "3-CGGallery":
						BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
						BurikoMemory.Instance.SetFlag("TipsMode", 1);
						stateTitle.RequestLeave();
						gameSystem.AudioController.FadeOutBGM(2, 1000, waitForFade: false);
						BurikoScriptSystem.Instance.CallBlock("ViewTipsDisplay");
						gameSystem.SceneController.DrawScene("black", 0.5f);
						gameSystem.ExecuteActions();
						break;
					case "4-Exit":
						GameSystem.Instance.PushStateObject(new StateDialogPrompt(PromptType.DialogExit, delegate
						{
							gameSystem.CanExit = true;
							Application.Quit();
						}, null));
						break;
					default:
						Debug.Log("Button ID not found: " + base.name);
						break;
					}
					if (base.name != "0-Start")
					{
						AudioController.Instance.PlaySystemSound("wa_038.ogg", 1);
					}
					IsEnabled = false;
				}
			}
		}

		private void OnHover(bool hover)
		{
			isHover = hover;
			if (GameSystem.Instance.GameState == GameState.TitleScreen)
			{
			}
		}

		private void Update()
		{
			time -= Time.deltaTime;
		}

		private void Awake()
		{
			button = GetComponent<UIButton>();
			hoverSprite = button.hoverSprite;
			pressedSprite = button.pressedSprite;
			button.hoverSprite = button.normalSprite;
			button.pressedSprite = button.normalSprite;
			button.enabled = false;
		}

		private void LateUpdate()
		{
			button.isEnabled = (!GameSystem.Instance.MODIgnoreInputs && GameSystem.Instance.GameState == GameState.TitleScreen && !IsLeaving);
			if (!isReady && time < 0f)
			{
				if (!button.enabled)
				{
					button.enabled = true;
				}
				button.hoverSprite = hoverSprite;
				button.pressedSprite = pressedSprite;
				isReady = true;
				if (isHover)
				{
					UISprite component = GetComponent<UISprite>();
					component.spriteName = button.hoverSprite;
				}
			}
		}
	}
}
