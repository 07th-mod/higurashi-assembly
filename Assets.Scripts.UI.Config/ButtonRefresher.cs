using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.UI.Config
{
	internal class ButtonRefresher : MonoBehaviour
	{
		public string EnglishNormal;

		public string EnglishHover;

		public string EnglishDown;

		public string EnglishDisabled;

		public string JapaneseNormal;

		public string JapaneseHover;

		public string JapaneseDown;

		public string JapaneseDisabled;

		private UIButton button;

		private bool language;

		public void UpdateImage()
		{
			if (button == null)
			{
				button = GetComponent<UIButton>();
			}
			button.normalSprite = ((!language) ? JapaneseNormal : EnglishNormal);
			button.hoverSprite = ((!language) ? JapaneseHover : EnglishHover);
			button.pressedSprite = ((!language) ? JapaneseDown : EnglishDown);
			button.disabledSprite = ((!language) ? JapaneseDisabled : EnglishDisabled);
		}

		public void Start()
		{
			language = GameSystem.Instance.UseEnglishText;
			UpdateImage();
		}

		public void Update()
		{
			if (language != GameSystem.Instance.UseEnglishText)
			{
				language = GameSystem.Instance.UseEnglishText;
				UpdateImage();
			}
		}
	}
}
