using Assets.Scripts.Core;
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
			GameObject gameObject = GameObject.FindGameObjectWithTag("PrimaryUIPanel");
			int num = Mathf.RoundToInt(120f / (float)(count - 1));
			int num2 = 0;
			while (true)
			{
				if (num2 >= count)
				{
					return;
				}
				int id = num2;
				GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("ChoiceButton")) as GameObject;
				if (gameObject2 == null)
				{
					break;
				}
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.transform.localScale = Vector3.one;
				gameObject2.transform.localPosition = new Vector3(GameSystem.Instance.GetGUIOffset(), (float)(170 - num * num2), 0f);
				ChoiceButton component = gameObject2.GetComponent<ChoiceButton>();
				component.ChangeText(optstrings[num2]);
				component.SetCallback(this, delegate
				{
					GameSystem.Instance.ScriptSystem.SetFlag("SelectResult", id);
					GameSystem.Instance.ScriptSystem.SetFlag("LOCALWORK_NO_RESULT", id);
					Debug.Log("ID: " + id);
					FinishChoice();
				});
				options.Add(gameObject2.GetComponent<ChoiceButton>());
				num2++;
			}
			throw new Exception("Failed to instantiate ChoiceButton!");
		}
	}
}
