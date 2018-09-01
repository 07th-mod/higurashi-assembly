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
				string name = base.name;
				if (name != null)
				{
					if (!(name == "UpArrow"))
					{
						if (!(name == "DownArrow"))
						{
							if (name == "Return")
							{
								(GameSystem.Instance.GetStateObject() as StateHistory)?.RequestLeave();
							}
						}
						else
						{
							Hw.Step(-1f);
						}
					}
					else
					{
						Hw.Step(1f);
					}
				}
			}
		}
	}
}
