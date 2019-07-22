using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using TMPro;
using UnityEngine;
using static MOD.Scripts.UI.ChapterJump.MODChapterJumpController;

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

		private string BlockName;

		private int ArcNumber = 0;
		private string FileName = null;

		private TextMeshPro _text = null;
		public TextMeshPro Text
		{
			get
			{
				if (_text == null)
				{
					_text = GetComponent<TextMeshPro>();
				}
				return _text;
			}
		}

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
						if (FileName != null)
						{
							BurikoScriptSystem.Instance.JumpToScript(scriptname: FileName, blockname: BlockName ?? "Game");
						}
						else
						{
							BurikoScriptSystem.Instance.JumpToBlock(BlockName ?? "Game");
						}
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
			Text.text = GameSystem.Instance.ChooseJapaneseEnglish(japanese: Japanese, english: English);
			Text.ForceMeshUpdate();
			if (!(base.name == "Return")
			    && !BurikoMemory.Instance.GetGlobalFlag("GFlag_GameClear").BoolValue()
			    && BurikoMemory.Instance.GetHighestChapterFlag(ArcNumber).IntValue() < ChapterNumber)
			{
				base.gameObject.SetActive(false);
			}
		}

		public void UpdateTextAndActive()
		{
			Start();
		}

		public bool IsChapterButton => name != "Return";

		public void SetFontSize(float size)
		{
			Text.fontSize = size;
		}

		private void LateUpdate()
		{
		}

		public void UpdateFromChapterJumpEntry(ChapterJumpEntry entry)
		{
			English = entry.English;
			Japanese = entry.Japanese;
			ChapterNumber = entry.ChapterNumber;
			BlockName = entry.BlockName;
			FileName = entry.FileName;
			ArcNumber = entry.ArcNumber;
			UpdateTextAndActive();
		}
	}
}
