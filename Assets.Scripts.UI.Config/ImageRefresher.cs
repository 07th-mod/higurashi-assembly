using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.UI.Config
{
	internal class ImageRefresher : MonoBehaviour
	{
		public Texture2D English;

		public Texture2D Japanese;

		private UITexture background;

		private bool language;

		public void UpdateImage()
		{
			if (background == null)
			{
				background = GetComponent<UITexture>();
			}
			background.mainTexture = ((!language) ? Japanese : English);
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
