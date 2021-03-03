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

		public string Name(bool getJapanese) => getJapanese ? nameJP : nameEN;
		public string Description(bool getJapanese) => getJapanese ? descriptionJP : descriptionEN;

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

		private readonly List<PathCascadeList> BGMCascades = new List<PathCascadeList>();
		private readonly List<PathCascadeList> SECascades = new List<PathCascadeList>();
		private readonly List<PathCascadeList> voiceCascades = new List<PathCascadeList>();
		private readonly List<AudioSet> audioSets = new List<AudioSet>();


		public void SetFromZeroBasedIndex(int zeroBasedIndex)
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

		public string GetCurrentAudioSetName(bool includeAudioSetFlag = false)
		{
			int audioSetFlag = BurikoMemory.Instance.GetGlobalFlag("GAudioSet").IntValue();
			string name = audioSetFlag == 0 ? "Not Chosen!" : GetAudioSetNameFromZeroIndexed(OneToZeroIndexed(audioSetFlag));
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

			audioSet = null;
			return false;
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
