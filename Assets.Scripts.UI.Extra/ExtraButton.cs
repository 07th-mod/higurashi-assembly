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

		public string ExtraInfo;

		private UIButton button;

		private bool isActive;

		private bool isOver;

		public void Disable()
		{
			isActive = false;
			TextMesh.material = normalMaterial;
		}

		public void Enable()
		{
			if (base.gameObject.activeInHierarchy)
			{
				isActive = true;
				if (isOver)
				{
					OnHover(isOver: true);
				}
			}
		}

		private void OnClick()
		{
			if (!isActive || UICamera.currentTouchID < -1 || (GameSystem.Instance.GameState != GameState.ExtraScreen && GameSystem.Instance.GameState != GameState.OmakeSection))
			{
				return;
			}
			IGameState stateObject = GameSystem.Instance.GetStateObject();
			if (stateObject != null)
			{
				GameSystem.Instance.AudioController.PlaySystemSound("wa_038.ogg");
				switch (base.name)
				{
				case "CastReview":
					stateObject.RequestLeave();
					BurikoScriptSystem.Instance.CallBlock("StaffRoom15");
					BurikoMemory.Instance.SetFlag("OmakeState", 0);
					BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
					break;
				case "StaffRoom":
					stateObject.RequestLeave();
					BurikoScriptSystem.Instance.JumpToBlock("OmakeSubSection");
					BurikoMemory.Instance.SetFlag("OmakeState", 2);
					BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
					break;
				case "MusicRoom":
					stateObject.RequestLeave();
					BurikoScriptSystem.Instance.JumpToBlock("OmakeSubSection");
					BurikoMemory.Instance.SetFlag("OmakeState", 3);
					BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 1);
					break;
				case "ScenarioLock":
					stateObject.RequestLeave();
					BurikoScriptSystem.Instance.JumpToBlock("OmakeSubSection");
					BurikoMemory.Instance.SetFlag("OmakeState", 1);
					break;
				case "StaffRoomButton":
					stateObject.RequestLeave();
					BurikoScriptSystem.Instance.JumpToBlock(ExtraInfo);
					break;
				case "UnlockAll":
					stateObject.RequestLeave();
					BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 0);
					BurikoMemory.Instance.SetFlag("OmakeState", 0);
					BurikoScriptSystem.Instance.JumpToBlock("UnlockAll");
					break;
				case "Back":
					stateObject.RequestLeave();
					BurikoMemory.Instance.SetFlag("OmakeState", 0);
					break;
				case "Continue":
					stateObject.RequestLeave();
					BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 0);
					BurikoMemory.Instance.SetFlag("OmakeState", 0);
					AudioController.Instance.ClearTempAudio();
					break;
				}
			}
		}

		private void OnHover(bool isOver)
		{
			this.isOver = isOver;
			if (isActive)
			{
				if (isOver && (GameSystem.Instance.GameState == GameState.ExtraScreen || GameSystem.Instance.GameState == GameState.OmakeSection))
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
			if ((base.name == "CastReview" || base.name == "MusicRoom" || base.name == "StaffRoom") && BurikoMemory.Instance.GetGlobalFlag("MEHEND").IntValue() < 10)
			{
				base.gameObject.SetActive(value: false);
			}
			if (base.name == "ScenarioLock" && BurikoMemory.Instance.GetGlobalFlag("MEHEND").IntValue() > 10)
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
			GameState gameState = GameSystem.Instance.GameState;
			button.isEnabled = (gameState == GameState.ExtraScreen || gameState == GameState.OmakeSection);
		}
	}
}
