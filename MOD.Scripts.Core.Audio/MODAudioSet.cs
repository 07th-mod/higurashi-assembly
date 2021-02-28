using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko;
using MOD.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOD.Scripts.Core.Audio
{
	class AudioSet
	{
		public readonly string nameEN;
		public readonly string nameJP;
		public readonly int altBGM;
		public readonly int altBGMFlow;
		public readonly int altSE;
		public readonly int altSEFlow;

		public AudioSet(string nameEN, string nameJP, int altBGM, int altBGMFlow, int altSE, int altSEFlow)
		{
			this.nameEN = nameEN;
			this.nameJP = nameJP;
			this.altBGM = altBGM;
			this.altBGMFlow = altBGMFlow;
			this.altSE = altSE;
			this.altSEFlow = altSEFlow;
		}
	}

	class MODAudioSet
	{

		private static MODAudioSet _instance;
		public static MODAudioSet Instance => _instance ?? (_instance = new MODAudioSet());

		public List<string> bgmFlowNames = new List<string>()
		{
			"New BGM/SE",
			"Original BGM/SE"
		};

		// This is indexed by the altBGM flag
		public List<PathCascadeList> BGMCascades { get; private set; } = new List<PathCascadeList>()
		{
		};

		// This is indexed by the altSE flag
		public List<PathCascadeList> SECascades { get; private set; } = new List<PathCascadeList>()
		{
		};

		public List<PathCascadeList> voiceCascades { get; private set; } = new List<PathCascadeList>()
		{
			new PathCascadeList("PS3", "PS3", new string[] { "voice" }),
		};

		List<AudioSet> audioSets = new List<AudioSet>
		{
		};

		public void SetFromZeroBasedIndex(int zeroBasedIndex)
		{
			BurikoMemory.Instance.SetGlobalFlag("GAudioSet", zeroBasedIndex +  1);
			ReloadBGMSE(zeroBasedIndex);
		}

		public void Toggle()
		{
			int newAltBGMFlow = MODActions.IncrementGlobalFlagWithRollover("GAudioSet", 1, audioSets.Count);
			ReloadBGMSE(OneToZeroIndexed(newAltBGMFlow));
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
			return GetAudioSetNameFromZeroIndexed(OneToZeroIndexed(audioSetFlag)) + (includeAudioSetFlag ? $" ({audioSetFlag})" : "");
		}

		public string GetAudioSetNameFromZeroIndexed(int zeroBasedIndex)
		{
			return zeroBasedIndex < audioSets.Count ? audioSets[zeroBasedIndex].nameEN : $"Unknown BGM/SE {zeroBasedIndex}";
		}

		public string GetFlowName(int altBGMFlow)
		{
			return altBGMFlow < bgmFlowNames.Count ? bgmFlowNames[altBGMFlow] : $"Unknown BGM/SE {altBGMFlow}";
		}

		public void AddBGMSet(PathCascadeList cascade) => BGMCascades.Add(cascade);
		public void AddSESet(PathCascadeList cascade) => SECascades.Add(cascade);
		public void AddAudioSet(AudioSet audioset) => audioSets.Add(audioset);

		private static int OneToZeroIndexed(int value) => value > 0 ? value - 1 : 0;
	}
}
