using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Choice
{
	public class ChoiceController
	{
		private List<ChoiceButton> options = new List<ChoiceButton>();

		public void Destroy()
		{
			foreach (ChoiceButton option in options)
			{
				UnityEngine.Object.Destroy(option.gameObject);
			}
		}

		private void FinishChoice()
		{
			foreach (ChoiceButton option in options)
			{
				option.DisableButton();
			}
			GameSystem.Instance.LeaveChoices();
		}

		// ------ Note about spawned object using this method ------
		//
		// The final text will be placed in the center of the 4:3 window (when using TextAlignmentOptions.Center), or top left (when using TextAlignmentOptions.TopLeft)
		// when the local position and position are set 0, and has no parent
		// The unmodded screen height is 2.0, unmodded screen width is 2.0 * 4 / 3, full 16:9 modded screen width is 2.0 * 16/9
		// The offset when using top left mode is (moddedScreenWidth - unmoddedScreenWidth) / 2;
		// This is similar to the previous coordinate system but everything is divided by 384
		// The GUIOffset is not necessary when using centered text (TextAlignmentOptions.Center), so we remove it in the Create() function
		//
		// Note 2: previously I used UnityEngine.Object.Instantiate(gs.LoadingBox, gs.LoadingBox.transform.parent, instantiateInWorldSpace: false);
		// but could get similar results by dviding the font size by 384f and spawning in world space
		public static GameObject DuplicateTextObject()
		{
			GameObject loadingBox2 = UnityEngine.Object.Instantiate(GameSystem.Instance.LoadingBox);

			TextMeshPro tmp = loadingBox2.GetComponent<TextMeshPro>();
			tmp.alignment = TextAlignmentOptions.Center;
			tmp.font = GameSystem.Instance.MainUIController.GetCurrentFont();
			tmp.fontSize = 52/384f;
			tmp.text = "Hello World";

			loadingBox2.transform.localPosition = new Vector3(0f, 0f, 0f);
			loadingBox2.transform.position = new Vector3(0f, 0f, 0f);

			loadingBox2.SetActive(true);

			return loadingBox2;
		}

		public void Create(List<string> optstrings, int count)
		{
			for (int i = 0; i < count; i++)
			{
				int id = i;
				GameObject gameObject2 = DuplicateTextObject();
				if (gameObject2 == null)
				{
					throw new Exception("Failed to instantiate ChoiceButton!");
				}

				if (count > 8)
				{
					float x;
					if (i == count - 1 && count % 2 == 1)
					{
						x = GameSystem.Instance.GetGUIOffset();
					}
					else if (i % 2 == 0)
					{
						x = GameSystem.Instance.GetGUIOffset() - 300f;
					}
					else
					{
						x = GameSystem.Instance.GetGUIOffset() + 300f;
					}
					gameObject2.transform.localPosition = new Vector3(x, (float)(-75 * (i / 2) + 27 * count - 50), 0f);
				}
				else
				{
					gameObject2.transform.localPosition = new Vector3(GameSystem.Instance.GetGUIOffset(), (float)(-75 * i + 27 * count + 50), 0f);
				}

				// Make some adjustments to the above calculations in Rei due to using a different coordinate system than the previous chapters
				gameObject2.transform.localPosition -= new Vector3(GameSystem.Instance.GetGUIOffset(), 0, 0);
				gameObject2.transform.localPosition /= (384.0f);

				// Add a collider hitbox and resize it to match the text size (copied from HistoryButton)
				TextMeshPro tmp = gameObject2.GetComponent<TextMeshPro>();
				BoxCollider hitbox = gameObject2.AddComponent<BoxCollider>();

				// For Rei, since we're not using a prefab ChoiceButton, we need to fill in the ButtonTextMesh ourselves or it will be null
				ChoiceButton component = gameObject2.AddComponent<ChoiceButton>();
				component.ButtonTextMesh = tmp;

				component.ChangeText(optstrings[i]);

				// Force mesh update to update TextMeshPro bounds...I think
				tmp.ForceMeshUpdate();

				// Set hitbox size *after* changing text, so the hitbox matches the text bounds
				hitbox.size = tmp.bounds.size;
				hitbox.center = tmp.bounds.center;

				if (BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode43Aspect").IntValue() != 0)
				{
					component.SetFontSize(component.GetFontSize() * .75f);
				}

				component.SetCallback(this, delegate
				{
					GameSystem.Instance.ScriptSystem.SetFlag("SelectResult", id);
					GameSystem.Instance.ScriptSystem.SetFlag("LOCALWORK_NO_RESULT", id);
					Debug.Log("ID: " + id);
					this.FinishChoice();
				});
				this.options.Add(gameObject2.GetComponent<ChoiceButton>());
			}
		}
	}
}
