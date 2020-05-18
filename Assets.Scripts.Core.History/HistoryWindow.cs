using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Core.History
{
	public class HistoryWindow : MonoBehaviour
	{
		public TextMeshPro[] Labels;

		public UISlider Slider;

		public UITexture BackgroundTexture;

		public UIPanel HistoryPanel;

		private HistoryTextButton[] textButtons;

		private GameSystem gameSystem;

		private TextHistory textHistory;

		private int stepCount;

		private int lastStep = -1;
		// Stores the item and position of the line at the bottom of the window
		private int lastHistoryIndex = -1;
		private int lineInLastHistoryIndex = -1;

		private float stepsize;

		private const float textAreaHeight = 600;
		private const int totalLinesInWindow = 20;

		private IEnumerator LeaveMenuAnimation(MenuUIController.MenuCloseDelegate onClose)
		{
			yield return (object)new WaitForEndOfFrame();
			LeanTween.cancel(BackgroundTexture.gameObject);
			LeanTween.cancel(HistoryPanel.gameObject);
			LeanTween.value(this.BackgroundTexture.gameObject, delegate(float f)
			{
				this.BackgroundTexture.alpha = f;
			}, 0.5f, 0f, 0.2f);
			LeanTween.value(this.HistoryPanel.gameObject, delegate(float f)
			{
				this.HistoryPanel.alpha = f;
			}, 1f, 0f, 0.2f);
			GameSystem.Instance.MainUIController.FadeIn(0.2f);
			GameSystem.Instance.SceneController.RevealFace(0.2f);
			foreach (HistoryTextButton t in textButtons)
			{
				t.FadeOut(0.2f);
			}
			GameSystem.Instance.ExecuteActions();
			yield return (object)new WaitForSeconds(0.3f);
			onClose?.Invoke();
			Object.Destroy(base.gameObject);
		}

		public void Leave(MenuUIController.MenuCloseDelegate onClose)
		{
			foreach (HistoryTextButton t in textButtons) {
				t.SetIsClosing(true);
			}
			StartCoroutine(LeaveMenuAnimation(onClose));
		}

		/// <summary>
		/// Trims the text in the TextMeshPro object based on the given line number
		/// A positive line number will trim anything after the given line
		/// A negative line number will trim off the first `-line` lines
		/// </summary>
		/// <param name="tmp">The TextMeshPro object containing the text you want to trim</param>
		/// <param name="line">The line number for trimming</param>
		private void TrimTextInTMP(TextMeshPro tmp, int line)
		{
			tmp.ForceMeshUpdate();
			TMP_CharacterInfo[] charInfo = tmp.textInfo.characterInfo;
			if (tmp.textInfo.lineCount <= line - 1)
			{
				return;
			}
			if (tmp.textInfo.lineCount <= -line)
			{
				tmp.text = "";
				return;
			}
			//Debug.Log("Trimming " + tmp.text + " to " + line + " lines");
			int target = line;
			if (target < 0)
			{
				target = -target - 1;
			}
			int foundChar = -1;
			int multiCharChars = 0;
			foreach (TMP_CharacterInfo info in charInfo)
			{
				//Debug.Log("Character Info: index " + info.index + " meshIndex " + info.meshIndex + " visible " + info.isVisible + " xAdvance " + info.xAdvance);
				if (info.lineNumber > target)
				{
					foundChar = info.index;
					break;
				}
				// Things like \n take up two characters in the string but are parsed by TMP as one
				if (tmp.text[info.index + multiCharChars] == '\\')
				{
					//Debug.Log("Found multiChar char");
					multiCharChars += 1;
				}
			}
			if (foundChar == -1)
			{
				return;
			}
			string newText = tmp.text;
			if (line < 0)
			{
				newText = newText.Substring(foundChar + multiCharChars);
			}
			else {
				newText = newText.Substring(0, foundChar + multiCharChars);
			}
			//Debug.Log("Trimmed " + tmp.text + " to " + newText);
			tmp.text = newText;
		}

		private void FillText()
		{
			int lineNum = totalLinesInWindow;
			for (int i = 0; i < totalLinesInWindow; i++)
			{
				int index = lastHistoryIndex - i;
				HistoryLine line = textHistory.GetLine(index);
				TextMeshPro label = textButtons[i].GetTextMesh();
				if (line == null || lineNum <= 0)
				{
					label.gameObject.SetActive(false);
					continue;
				}
				label.gameObject.SetActive(true);
				// Put voice and text in it
				label.text = GameSystem.Instance.ChooseJapaneseEnglish(japanese: line.TextJapanese, english: line.TextEnglish);
				textButtons[i].RegisterVoices(line.VoiceFiles);

				// Update lineNum and trim text if needed
				int lineCount = 0;
				if (i == 0)
				{
					lineCount = (lineInLastHistoryIndex + 1);
					TrimTextInTMP(label, lineInLastHistoryIndex);
				}
				else
				{
					lineCount = GameSystem.Instance.ChooseJapaneseEnglish(japanese: line.JapaneseHeight, english: line.EnglishHeight);
					if (lineCount > lineNum)
					{
						TrimTextInTMP(label, lineNum - lineCount);
						lineCount = lineNum;
					}
				}
				lineNum -= lineCount;

				// Move the label into position
				Vector3 position = textButtons[i].gameObject.transform.localPosition;
				position.y = -(textAreaHeight / totalLinesInWindow) * lineNum;
				textButtons[i].gameObject.transform.localPosition = position;

				// Resize the line's hitbox
				label.ForceMeshUpdate();
				BoxCollider hitbox = textButtons[i].gameObject.GetComponent<BoxCollider>();
				hitbox.size = label.bounds.size;
				hitbox.center = label.bounds.center;

				//Debug.Log("Outputting to line " + lineNum + ": " + label.text);
			}
			// Move lines up to the top if we didn't fully fill the screen
			if (lineNum > 0)
			{
				foreach (HistoryTextButton textButton in textButtons)
				{
					Vector3 position = textButton.gameObject.transform.localPosition;
					position.y += 30 * lineNum;
					textButton.gameObject.transform.localPosition = position;
				}
			}
		}

		public void Step(float f)
		{
			if (Slider.numberOfSteps == 2)
			{
				f *= 10f;
			}
			Slider.value -= f * stepsize;
		}

		private int GetNumberOfSteps()
		{
			return GameSystem.Instance.ChooseJapaneseEnglish(japanese: textHistory.JapaneseLineCount, english: textHistory.EnglishLineCount) - totalLinesInWindow + 1;
		}

		private int GetLinesInHistoryItemAtIndex(int index)
		{
			HistoryLine line = textHistory.GetLine(index);
			if (line == null)
			{
				return 0;
			}
			return GameSystem.Instance.ChooseJapaneseEnglish(japanese: line.JapaneseHeight, english: line.EnglishHeight);
		}

		private void Awake()
		{
			gameSystem = GameSystem.Instance;
			textHistory = gameSystem.TextHistory;
			stepCount = GetNumberOfSteps();
			Slider.numberOfSteps = Mathf.Max(stepCount, 1);
			Slider.value = 1f;
			lastStep = -1;
			stepsize = 1f / (float)Slider.numberOfSteps;

			lastHistoryIndex = textHistory.LineCount - 1;
			lineInLastHistoryIndex = GetLinesInHistoryItemAtIndex(lastHistoryIndex) - 1;

			textButtons = new HistoryTextButton[totalLinesInWindow];
			TextMeshProFont currentFont = GameSystem.Instance.MainUIController.GetCurrentFont();
			for (int i = 0; i < textButtons.Length; i++)
			{
				GameObject tmp = Instantiate(Labels[0].gameObject);
				tmp.transform.SetParent(Labels[0].gameObject.transform.parent, worldPositionStays: false);
				textButtons[i] = tmp.GetComponent<HistoryTextButton>();
				textButtons[i].GetTextMesh().text = "";
				textButtons[i].GetTextMesh().font = currentFont;
			}
			// We're not using these anymore because there's only 5 and we might need more than that.
			foreach (TextMeshPro label in Labels) {
				Destroy(label.gameObject);
			}

			FillText();
			foreach (HistoryTextButton historyTextButton in textButtons)
			{
				if (historyTextButton.isActiveAndEnabled)
				{
					historyTextButton.FadeIn(0.2f);
				}
			}
			BackgroundTexture.alpha = 0f;
			HistoryPanel.alpha = 0f;
			LeanTween.value(BackgroundTexture.gameObject, delegate(float f)
			{
				BackgroundTexture.alpha = f;
			}, 0f, 0.5f, 0.2f);
			LeanTween.value(HistoryPanel.gameObject, delegate(float f)
			{
				HistoryPanel.alpha = f;
			}, 0f, 1f, 0.2f);
		}

		/// <summary>
		/// Updates all the things needed to scroll to the given line
		/// </summary>
		/// <param name="line">The line to scroll to</param>
		private void MoveToLine(int line)
		{
			if (lastStep == -1 || textHistory.LineCount == 0) { lastStep = line; }
			int distance = lastStep - line;
			lastStep = line;
			if (distance < 0)
			{
				distance = -distance;
				// Scrolling down
				int linesInHistoryItem = GetLinesInHistoryItemAtIndex(lastHistoryIndex);
				while (true)
				{
					if (linesInHistoryItem - lineInLastHistoryIndex > distance)
					{
						// Scroll within the current history item
						lineInLastHistoryIndex += distance;
						break;
					}
					else if (lastHistoryIndex >= textHistory.LineCount - 1)
					{
						// This shouldn't happen but who knows
						Debug.Log("Tried to scroll off the bottom of the text history!");
						lineInLastHistoryIndex = linesInHistoryItem - 1;
						break;
					}
					else
					{
						// Scroll to next history item
						distance -= (linesInHistoryItem - lineInLastHistoryIndex);
						lastHistoryIndex += 1;
						lineInLastHistoryIndex = 0;
						linesInHistoryItem = GetLinesInHistoryItemAtIndex(lastHistoryIndex);
					}
				}
			}
			else if (distance > 0)
			{
				// Scrolling up
				while (true)
				{
					if (lineInLastHistoryIndex >= distance)
					{
						// Scroll within the current history item
						lineInLastHistoryIndex -= distance;
						break;
					}
					else if (lastHistoryIndex <= 0)
					{
						// This shouldn't happen but who knows
						Debug.Log("Tried to scroll off the top of the text history!");
						lineInLastHistoryIndex = 0;
						break;
					}
					else
					{
						// Scroll to previous history item
						distance -= (lineInLastHistoryIndex + 1);
						lastHistoryIndex -= 1;
						lineInLastHistoryIndex = GetLinesInHistoryItemAtIndex(lastHistoryIndex) - 1;
					}
				}
			}
		}

		private void Update()
		{
			if (!Mathf.Approximately(Input.GetAxis("Mouse ScrollWheel"), 0f))
			{
				Step(Input.GetAxis("Mouse ScrollWheel") * 10f);
			}
			int num = Mathf.RoundToInt(Slider.value * (float)(Slider.numberOfSteps - 1));
			if (num != lastStep)
			{
				MoveToLine(num);
				FillText();
			}
		}
	}
}
