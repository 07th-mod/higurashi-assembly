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
			}
			else
			{
				switch (Width)
				{
				case 640:
				{
					int num2 = Mathf.RoundToInt(480f * GameSystem.Instance.AspectRatio);
					GameSystem.Instance.DeFullscreen(width: num2, height: 480);
					PlayerPrefs.SetInt("width", num2);
					PlayerPrefs.SetInt("height", 480);
					break;
				}
				case 800:
				{
					int num3 = Mathf.RoundToInt(600f * GameSystem.Instance.AspectRatio);
					GameSystem.Instance.DeFullscreen(width: num3, height: 600);
					PlayerPrefs.SetInt("width", num3);
					PlayerPrefs.SetInt("height", 600);
					break;
				}
				case 1024:
				{
					int num = Mathf.RoundToInt(768f * GameSystem.Instance.AspectRatio);
					GameSystem.Instance.DeFullscreen(width: num, height: 768);
					PlayerPrefs.SetInt("width", num);
					PlayerPrefs.SetInt("height", 768);
					break;
				}
				}
			}
		}

		private bool ShouldBeDown()
		{
			if (IsFullscreen)
			{
				return GameSystem.Instance.IsFullscreen;
			}
			return (3 * Width / 4) == Screen.height;
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
