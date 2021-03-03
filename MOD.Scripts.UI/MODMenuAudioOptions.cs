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

			this.radioBGMSESet = new MODRadio("Choose BGM/SE (Hotkey: 2)", new GUIContent[] { }, styleManager, itemsPerRow: 2);
		}

		public void OnBeforeMenuVisible()
		{
			if (this.radioBGMSESet.GetContents().Length == 0)
			{
				bool japanese = c.GetGlobal("GLanguage") == 0;

				List<GUIContent> buttonContents = new List<GUIContent>();
				foreach(AudioSet audioSet in MODAudioSet.Instance.GetAudioSets())
				{
					string buttonText = audioSet.Name(japanese);
					string tooltipText = audioSet.Description(japanese);

					// Append message to button text/tooltip if audioSet is not installed
					if(!audioSet.IsInstalledCached())
					{
						string bgmPrimaryInfo = "Invalid BGM cascade";
						if(audioSet.BGMCascade(out var bgmCascade) && bgmCascade.PrimaryFolder(out string bgmPrimary))
						{
							bgmPrimaryInfo = bgmPrimary;
						}

						string sePrimaryInfo = "Invalid SE cascade";
						if (audioSet.SECascade(out var seCascade) && seCascade.PrimaryFolder(out string sePrimary))
						{
							sePrimaryInfo = sePrimary;
						}

						buttonText += " (NOT INSTALLED)";
						tooltipText += $"\n\nWARNING: This audio set is not installed! You can try to run the installer again to update your mod with this option.\n\n" +
							$"\n\nDetailed Info: you're either missing the BGM folder '{bgmPrimaryInfo}' or the SE folder '{sePrimaryInfo}' in the StreamingAssets folder.";
					}

					buttonContents.Add(new GUIContent(buttonText, tooltipText));
				}

				this.radioBGMSESet.SetContents(buttonContents.ToArray());
			}
		}

		public void OnGUI()
		{
			if (MODAudioSet.Instance.HasAudioSetsDefined())
			{
				// Set GAltBGM, GAltSE, GAltBGMFlow, GAltSEFlow to the same value. In the future we may set them to different values.
				if (this.radioBGMSESet.OnGUIFragment(c.GetGlobal("GAudioSet") > 0 ? c.GetGlobal("GAudioSet") - 1 : 0) is int newAudioSetZeroBased)
				{
					if(MODAudioSet.Instance.GetAudioSet(newAudioSetZeroBased, out AudioSet audioSet) && audioSet.IsInstalledCached())
					{
						MODAudioSet.Instance.SetAndSaveAudioFlags(newAudioSetZeroBased);
					}
				}
			}
		}
	}
}
