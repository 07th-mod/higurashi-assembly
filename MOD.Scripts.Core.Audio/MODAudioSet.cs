using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko;
using MOD.Scripts.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.Core.Audio
{
	class AudioSet
	{
		public readonly string nameEN;
		public readonly string nameJP;
		public readonly string descriptionEN;
		public readonly string descriptionJP;
		public readonly int altBGM;
		public readonly int altBGMFlow;
		public readonly int altSE;
		public readonly int altSEFlow;
		private bool? isInstalled;

		public AudioSet(string nameEN, string nameJP, string descriptionEN, string descriptionJP, int altBGM, int altBGMFlow, int altSE, int altSEFlow)
		{
			this.nameEN = nameEN;
			this.nameJP = nameJP;
			this.descriptionEN = descriptionEN;
			this.descriptionJP = descriptionJP;
			this.altBGM = altBGM;
			this.altBGMFlow = altBGMFlow;
			this.altSE = altSE;
			this.altSEFlow = altSEFlow;
		}

		private bool JapaneseMode() => BurikoMemory.Instance.GetGlobalFlag("GLanguage").IntValue() == 0;

		public string LocalizedDisplayName() => JapaneseMode() ? nameJP : nameEN + (IsCurrentAndCustom() ? " (custom)" : "");
		public string LocalizedDescription() => JapaneseMode() ? descriptionJP : descriptionEN;

		public bool IsInstalledCached()
		{
			if (isInstalled is bool isInstalledBool)
			{
				return isInstalledBool;
			}

			if(!BGMCascade(out PathCascadeList bgmCascade))
			{
				return false;
			}

			if(!SECascade(out PathCascadeList seCascade))
			{
				return false;
			}

			bool audioSetInstalled = seCascade.IsInstalled(Application.streamingAssetsPath) && bgmCascade.IsInstalled(Application.streamingAssetsPath);
			isInstalled = audioSetInstalled;
			return audioSetInstalled;
		}

		public bool BGMCascade(out PathCascadeList bgmCascade) => MODAudioSet.Instance.GetBGMCascade(altBGM, out bgmCascade);
		public bool SECascade(out PathCascadeList seCascade) => MODAudioSet.Instance.GetSECascade(altSE, out seCascade);

		/// <summary>
		/// Returns true if this Audio Set is selected, and it has been customized by altering one of the audio flags
		/// </summary>
		public bool IsCurrentAndCustom()
		{
			if (MODAudioSet.Instance.GetCurrentAudioSet(out AudioSet set) && set == this)
			{
				return BurikoMemory.Instance.GetGlobalFlag("GAltBGM").IntValue() != set.altBGM ||
					BurikoMemory.Instance.GetGlobalFlag("GAltBGMflow").IntValue() != set.altBGMFlow ||
					BurikoMemory.Instance.GetGlobalFlag("GAltSE").IntValue() != set.altSE ||
					BurikoMemory.Instance.GetGlobalFlag("GAltSEflow").IntValue() != set.altSEFlow;
			}

			return false;
		}
	}

	class MODAudioSet
	{

		private static MODAudioSet _instance;
		public static MODAudioSet Instance => _instance ?? (_instance = new MODAudioSet());

		private readonly List<string> bgmFlowNames = new List<string>()
		{
			"New BGM/SE",
			"Original BGM/SE"
		};

		private readonly PathCascadeList defaultBGMCascade = new PathCascadeList("New BGM", "New BGM", new string[] { "BGM" });
		private readonly PathCascadeList defaultSECascade = new PathCascadeList("New SE", "New SE", new string[] { "SE" });
		private readonly PathCascadeList defaultVoiceCascade = new PathCascadeList("PS3", "PS3", new string[] { "voice" });
		private readonly AudioSet defaultAudioSet = new AudioSet(
			"default/not chosen",
			"default/not chosen",
			"This default audio set should never be used",
			"This default audio set should never be used",
			0,
			0,
			0,
			0);

		public readonly List<PathCascadeList> BGMCascades = new List<PathCascadeList>();
		public readonly List<PathCascadeList> SECascades = new List<PathCascadeList>();
		private readonly List<PathCascadeList> voiceCascades = new List<PathCascadeList>();
		private readonly List<AudioSet> audioSets = new List<AudioSet>();


		public void SetAndSaveAudioFlags(int zeroBasedIndex)
		{
			BurikoMemory.Instance.SetGlobalFlag("GAudioSet", zeroBasedIndex +  1);
			ReloadBGMSE(zeroBasedIndex);
		}

		/// <summary>
		/// Toggles to the next installed audio set (skips audio sets which are not installed)
		/// </summary>
		public void Toggle()
		{
			for(int i = 0; i < audioSets.Count; i++)
			{
				int audioSetZeroBasedIndex = OneToZeroIndexed(
					MODActions.IncrementGlobalFlagWithRollover("GAudioSet", 1, audioSets.Count)
				);

				// Only apply the audio set if it is installed
				if (GetAudioSet(audioSetZeroBasedIndex, out AudioSet audioSet))
				{
					if(audioSet.IsInstalledCached())
					{
						ReloadBGMSE(audioSetZeroBasedIndex);
						return;
					}
				}
			}

			Logger.Log("MODAudioSet: Failed to toggle audio set, as no audio sets were installed");
		}

		public void ReloadBGMSE(int zeroBasedIndex)
		{
			if(zeroBasedIndex < audioSets.Count)
			{
				AudioSet selectedAudioSet = audioSets[zeroBasedIndex];
				BurikoMemory.Instance.SetGlobalFlag("GAltBGM", selectedAudioSet.altBGM);
				BurikoMemory.Instance.SetGlobalFlag("GAltBGMflow", selectedAudioSet.altBGMFlow);
				BurikoMemory.Instance.SetGlobalFlag("GAltSE", selectedAudioSet.altSE);
				BurikoMemory.Instance.SetGlobalFlag("GAltSEflow", selectedAudioSet.altSEFlow);
				MODAudioTracking.Instance.RestoreBGM(selectedAudioSet.altBGMFlow);
			}
		}

		// <summary>
		// Only use this function for displaying to user.Do not use for string comparisons etc.
		// </summary>
		public string GetCurrentAudioSetDisplayName(bool includeAudioSetFlag = false)
		{
			string name;
			if(GetCurrentAudioSet(out AudioSet audioSet, out int audioSetFlag))
			{
				name = audioSet.nameEN + (audioSet.IsCurrentAndCustom() ? "(custom)" : "");
			}
			else
			{
				includeAudioSetFlag = true;
				name = $"Not Chosen/Invalid";
			}

			return name + (includeAudioSetFlag ? $" ({audioSetFlag})" : "");
		}

		public string GetAudioSetNameFromZeroIndexed(int zeroBasedIndex)
		{
			return zeroBasedIndex < audioSets.Count ? audioSets[zeroBasedIndex].nameEN : $"Unknown BGM/SE {zeroBasedIndex}";
		}

		public List<AudioSet> GetAudioSets() => audioSets;
		public bool GetAudioSet(int zeroBasedIndex, out AudioSet audioSet)
		{
			if(zeroBasedIndex < audioSets.Count)
			{
				audioSet = audioSets[zeroBasedIndex];
				return true;
			}

			audioSet = defaultAudioSet;
			return false;
		}

		public bool GetCurrentAudioSet(out AudioSet audioSet) => GetCurrentAudioSet(out audioSet, out _);
		public bool GetCurrentAudioSet(out AudioSet audioSet, out int audioSetFlag)
		{
			audioSetFlag = BurikoMemory.Instance.GetGlobalFlag("GAudioSet").IntValue();

			if(audioSetFlag == 0)
			{
				GetAudioSet(0, out audioSet);
				return false;
			}
			else
			{
				return GetAudioSet(audioSetFlag - 1, out audioSet);
			}
		}

		public string GetBGMFlowName(int altBGMFlow)
		{
			return altBGMFlow < bgmFlowNames.Count ? bgmFlowNames[altBGMFlow] : $"Unknown BGM/SE {altBGMFlow}";
		}

		public void AddBGMSet(PathCascadeList cascade) => BGMCascades.Add(cascade);
		public void AddSESet(PathCascadeList cascade) => SECascades.Add(cascade);
		public void AddAudioSet(AudioSet audioset) => audioSets.Add(audioset);

		public bool GetBGMCascade(int altBGM, out PathCascadeList cascade) => GetCascade(altBGM, BGMCascades, defaultBGMCascade, out cascade);
		public bool GetSECascade(int altSE, out PathCascadeList cascade) => GetCascade(altSE, SECascades, defaultSECascade, out cascade);
		public bool GetVoiceCascade(int altVoice, out PathCascadeList cascade) => GetCascade(altVoice, voiceCascades, defaultVoiceCascade, out cascade);

		private static bool GetCascade(int i, List<PathCascadeList> inputCascades, PathCascadeList defaultCascade, out PathCascadeList cascade)
		{
			if(i < inputCascades.Count)
			{
				cascade = inputCascades[i];
				return true;
			}

			cascade = defaultCascade;
			return false;
		}

		private static int OneToZeroIndexed(int value) => value > 0 ? value - 1 : 0;

		public bool HasAudioSetsDefined() => audioSets.Count > 0;
	}
}
