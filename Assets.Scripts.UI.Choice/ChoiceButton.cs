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

		public static float fadeTime = .25f;
		private float tweenTimer;

		// This prevents the user selecting a choice before the choices are fully loaded
		private float fadeInTime = fadeTime/2;

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
			tweenTimer = fadeTime;
			LeanTween.value(base.gameObject, UpdateColor, color, c, tweenTimer);
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
			if (GameSystem.Instance.GameState == GameState.ChoiceScreen && isEnabled)
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
			if(tweenTimer > 0)
			{
				tweenTimer -= Time.deltaTime;
			}

			if(fadeInTime > 0)
			{
				fadeInTime -= Time.deltaTime;
			}
		}
	}
}
