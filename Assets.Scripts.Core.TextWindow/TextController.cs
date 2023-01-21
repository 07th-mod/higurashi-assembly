using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using MOD.Scripts.Core.TextWindow;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Core.TextWindow
{
	public class TextController
	{
		public readonly TextMeshPro TextArea;

		private GameSystem gameSystem;

		private bool appendNext;

		private bool prevAppendState;

		private bool isFading;

		private float textTimeRemaining;

		public int TextSpeed = 50;

		public int AutoSpeed = 50;

		public int AutoPageSpeed = 50;

		public int OverrideTextSpeed = -1;

		private float timePerChar = 0.025f;

		private float timeForFade = 0.15f;

		private float timePerLine = 1f;

		private string txt = "";

		private string englishtext = "";

		private string japanesetext = "";

		private string englishprev = "";

		private string japaneseprev = "";

		private string NameFormat = "";

		public static Color32 TextColor = Color.white;

		private BurikoTextModes lastMode;

		private List<TextCharacter> charList = new List<TextCharacter>();

		// This is only necessary for Higurashi Rei - on earlier chapters, nametags would automatically escape
		// newlines, even on the unmodded game, but on Rei we need to do it ourselves (might be due to changes
		// in TextMeshPro).
		public void SetNameFormat(string rawNameFormat) => NameFormat = rawNameFormat.Replace("\\n", "\n");
		public string GetNameFormat() => NameFormat;

		public bool IsTyping()
		{
			return isFading;
		}

		public void SetTextFade(bool value)
		{
			if (value)
			{
				timeForFade = 0.15f;
			}
			else
			{
				timeForFade = 0f;
			}
		}

		public void SwapLanguages()
		{
			if (GameSystem.Instance.UseEnglishText)
			{
				japanesetext = txt;
				txt = englishtext;
			}
			else
			{
				englishtext = txt;
				txt = japanesetext;
			}
			string text = japanesetext;
			string text2 = englishtext;
			bool flag = appendNext;
			TypeTextImmediately(txt);
			FinishTyping();
			UpdateTextArea();
			appendNext = flag;
			japanesetext = text;
			englishtext = text2;
		}

		public Vector3 GetCarretPosition()
		{
			if (TextArea.text == "")
			{
				return Vector3.zero;
			}
			TMP_LineInfo tMP_LineInfo = TextArea.textInfo.lineInfo[TextArea.textInfo.lineCount - 1];
			Vector3 localPosition = TextArea.gameObject.transform.localPosition;
			float x = localPosition.x + Mathf.RoundToInt(20f + TextArea.textInfo.lineInfo[TextArea.textInfo.lineCount - 1].lineExtents.max.x);
			Vector3 localPosition2 = TextArea.gameObject.transform.localPosition;
			float y = localPosition2.y + TextArea.textInfo.characterInfo[tMP_LineInfo.lastCharacterIndex].baseLine + 12f;
			return new Vector3(x, y, 0f);
		}

		public bool GetPrevAppendState()
		{
			return prevAppendState;
		}

		public bool GetAppendState()
		{
			return appendNext;
		}

		public void SetAppendState(bool append)
		{
			appendNext = append;
			prevAppendState = append;
		}

		public void TypeTextImmediatelyNoClear(string text, BurikoTextModes textMode)
		{
			if (appendNext)
			{
				txt = text;
				if (GameSystem.Instance.UseEnglishText)
				{
					englishprev = text;
				}
				else
				{
					japaneseprev = text;
				}
			}
			else
			{
				txt = text;
				if (GameSystem.Instance.UseEnglishText)
				{
					englishprev = text;
				}
				else
				{
					japaneseprev = text;
				}
			}
			string text2 = txt;
			foreach (char character in text2)
			{
				charList.Add(new TextCharacter(character, 0f, 0f));
			}
		}

		public void TypeTextImmediately(string text)
		{
			ClearText();
			txt = text;
			if (GameSystem.Instance.UseEnglishText)
			{
				englishprev = text;
			}
			else
			{
				japaneseprev = text;
			}
			string text2 = txt;
			foreach (char character in text2)
			{
				charList.Add(new TextCharacter(character, 0f, 0f));
			}
		}

		public void SetPrevText(string text, int language)
		{
			if (language == 1)
			{
				englishprev = text;
			}
			else
			{
				japaneseprev = text;
			}
		}

		public string GetPrevText(int language)
		{
			if (language == 1)
			{
				return englishprev;
			}
			return japaneseprev;
		}

		public void SetFullText(string text, int language)
		{
			if (language == 1)
			{
				if (GameSystem.Instance.UseEnglishText)
				{
					txt = text;
				}
				englishtext = text;
			}
			else
			{
				if (!GameSystem.Instance.UseEnglishText)
				{
					txt = text;
				}
				japanesetext = text;
			}
		}

		public string GetFullText(int language)
		{
			if (language == 1)
			{
				return englishtext;
			}
			return japanesetext;
		}

		public void FinishedTyping()
		{
			textTimeRemaining = 0f;
		}

		public void FinishTyping()
		{
			textTimeRemaining = 0f;
		}

		public void AddWaitToFinishTyping()
		{
			if (isFading)
			{
				gameSystem.AddWait(new Wait(textTimeRemaining, WaitTypes.WaitForText, FinishedTyping));
			}
		}

		public void ClearText()
		{
			TextArea.text = "";
			appendNext = false;
			charList.Clear();
			isFading = false;
			englishprev = englishtext;
			japaneseprev = japanesetext;
			englishtext = "";
			japanesetext = "";
			txt = "";
			gameSystem.MainUIController.HideCarret();
			AudioController.Instance.ClearVoiceQueue();
		}

		public void UpdateTextArea()
		{
			string text = "";
			if (textTimeRemaining <= 0f)
			{
				if (isFading)
				{
					foreach (TextCharacter @char in charList)
					{
						@char.Finish();
					}
				}
				isFading = false;
			}
			foreach (TextCharacter char2 in charList)
			{
				text = ((!isFading) ? (text + char2.GetCharacter().ToString()) : (text + char2.GetCharacter(Time.deltaTime)));
			}
			if (textTimeRemaining > 0f)
			{
				textTimeRemaining -= Time.deltaTime;
			}
			TextArea.text = text;
			TextColor = BurikoMemory.Instance.GetFlag("LTextColor").IntValue().ToColor32();
			TextArea.color = TextColor;
		}

		public void SetTextPoint(int x, int y)
		{
			string text = "";
			int num = 0;
			if (!appendNext)
			{
				ClearText();
			}
			else
			{
				num = TextArea.textInfo.lineCount;
			}
			for (int i = num; i < y; i++)
			{
				text += "\n";
			}
			for (int j = num; j < x; j++)
			{
				text += "\u3000";
			}
			englishtext += text;
			japanesetext += text;
			txt += text;
			string text2 = text;
			foreach (char character in text2)
			{
				charList.Add(new TextCharacter(character, 0f, 0f));
			}
			appendNext = true;
			FinishTyping();
		}

		private void AddText(string str, int displayimmediate, bool isFade, bool addToTime)
		{
			float num = (float)(100 - TextSpeed) / 100f * 2f;
			if (gameSystem.IsAuto)
			{
				num = (float)(100 - AutoSpeed) / 100f * 2f;
			}
			if (OverrideTextSpeed != -1)
			{
				num = (float)(100 - OverrideTextSpeed) / 100f * 2f;
			}
			int num2 = 1;
			if (!GameSystem.Instance.UseEnglishText)
			{
				num2 = 2;
			}
			bool flag = false;
			bool flag2 = false;
			foreach (char c in str)
			{
				if (c == '<')
				{
					flag = true;
				}
				if (c == '\\')
				{
					flag2 = true;
				}
				if (!isFade || flag || flag2)
				{
					charList.Add(new TextCharacter(c, 0f, 0f));
					if (c == '>')
					{
						flag = false;
					}
					if (c != '\\')
					{
						flag2 = false;
					}
				}
				else if (displayimmediate > 0)
				{
					charList.Add(new TextCharacter(c, 0f, 0f));
					displayimmediate--;
				}
				else
				{
					charList.Add(new TextCharacter(c, textTimeRemaining, textTimeRemaining + timeForFade));
					if (addToTime)
					{
						textTimeRemaining += timePerChar * num * (float)num2;
					}
				}
			}
		}

		private void CreateText(string name, string text, BurikoTextModes textMode, bool noUpdate = false)
		{
			if (textMode == BurikoTextModes.Continue)
			{
				MODTextController.MODLineContinueDetect = true;
			}
			bool mODLineContinueDetect = MODTextController.MODLineContinueDetect;
			int mODCurrentVoiceLayerDetect = MODTextController.MODCurrentVoiceLayerDetect;
			isFading = (!gameSystem.IsSkipping && !noUpdate);
			string text2 = string.Format(NameFormat, name);
			if (appendNext)
			{
				int length = txt.Length;
				txt += text;
				string text3 = txt;
				string text4 = text3.Substring(length);
				if (noUpdate)
				{
					AddText(text4, text4.Length, isFade: false, addToTime: false);
				}
				else
				{
					AddText(text4, 0, isFading, addToTime: true);
				}
				txt = text3;
			}
			else
			{
				charList.Clear();
				string text3 = text;
				int length2 = text2.Length;
				AddText(text3, length2, isFading, addToTime: true);
				txt = text3;
			}
			if (isFading)
			{
				switch (textMode)
				{
				case BurikoTextModes.Normal:
				case BurikoTextModes.WaitForInput:
				case BurikoTextModes.ContinueAftertyping:
				case BurikoTextModes.WaitThenContinue:
					gameSystem.AddWait(new Wait(textTimeRemaining, WaitTypes.WaitForText, FinishedTyping));
					break;
				}
			}
			else if (!noUpdate)
			{
				UpdateTextArea();
			}
			bool flag = false;
			switch (textMode)
			{
			case BurikoTextModes.Normal:
				gameSystem.AddWait(new Wait(0f, WaitTypes.WaitForInput, ClearText));
				flag = true;
				break;
			case BurikoTextModes.WaitForInput:
				gameSystem.AddWait(new Wait(0f, WaitTypes.WaitForInput, null));
				flag = true;
				break;
			}
			lastMode = textMode;
			if (gameSystem.IsAuto && flag)
			{
				SetAutoTextWait();
			}
			if (gameSystem.IsAuto && mODLineContinueDetect && GameSystem.Instance.AudioController.IsVoicePlaying(mODCurrentVoiceLayerDetect))
			{
				switch (textMode)
				{
				case BurikoTextModes.Continue:
					break;
				case BurikoTextModes.Normal:
				case BurikoTextModes.WaitForInput:
				case BurikoTextModes.ContinueAftertyping:
				case BurikoTextModes.WaitThenContinue:
					GameSystem.Instance.AddWait(new Wait(GameSystem.Instance.AudioController.GetRemainingVoicePlayTime(mODCurrentVoiceLayerDetect), WaitTypes.WaitForVoice, null));
					MODTextController.MODLineContinueDetect = false;
					break;
				}
			}
		}

		public void SetAutoTextWait()
		{
			if (gameSystem.IsAuto)
			{
				float num = (float)(100 - AutoPageSpeed) / 100f * 2f;
				float num2 = textTimeRemaining + timePerLine * num;
				if (lastMode != BurikoTextModes.Continue && lastMode != BurikoTextModes.ContinueAftertyping)
				{
					num2 += (float)charList.Count * 0.01f * num;
				}
				gameSystem.AddWait(new Wait(num2, WaitTypes.WaitForAuto, delegate
				{
					gameSystem.ClearInputWaits();
				}));
			}
		}

		public void SetText(string name, string text, BurikoTextModes textMode, int language, bool skipUpdate = false)
		{
			if (!appendNext)
			{
				text = string.Format(NameFormat, name) + text;
			}
			if (language == 1 && GameSystem.Instance.UseEnglishText)
			{
				japaneseprev = japanesetext;
				if (appendNext)
				{
					japanesetext += text;
				}
				else
				{
					japanesetext = text;
				}
				return;
			}
			if (language == 2 && !GameSystem.Instance.UseEnglishText)
			{
				if (englishprev.EndsWith(",") || englishprev.EndsWith(".") || englishprev.EndsWith("!") || englishprev.EndsWith("?"))
				{
					text += " ";
				}
				if (englishprev.Length > 0 && englishprev[englishprev.Length - 1] > 'a' && englishprev[englishprev.Length - 1] < 'Z')
				{
					text += " ";
				}
				englishprev = englishtext;
				if (prevAppendState)
				{
					englishtext += text;
				}
				else
				{
					englishtext = text;
				}
				return;
			}
			if (textTimeRemaining < 0f)
			{
				textTimeRemaining = 0f;
				isFading = false;
			}
			if (GameSystem.Instance.UseEnglishText)
			{
				englishprev = txt;
				englishtext += text;
			}
			else
			{
				japaneseprev = txt;
				japanesetext += text;
			}
			prevAppendState = appendNext;
			if (language == 0)
			{
				if (!GameSystem.Instance.UseEnglishText)
				{
					englishprev = englishtext;
				}
				else
				{
					japaneseprev = japanesetext;
				}
				if (appendNext)
				{
					englishtext += text;
					japanesetext += text;
				}
				else
				{
					englishtext = text;
					japanesetext = text;
				}
			}
			if (!gameSystem.MessageBoxVisible)
			{
				if (skipUpdate)
				{
					CreateText(name, text, textMode, noUpdate: true);
					appendNext = (textMode != BurikoTextModes.Normal);
					gameSystem.ExecuteActions();
					return;
				}
				if (gameSystem.IsSkipping)
				{
					gameSystem.MainUIController.ShowMessageBox();
					CreateText(name, text, textMode);
					appendNext = (textMode != BurikoTextModes.Normal);
					gameSystem.ExecuteActions();
				}
				else
				{
					gameSystem.MainUIController.FadeIn(0.2f);
					gameSystem.AddWait(new Wait(0.2f, WaitTypes.WaitForMove, delegate
					{
						CreateText(name, text, textMode);
						appendNext = (textMode != BurikoTextModes.Normal);
					}));
				}
			}
			else
			{
				CreateText(name, text, textMode);
				appendNext = (textMode != BurikoTextModes.Normal);
			}
			gameSystem.ExecuteActions();
		}

		public TextController()
		{
			gameSystem = GameSystem.Instance;
			TextArea = GameObject.FindGameObjectWithTag("TextArea").GetComponent<TextMeshPro>();
		}

		public void Update()
		{
			if (isFading)
			{
				UpdateTextArea();
			}
			if (gameSystem.IsAuto && lastMode == BurikoTextModes.Continue)
			{
				gameSystem.ClearVoiceWaits();
			}
		}
	}
}
