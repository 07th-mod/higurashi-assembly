using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.UI.Config
{
	public class ScreenSwitcherButton : MonoBehaviour
	{
		public int Width;

		public bool IsFullscreen;

		private UIButton button;

		private void OnClick()
		{
			if (IsFullscreen)
			{
				GameSystem.Instance.GoFullscreen();
				return;
			}
			switch (Width)
			{
			case 640:
			{
				if (Application.platform == RuntimePlatform.OSXPlayer)
				{
					Screen.fullScreen = false;
				}
				int num2 = Mathf.RoundToInt(480f * GameSystem.Instance.AspectRatio);
				Screen.SetResolution(num2, 480, fullscreen: false);
				PlayerPrefs.SetInt("width", num2);
				PlayerPrefs.SetInt("height", 480);
				break;
			}
			case 800:
			{
				if (Application.platform == RuntimePlatform.OSXPlayer)
				{
					Screen.fullScreen = false;
				}
				int num3 = Mathf.RoundToInt(600f * GameSystem.Instance.AspectRatio);
				Screen.SetResolution(num3, 600, fullscreen: false);
				PlayerPrefs.SetInt("width", num3);
				PlayerPrefs.SetInt("height", 600);
				break;
			}
			case 1024:
			{
				if (Application.platform == RuntimePlatform.OSXPlayer)
				{
					Screen.fullScreen = false;
				}
				int num = Mathf.RoundToInt(768f * GameSystem.Instance.AspectRatio);
				Screen.SetResolution(num, 768, fullscreen: false);
				PlayerPrefs.SetInt("width", num);
				PlayerPrefs.SetInt("height", 768);
				break;
			}
			}
		}

		private bool ShouldBeDown()
		{
			if (IsFullscreen)
			{
				return Screen.fullScreen;
			}
			return Width == Screen.width;
		}

		private void Start()
		{
			button = GetComponent<UIButton>();
		}

		private void FixedUpdate()
		{
			button.isEnabled = !ShouldBeDown();
		}
	}
}
