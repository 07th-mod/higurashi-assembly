using Assets.Scripts.Core.Audio;
using System.Collections.Generic;

namespace Assets.Scripts.Core.History
{
	public class TextHistory
	{
		private const int MaxEntries = 100;

		private List<HistoryLine> lines = new List<HistoryLine>();

		private AudioInfo lastVoice;

		private HistoryLine last;

		public int LineCount => lines.Count;

		public void ClearHistory()
		{
			lines = new List<HistoryLine>();
		}

		public void RegisterLine(string english, string japanese, string nameen, string namejp)
		{
			if (english.StartsWith("\n"))
			{
				english = english.Replace("\n", string.Empty);
				japanese = japanese.Replace("\n", string.Empty);
				PushHistory();
				if (english == string.Empty || japanese == string.Empty)
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
				string english2 = string.Format(GameSystem.Instance.TextController.NameFormat, nameen) + english;
				string japanese2 = string.Format(GameSystem.Instance.TextController.NameFormat, namejp) + japanese;
				last = new HistoryLine(english2, japanese2, null);
			}
		}

		public void PushHistory()
		{
			if (last != null)
			{
				lines.Add(last);
			}
			if (lines.Count > 100)
			{
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
	}
}
