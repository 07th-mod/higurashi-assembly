using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;

namespace MOD.Scripts.UI
{
	class MODMenuAudioSetup : MODMenuModuleInterface
	{
		MODMenu modMenu;
		MODMenuAudioOptions audioOptions;

		public MODMenuAudioSetup(MODMenu modMenu, MODMenuAudioOptions audioOptions)
		{
			this.modMenu = modMenu;
			this.audioOptions = audioOptions;
		}

		public void OnBeforeMenuVisible()
		{
			audioOptions.OnBeforeMenuVisible();
		}

		public void OnGUI()
		{
			Label("The patch supports different BGM/SE types, they can vary what you will hear and when. Choose the one that feels most appropriate for your experience.\n\n" +
				"You can change this at any time via the mod menu.");

			audioOptions.OnGUI(hideLabel: true);

			GUILayout.Space(20);

			if (GetGlobal("GAudioSet") != 0 && Button(new GUIContent("Click here when you're finished.")))
			{
				modMenu.SetMode(ModMenuMode.Normal);
				modMenu.ForceHide();
			}
		}

		public bool UserCanClose() => false;
		public string Heading() => "First-Time Setup Menu";
		public string DefaultTooltip() => "Please choose the options on the left before continuing. You can hover over a button to view its description.";
	}
}
