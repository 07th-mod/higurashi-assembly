using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko;
using MOD.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOD.Scripts.Core.Audio
{
	class MODAudioSet
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
			new PathCascadeList("April Update", "April Update", new string[] { "BGM" }),
			new PathCascadeList("Original", "Original", new string[] { "OGBGM" , "BGM" }),
			new PathCascadeList("Italo", "Italo", new string[] {"ItaloBGM", "OGBGM" , "BGM" }),
		};

		// This is indexed by the altSE flag
		public List<PathCascadeList> SECascades { get; private set; } = new List<PathCascadeList>()
		{
			new PathCascadeList("April Update", "April Update", new string[] { "SE" }),
			new PathCascadeList("Original", "Original", new string[] { "OGSE" , "SE" }),
		};

		public List<PathCascadeList> voiceCascades { get; private set; } = new List<PathCascadeList>()
		{
			new PathCascadeList("PS3", "PS3", new string[] { "voice" }),
		};

		List<AudioSet> audioSetList = new List<AudioSet>
		{
			new AudioSet(
				"New BGM/SE",
				"New BGM/SE",
				0, //altBGM
				0, //altBGMFlow
				0, //altSE
				0  //altSEFlow
			),
			new AudioSet(
				"Original BGM/SE",
				"Original BGM/SE",
				1, //altBGM
				1, //altBGMFlow
				1, //altSE
				1 //altSEFlow
			),
			new AudioSet(
				"Italo BGM Remake",
				"Italo BGM Remake",
				2, //altBGM
				1, //altBGMFlow
				1, //altSE
				1  //altSEFlow
			),
		};

		public void SetFromZeroBasedIndex(int zeroBasedIndex)
		{
			BurikoMemory.Instance.SetGlobalFlag("GAudioSet", zeroBasedIndex +  1);
			ReloadBGMSE(zeroBasedIndex);
		}

		public void Toggle()
		{
			int newAltBGMFlow = MODActions.IncrementGlobalFlagWithRollover("GAudioSet", 1, audioSetList.Count);
			ReloadBGMSE(OneToZeroIndexed(newAltBGMFlow));
		}

		public void ReloadBGMSE(int zeroBasedIndex)
		{
			if(zeroBasedIndex < audioSetList.Count)
			{
				AudioSet selectedAudioSet = audioSetList[zeroBasedIndex];
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
			return zeroBasedIndex < audioSetList.Count ? audioSetList[zeroBasedIndex].nameEN : $"Unknown BGM/SE {zeroBasedIndex}";
		}

		public string GetFlowName(int altBGMFlow)
		{
			return altBGMFlow < bgmFlowNames.Count ? bgmFlowNames[altBGMFlow] : $"Unknown BGM/SE {altBGMFlow}";
		}

		private static int OneToZeroIndexed(int value) => value > 0 ? value - 1 : 0;
	}
}
