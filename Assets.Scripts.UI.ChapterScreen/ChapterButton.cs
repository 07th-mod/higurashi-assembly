using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using UnityEngine;

namespace Assets.Scripts.UI.ChapterScreen
{
	public class ChapterButton : MonoBehaviour
	{
		public Material normalMaterial;

		public Material hoverMaterial;

		public MeshRenderer TextMesh;

		private UIButton button;

		private bool isActive = true;

		public void Disable()
		{
			isActive = false;
			TextMesh.material = normalMaterial;
		}

		private void OnClick()
		{
			if (!isActive || UICamera.currentTouchID < -1 || GameSystem.Instance.GameState != GameState.ChapterScreen)
			{
				return;
			}
			StateChapterScreen stateChapterScreen = GameSystem.Instance.GetStateObject() as StateChapterScreen;
			if (stateChapterScreen == null)
			{
				return;
			}
			string name = base.name;
			switch (name)
			{
			case "NewTips":
				stateChapterScreen.RequestLeave();
				BurikoMemory.Instance.SetFlag("TipsMode", 4);
				BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
				return;
			case "ViewAllTips":
				stateChapterScreen.RequestLeave();
				BurikoMemory.Instance.SetFlag("TipsMode", 3);
				BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
				return;
			case "SaveLoad":
				GameSystem.Instance.PushStateObject(new StateSaveLoad(restoreUI: false));
				return;
			case "Fragments":
				stateChapterScreen.RequestLeave();
				BurikoMemory.Instance.SetFlag("TipsMode", 1);
				return;
			}
			if (name == "Continue")
			{
				stateChapterScreen.RequestLeave();
				BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 0);
				AudioController.Instance.ClearTempAudio();
			}
		}

		private void OnHover(bool isOver)
		{
			if (isActive)
			{
				if (isOver && GameSystem.Instance.GameState == GameState.ChapterScreen)
				{
					TextMesh.material = hoverMaterial;
				}
				else
				{
					TextMesh.material = normalMaterial;
				}
			}
		}

		private void Awake()
		{
			button = GetComponent<UIButton>();
		}

		private void LateUpdate()
		{
			button.isEnabled = (GameSystem.Instance.GameState == GameState.ChapterScreen);
		}
	}
}
