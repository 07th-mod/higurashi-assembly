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

		private float stepsize;

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
			HistoryTextButton[] array = textButtons;
			foreach (HistoryTextButton t in array)
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
			StartCoroutine(LeaveMenuAnimation(onClose));
		}

		private void FillText()
		{
			for (int i = 0; i < 5; i++)
			{
				int id = i + lastStep;
				HistoryLine line = textHistory.GetLine(id);
				if (line == null)
				{
					Labels[i].text = string.Empty;
					textButtons[i].ClearVoice();
				}
				else
				{
					if (GameSystem.Instance.UseEnglishText)
					{
						Labels[i].text = line.TextEnglish;
					}
					else
					{
						Labels[i].text = line.TextJapanese;
					}
					if (line.VoiceFile != null)
					{
						textButtons[i].RegisterVoice(line.VoiceFile);
					}
					else
					{
						textButtons[i].ClearVoice();
					}
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

		private void Awake()
		{
			gameSystem = GameSystem.Instance;
			textHistory = gameSystem.TextHistory;
			stepCount = textHistory.LineCount - 4;
			Slider.numberOfSteps = Mathf.Clamp(stepCount, 1, 100);
			Slider.value = 1f;
			lastStep = Slider.numberOfSteps;
			stepsize = 1f / (float)lastStep;
			textButtons = new HistoryTextButton[5];
			TextMeshProFont currentFont = GameSystem.Instance.MainUIController.GetCurrentFont();
			for (int i = 0; i < 5; i++)
			{
				textButtons[i] = Labels[i].gameObject.GetComponent<HistoryTextButton>();
				textButtons[i].GetTextMesh().font = currentFont;
			}
			FillText();
			HistoryTextButton[] array = textButtons;
			foreach (HistoryTextButton historyTextButton in array)
			{
				historyTextButton.FadeIn(0.2f);
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

		private void Update()
		{
			if (!Mathf.Approximately(Input.GetAxis("Mouse ScrollWheel"), 0f))
			{
				Step(Input.GetAxis("Mouse ScrollWheel") * 10f);
			}
			int num = Mathf.RoundToInt(Slider.value * (float)(Slider.numberOfSteps - 1));
			if (num != lastStep)
			{
				lastStep = num;
				FillText();
			}
		}
	}
}
