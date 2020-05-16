using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Fragments
{
	internal class FragmentsManager : MonoBehaviour
	{
		public UIPanel Panel;

		public Material TextMaterial;

		public Material DescriptionMaterial;

		public TextMeshPro tipsTitleText;

		public TextMeshPro tipsDescriptionText;

		public TextMeshPro tipsDescriptionTextShadow;

		public TextMeshPro tipsPageText;

		public UIButton PageLeft;

		public UIButton PageRight;

		public List<FragmentEntryButton> FragmentEntries;

		public List<FragmentUiButton> FragmentButtons;

		private List<FragmentDataEntry> validFragmentData;

		public bool isActive = true;

		public bool hasTips;

		private int page;

		private int numPages = 1;

		public static int ReturnPage;

		private void SetFade(float f)
		{
			Panel.alpha = f;
			TextMaterial.SetColor("_FaceColor", new Color(1f, 1f, 1f, f));
			TextMaterial.SetColor("_UnderlayColor", new Color(0f, 0f, 0f, 0.8f * f));
			DescriptionMaterial.SetColor("_FaceColor", new Color(0f, 0f, 0f, f));
			DescriptionMaterial.SetColor("_UnderlayColor", new Color(0f, 0f, 0f, 0.8f * f));
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

		public void ShowDescription(string description)
		{
			tipsDescriptionText.text = description;
			tipsDescriptionTextShadow.text = description;
		}

		public void ClearTitle()
		{
			tipsTitleText.text = string.Empty;
			tipsDescriptionText.text = string.Empty;
			tipsDescriptionTextShadow.text = string.Empty;
		}

		public void UpdatePage()
		{
			if (numPages == 0)
			{
				tipsPageText.text = string.Empty;
			}
			else
			{
				tipsPageText.text = page + 1 + "/" + (numPages + 1);
			}
			PageLeft.isEnabled = (page > 0);
			PageRight.isEnabled = (page < numPages);
		}

		public bool FufilledPrereqs(FragmentDataEntry fragment)
		{
			bool result = true;
			for (int i = 0; i < fragment.Prereqs.Count; i++)
			{
				if (BurikoMemory.Instance.GetFragmentReadStatus(fragment.Prereqs[i]) == 0)
				{
					result = false;
				}
			}
			return result;
		}

		public void GetValidFragments()
		{
			List<FragmentDataEntry> fragmentDataList = FragmentData.FragmentDataList;
			validFragmentData = new List<FragmentDataEntry>();
			BurikoMemory instance = BurikoMemory.Instance;
			for (int i = 0; i < fragmentDataList.Count; i++)
			{
				FragmentDataEntry fragmentDataEntry = fragmentDataList[i];
				bool flag = FufilledPrereqs(fragmentDataEntry);
				int fragmentValueStatus = BurikoMemory.Instance.GetFragmentValueStatus(fragmentDataEntry.Id);
				if (fragmentValueStatus == 0)
				{
					fragmentDataEntry.ViewType = FragmentViewType.Unviewed;
				}
				if (fragmentValueStatus == 1)
				{
					fragmentDataEntry.ViewType = FragmentViewType.Viewed;
				}
				if (fragmentValueStatus == 2)
				{
					if (flag)
					{
						fragmentDataEntry.ViewType = FragmentViewType.BrokenButFixable;
					}
					else
					{
						fragmentDataEntry.ViewType = FragmentViewType.Broken;
					}
				}
				if (fragmentDataEntry.Id < 51)
				{
					validFragmentData.Add(fragmentDataEntry);
				}
				if (fragmentDataEntry.Id == 51 && flag)
				{
					validFragmentData.Add(fragmentDataEntry);
				}
				if (fragmentDataEntry.Id == 52)
				{
					if (instance.GetGlobalFlag("GFlag_GameClear").BoolValue() && instance.GetFlag("LFragmentMiss").IntValue() == 0 && flag)
					{
						validFragmentData.Add(fragmentDataEntry);
					}
					else
					{
						Debug.Log("Skipped showing fagment 51 because: Clear status: " + instance.GetGlobalFlag("GFlag_GameClear").BoolValue() + " MissValue: " + instance.GetFlag("LFragmentMiss").IntValue() + " Prereq: " + flag);
					}
				}
			}
		}

		public void UpdateEntries()
		{
			for (int i = 0; i < FragmentEntries.Count; i++)
			{
				int num = i + page * 8;
				FragmentEntries[i].Reset();
				FragmentEntries[i].gameObject.name = "FragmentEntry" + i;
				if (num >= validFragmentData.Count)
				{
					FragmentEntries[i].gameObject.SetActive(value: false);
					continue;
				}
				FragmentEntries[i].gameObject.SetActive(value: true);
				FragmentEntries[i].Init(validFragmentData[num], this);
			}
			UpdatePage();
			ClearTitle();
		}

		public void Show()
		{
			LTDescr lTDescr = LeanTween.value(base.gameObject, SetFade, 0f, 1f, 0.8f);
			GameSystem.Instance.AudioController.PlayAudio("it_move2.ogg", Assets.Scripts.Core.Audio.AudioType.BGM, 0, 0.8f);
			if (GameSystem.Instance.UseEnglishText)
			{
				TextMeshProFont englishFont = GameSystem.Instance.MainUIController.GetEnglishFont();
				tipsTitleText.font = englishFont;
				tipsDescriptionText.font = englishFont;
				tipsDescriptionTextShadow.font = englishFont;
			}
			else
			{
				TextMeshProFont japaneseFont = GameSystem.Instance.MainUIController.GetJapaneseFont();
				tipsTitleText.font = japaneseFont;
				tipsDescriptionText.font = japaneseFont;
				tipsDescriptionTextShadow.font = japaneseFont;
			}
			GetValidFragments();
			page = BurikoMemory.Instance.GetFlag("LFragmentPage").IntValue();
			numPages = Mathf.CeilToInt((float)validFragmentData.Count / 8f) - 1;
			if (numPages < 0)
			{
				numPages = 0;
			}
			if (page > numPages)
			{
				page = numPages;
			}
			UpdateEntries();
			FragmentButtons.ForEach(delegate(FragmentUiButton a)
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
			FragmentButtons.ForEach(delegate(FragmentUiButton a)
			{
				a.Disable();
			});
			BurikoMemory.Instance.SetFlag("LFragmentPage", page);
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
