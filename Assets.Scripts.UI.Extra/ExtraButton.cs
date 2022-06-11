using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
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
			if (!isActive || UICamera.currentTouchID < -1 || GameSystem.Instance.GameState != GameState.ExtraScreen)
			{
				return;
			}
			StateExtraScreen stateExtraScreen = GameSystem.Instance.GetStateObject() as StateExtraScreen;
			if (stateExtraScreen == null)
			{
				return;
			}
			string name = base.name;
			if (!(name == "CastReview"))
			{
				if (!(name == "StaffRoom"))
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
					BurikoScriptSystem.Instance.CallScript("staffroom");
				}
			}
			else
			{
				stateExtraScreen.RequestLeave();
				BurikoScriptSystem.Instance.CallScript("omake");
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
			if (base.name == "StaffRoom" && (!BurikoMemory.Instance.GetGlobalFlag("GCastReview").BoolValue() || !BurikoMemory.Instance.GetGlobalFlag("GFlag_GameClear").BoolValue()))
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
