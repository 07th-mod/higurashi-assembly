using MOD.Scripts.Core.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	class MODMenuAudioOptions
	{
		private readonly MODRadio radioBGMSESet;
		private readonly MODMenuCommon c;

		public MODMenuAudioOptions(MODMenuCommon c, MODStyleManager styleManager)
		{
			this.c = c;

			this.radioBGMSESet = new MODRadio("Choose BGM/SE (Hotkey: 2)", new GUIContent[] { }, styleManager);
		}

		public void OnBeforeMenuVisible()
		{
			if (this.radioBGMSESet.GetContents().Length == 0)
			{
				bool japanese = c.GetGlobal("GLanguage") == 0;
				this.radioBGMSESet.SetContents(
					MODAudioSet.Instance.GetAudioSets().Select(x => new GUIContent(x.Name(japanese), x.Description(japanese))).ToArray()
				);
			}
		}

		public void OnGUI()
		{
			if (MODAudioSet.Instance.HasAudioSetsDefined())
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
