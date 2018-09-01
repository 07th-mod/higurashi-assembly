using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Choice
{
	public class ChoiceButton : MonoBehaviour
	{
		public delegate void ClickCallback();

		public TextMeshPro ButtonTextMesh;

		private float fadeInTime = 0.5f;

		private ClickCallback clickCallback;

		private bool isEnabled = true;

		private void UpdateColor(Color c)
		{
			ButtonTextMesh.color = c;
		}

		private void FadeToColor(Color c)
		{
			LeanTween.cancel(base.gameObject);
			Color color = ButtonTextMesh.color;
			LeanTween.value(base.gameObject, UpdateColor, color, c, fadeInTime);
		}

		private void OnClick()
		{
			if (GameSystem.Instance.GameState == GameState.ChoiceScreen && !(fadeInTime > 0f) && isEnabled && UICamera.currentTouchID >= -1)
			{
				if (clickCallback != null)
				{
					clickCallback();
				}
				AudioController.Instance.PlaySystemSound("sysse02.ogg");
			}
		}

		private void OnHover(bool ishover)
		{
			if (GameSystem.Instance.GameState == GameState.ChoiceScreen)
			{
				if (ishover)
				{
					FadeToColor(new Color(1f, 0f, 0f, 1f));
				}
				else
				{
					FadeToColor(new Color(1f, 1f, 1f, 1f));
				}
			}
		}

		public void ChangeText(string newtxt)
		{
			TextMeshProFont currentFont = GameSystem.Instance.MainUIController.GetCurrentFont();
			ButtonTextMesh.font = currentFont;
			ButtonTextMesh.text = newtxt;
		}

		public void DisableButton()
		{
			isEnabled = false;
			FadeToColor(new Color(1f, 1f, 1f, 0f));
		}

		public void SetCallback(ChoiceController controller, ClickCallback callback)
		{
			clickCallback = callback;
		}

		private void Start()
		{
			UpdateColor(new Color(1f, 1f, 1f, 0f));
			FadeToColor(new Color(1f, 1f, 1f, 1f));
		}

		private void Update()
		{
			fadeInTime -= Time.deltaTime;
		}
	}
}
