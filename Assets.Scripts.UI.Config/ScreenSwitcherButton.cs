using Assets.Scripts.Core;
using MOD.Scripts.Core;
using MOD.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.UI.Config
{
	public class ScreenSwitcherButton : MonoBehaviour
	{
		public int Width;

		public bool IsFullscreen;

		private UIButton button;

		private int? Height()
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
					return null;
			}
		}

		private void OnClick()
		{
			if (IsFullscreen)
			{
				GameSystem.Instance.GoFullscreen();
				return;
			}

			// Check the button's Width is valid. I've seen error messages where the above Height() switch statement states that Width is 0.
			int? maybe_height = Height();
			if(maybe_height == null)
			{
				MODToaster.Show($"Button has invalid width: [{Width}]");
				return;
			}

			int height = maybe_height.Value;
			int width = Mathf.RoundToInt(height * GameSystem.Instance.AspectRatio);

			// Check the windowed resolution would actually fit in the monitor
			if (!GameSystem.Instance.MODWindowedResolutionValid(width, height))
			{
				MODToaster.Show($"Resolution Too Big/Small: [{width}x{height}]");
				return;
			}

			GameSystem.Instance.DeFullscreen(width: width, height: height);
			PlayerPrefs.SetInt("width", width);
			PlayerPrefs.SetInt("height", height);

			// Check if the resolution was set correctly. If not, revert to fullscreen resolution.
			// See MODResolutionMonitor for details
			MODResolutionMonitor.RevertToFullscreenIfResolutionChangeFails(new ResolutionChangeInfo(width, height, delayBeforeResolutionCheck: 10));

			Debug.Log($"Attempted to set Windowed resolution {width}x{height}");
		}

		private bool ShouldBeDown()
		{
			if (GameSystem.Instance.IsFullscreen)
			{
				return IsFullscreen;
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
