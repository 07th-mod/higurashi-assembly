using Assets.Scripts.Core.Buriko;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.UI.TitleScreen
{
	public class TitleScreen : MonoBehaviour
	{
		public TweenAlpha BackgroundTween;

		public UITexture BackgroundTexture;

		public Texture2D BG1;

		public Texture2D BG2;

		public List<UISprite> Sprites;

		public bool IsActive = true;

		private IEnumerator LeaveMenuAnimation(MenuUIController.MenuCloseDelegate onClose)
		{
			this.BackgroundTween.PlayReverse();
			yield return new WaitForSeconds(0.5f);
			if (onClose != null)
			{
				onClose();
			}
			UnityEngine.Object.Destroy(base.gameObject);
			yield break;
		}

		public void FadeOut()
		{
			BackgroundTween.PlayReverse();
		}

		public void FadeIn()
		{
			BackgroundTween.PlayForward();
		}

		public void Leave(MenuUIController.MenuCloseDelegate onClose)
		{
			for (int i = 0; i < Sprites.Count; i++)
			{
				TitleScreenButton component = Sprites[i].GetComponent<TitleScreenButton>();
				if (component != null)
				{
					component.IsLeaving = true;
				}
			}
			StartCoroutine(LeaveMenuAnimation(onClose));
		}

		private void OpeningAnimation()
		{
			for (int j = 0; j < Sprites.Count; j++)
			{
				int i = j;
				LTDescr lTDescr = LeanTween.value(Sprites[j].gameObject, delegate(float f)
				{
					Sprites[i].color = new Color(1f, 1f, 1f, f);
				}, 0f, 1f, 1f);
				lTDescr.delay = (float)j * 0.25f;
			}
		}

		public void Enter()
		{
			BurikoVariable globalFlag = BurikoMemory.Instance.GetGlobalFlag("GFlag_GameClear");
			BackgroundTexture.mainTexture = (globalFlag.BoolValue() ? BG2 : BG1);
			if (!BurikoMemory.Instance.GetHighestChapterFlags().Where( x => x > 0 ).Any())
			{
				Sprites[4].transform.localPosition = new Vector3(0f, 25f, 0f);
				UISprite uISprite = Sprites[3];
				Sprites.RemoveAt(3);
				Object.Destroy(uISprite.gameObject);
			}
			foreach (UISprite sprite in Sprites)
			{
				sprite.color = new Color(1f, 1f, 1f, 0f);
			}
			OpeningAnimation();
		}
	}
}
