using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.UI.Config
{
	public class ScreenSwitcherButton : MonoBehaviour
	{
		public int Width;

		public bool IsFullscreen;

		private UIButton button;

		private int Height()
		{
			switch (Width)
			{
				case 640:
					return 720;
				case 800:
					return 1080;
				case 1024:
					return 1440;
				default:
					Debug.LogWarning("Found unexpected width button " + Width);
					return Mathf.RoundToInt(Width / GameSystem.Instance.AspectRatio);
			}
		}

		private void OnClick()
		{
			if (IsFullscreen)
			{
				GameSystem.Instance.GoFullscreen();
			}
			else
			{
				int height = Height();
				int width = Mathf.RoundToInt(height * GameSystem.Instance.AspectRatio);
				GameSystem.Instance.DeFullscreen(width: width, height: height);
				PlayerPrefs.SetInt("width", width);
				PlayerPrefs.SetInt("height", height);
			}
		}

		private bool ShouldBeDown()
		{
			if (IsFullscreen)
			{
				return GameSystem.Instance.IsFullscreen;
			}
			return Screen.height == Height();
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
