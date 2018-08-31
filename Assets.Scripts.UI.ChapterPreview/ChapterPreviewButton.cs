using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using UnityEngine;

namespace Assets.Scripts.UI.ChapterPreview
{
	internal class ChapterPreviewButton : MonoBehaviour
	{
		public UIButton Button;

		public void Disable()
		{
			Button.isEnabled = false;
		}

		public void OnClick()
		{
			if (UICamera.currentTouchID == -1 && GameSystem.Instance.GameState == GameState.ChapterPreview)
			{
				StateChapterPreview stateChapterPreview = GameSystem.Instance.GetStateObject() as StateChapterPreview;
				if (stateChapterPreview != null)
				{
					if (base.gameObject.name == "StartButton")
					{
						BurikoScriptSystem.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
					}
					else
					{
						BurikoScriptSystem.Instance.SetFlag("LOCALWORK_NO_RESULT", 0);
					}
					stateChapterPreview.RequestLeave();
				}
			}
		}
	}
}
