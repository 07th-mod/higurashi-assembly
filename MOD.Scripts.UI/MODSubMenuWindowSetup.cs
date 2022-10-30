using MOD.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;

namespace MOD.Scripts.UI
{
	class MODSubMenuWindowSetup : MODMenuModuleInterface
	{
		MODMenu modMenu;
		MODMenuNormal normalMenu;
		MODSimpleTimer setWindowAgainDelay;

		public MODSubMenuWindowSetup(MODMenu modMenu, MODMenuNormal normalMenu)
		{
			this.modMenu = modMenu;
			this.normalMenu = normalMenu;
			this.setWindowAgainDelay = new MODSimpleTimer();
		}

		public void OnBeforeMenuVisible()
		{

		}

		public void Update()
		{
			setWindowAgainDelay.Update();
		}

		public void OnGUI()
		{
			// Fix a bug where windowed resolution becomes 640x480 when set the first time game starts (if playerprefs doesn't exist).
			// Resolution is set correctly if set again after a short delay.
			// Bug was first noticed on Ep1 Native Ubuntu 22.04.
			if(setWindowAgainDelay.Finished())
			{
				MODWindowManager.SetResolution(maybe_width: null, maybe_height: null, maybe_fullscreen: false, showToast: true);
				setWindowAgainDelay.Cancel();
			}

			HeadingLabel("Linux Resolution/Windowed Mode Setup");

			GUILayout.Space(20);

			Label("Some Native Linux users experience crashes or softlocks when entering windowed mode, or when moving the window around (the 'Gnome Crash Bug').\n\n" +
				"Please click the button below to enter windowed mode, then drag the window around.");

			GUILayout.Space(20);

			Label("NOTE: This may crash your entire desktop environment! Please save your work before this test!");

			GUILayout.Space(20);

			if (
				Button(
					new GUIContent(
						MODWindowManager.IsFullscreen ? "Click here to test Windowed Mode (may flicker)" : "Now try dragging the window around!",
						"This button will switch the game to Windowed mode.\n\n" +
						"Please try moving the window around to see if the game crashes\n\n" +
						"If the game freezes, you may need to force close it and open the game again."
					),
					selected: !MODWindowManager.IsFullscreen
				)
			)
			{
				MODWindowManager.SetResolution(maybe_width: null, maybe_height: null, maybe_fullscreen: false, showToast: true);
				setWindowAgainDelay.Start(1);
			}

			GUILayout.Space(20);

			Label("If your game crashed in windowed mode, choose 'Force Fullscreen Always' to avoid future crashes.");

			GUILayout.Space(20);

			Label("If your game runs fine even when the window is dragged, choose 'No Lock' to run the game normally.");

			GUILayout.Space(20);

			normalMenu.OnGUIFullscreenLock();

			GUILayout.Space(20);

			if (MODWindowManager.FullscreenLockConfigured())
			{
				if(Button(new GUIContent("Click here when you're finished.")))
				{
					modMenu.PopSubMenu();
				}
			}
			else
			{
				Label("You must choose an option to continue.");
			}
		}

		public bool UserCanClose() => false;
		public string Heading()
		{
			if (setWindowAgainDelay.Running())
			{
				return $"Please Wait ({setWindowAgainDelay.timeLeft:F1})";
			}
			else
			{
				return "Linux Resolution/Windowed Mode Setup Menu";
			}
		}

        public string DefaultTooltip() => "Please choose the options on the left before continuing. You can hover over a button to view its description.";
	}
}
