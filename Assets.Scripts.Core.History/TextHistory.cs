using Assets.Scripts.Core.Audio;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

namespace Assets.Scripts.Core.History
{
	public class TextHistory
	{
		private const int MaxEntries = 100;

		private static readonly Regex sizeTagRegex = new Regex("<\\/?size(?:=[+-]?\\d+)?>");

		private List<HistoryLine> lines = new List<HistoryLine>();

		private AudioInfo lastVoice;

		private HistoryLine last;

		private TextMeshPro textMeasurer;

		public int EnglishLineCount { get; private set; } = 0;

		public int JapaneseLineCount { get; private set; } = 0;

		public int LineCount => lines.Count;

		public TextHistory() {
			TextMeshPro prefabTMP = GameSystem.Instance.HistoryPrefab.GetComponent<HistoryWindow>().Labels[0];
			GameObject tmpObject = Object.Instantiate(prefabTMP.gameObject);
			tmpObject.SetActive(false);
			textMeasurer = tmpObject.GetComponent<TextMeshPro>();
		}


		public void ClearHistory()
		{
			lines = new List<HistoryLine>();
			lastVoice = null;
			last = null;
			EnglishLineCount = 0;
			JapaneseLineCount = 0;
		}

		private string FormatName(string name)
		{
			if (name == "")
			{
				return name;
			}
			else
			{
				return sizeTagRegex.Replace(string.Format(GameSystem.Instance.TextController.NameFormat, name), "");
			}
		}

		public void RegisterLine(string english, string japanese, string nameen, string namejp)
		{
			english = sizeTagRegex.Replace(english, "");
			japanese = sizeTagRegex.Replace(japanese, "");
			if (english.StartsWith("\n"))
			{
				english = english.Replace("\n", string.Empty);
				japanese = japanese.Replace("\n", string.Empty);
				PushHistory();
				if (english == string.Empty && japanese == string.Empty)
				{
					return;
				}
			}
			if (last != null)
			{
				last.TextEnglish += english;
				last.TextJapanese += japanese;
			}
			else
			{
				string english2 = FormatName(nameen) + english;
				string japanese2 = FormatName(namejp) + japanese;
				last = new HistoryLine(english2, japanese2);
			}
			if (lastVoice != null)
			{
				last.AddVoiceFile(lastVoice);
				lastVoice = null;
			}
		}

		public void PushHistory()
		{
			if (last != null)
			{
				last.TextEnglish += "\n ";
				last.TextJapanese += "\n ";
				last.CalculateHeight(textMeasurer);
				EnglishLineCount += last.EnglishHeight;
				JapaneseLineCount += last.JapaneseHeight;
				lines.Add(last);
			}
			if (lines.Count > 100)
			{
				EnglishLineCount -= lines[0].EnglishHeight;
				JapaneseLineCount -= lines[0].JapaneseHeight;
				lines.RemoveAt(0);
			}
			last = null;
		}

		public HistoryLine GetLine(int id)
		{
			if (id < 0 || lines.Count <= id)
			{
				return null;
			}
			return lines[id];
		}

		public void RegisterVoice(AudioInfo voice)
		{
			lastVoice = voice;
		}
	}
}
