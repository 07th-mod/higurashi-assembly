using Assets.Scripts.Core.State;
using UnityEngine;

namespace Assets.Scripts.Core.History
{
	public class HistoryButton : MonoBehaviour
	{
		public HistoryWindow Hw;

		private void OnPress(bool isDown)
		{
			if (isDown)
			{
				switch (base.name)
				{
				case "UpArrow":
					Hw.Step(1f);
					break;
				case "DownArrow":
					Hw.Step(-1f);
					break;
				case "Return":
					(GameSystem.Instance.GetStateObject() as StateHistory)?.RequestLeave();
					break;
				}
			}
		}
	}
}
