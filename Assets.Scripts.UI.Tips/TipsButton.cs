using Assets.Scripts.Core;
using Assets.Scripts.Core.State;
using UnityEngine;

namespace Assets.Scripts.UI.Tips
{
	public class TipsButton : MonoBehaviour
	{
		private TipsManager manager;

		private bool isEnabled = true;

		public void Setup(TipsManager mg)
		{
			manager = mg;
		}

		public void Disable()
		{
			isEnabled = false;
		}

		private void OnClick()
		{
			GameSystem instance = GameSystem.Instance;
			if (instance.GameState == GameState.TipsScreen && isEnabled && manager.isActive && UICamera.currentTouchID >= -1)
			{
				StateViewTips stateViewTips = instance.GetStateObject() as StateViewTips;
				if (stateViewTips != null)
				{
					switch (base.name)
					{
					case "PageLeft":
						manager.ChangePage(-1);
						break;
					case "PageRight":
						manager.ChangePage(1);
						break;
					case "ExitButton":
						stateViewTips.RequestLeave();
						instance.CanSave = true;
						break;
					}
				}
			}
		}
	}
}
