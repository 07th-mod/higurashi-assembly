using System.Collections.Generic;
using Assets.Scripts.Core.Audio;
using TMPro;

namespace Assets.Scripts.Core.History
{
	public class HistoryLine
	{
		public string TextEnglish;

		public string TextJapanese;

		public int EnglishHeight { get; private set; }

		public int JapaneseHeight { get; private set; }

		public List<List<AudioInfo>> VoiceFiles;

		public HistoryLine(string english, string japanese)
		{
			TextJapanese = japanese;
			TextEnglish = english;
			VoiceFiles = new List<List<AudioInfo>>();
		}

		private int CalculateHeight(TextMeshPro measurer, TextMeshProFont font, string text)
		{
			measurer.font = font;
			measurer.text = text;
			measurer.ForceMeshUpdate();
			return measurer.textInfo.lineCount;
		}

		public void CalculateHeight(TextMeshPro measurer)
		{
			EnglishHeight = CalculateHeight(measurer, GameSystem.Instance.MainUIController.GetEnglishFont(), TextEnglish);
			JapaneseHeight = CalculateHeight(measurer, GameSystem.Instance.MainUIController.GetJapaneseFont(), TextJapanese);
		}

		public void AddVoiceFile(List<AudioInfo> voice)
		{
			VoiceFiles.Add(voice);
		}
	}
}
