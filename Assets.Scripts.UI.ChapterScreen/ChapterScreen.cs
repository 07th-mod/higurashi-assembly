using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.UI.Tips;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.ChapterScreen
{
	public class ChapterScreen : MonoBehaviour
	{
		public UIPanel Panel;

		public Material TextMaterial;

		public List<ChapterButton> ChapterButtons;

		public bool isActive = true;

		public void Show()
		{
			SetFade(0f);
			GameSystem.Instance.RegisterAction(delegate
			{
				LeanTween.value(base.gameObject, SetFade, 0f, 1f, 0.8f);
			});
			GameSystem.Instance.ExecuteActions();
			TipsManager.ReturnPage = 0;
		}

		public void Hide(Action onFinish)
		{
			Debug.Log("Hide ChapterScreen");
			isActive = false;
			LTDescr lTDescr = LeanTween.value(base.gameObject, SetFade, 1f, 0f, 0.8f);
			lTDescr.onComplete = onFinish;
			lTDescr.onComplete = (Action)Delegate.Combine(lTDescr.onComplete, (Action)delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			ChapterButtons.ForEach(delegate(ChapterButton a)
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
