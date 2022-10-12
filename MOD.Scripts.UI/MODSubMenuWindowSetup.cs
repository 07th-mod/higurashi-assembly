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

		public MODSubMenuWindowSetup(MODMenu modMenu, MODMenuNormal normalMenu)
		{
			this.modMenu = modMenu;
			this.normalMenu = normalMenu;
		}

		public void OnBeforeMenuVisible()
		{

		}

		public void OnGUI()
		{
			HeadingLabel("Linux Resolution/Windowed Mode Setup");

			GUILayout.Space(20);

			Label("Some Native Linux users experience crashes or softlocks when entering windowed mode, or when moving the window around (the 'Gnome Crash Bug').\n\n" +
				"Please click the button below to enter windowed mode, then drag the window around.");

			GUILayout.Space(20);

			if(Button(
				new GUIContent(
					"Click here to test Windowed Mode",
					"This button will switch the game to Windowed mode.\n\n" +
					"Please try moving the window around to see if the game crashes\n\n" +
					"If the game freezes, you may need to force close it and open the game again."
				)))
			{
				MODWindowManager.SetResolution(maybe_width: null, maybe_height: null, maybe_fullscreen: false, showToast: true);
			}

			GUILayout.Space(20);

			Label("If your game crashed in windowed mode, choose 'Force Fullscreen Always'. Otherwise choose 'No Lock'");

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
		public string Heading() => "Linux Resolution/Windowed Mode Setup Menu";
		public string DefaultTooltip() => "Please choose the options on the left before continuing. You can hover over a button to view its description.";
	}
}
