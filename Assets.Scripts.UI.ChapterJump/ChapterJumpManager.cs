using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MOD.Scripts.UI.ChapterJump;
using TMPro;

namespace Assets.Scripts.UI.ChapterJump
{
	public class ChapterJumpManager : MonoBehaviour
	{
		public UIPanel Panel;

		public Material TextMaterial;

		public List<ChapterJumpButton> JumpButtons;

		public GameObject GridObject;

		public bool isActive = true;

		private List<List<ChapterJumpButton>> allButtons;
		private int currentPage = 0;
		private int NumPages => allButtons.Count();
		private List<ChapterJumpButton> CurrentButtons => allButtons[currentPage];
		private UIButton PageLeft;
		private UIButton PageRight;
		private TextMeshPro PageNumberText;
		private ChapterJumpButton returnButton;

		private bool relayoutDone = false;
		private static readonly float windowStartPos = 0;
		private static readonly float windowEndPos = 440;
		/// <summary>
		/// The minimum space allowed between left and right columns
		/// </summary>
		private static readonly float minPadding = 10;
		/// <summary>
		/// The maximum space allowed between left and right columns
		/// </summary>
		private static readonly float maxPadding = 100;

		private static readonly float minYPos = -330;
		private static readonly float ySpacing = 30;

		private void PopulateJumpButtonList()
		{
			var childComponents = GetComponentsInChildren<ChapterJumpButton>();
			if (allButtons == null)
			{
				allButtons = new List<List<ChapterJumpButton>>
				{
					childComponents.Where( button => button.IsChapterButton ).ToList()
				};
			}
			JumpButtons = allButtons.SelectMany( x => x ).ToList();

			returnButton = childComponents.Where( button => button.name == "Return" ).First();

			/*
			foreach (var button in JumpButtons)
			{
				Debug.Log("Found button at " + button.gameObject.transform.localPosition.ToString()
					+ "\n\tEnglish: " + button.English
					+ "\n\tJapanese: " + button.Japanese
					+ "\n\tChapter Number: " + button.ChapterNumber
					+ "\n\tBlockName: " + button.BlockName);
			}
			*/
		}

		/// <summary>
		/// Switches pages
		/// Run with 0 movement if you change the button list to properly the buttons on the current page
		/// </summary>
		/// <param name="movement">The number of pages to move forward (negative will move backward)</param>
		private void UpdatePage(int movement)
		{
			currentPage = Math.Max(Math.Min(currentPage + movement, NumPages - 1), 0);
			if (PageLeft != null)
			{
				PageLeft.enabled = (currentPage != 0);
				PageRight.enabled = (currentPage != NumPages - 1);
				PageNumberText.text = (currentPage + 1) + "/" + NumPages;
			}
			JumpButtons.ForEach( button => button.gameObject.SetActive(false) );
			CurrentButtons.ForEach( button => button.gameObject.SetActive(true) );
			RelayoutButtons();
		}

		/// <summary>
		/// Steals page button objects from the tips screen and sets them up here
		/// </summary>
		private void SetupPageButtons()
		{
			if (NumPages <= 1) { return; }
			var gameSystem = GameSystem.Instance;
			var tipsManager = gameSystem.TipsPrefab.GetComponent<Tips.TipsManager>();

			// Steal buttons from tips manager
			var leftButton = Instantiate(tipsManager.PageLeft.gameObject);
			leftButton.transform.SetParent(returnButton.gameObject.transform.parent, worldPositionStays: false);
			PageLeft = leftButton.GetComponent<UIButton>();
			var rightButton = Instantiate(tipsManager.PageRight.gameObject);
			rightButton.transform.SetParent(returnButton.gameObject.transform.parent, worldPositionStays: false);
			PageRight = rightButton.GetComponent<UIButton>();
			var pageNumberText = Instantiate(returnButton.gameObject);
			Destroy(pageNumberText.GetComponent<ChapterJumpButton>());
			pageNumberText.transform.SetParent(returnButton.gameObject.transform.parent, worldPositionStays: false);
			PageNumberText = pageNumberText.GetComponent<TextMeshPro>();

			// Setup page text
			var returnButtonPos = returnButton.transform.localPosition;
			var returnButtonBounds = returnButton.Text.bounds;
			PageNumberText.fontSize = returnButton.Text.fontSize;
			PageNumberText.alignment = TextAlignmentOptions.Center;
			PageNumberText.transform.localPosition = new Vector3(returnButtonPos.x + returnButtonBounds.center.x, returnButtonPos.y + 25, 0f);

			// Setup buttons
			PageLeft.transform.localPosition = new Vector3(20 + returnButtonPos.x + returnButtonBounds.min.x, returnButtonPos.y + 25, 0f);
			PageRight.transform.localPosition = new Vector3(-20 + returnButtonPos.x + returnButtonBounds.max.x, returnButtonPos.y + 25, 0f);
			PageLeft.transform.localScale = new Vector3(0.5f, 0.5f, 0);
			PageRight.transform.localScale = new Vector3(0.5f, 0.5f, 0);
			// With pixelSnap on, UIButton keeps resizing the buttons to have a 1:1 ratio with the sprite image
			// which undoes the local scale we just set
			PageLeft.pixelSnap = false;
			PageRight.pixelSnap = false;

			leftButton.GetComponent<Tips.TipsButton>().Setup( () => UpdatePage(movement: -1) );
			rightButton.GetComponent<Tips.TipsButton>().Setup( () => UpdatePage(movement: 1) );
			UpdatePage(movement: 0);
		}

