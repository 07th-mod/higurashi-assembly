using MOD.Scripts.Core.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	class MODMenuAudioOptions
	{
		private readonly MODRadio radioBGMSESet;
		private readonly MODMenuCommon c;
		private readonly bool hasBGMSEOptions;

		public MODMenuAudioOptions(MODMenuCommon c, MODStyleManager styleManager)
		{
			this.c = c;
			hasBGMSEOptions = MODActions.HasMusicToggle();

			this.radioBGMSESet = new MODRadio("Choose BGM/SE (Hotkey: 2)", new GUIContent[]
			{
				new GUIContent("New BGM/SE", "Use the new BGM/SE introduced by MangaGamer in the April 2019 update."),
				new GUIContent("Original BGM/SE", "Use the original BGM/SE from the Japanese version of the game. This option was previously known as 'BGM/SE fix'.\n\n" +
				"Note that this not only changes which audio files are played, but also when BGM starts to play/stops playing, in certain cases."),
			}, styleManager);
		}

		public void OnGUI()
		{
			if (this.hasBGMSEOptions)
			{
				// Set GAltBGM, GAltSE, GAltBGMFlow, GAltSEFlow to the same value. In the future we may set them to different values.
				if (this.radioBGMSESet.OnGUIFragment(c.GetGlobal("GAudioSet") > 0 ? c.GetGlobal("GAudioSet") - 1 : 0) is int newBGMSEValue)
				{
					MODAudioSet.Instance.SetFromZeroBasedIndex(newBGMSEValue);
				}
			}
		}
	}
}
