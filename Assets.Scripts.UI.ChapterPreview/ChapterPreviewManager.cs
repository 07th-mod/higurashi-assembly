using Assets.Scripts.Core.Buriko;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.ChapterPreview
{
	internal class ChapterPreviewManager : MonoBehaviour
	{
		public UIPanel Panel;

		public List<ChapterPreviewButton> ChapterPreviewButtons;

		public ChapterPreviewButton ExitButton;

		public List<UITexture> ChapterFlashes;

		private bool isIntro = true;

		private bool[] activeChapters = new bool[4];

		private bool[] visibleChapters = new bool[4];

		private float delayTime = 9999999f;

		public void Show()
		{
			Panel.alpha = 0f;
			LTDescr lTDescr = LeanTween.value(base.gameObject, delegate(float f)
			{
				Panel.alpha = f;
			}, 0f, 1f, 0.2f);
			lTDescr.onComplete = (Action)Delegate.Combine(lTDescr.onComplete, new Action(ShowAnimation));
			if (ChapterPreviewButtons.Count >= 3)
			{
				ChapterPreviewButtons.ForEach(delegate(ChapterPreviewButton a)
				{
					a.Disable();
				});
				int num = BurikoMemory.Instance.GetGlobalFlag("HIGUEND").IntValue();
				int num2 = BurikoMemory.Instance.GetGlobalFlag("TEIEND").IntValue();
				BurikoMemory.Instance.GetGlobalFlag("MEHEND").IntValue();
				for (int i = 0; i < ChapterPreviewButtons.Count; i++)
				{
					ChapterPreviewButtons[i].Disable();
					ChapterPreviewButtons[i].Sprite.color = new Color(1f, 1f, 1f, 0f);
				}
				ExitButton.Disable();
				ExitButton.Sprite.color = new Color(1f, 1f, 1f, 0f);
				activeChapters[0] = true;
				activeChapters[2] = true;
				visibleChapters[0] = true;
				visibleChapters[1] = true;
				visibleChapters[2] = true;
				if (num < 11)
				{
					ChapterPreviewButtons[1].BlockChapter();
				}
				else
				{
					activeChapters[1] = true;
				}
				if (num >= 20 && num2 >= 10)
				{
					activeChapters[3] = true;
					visibleChapters[3] = true;
				}
				else
				{
					ChapterPreviewButtons[3].gameObject.SetActive(value: false);
				}
			}
		}

		public void ShowAnimation()
		{
			float num = 0f;
			for (int i = 0; i < visibleChapters.Length; i++)
			{
				if (visibleChapters[i])
				{
					UISprite sprite = ChapterPreviewButtons[i].Sprite;
					sprite.color = new Color(1f, 1f, 1f, 0f);
					LeanTween.value(base.gameObject, delegate(float f)
					{
						sprite.color = new Color(1f, 1f, 1f, f);
					}, 0f, 1f, 0.4f).delay = num;
					num += 0.4f;
				}
			}
			for (int j = 0; j < visibleChapters.Length; j++)
			{
				if (activeChapters[j])
				{
					UITexture sprite2 = ChapterFlashes[j];
					sprite2.color = new Color(1f, 1f, 1f, 0f);
					LTDescr lTDescr = LeanTween.value(base.gameObject, delegate(float f)
					{
						sprite2.color = new Color(1f, 1f, 1f, f);
					}, 0f, 1f, 0.6f);
					lTDescr.delay = num;
					lTDescr.onComplete = (Action)Delegate.Combine(lTDescr.onComplete, (Action)delegate
					{
						LeanTween.value(base.gameObject, delegate(float f)
						{
							sprite2.color = new Color(1f, 1f, 1f, f);
						}, 1f, 0f, 0.6f).delay = 0.2f;
					});
				}
			}
			num += 1.4f;
			UISprite exit = ExitButton.Sprite;
			exit.color = new Color(1f, 1f, 1f, 0f);
			LeanTween.value(base.gameObject, delegate(float f)
			{
				exit.color = new Color(1f, 1f, 1f, f);
			}, 0f, 1f, 0.8f).delay = num;
			num = (delayTime = num + 0.8f);
		}

		public void FinishAnimation()
		{
			for (int i = 0; i < ChapterPreviewButtons.Count; i++)
			{
				if (visibleChapters[i])
				{
					EnableButton(ChapterPreviewButtons[i]);
				}
				ChapterFlashes[i].gameObject.SetActive(value: false);
			}
			EnableButton(ExitButton);
			isIntro = false;
		}

		private void EnableButton(ChapterPreviewButton button)
		{
			button.Enable();
			button.Button.defaultColor = new Color(1f, 1f, 1f, 1f);
			button.Button.disabledColor = new Color(1f, 1f, 1f, 1f);
		}

		public void Update()
		{
			delayTime -= Time.deltaTime;
			if (isIntro && delayTime < 0f)
			{
				FinishAnimation();
			}
		}

		public void Hide(Action onFinish)
		{
			LTDescr lTDescr = LeanTween.value(base.gameObject, delegate(float f)
			{
				Panel.alpha = f;
			}, 1f, 0f, 0.8f);
			lTDescr.onComplete = (Action)Delegate.Combine(lTDescr.onComplete, (Action)delegate
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