		private void LoadJSONButtons()
		{
			if (MODChapterJumpController.ChapterJumpsOrNull != null)
			{
				var newList = new List<List<ChapterJumpButton>>();
				var baseButton = JumpButtons[0];

				// Copied from Onikakushi
				// Himatsubushi has less options so they moved the object down further, which messes up positioning
				baseButton.transform.parent.localPosition = new Vector3(-215.0f, 223.0f, 0.0f);

				var position = new Vector3(0, 0, 0);
				List<ChapterJumpButton> newButtons = new List<ChapterJumpButton>();
				foreach (var entry in MODChapterJumpController.ChapterJumpsOrNull)
				{

					var newButtonObject = Instantiate(baseButton.gameObject);
					newButtonObject.transform.SetParent(baseButton.gameObject.transform.parent, worldPositionStays: false);
					newButtonObject.transform.localPosition = position;
					ChapterJumpButton newButton = newButtonObject.GetComponent<ChapterJumpButton>();

					newButton.UpdateFromChapterJumpEntry(entry);
					if (!newButton.isActiveAndEnabled)
					{
						Destroy(newButton);
						continue;
					}

					newButtons.Add(newButton);

					position.y -= ySpacing;
					if (position.y < minYPos)
					{
						if (position.x > 0)
						{
							position = new Vector3(0, 0, 0);
							newList.Add(newButtons);
							newButtons = new List<ChapterJumpButton>();
						}
						else
						{
							position.y = 0;
							position.x += 200; // Will be set properly by RelayoutButtons
						}
					}
				}
				newList.Add(newButtons);
				foreach (var oldList in allButtons)
				{
					foreach (var old in oldList)
					{
						Destroy(old.gameObject);
					}
				}
				allButtons = newList;
				PopulateJumpButtonList();
				UpdatePage(movement: 0);
			}
		}

		// Rearranges buttons to fit on screen nicely
		private void RelayoutButtons()
		{
			// Every single game seems to have this in a slightly different position
			var windowPos = CurrentButtons[0].transform.parent.localPosition;
			windowPos.x = -215; // What Onikakushi uses
			CurrentButtons[0].transform.parent.localPosition = windowPos;

			var activeButtons = CurrentButtons.Where( button => button.isActiveAndEnabled ).ToList();
			activeButtons.ForEach( button => button.Text.ForceMeshUpdate() );

			// Things with x values below this are considered to be on the left half, others are on the right
			var cutoff = (windowEndPos - windowStartPos) / 4 + windowStartPos;

			// Split buttons into left and right half
			var leftButtons = activeButtons.Where( button => button.transform.localPosition.x < cutoff ).ToList();
			var rightButtons = activeButtons.Where( button => button.transform.localPosition.x >= cutoff ).ToList();

			// Find how many items are active on the right side
			// (This will allows overlap of right side buttons if they're disabled)
			// Buttons on top are at y=0, buttons below have negative y
			float lowestRightButton = rightButtons.Select( button => button.transform.localPosition.y ).DefaultIfEmpty().Min();

			// Calculate the maximum sizes of the buttons that actually could overlap
			float leftSize = leftButtons
				// Only take into account buttons that have another button next to them
				.Where( button => button.transform.localPosition.y >= lowestRightButton )
				.Select( button => button.Text.bounds.size.x )
				.DefaultIfEmpty()
				.Max();
			float rightSize = rightButtons.Select( button => button.Text.bounds.size.x ).DefaultIfEmpty().Max();

			// Left-aligns left buttons and right-aligns the longest right button
			float leftPos = windowStartPos, rightPos = Mathf.Min(windowEndPos - rightSize, leftPos + leftSize + maxPadding);

			// If that causes the left and right buttons to overlap, push them apart
			if (leftPos + leftSize + minPadding > rightPos)
			{
				float amountOver = (leftPos + leftSize + minPadding) - rightPos;
				// If they're way over, it looks better to center everything
				if (amountOver > 70)
				{
					leftPos -= (amountOver / 2);
				}
				// It doesn't look to great when the starts of the left side are just 
				// barely in the red strip, so in this case just push everything
				// as close to the red strip as possible, which is about 5px
				else
				{
					leftPos -= Mathf.Min(5, amountOver / 2);
				}
				rightPos = leftPos + leftSize + minPadding;
			}

			// Set the new positions into the buttons
			foreach (var button in CurrentButtons)
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
			JumpButtons.ForEach( button => button.Disable() );
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
			// Something else keeps rearranging the buttons after `Show` is called
			// Not sure what, but if we adjust them in the first update instead everything goes fine
			if (!relayoutDone)
			{
				relayoutDone = true;
				RelayoutButtons();
				LoadJSONButtons();
				SetupPageButtons();
			}
		}
	}
}
