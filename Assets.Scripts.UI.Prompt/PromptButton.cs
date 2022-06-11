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
			if (!isEnabled || GameSystem.Instance.GameState != GameState.DialogPrompt || time > 0f || UICamera.currentTouchID < -1)
			{
				return;
			}
			string name = base.name;
			if (!(name == "_Yes"))
			{
				if (name == "_No")
				{
					promptController.Hide(affirmative: false);
				}
				else
				{
					Debug.Log("Button ID not found: " + base.name);
				}
			}
			else
			{
				promptController.Hide(affirmative: true);
			}
			AudioController.Instance.PlaySystemSound("wa_038.ogg", 1);
			isEnabled = false;
		}

		private void OnHover(bool hover)
		{
			_ = isEnabled;
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
