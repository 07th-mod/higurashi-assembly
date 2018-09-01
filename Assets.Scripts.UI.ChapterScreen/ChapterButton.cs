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
			if (isActive && UICamera.currentTouchID >= -1 && GameSystem.Instance.GameState == GameState.ChapterScreen)
			{
				StateChapterScreen stateChapterScreen = GameSystem.Instance.GetStateObject() as StateChapterScreen;
				if (stateChapterScreen != null)
				{
					string name = base.name;
					if (name != null)
					{
						if (!(name == "NewTips"))
						{
							if (!(name == "ViewAllTips"))
							{
								if (!(name == "SaveLoad"))
								{
									if (name == "Continue")
									{
										stateChapterScreen.RequestLeave();
										BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 0);
										AudioController.Instance.ClearTempAudio();
									}
								}
								else
								{
									GameSystem.Instance.PushStateObject(new StateSaveLoad(restoreUI: false));
								}
							}
							else
							{
								stateChapterScreen.RequestLeave();
								BurikoMemory.Instance.SetFlag("TipsMode", 3);
								BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
							}
						}
						else
						{
							stateChapterScreen.RequestLeave();
							BurikoMemory.Instance.SetFlag("TipsMode", 4);
							BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
						}
					}
				}
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
