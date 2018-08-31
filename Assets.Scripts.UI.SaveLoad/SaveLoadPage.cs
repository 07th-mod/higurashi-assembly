using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using UnityEngine;

namespace Assets.Scripts.UI.SaveLoad
{
	public class SaveLoadPage : MonoBehaviour
	{
		private UISprite sprite;

		private UIButton button;

		private SaveLoadManager manager;

		private int page;

		private void OnClick()
		{
			if (GameSystem.Instance.GameState == GameState.SaveLoadScreen)
			{
				manager.ChangePage(page);
				AudioController.Instance.PlaySystemSound("wa_038.ogg", 1);
			}
		}

		public void Enable()
		{
			button.isEnabled = true;
			sprite.spriteName = button.normalSprite;
		}

		public void Disable()
		{
			button.isEnabled = false;
			sprite.spriteName = button.disabledSprite;
		}

		public void Setup(int number, SaveLoadManager saveLoadManager)
		{
			manager = saveLoadManager;
			page = number - 1;
			button = GetComponent<UIButton>();
			sprite = GetComponent<UISprite>();
			string str = page.ToString("D1");
			button.normalSprite = "page" + str + "-normal";
			button.hoverSprite = "page" + str + "-hover";
			button.pressedSprite = "page" + str + "-hover";
			button.disabledSprite = "page" + str + "-down";
			if (page == 0)
			{
				button.isEnabled = false;
				sprite.spriteName = button.disabledSprite;
			}
			else
			{
				sprite.spriteName = button.normalSprite;
			}
		}
	}
}
