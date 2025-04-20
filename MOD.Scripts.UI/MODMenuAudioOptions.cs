using MOD.Scripts.Core.Audio;
using MOD.Scripts.Core.Localization;
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

			this.radioBGMSESet = new MODRadio(Loc.MODMenuAudioOptions_0, new GUIContent[] { }, itemsPerRow: 2, asButtons: true); //Audio Presets (Hotkey: 2)
			this.radioSE = new MODRadio(Loc.MODMenuAudioOptions_1, new GUIContent[] { },  itemsPerRow: 2); //Override SE
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
					string bgmPrimaryInfo = Loc.MODMenuAudioOptions_2; //Invalid BGM cascade
					if (audioSet.BGMCascade(out var bgmCascade) && bgmCascade.PrimaryFolder(out string bgmPrimary))
					{
						bgmPrimaryInfo = bgmPrimary;
					}

					string sePrimaryInfo = Loc.MODMenuAudioOptions_3; //Invalid SE cascade
					if (audioSet.SECascade(out var seCascade) && seCascade.PrimaryFolder(out string sePrimary))
					{
						sePrimaryInfo = sePrimary;
					}

					buttonText += Loc.MODMenuAudioOptions_4; //(NOT INSTALLED)
					tooltipText += Loc.MODMenuAudioOptions_5 + $" '{bgmPrimaryInfo}'/'{sePrimaryInfo}'"; //\n\nWARNING: This audio set is not installed! You can try to run the installer again to update your mod with this option.\nYou're missing the BGM or SE folder in the StreamingAssets folder:
				}

				buttonContents.Add(new GUIContent(buttonText, tooltipText));
			}

			this.radioBGMSESet.SetContents(buttonContents.ToArray());

			this.radioSE.SetContents(
				MODAudioSet.Instance.SECascades.Select(
					c => new GUIContent(c.nameEN, Loc.MODMenuAudioOptions_6 + $"'{c.nameEN}'") //Force enable the following sound effects (ignore preset SE):
				).ToArray()
			);
		}

		public void OnBeforeMenuVisible()
		{
			ReloadMenu();
		}

		public void OnGUI(bool setupMenu=false)
		{
			bool hideLabel = setupMenu;

			Label(Loc.MODMenuAudioOptions_7); //The patch supports different Background Music (BGM) and Sound Effects(SE). Please click the button below for more information.

			// Add extra spacing on setup menu so it looks nicer
			if(setupMenu)
			{
				GUILayout.Space(20);
			}

			if (Button(new GUIContent(Loc.MODMenuAudioOptions_8, Loc.MODMenuAudioOptions_9))) //Open BGM/SE FAQ: 07th-mod.com/wiki/Higurashi/BGM-SE-FAQ/ | Click here to open the  Background Music (BGM) and Sound Effects (SE) FAQ in your browser.\n\nThe BGM/SE FAQ contains information on the settings below.
			{
				Application.OpenURL(Loc.MODMenuAudioOptions_10); //https://07th-mod.com/wiki/Higurashi/BGM-SE-FAQ/
			}

			GUILayout.Space(20);

			if (setupMenu)
			{
				Label(Loc.MODMenuAudioOptions_11); //To continue, please choose a BGM/SE option below (hover button for info).\nYou can change this option later via the mod menu.
			}

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
							MODToaster.Show(Loc.MODMenuAudioOptions_12, maybeSound: null); //Option Not Installed!
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
