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

		public GameObject GridObject;

		public bool isActive = true;

		private bool relayoutDone = false;
		private static readonly float windowStartPos = 0;
		private static readonly float windowEndPos = 440;
		private static readonly float minPadding = 10;

		private void PopulateJumpButtonList() 
		{
			JumpButtons.Clear();
			foreach (var button in GetComponentsInChildren<ChapterJumpButton>())
			{
				if (button.IsChapterButton)
				{
					JumpButtons.Add(button);
				}
			}
		}

		// Rearranges buttons to fit on screen nicely
		private void RelayoutButtons()
		{
			// Every single game seems to have this in a slightly different position
			var windowPos = JumpButtons[0].transform.parent.localPosition;
			windowPos.x = -215;
			JumpButtons[0].transform.parent.localPosition = windowPos;

			var cutoff = (windowEndPos - windowStartPos) / 4 + windowStartPos;
			float lowestRightButton = 0; // Buttons on top are at y=0, buttons below have negative y
			// Find how many items are active on the right side
			// (This will allows overlap of right side buttons if they're disabled)
			foreach (var button in JumpButtons)
			{
				if (button.isActiveAndEnabled)
				{
					button.Text.ForceMeshUpdate();
					if (button.transform.localPosition.x >= cutoff)
					{
						lowestRightButton = Mathf.Min(lowestRightButton, button.transform.localPosition.y);
					}
				}
			}
			// Calculate the maximum sizes of the buttons that actually could overlap
			float leftSize = 0, rightSize = 0;
			foreach (var button in JumpButtons)
			{
				if (button.isActiveAndEnabled)
				{
					var size = button.Text.bounds.size.x;
					if (button.transform.localPosition.x < cutoff)
					{
						// Only take into account buttons that have another button next to them
						if (button.transform.localPosition.y >= lowestRightButton)
						{
							leftSize = Mathf.Max(size, leftSize);
						}
					}
					else
					{
						rightSize = Mathf.Max(size, rightSize);
					}
				}
			}
			// Left-aligns left buttons and right-aligns the longest right button
			float leftPos = windowStartPos, rightPos = windowEndPos - rightSize;
			// If that causes the left and right buttons to overlap, push them apart (centered)
			if (leftPos + leftSize + minPadding > rightPos)
			{
				float amountOver = (leftPos + leftSize + minPadding) - rightPos;
				if (amountOver > 70)
				{
					leftPos -= (amountOver / 2);
				}
				else
				{
					leftPos -= Mathf.Min(5, amountOver / 2);
				}
				rightPos = leftPos + leftSize + minPadding;
			}
			// Set the new positions into the buttons
			foreach (var button in JumpButtons)
			{
				Vector3 posVector = button.transform.localPosition;
				var pos = posVector.x;
				if (pos < cutoff)
				{
					posVector.x = leftPos;
				}
				else
				{
					posVector.x = rightPos;
				}
				button.Text.gameObject.transform.localPosition = posVector;
				BoxCollider hitbox = button.gameObject.GetComponent<BoxCollider>();
				hitbox.size = button.Text.bounds.size;
				hitbox.center = button.Text.bounds.center;
			}
		}

		public void Show()
		{
			PopulateJumpButtonList();
			float fontSize = GameSystem.Instance.ChapterJumpFontSize;
			if (fontSize > 0)
			{
				foreach (var button in JumpButtons)
				{
					button.SetFontSize(fontSize);
				}
			}
			RelayoutButtons();
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

		private void Update()
		{
			if (!relayoutDone)
			{
				relayoutDone = true;
				RelayoutButtons();
			}
		}
	}
}
