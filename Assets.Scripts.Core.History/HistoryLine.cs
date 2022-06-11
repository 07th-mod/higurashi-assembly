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

		/// <summary>
		/// Calculates the height (number of lines) of the text
		/// </summary>
		/// <param name="targetLocation">The location that you will display the text (number of lines is dependent on the width of the place it's going)</param>
		/// <param name="font">The font the text will be rendered with (fonts have different widths which means different line wrappings)</param>
		/// <param name="text">The text that will be attempted to be displayed</param>
		/// <returns>The number of lines the given text will take up if displayed in the given location with the given font</returns>
		private static int CalculateHeight(TextMeshPro targetLocation, TMP_FontAsset font, string text)
		{
			targetLocation.font = font;
			targetLocation.text = text;
			targetLocation.ForceMeshUpdate();
			return targetLocation.textInfo.lineCount;
		}

		/// <summary>
		/// Calculates and saves the height (number of lines) of the text
		/// </summary>
		/// <param name="targetLocation">The location that you will display the text (number of lines is dependent on the width of the place it's going)</param>
		public void CalculateHeight(TextMeshPro targetLocation)
		{
			EnglishHeight = CalculateHeight(targetLocation, GameSystem.Instance.MainUIController.GetEnglishFont(), TextEnglish);
			JapaneseHeight = CalculateHeight(targetLocation, GameSystem.Instance.MainUIController.GetJapaneseFont(), TextJapanese);
		}

		public void AddVoiceFile(List<AudioInfo> voice)
		{
			VoiceFiles.Add(voice);
		}
	}
}
