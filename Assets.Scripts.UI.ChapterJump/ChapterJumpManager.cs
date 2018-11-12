using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.ChapterJump
{
	public class ChapterJumpManager : MonoBehaviour
	{
		public UIPanel Panel;

		public Material TextMaterial;

		public List<ChapterJumpButton> JumpButtons;

		public bool isActive = true;

		public void Show()
		{
			float fontSize = GameSystem.Instance.ChapterJumpFontSize;
			if (fontSize > 0)
			{
				foreach (var button in GetComponentsInChildren<ChapterJumpButton>())
				{
					button.SetFontSize(fontSize);
				}
			}
			LeanTween.value(base.gameObject, SetFade, 0f, 1f, 0.8f);
		}

		public void Hide(Action onFinish)
		{
			Debug.Log("Hide ChapterJumpScreen");
			isActive = false;
			LTDescr lTDescr = LeanTween.value(base.gameObject, SetFade, 1f, 0f, 0.8f);
			lTDescr.onComplete = onFinish;
			LTDescr lTDescr2 = lTDescr;
			lTDescr2.onComplete = (Action)Delegate.Combine(lTDescr2.onComplete, (Action)delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			JumpButtons.ForEach(delegate(ChapterJumpButton a)
			{
				a.Disable();
			});
		}

		private void SetFade(float f)
		{
			Panel.alpha = f;
			TextMaterial.SetColor("_FaceColor", new Color(1f, 1f, 1f, f));
			TextMaterial.SetColor("_UnderlayColor", new Color(0f, 0f, 0f, 0.8f * f));
		}

		public void Start()
		{
			if (base.name == "CastReview" && !BurikoMemory.Instance.GetGlobalFlag("GFlag_GameClear").BoolValue())
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
