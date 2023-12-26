using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using UnityEngine;

namespace Assets.Scripts.UI.Prompt
{
	public class PromptButton : MonoBehaviour
	{
		private PromptController promptController;

		private float time = 0.45f;

		public bool isEnabled = true;

		public void ChangeButtonImages(string normal, string hover, string down)
		{
			UIButton component = GetComponent<UIButton>();
			component.normalSprite = normal;
			component.hoverSprite = hover;
			component.pressedSprite = down;
		}

		private void OnClick()
		{
			if (isEnabled && GameSystem.Instance.GameState == GameState.DialogPrompt && !(time > 0f) && UICamera.currentTouchID >= -1)
			{
				switch (base.name)
				{
				case "_Yes":
					promptController.Hide(affirmative: true);
					break;
				case "_No":
					promptController.Hide(affirmative: false);
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
			if (isEnabled)
			{
			}
		}

		private void Update()
		{
			time -= Time.deltaTime;
		}

		public void RegisterController(PromptController controller)
		{
			promptController = controller;
		}
	}
}
