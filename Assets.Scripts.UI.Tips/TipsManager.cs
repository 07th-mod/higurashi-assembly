using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Tips
{
	public class TipsManager : MonoBehaviour
	{
		public UIPanel Panel;

		public Material TextMaterial;

		public TextMeshPro tipsTitleText;

		public TextMeshPro tipsPageText;

		public UIButton PageLeft;

		public UIButton PageRight;

		public List<TipsButton> TipsButtons;

		public List<TipsEntry> TipsEntries;

		public bool isActive = true;

		public bool hasTips;

		private int page;

		private int numPages = 1;

		private TipsDataGroup tipsData;

		public static int ReturnPage;

		private void SetFade(float f)
		{
			Panel.alpha = f;
			TextMaterial.SetColor("_FaceColor", new Color(1f, 1f, 1f, f));
			TextMaterial.SetColor("_UnderlayColor", new Color(0f, 0f, 0f, 0.8f * f));
		}

		public void ChangePage(int change)
		{
			if (page + change >= 0 && page + change <= numPages)
			{
				page += change;
				UpdateEntries();
			}
		}

		public void ShowTitle(string title)
		{
			tipsTitleText.text = title;
		}

		public void ClearTitle()
		{
			if (hasTips)
			{
				tipsTitleText.text = "";
			}
			else
			{
				tipsTitleText.text = ((!GameSystem.Instance.UseEnglishText) ? "入手ＴＩＰＳはありません" : "No new tips available.");
			}
		}

		public void UpdatePage()
		{
			if (numPages == 0)
			{
				tipsPageText.text = "";
			}
			else
			{
				tipsPageText.text = page + 1 + "/" + (numPages + 1);
			}
			PageLeft.isEnabled = (page > 0);
			PageRight.isEnabled = (page < numPages);
		}

		public void UpdateEntries()
		{
			for (int i = 0; i < TipsEntries.Count; i++)
			{
				int num = i + page * 8;
				TipsEntries[i].Reset();
				TipsEntries[i].gameObject.name = "TipsEntry" + i;
				if (num >= tipsData.TipsUnlocked)
				{
					TipsEntries[i].gameObject.SetActive(value: false);
				}
				if (num < tipsData.TipsUnlocked)
				{
					TipsEntries[i].gameObject.SetActive(value: true);
					TipsEntries[i].Init(tipsData.Tips[num], this);
					hasTips = true;
				}
				if (num < tipsData.TipsAvailable)
				{
					TipsEntries[i].gameObject.SetActive(value: true);
					hasTips = true;
				}
			}
			UpdatePage();
			ClearTitle();
		}

		public void Show(int tipstype)
		{
			if (GameSystem.Instance.UseEnglishText)
			{
				TMP_FontAsset englishFont = GameSystem.Instance.MainUIController.GetEnglishFont();
				tipsTitleText.font = englishFont;
			}
			else
			{
				TMP_FontAsset japaneseFont = GameSystem.Instance.MainUIController.GetJapaneseFont();
				tipsTitleText.font = japaneseFont;
			}
			LeanTween.value(base.gameObject, SetFade, 0f, 1f, 0.8f).onComplete = delegate
			{
				GameSystem.Instance.AudioController.PlayAudio("OMAKE2.ogg", Assets.Scripts.Core.Audio.AudioType.BGM, 0, 0.7f);
			};
			switch (tipstype)
			{
			case 0:
				tipsData = TipsData.GetVisibleTips(onlyNew: false, global: false);
				break;
			case 1:
				tipsData = TipsData.GetVisibleTips(onlyNew: true, global: false);
				break;
			case 2:
				tipsData = TipsData.GetVisibleTips(onlyNew: false, global: true);
				break;
			default:
				throw new ArgumentOutOfRangeException("tipstype for TipsManager.Show() must be  between 0 and 2 (" + tipstype + " given)");
			}
			tipsTitleText.font = GameSystem.Instance.MainUIController.GetCurrentFont();
			page = 0;
			numPages = Mathf.CeilToInt((float)tipsData.TipsAvailable / 8f) - 1;
			if (numPages < 0)
			{
				numPages = 0;
			}
			if (numPages >= ReturnPage)
			{
				page = ReturnPage;
			}
			UpdateEntries();
			TipsButtons.ForEach(delegate(TipsButton a)
			{
				a.Setup(this);
			});
		}

		public void Hide(Action onFinish)
		{
			isActive = false;
			LTDescr lTDescr = LeanTween.value(base.gameObject, SetFade, 1f, 0f, 0.8f);
			lTDescr.onComplete = onFinish;
			lTDescr.onComplete = (Action)Delegate.Combine(lTDescr.onComplete, (Action)delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			TipsButtons.ForEach(delegate(TipsButton a)
			{
				a.Disable();
			});
			ReturnPage = page;
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
