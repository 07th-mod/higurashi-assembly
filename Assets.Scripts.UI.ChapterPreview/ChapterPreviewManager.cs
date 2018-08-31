using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.ChapterPreview
{
	internal class ChapterPreviewManager : MonoBehaviour
	{
		public UIPanel Panel;

		public List<ChapterPreviewButton> ChapterPreviewButtons;

		public void Show()
		{
			Panel.alpha = 0f;
			LeanTween.value(base.gameObject, delegate(float f)
			{
				Panel.alpha = f;
			}, 0f, 1f, 0.8f);
		}

		public void Hide(Action onFinish)
		{
			LTDescr lTDescr = LeanTween.value(base.gameObject, delegate(float f)
			{
				Panel.alpha = f;
			}, 1f, 0f, 0.8f);
			LTDescr lTDescr2 = lTDescr;
			lTDescr2.onComplete = (Action)Delegate.Combine(lTDescr2.onComplete, (Action)delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			onFinish?.Invoke();
			ChapterPreviewButtons.ForEach(delegate(ChapterPreviewButton a)
			{
				a.Disable();
			});
		}
	}
}
