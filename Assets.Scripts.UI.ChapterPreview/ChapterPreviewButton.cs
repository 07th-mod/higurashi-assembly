using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using UnityEngine;

namespace Assets.Scripts.UI.ChapterPreview
{
	internal class ChapterPreviewButton : MonoBehaviour
	{
		public UIButton Button;

		public GameObject Description;

		public GameObject DescriptionJp;

		public int ValueFlag;

		private float lockout = 1.1f;

		public void Disable()
		{
			Button.isEnabled = false;
		}

		public void Update()
		{
			lockout -= Time.deltaTime;
			if (!(lockout > 0f))
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
			if (!(lockout > 0f) && UICamera.currentTouchID >= -1 && GameSystem.Instance.GameState == GameState.ChapterPreview)
			{
				StateChapterPreview stateChapterPreview = GameSystem.Instance.GetStateObject() as StateChapterPreview;
				if (stateChapterPreview != null)
				{
					BurikoScriptSystem.Instance.SetFlag("LOCALWORK_NO_RESULT", ValueFlag);
					stateChapterPreview.RequestLeave();
				}
			}
		}
	}
}
