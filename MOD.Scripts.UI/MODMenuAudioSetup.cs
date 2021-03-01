using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	class MODMenuAudioSetup : MODMenuModuleInterface
	{
		MODMenu modMenu;
		MODMenuCommon c;
		MODMenuAudioOptions audioOptions;

		public MODMenuAudioSetup(MODMenu modMenu, MODMenuCommon c, MODMenuAudioOptions audioOptions)
		{
			this.modMenu = modMenu;
			this.c = c;
			this.audioOptions = audioOptions;
		}

		public void OnBeforeMenuVisible()	{}

		public void OnGUI()
		{
			c.Label("The patch supports different BGM/SE types, they can vary what you will hear and when. Choose the one that feels most appropriate for your experience.");

			audioOptions.OnGUI();

			GUILayout.Space(20);

			if (c.GetGlobal("GAudioSet") != 0 && c.Button(new GUIContent("Click here when you're finished.")))
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
