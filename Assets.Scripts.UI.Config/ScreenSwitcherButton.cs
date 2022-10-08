using Assets.Scripts.Core;
using MOD.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.UI.Config
{
	public class ScreenSwitcherButton : MonoBehaviour
	{
		public int Width;

		// This variable doesn't actually change/doesn't really relate to the game's fullscreen state
		// It is actually used to indicate which button is labelled as "fullscreen" (it is true only for that button)

		public bool IsFullscreen;

		private UIButton button;

		private int Height()
		{
			// Old resolutions (which the assetbundle button widths are based on) were 640x480, 800x600, and 1024x768
			// New resolutions are 1280x720, 1920x1080, and 2560x1440
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
			if(IsFullscreen)
			{
				MODWindowManager.FullscreenToggle(showToast: true);
			}
			else
			{
				MODWindowManager.SetResolution(Height(), showToast: true);
			}
		}

		private bool ShouldBeDown()
		{
			// Make the 'fullscreen' button always clickable because it toggles fullscreen
			if (IsFullscreen)
			{
				return false;
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
