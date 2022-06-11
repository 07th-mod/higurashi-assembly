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
			yield return new WaitForEndOfFrame();
			LeanTween.cancel(BackgroundTexture.gameObject);
			LeanTween.cancel(HistoryPanel.gameObject);
			LeanTween.value(BackgroundTexture.gameObject, delegate(float f)
			{
				BackgroundTexture.alpha = f;
			}, 0.5f, 0f, 0.2f);
			LeanTween.value(HistoryPanel.gameObject, delegate(float f)
			{
				HistoryPanel.alpha = f;
			}, 1f, 0f, 0.2f);
			GameSystem.Instance.MainUIController.FadeIn(0.2f);
			GameSystem.Instance.SceneController.RevealFace(0.2f);
			HistoryTextButton[] array = textButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FadeOut(0.2f);
			}
			GameSystem.Instance.ExecuteActions();
			yield return new WaitForSeconds(0.3f);
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
					Labels[i].text = "";
					textButtons[i].ClearVoice();
					continue;
				}
				string text = "";
				text = ((!GameSystem.Instance.UseEnglishText) ? line.TextJapanese : line.TextEnglish);
				if (line.VoiceFile != null)
				{
					textButtons[i].RegisterVoice(line.VoiceFile);
				}
				else
				{
					textButtons[i].ClearVoice();
				}
				Labels[i].text = text;
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
			TMP_FontAsset currentFont = GameSystem.Instance.MainUIController.GetCurrentFont();
			Debug.Log(currentFont);
			for (int i = 0; i < 5; i++)
			{
				textButtons[i] = Labels[i].gameObject.GetComponent<HistoryTextButton>();
				textButtons[i].GetTextMesh().font = currentFont;
			}
			FillText();
			HistoryTextButton[] array = textButtons;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].FadeIn(0.2f);
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
