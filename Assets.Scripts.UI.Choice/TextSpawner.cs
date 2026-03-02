using UnityEngine;
using System;

namespace Assets.Scripts.UI.Choice
{
	public class TextSpawner
	{
		public ChoiceButton SpawnText(string text)
		{
			GameObject primaryUIPanelGameObject = GameObject.FindGameObjectWithTag("PrimaryUIPanel");

			GameObject choiceButtonGameObjectClone = UnityEngine.Object.Instantiate(Resources.Load("ChoiceButton")) as GameObject;
			if (choiceButtonGameObjectClone == null)
			{
				throw new Exception("Failed to instantiate ChoiceButton!");
			}

			Debug.Log($"Name of thing: {primaryUIPanelGameObject.transform.parent.parent.gameObject.name}");

			choiceButtonGameObjectClone.transform.parent = primaryUIPanelGameObject.transform.parent;
			choiceButtonGameObjectClone.transform.localScale = Vector3.one;

			ChoiceButton cb = choiceButtonGameObjectClone.GetComponent<ChoiceButton>();

			cb.ChangeText(text);

			//choiceButtonGameObjectClone.SetActive(true);
			// Had some issue with center alignment earlier, but works now?
			// Maybe calling CalculateBounds fixes it?
			cb.ButtonTextMesh.alignment = TMPro.TextAlignmentOptions.Center;

			//cb.ButtonTextMesh.mesh.bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(200, 200, 0));
			cb.ButtonTextMesh.ForceMeshUpdate();
			cb.ButtonTextMesh.UpdateMeshPadding();


			cb.ButtonTextMesh.fontSize = 15;

			// 0, 0, 0 is center of screen
			// When screen is 16:9, right hand border = 640, left hand border = -640 
			// When screen is 4:3, right hand border = 480, left hand border = -480 
			// Top of screen is y = 384, bottom is y = -384
			choiceButtonGameObjectClone.transform.localPosition = new Vector3(640, 384, 0);

			// Might need to use this instead of using TMP's built in alignment?
			Bounds meshBounds = cb.ButtonTextMesh.bounds;
			Debug.Log($"SpawnText MeshBounds: {meshBounds}");

			//cb.ButtonTextMesh.transform.position = new Vector3(-meshBounds.size.x / 2, 0, 0);

			//component.ButtonTextMesh.();


			//component.but

			return cb;
		}
	}
}
