using UnityEngine;
using System;

namespace Assets.Scripts.UI.Choice
{
	public class TextSpawner
	{
		public GameObject SpawnText(string text)
		{
			GameObject primaryUIPanelGameObject = GameObject.FindGameObjectWithTag("PrimaryUIPanel");

			GameObject choiceButtonGameObjectClone = UnityEngine.Object.Instantiate(Resources.Load("ChoiceButton")) as GameObject;
			if (choiceButtonGameObjectClone == null)
			{
				throw new Exception("Failed to instantiate ChoiceButton!");
			}
			choiceButtonGameObjectClone.transform.parent = primaryUIPanelGameObject.transform;
			choiceButtonGameObjectClone.transform.localScale = Vector3.one;

			choiceButtonGameObjectClone.transform.localPosition = new Vector3(0,0,0);
			ChoiceButton component = choiceButtonGameObjectClone.GetComponent<ChoiceButton>();
			component.ChangeText(text);

			return choiceButtonGameObjectClone;
		}
	}
}
