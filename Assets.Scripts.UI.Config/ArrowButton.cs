using UnityEngine;

namespace Assets.Scripts.UI.Config
{
	public class ArrowButton : MonoBehaviour
	{
		public ConfigSlider Slider;

		public float StepChange;

		private void OnClick()
		{
			if (UICamera.currentTouchID >= -1)
			{
				Slider.Changestep(StepChange);
			}
		}

		private void OnHover(bool ishover)
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
