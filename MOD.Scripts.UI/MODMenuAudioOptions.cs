using MOD.Scripts.Core.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;

namespace MOD.Scripts.UI
{
	class MODMenuAudioOptions
	{
		private readonly MODRadio radioBGMSESet;
		private readonly MODRadio radioSE;
		private readonly MODMenu modMenu;
		public MODMenuAudioOptions(MODMenu m)
		{
			this.modMenu = m;

			this.radioBGMSESet = new MODRadio("Audio Presets (Hotkey: 2)", new GUIContent[] { }, itemsPerRow: 2, asButtons: true);
			this.radioSE = new MODRadio("Override SE", new GUIContent[] { },  itemsPerRow: 2);
		}

		public void ReloadMenu()
		{
			bool japanese = GetGlobal("GLanguage") == 0;

			List<GUIContent> buttonContents = new List<GUIContent>();
			foreach (AudioSet audioSet in MODAudioSet.Instance.GetAudioSets())
			{
				string buttonText = audioSet.LocalizedDisplayName();
				string tooltipText = audioSet.LocalizedDescription();

				// Append message to button text/tooltip if audioSet is not installed
				if (!audioSet.IsInstalledCached())
				{
					string bgmPrimaryInfo = "Invalid BGM cascade";
					if (audioSet.BGMCascade(out var bgmCascade) && bgmCascade.PrimaryFolder(out string bgmPrimary))
					{
						bgmPrimaryInfo = bgmPrimary;
					}

					string sePrimaryInfo = "Invalid SE cascade";
					if (audioSet.SECascade(out var seCascade) && seCascade.PrimaryFolder(out string sePrimary))
					{
						sePrimaryInfo = sePrimary;
					}

					buttonText += " (NOT INSTALLED)";
					tooltipText += $"\n\nWARNING: This audio set is not installed! You can try to run the installer again to update your mod with this option.\n" +
						$"You're either missing the BGM folder '{bgmPrimaryInfo}' or the SE folder '{sePrimaryInfo}' in the StreamingAssets folder.";
				}

				buttonContents.Add(new GUIContent(buttonText, tooltipText));
			}

			this.radioBGMSESet.SetContents(buttonContents.ToArray());

			this.radioSE.SetContents(
				MODAudioSet.Instance.SECascades.Select(
					c => new GUIContent(c.nameEN, $"This allows you to use the '{c.nameEN}' sound effects, regardless of what the audio preset would use.")
				).ToArray()
			);
		}

		public void OnBeforeMenuVisible()
		{
			ReloadMenu();
		}

		public void OnGUI(bool hideLabel=false)
		{
			if (MODAudioSet.Instance.HasAudioSetsDefined())
			{
				// Set GAltBGM, GAltSE, GAltBGMFlow, GAltSEFlow to the same value. In the future we may set them to different values.
				if (this.radioBGMSESet.OnGUIFragment(GetGlobal("GAudioSet") > 0 ? GetGlobal("GAudioSet") - 1 : -1, hideLabel: hideLabel) is int newAudioSetZeroBased)
				{
					if(MODAudioSet.Instance.GetAudioSet(newAudioSetZeroBased, out AudioSet audioSet))
					{
						if (audioSet.IsInstalledCached())
						{
							MODAudioSet.Instance.SetAndSaveAudioFlags(newAudioSetZeroBased);
							ReloadMenu();
						}
						else
						{
							MODToaster.Show("Option Not Installed!", maybeSound: null);
							this.modMenu.OverrideClickSound(GUISound.Disable);
						}
					}
				}
			}

		}

		public void AdvancedOnGUI()
		{
			if (MODAudioSet.Instance.SECascades.Count > 0)
			{
				if (this.radioSE.OnGUIFragment(GetGlobal("GAltSE")) is int newAltSE)
				{
					SetGlobal("GAltSE", newAltSE);
					ReloadMenu();
				}
			}
		}
	}
}
