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

		public AudioInfo VoiceFile;

		public HistoryLine(string english, string japanese, AudioInfo voice)
		{
			TextJapanese = japanese;
			TextEnglish = english;
			VoiceFile = voice;
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
	}
}
