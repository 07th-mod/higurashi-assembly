using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using System.Linq;
using Assets.Scripts.Core.Audio;
using System.Text;

namespace Assets.Scripts.Core.History
{
	public class TextHistory
	{
		private const int MaxEntries = 100;

		private static readonly Regex sizeTagRegex = new Regex("<\\/?size(?:=[+-]?\\d+)?>");
		private static readonly Regex uncoloredNameRegex = new Regex("</color>([^<]+)<color");

		private List<HistoryLine> lines = new List<HistoryLine>();

		private List<AudioInfo> lastVoice = new List<AudioInfo>();

		private HistoryLine last;

		private static string lastEN = "<No line outputted yet>";
		public static string GetLastEN() => lastEN;

		private TextMeshPro textMeasurer;

		private TMP_FontAsset fontJapanese;

		private TMP_FontAsset fontEnglish;

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
			lastVoice = new List<AudioInfo>();
			last = null;
			EnglishLineCount = 0;
			JapaneseLineCount = 0;
		}

		private string FormatName(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
			{
				return "";
			}
			else
			{
				// Format the name according to the NameHistoryFormat defined in init.txt. For Hou+ we use "{0}\n"
				string formattedName = string.Format(GameSystem.Instance.TextController.NameHistoryFormat, name);

				// Sometimes, there is a name like "Keiichi & Rena", where Keiichi is one color, '&' is white, and 'Rena' is another color.
				// When you mouseover the line to play it, the "&" will turn blue, when it should stay white. To workaround this, the following makes any uncolored text
				// explicitly white so it stays white during hover.
				string explicitlyWhite = uncoloredNameRegex.Replace(formattedName, (match) => "</color><color=#ffffff>" + match.Groups[1].Value + "</color><color");
				return explicitlyWhite;
			}
		}

		public void RegisterLine(string english, string japanese, string nameen, string namejp)
		{
			lastEN = english ?? "<Null english line>";

			// Remove any size tags as this will mess up the history window formatting
			english = sizeTagRegex.Replace(english, "");
			japanese = sizeTagRegex.Replace(japanese, "");

			//Push history immediately if english line starts with newline?
			//I guess this makes sure you don't end up with one huge paragraph which plays voices sequentially
			if (english.StartsWith("\n"))
			{
				english = english.Replace("\n", "");
				japanese = japanese.Replace("\n", "");

				PushHistory();
				if (english == string.Empty && japanese == string.Empty)
				{
					return;
				}
			}

			// If there is an existing History Line, continue to collect text into that history line, but don't add it to the backlog yet
			if (last != null)
			{
				last.TextEnglish += english;
				last.TextJapanese += japanese;
			}
			else
			{
				// If reached this point, then last == null, eg we are starting a new History Line in the backlog
				// We need to add the name at the start of the backlog, which is done below, then create a new HistoryLine
				string english2 = FormatName(nameen) + english;
				string japanese2 = FormatName(namejp) + japanese;
				last = new HistoryLine(english2, japanese2);
			}

			// If any voices were recorded, append them to the History Line
			// This records a list of voices, instead of a single voice, in case there were multiple voices
			// since the last time RegisterLine() was called, or multiple voices played at same time (?).
			if (lastVoice.Count > 0)
			{
				last.AddVoiceFile(lastVoice);
				lastVoice = new List<AudioInfo>();
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
			lastVoice.Add(voice);
		}

		/// <summary>
		/// The latest voices that have played, if any. If none, returns an empty list; never returns null.
		/// </summary>
		public List<List<AudioInfo>> LatestVoice
		{
			get
			{
				// Do we currently have any voices registered that are not attached to a history line?
				// This probably shouldn't happen, because conventionally voice commands precede OutputLine commands and they all execute quickly in sequence.
				// Once the OutputLine hits, the voices in lastVoice get thrown into the "last" field's VoiceFiles and "lastVoice" is reinitialized
				// to an empty list.  However, it seems more internally consistent/correct to keep this case.
				if (lastVoice.Any())
					return new List<List<AudioInfo>> { lastVoice };
				// The the last voice has nothing, as expected.  But does the latest dialogue on screen have any voices associated?
				if (last != null && last.VoiceFiles.Any())
					return last.VoiceFiles;
				// No voice associated with the current text on screen (narration / character POV monologue etc.), so we'll look for the last line in the history with voices.
				var candidate = lines.LastOrDefault(x => x.VoiceFiles.Any());
				// But there might not be one yet as there may have only been narration so far, so we check for that too.
				return candidate == null ? new List<List<AudioInfo>>() : candidate.VoiceFiles;
			}
		}
	}
}
