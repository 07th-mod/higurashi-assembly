using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using System;
using System.Collections.Generic;
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

		public void Create(List<string> optstrings, int count)
		{
			// The text X offset is only necessary when in 16:9 mode - when in 4:3 mode just use an offset of 0
			float GUIXOffset = GameSystem.Instance.GetGUIOffset();
			if(BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode").IntValue() == 1)
			{
				GUIXOffset = 0;
			}

			GameObject gameObject = GameObject.FindGameObjectWithTag("PrimaryUIPanel");
			for (int i = 0; i < count; i++)
			{
				int id = i;
				GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("ChoiceButton")) as GameObject;
				if (gameObject2 == null)
				{
					throw new Exception("Failed to instantiate ChoiceButton!");
				}
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.transform.localScale = Vector3.one;
				if (count > 8)
				{
					float x;
					if (i == count - 1 && count % 2 == 1)
					{
						x = GUIXOffset;
					}
					else if (i % 2 == 0)
					{
						x = GUIXOffset - 300f;
					}
					else
					{
						x = GUIXOffset + 300f;
					}
					gameObject2.transform.localPosition = new Vector3(x, (float)(-75 * (i / 2) + 27 * count - 50), 0f);
				}
				else
				{
					gameObject2.transform.localPosition = new Vector3(GUIXOffset, (float)(-75 * i + 27 * count + 50), 0f);
				}
				ChoiceButton component = gameObject2.GetComponent<ChoiceButton>();
				component.ChangeText(optstrings[i]);
				if (BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode43Aspect").IntValue() != 0)
				{
					component.SetFontSize(component.GetFontSize() * .75f);
				}
				component.SetCallback(this, delegate
				{
					GameSystem.Instance.ScriptSystem.SetFlag("SelectResult", id);
					Debug.Log("ID: " + id);
					this.FinishChoice();
				});
				this.options.Add(gameObject2.GetComponent<ChoiceButton>());
			}
		}
	}
}
