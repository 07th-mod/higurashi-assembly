using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.ChapterJump
{
	public class ChapterJumpButton : MonoBehaviour
	{
		public Material normalMaterial;

		public Material hoverMaterial;

		public MeshRenderer TextMesh;

		public string English;

		public string Japanese;

		public int ChapterNumber;

		private bool isActive = true;

		public void Disable()
		{
			isActive = false;
			TextMesh.material = normalMaterial;
		}

		private void OnClick()
		{
			if (isActive && UICamera.currentTouchID >= -1 && GameSystem.Instance.GameState == GameState.ChapterJumpScreen)
			{
				StateChapterJump stateChapterJump = GameSystem.Instance.GetStateObject() as StateChapterJump;
				if (stateChapterJump != null)
				{
					stateChapterJump.RequestLeave();
					if (!(base.name == "Return"))
					{
						BurikoMemory.Instance.SetFlag("s_jump", ChapterNumber);
						Debug.Log("Setting chapter to " + ChapterNumber);
						BurikoScriptSystem.Instance.JumpToBlock("Game");
					}
				}
			}
		}

		private void OnHover(bool isOver)
		{
			if (isActive)
			{
				if (isOver && GameSystem.Instance.GameState == GameState.ChapterJumpScreen)
				{
					TextMesh.material = hoverMaterial;
				}
				else
				{
					TextMesh.material = normalMaterial;
				}
			}
		}

		private void Start()
		{
			TextMeshPro component = GetComponent<TextMeshPro>();
			component.text = ((!GameSystem.Instance.UseEnglishText) ? Japanese : English);
			if (!(base.name == "Return"))
			{
				if (BurikoMemory.Instance.GetGlobalFlag("GFlag_GameClear").BoolValue())
				{
					Debug.Log("GameClear, Unlocking");
				}
				else if (BurikoMemory.Instance.GetGlobalFlag("GHighestChapter").IntValue() < ChapterNumber)
				{
					base.gameObject.SetActive(value: false);
				}
			}
		}

		private void LateUpdate()
		{
		}
	}
}
