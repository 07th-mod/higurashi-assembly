using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using Assets.Scripts.UI.Tips;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI.Extra
{
	public class ExtraButton : MonoBehaviour
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
			if (isActive && UICamera.currentTouchID >= -1 && GameSystem.Instance.GameState == GameState.ExtraScreen)
			{
				StateExtraScreen stateExtraScreen = GameSystem.Instance.GetStateObject() as StateExtraScreen;
				if (stateExtraScreen != null)
				{
					string name = base.name;
					if (name != null)
					{
						if (!(name == "CastReview"))
						{
							if (!(name == "ChapterJump"))
							{
								if (!(name == "ViewTips"))
								{
									if (name == "Continue")
									{
										stateExtraScreen.RequestLeave();
										BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 0);
										AudioController.Instance.ClearTempAudio();
										AudioController.Instance.FadeOutBGM(0, 1000, waitForFade: false);
										BurikoScriptSystem.Instance.JumpToBlock("Title");
									}
								}
								else
								{
									stateExtraScreen.RequestLeave();
									BurikoMemory.Instance.SetFlag("TipsMode", 5);
									BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
								}
							}
							else
							{
								stateExtraScreen.RequestLeave();
								GameSystem.Instance.AddWait(new Wait(0.5f, WaitTypes.WaitForTime, delegate
								{
									GameSystem.Instance.PushStateObject(new StateChapterJump());
								}));
							}
						}
						else
						{
							stateExtraScreen.RequestLeave();
							BurikoScriptSystem.Instance.CallScript("staffroom");
						}
					}
				}
			}
		}

		private void OnHover(bool isOver)
		{
			if (isActive)
			{
				if (isOver && GameSystem.Instance.GameState == GameState.ExtraScreen)
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
			if (base.name == "CastReview" && !BurikoMemory.Instance.GetGlobalFlag("GFlag_GameClear").BoolValue())
			{
				base.gameObject.SetActive(value: false);
			}
			if (base.name == "ViewTips" && !TipsData.GetVisibleTips(false, true).Tips.Any())
			{
				base.gameObject.SetActive(value: false);
			}
		}

		private void LateUpdate()
		{
			button.isEnabled = (GameSystem.Instance.GameState == GameState.ExtraScreen);
		}
	}
}
