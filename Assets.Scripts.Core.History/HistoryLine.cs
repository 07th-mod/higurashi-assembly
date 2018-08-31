using Assets.Scripts.Core.Audio;

namespace Assets.Scripts.Core.History
{
	public class HistoryLine
	{
		public string TextEnglish;

		public string TextJapanese;

		public AudioInfo VoiceFile;

		public HistoryLine(string english, string japanese, AudioInfo voice)
		{
			TextJapanese = japanese;
			TextEnglish = english;
			VoiceFile = voice;
		}
	}
}
