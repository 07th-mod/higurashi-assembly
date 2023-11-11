using System.Collections.Generic;
using TMPro;

namespace Assets.Scripts.Core.History
{
	public class TextHistory
	{
		private List<HistoryLine> lines = new List<HistoryLine>();

		private HistoryLine last;

		private TextMeshPro historyTextMesh;

		private TMP_FontAsset fontJapanese;

		private TMP_FontAsset fontEnglish;

		private const int MaxEntries = 100;

		public int LineCount => lines.Count;

		public void ClearHistory()
		{
			lines = new List<HistoryLine>();
		}

		public void RegisterLine(string english, string japanese, string nameen, string namejp)
		{
			if (english.StartsWith("\n"))
			{
				english = english.Replace("\n", "");
				japanese = japanese.Replace("\n", "");
				PushHistory();
				if (english == "" || japanese == "")
				{
					return;
				}
			}
			if (last != null)
			{
				last.TextEnglish += english;
				last.TextJapanese += japanese;
				return;
			}
			string english2 = english;
			string japanese2 = japanese;
			if (!string.IsNullOrWhiteSpace(nameen))
			{
				english2 = string.Format(GameSystem.Instance.TextController.NameHistoryFormat, nameen) + english;
			}
			if (!string.IsNullOrEmpty(namejp))
			{
				japanese2 = string.Format(GameSystem.Instance.TextController.NameHistoryFormat, namejp) + japanese;
			}
			last = new HistoryLine(english2, japanese2, null);
		}

		public void PushHistory()
		{
			if (historyTextMesh == null)
			{
				historyTextMesh = GameSystem.Instance.HistoryTextMesh;
				fontEnglish = GameSystem.Instance.MainUIController.GetEnglishFont();
				fontJapanese = GameSystem.Instance.MainUIController.GetJapaneseFont();
			}
			while (last != null)
			{
				int num = -1;
				int num2 = -1;
				historyTextMesh.font = fontJapanese;
				historyTextMesh.text = last.TextJapanese;
				historyTextMesh.ForceMeshUpdate();
				TMP_TextInfo textInfo = historyTextMesh.GetTextInfo(last.TextJapanese);
				if (textInfo.lineCount > 4)
				{
					num = textInfo.lineInfo[3].lastCharacterIndex + 1;
				}
				historyTextMesh.font = fontEnglish;
				historyTextMesh.text = last.TextEnglish;
				textInfo = historyTextMesh.GetTextInfo(last.TextEnglish);
				if (textInfo.lineCount > 4)
				{
					num2 = textInfo.lineInfo[3].lastCharacterIndex + 1;
				}
				if (num == -1 && num2 == -1)
				{
					lines.Add(last);
					last = null;
					continue;
				}
				string japanese = "";
				string english = "";
				if (num > 0)
				{
					japanese = last.TextJapanese.Substring(num);
					last.TextJapanese = last.TextJapanese.Substring(0, num);
				}
				if (num2 > 0)
				{
					english = last.TextEnglish.Substring(num2).Trim();
					last.TextEnglish = last.TextEnglish.Substring(0, num2);
				}
				lines.Add(last);
				last = new HistoryLine(english, japanese, null);
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
