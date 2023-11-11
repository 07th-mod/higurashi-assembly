using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using UnityEngine;

namespace Assets.Scripts.UI.ChapterPreview
{
	internal class ChapterPreviewButton : MonoBehaviour
	{
		public UIButton Button;

		public UISprite Sprite;

		public GameObject Description;

		public GameObject DescriptionJp;

		public GameObject LockedDescription;

		public GameObject LockedDescriptionJp;

		public int ValueFlag;

		private float lockout = 0.3f;

		public int ChapterNum;

		public bool Locked;

		public void Disable()
		{
			Button.enabled = false;
		}

		public void Enable()
		{
			Button.enabled = true;
		}

		public void BlockChapter()
		{
			Description = LockedDescription;
			DescriptionJp = LockedDescriptionJp;
			Button.hoverSprite = Button.normalSprite;
			Button.pressedSprite = Button.normalSprite;
			Locked = true;
		}

		public void Update()
		{
			lockout -= Time.deltaTime;
			if (!(lockout > 0f) && Button.isEnabled && !(Description == null) && !(DescriptionJp == null))
			{
				if (GameSystem.Instance.UseEnglishText)
				{
					Description.gameObject.SetActive(Button.state == UIButtonColor.State.Hover && GameSystem.Instance.GameState == GameState.ChapterPreview);
					DescriptionJp.gameObject.SetActive(value: false);
				}
				else
				{
					Description.gameObject.SetActive(value: false);
					DescriptionJp.gameObject.SetActive(Button.state == UIButtonColor.State.Hover && GameSystem.Instance.GameState == GameState.ChapterPreview);
				}
			}
		}

		public void OnClick()
		{
			lockout -= Time.deltaTime;
			if (lockout > 0f || Locked || UICamera.currentTouchID < -1 || GameSystem.Instance.GameState != GameState.ChapterPreview)
			{
				return;
			}
			StateChapterPreview stateChapterPreview = GameSystem.Instance.GetStateObject() as StateChapterPreview;
			if (stateChapterPreview != null)
			{
				if (ValueFlag != 0)
				{
					Button.normalSprite = Button.hoverSprite;
					Button.disabledSprite = Button.hoverSprite;
				}
				BurikoScriptSystem.Instance.SetFlag("LOCALWORK_NO_RESULT", ValueFlag);
				stateChapterPreview.RequestLeave();
			}
		}
	}
}
