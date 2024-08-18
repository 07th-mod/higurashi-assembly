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

		private static bool useHoverSpriteOnlyForTranslator = false;
		private static float hoverOpacityForTranslators = 0.5f;

		private void OnClick()
		{
			GameSystem gameSystem = GameSystem.Instance;

			if(gameSystem.MODIgnoreInputs()) { return; }

			if (gameSystem.GameState != GameState.TitleScreen || time > 0f || UICamera.currentTouchID < -1)
			{
				return;
			}
			StateTitle stateTitle = gameSystem.GetStateObject() as StateTitle;
			if (stateTitle != null)
			{
				switch (base.name)
				{
				case "0-Start":
					BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
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
					BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 0);
					BurikoMemory.Instance.SetFlag("TipsMode", 1);
					stateTitle.RequestLeave();
					gameSystem.AudioController.FadeOutBGM(2, 1000, waitForFade: false);
					BurikoScriptSystem.Instance.CallBlock("OmakeLoop");
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
				case "TitleArea":
					BurikoScriptSystem.Instance.CallBlock("TitleClick");
					gameSystem.PopStateStack();
					return;
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

		private void OnHover(bool hover)
		{
			isHover = hover;
			_ = GameSystem.Instance.GameState;
			_ = 7;
		}

		private void Update()
		{
			time -= Time.deltaTime;
		}

		private void Awake()
		{
			button = GetComponent<UIButton>();
			hoverSprite = button.hoverSprite;

			if (useHoverSpriteOnlyForTranslator)
			{
				pressedSprite = hoverSprite;
				button.hoverSprite = hoverSprite;
				button.pressedSprite = hoverSprite;
				button.disabledSprite = hoverSprite;
				GetComponent<UISprite>().spriteName = hoverSprite;
				button.hover.a = hoverOpacityForTranslators;
			}

			pressedSprite = button.pressedSprite;
			button.hoverSprite = button.normalSprite;
			button.pressedSprite = button.normalSprite;
			button.enabled = false;
		}

		private void LateUpdate()
		{
			button.isEnabled = (!GameSystem.Instance.MODIgnoreInputs() && GameSystem.Instance.GameState == GameState.TitleScreen && !IsLeaving);
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
					GetComponent<UISprite>().spriteName = button.hoverSprite;
				}
			}
		}

		// MUST BE CALLED BEFORE BUTTONS APPEAR. Has no effect if buttons already on screen.
		//
		// On hou+, in the UI file, there are two images for the menu buttons.
		// One is very wide (button hovered), and one only is as long as the english text
		// (button not hovered). Translators may be unable to fit the text in the short
		// button, so I've added a mode which only uses the long button, and uses the engine
		// to give a fade effect when you hover the button.
		//
		// Then translators can use the wide button to fit in more text.
		public static void UseHoverSpriteOnlyForTranslators(bool useHoverSprite, float hoverOpacity)
		{
			useHoverSpriteOnlyForTranslator = useHoverSprite;
			hoverOpacityForTranslators = hoverOpacity;
		}
	}
}
