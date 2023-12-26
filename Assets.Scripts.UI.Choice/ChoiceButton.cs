using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using UnityEngine;
using TMPro;

namespace Assets.Scripts.UI.Choice
{
	public class ChoiceButton : MonoBehaviour
	{
		public delegate void ClickCallback();

		public UILabel ButtonText;

		public UITweener AlphaTweener;

		public UITweener ColorTweener;

		public static float fadeTime = .25f;
		private float tweenTimer;

		// This prevents the user selecting a choice before the choices are fully loaded
		private float fadeInTime = fadeTime/2;

		private ClickCallback clickCallback;

		private bool isEnabled = true;

		private TextMeshPro ButtonTextMesh;

		private void UpdateAlpha(float alpha)
		{
			Color color = ButtonTextMesh.color;
			color.a = alpha;
			ButtonTextMesh.color = color;
		}

		private void UpdateColor(Vector3 c)
		{
			Color color = ButtonTextMesh.color;
			color.r = c.x;
			color.b = c.y;
			color.g = c.z;
			ButtonTextMesh.color = color;
		}

		private void FadeToAlpha(float alpha)
		{
			LeanTween.cancel(base.gameObject);
			Color color = ButtonTextMesh.color;
			tweenTimer = fadeTime;
			LeanTween.value(base.gameObject, UpdateAlpha, color.a, alpha, tweenTimer);
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
					UpdateColor(new Vector3(1f, 0f, 0f));
				}
				else
				{
					UpdateColor(new Vector3(1f, 1f, 1f));
				}
			}
		}

		public void ChangeText(string newtxt)
		{
			ButtonTextMesh = gameObject.AddComponent<TextMeshPro>();

			// These seem to be the values that are important to
			//  copy from the main text controller's settings
			// We can't just clone the main text controller's TextArea
			//  because the only way to make Unity objects appear onscreen
			//  seems to be to make them using AddComponent
			TextMeshPro baseText = GameSystem.Instance.TextController.TextArea;
			ButtonTextMesh.isOrthographic = baseText.isOrthographic; // Font is 1/10 the size with this off.  Who knows why
			ButtonTextMesh.fontStyle = baseText.fontStyle;
			ButtonTextMesh.fontSize = 48f; // Taken from Himatsubushi

			ButtonTextMesh.font = GameSystem.Instance.MainUIController.GetCurrentFont();
			ButtonTextMesh.text = newtxt;
			ButtonTextMesh.transform.position = ButtonText.transform.position;
			ButtonTextMesh.name = "ChoiceButton Text";

			// Don't show the buttonText, we want to use the TMP font
			ButtonText.alpha = 0f;
		}

		public void DisableButton()
		{
			isEnabled = false;
			UpdateAlpha(0f);
		}

		public void SetCallback(ChoiceController controller, ClickCallback callback)
		{
			clickCallback = callback;
		}

		private void Start()
		{
			UpdateColor(new Vector3(1f, 1f, 1f));
			UpdateAlpha(0f);
			FadeToAlpha(1f);
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

		public float GetFontSize() => ButtonTextMesh.fontSize;
		public void SetFontSize(float fontSize) => ButtonTextMesh.fontSize = fontSize;
	}
}
