using Assets.Scripts.Core;
using MOD.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.Core
{
	public class ResolutionChangeInfo
	{
		public int targetWidth;
		public int targetHeight;
		public int framesSinceResolutionChange;
		public int delayBeforeResolutionCheck;

		public ResolutionChangeInfo(int targetWidth, int targetHeight, int delayBeforeResolutionCheck)
		{
			this.targetHeight = targetHeight;
			this.targetWidth = targetWidth;
			this.framesSinceResolutionChange = 0;
			this.delayBeforeResolutionCheck = delayBeforeResolutionCheck;
		}
	}

	/// <summary>
	/// This class monitors the game's resolution, to see if it was set correctly, and reverts
	/// to fullscreen if it was not set correctly after some number of frames.
	///
	/// I noticed that on Ubuntu, on Ch9 and Ch10 if you set a windowed resolution greater than your screen's resolution, the
	/// window manager would resize the window back, but this would break something internally. If you then
	/// tried to set the same resolution again, the mouse cursor would no longer align with the screen.
	///
	/// The below code tries to catch this condition, and revert to fullscreen as a 'safe' fallback.
	///
	/// Currently (2024-08-12) the below is only run when you use the main menu config buttons
	/// to set the windowed resolution. The F10 mod menu resolution buttons will forcibly set the resolution.
	/// </summary>
	static class MODResolutionMonitor
	{
		private static ResolutionChangeInfo lastChangeInfo = null;
		public static void RevertToFullscreenIfResolutionChangeFails(ResolutionChangeInfo info) => lastChangeInfo = info;

		public static void Update()
		{
			//This can bug out if you click the resolution change buttons in less than "delayBeforeResolutionCheck"
			if (lastChangeInfo is ResolutionChangeInfo info)
			{
				if (info.framesSinceResolutionChange >= info.delayBeforeResolutionCheck)
				{
					if (Screen.width == info.targetWidth && Screen.height == info.targetHeight)
					{
						MODToaster.Show($"Successfuly set resolution: [{Screen.width}x{Screen.height}]");
					}
					else
					{
						MODToaster.Show($"Failed to set Windowed Resolution properly [currently is {Screen.width}x{Screen.height}] - reverting to fullscreen");
						GameSystem.Instance.GoFullscreen();
					}
					lastChangeInfo = null;
				}

				info.framesSinceResolutionChange++;
			}
		}
	}
}
