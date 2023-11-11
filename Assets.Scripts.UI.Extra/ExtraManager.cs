using Assets.Scripts.Core;
using Assets.Scripts.UI.Tips;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.Extra
{
	public class ExtraManager : MonoBehaviour
	{
		public UIPanel Panel;

		public Material TextMaterial;

		public List<ExtraButton> ExtraButtons;

		public bool isActive = true;

		public void Show()
		{
			if (base.gameObject.name.Contains("StaffRoom") && GameSystem.Instance.UseEnglishText)
			{
				for (int i = 0; i < ExtraButtons.Count; i++)
				{
					ExtraButtons[i].transform.localPosition -= new Vector3(0f, 10f, 0f);
				}
			}
			SetFade(0f);
			GameSystem.Instance.RegisterAction(delegate
			{
				LTDescr lTDescr = LeanTween.value(base.gameObject, SetFade, 0f, 1f, 0.8f);
				lTDescr.onComplete = (Action)Delegate.Combine(lTDescr.onComplete, (Action)delegate
				{
					ExtraButtons.ForEach(delegate(ExtraButton a)
					{
						a.Enable();
					});
				});
			});
			GameSystem.Instance.ExecuteActions();
			TipsManager.ReturnPage = 0;
		}

		public void Hide(Action onFinish)
		{
			Debug.Log("Hide ExtraScreen");
			isActive = false;
			LTDescr lTDescr = LeanTween.value(base.gameObject, SetFade, 1f, 0f, 0.8f);
			lTDescr.onComplete = onFinish;
			lTDescr.onComplete = (Action)Delegate.Combine(lTDescr.onComplete, (Action)delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			ExtraButtons.ForEach(delegate(ExtraButton a)
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
	}
}
