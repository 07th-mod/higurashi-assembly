using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using UnityEngine;

namespace Assets.Scripts.UI.Choice
{
	public class ChoiceButton : MonoBehaviour
	{
		public delegate void ClickCallback();

		public UILabel ButtonText;

		public UITweener AlphaTweener;

		public UITweener ColorTweener;

		private float fadeInTime = 0.5f;

		private ClickCallback clickCallback;

		private bool isEnabled = true;

		private void OnClick()
		{
			if (GameSystem.Instance.GameState == GameState.ChoiceScreen && !(fadeInTime > 0f) && isEnabled && UICamera.currentTouchID == -1)
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
					ColorTweener.PlayForward();
				}
				else
				{
					ColorTweener.PlayReverse();
				}
			}
		}

		public void ChangeText(string newtxt)
		{
			ButtonText.text = newtxt;
		}

		public void DisableButton()
		{
			isEnabled = false;
			AlphaTweener.PlayReverse();
		}

		public void SetCallback(ChoiceController controller, ClickCallback callback)
		{
			clickCallback = callback;
		}

		private void Start()
		{
			AlphaTweener.PlayForward();
		}

		private void Update()
		{
			fadeInTime -= Time.deltaTime;
		}
	}
}
