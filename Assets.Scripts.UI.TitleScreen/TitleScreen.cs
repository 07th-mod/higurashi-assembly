using Assets.Scripts.Core.Buriko;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			BackgroundTween.PlayReverse();
			yield return (object)new WaitForSeconds(0.5f);
			onClose?.Invoke();
			Object.Destroy(base.gameObject);
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
			// -- Begin hide extras menu for now since they don't work yet
			const bool forceHideExtras = true;
			// -- End hide extras 
			if (forceHideExtras || BurikoMemory.Instance.GetGlobalFlag("GHimatsubushiDay").IntValue() < 1)
			{
				Sprites[4].transform.localPosition = new Vector3(0f, -224f, 0f);
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
