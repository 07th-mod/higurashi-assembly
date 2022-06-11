using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using UnityEngine;

namespace Assets.Scripts.UI.Fragments
{
	internal class FragmentUiButton : MonoBehaviour
	{
		private FragmentsManager manager;

		private bool isEnabled = true;

		public void Setup(FragmentsManager mg)
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
			if (instance.GameState != GameState.FragmentScreen || !isEnabled || !manager.isActive || UICamera.currentTouchID < -1)
			{
				return;
			}
			StateFragmentList stateFragmentList = instance.GetStateObject() as StateFragmentList;
			if (stateFragmentList == null)
			{
				return;
			}
			string name = base.name;
			if (!(name == "PageLeft"))
			{
				if (!(name == "PageRight"))
				{
					if (name == "ExitButton")
					{
						stateFragmentList.RequestLeave();
						BurikoMemory.Instance.SetFlag("TipsMode", 0);
						instance.CanSave = true;
					}
				}
				else
				{
					manager.ChangePage(1);
				}
			}
			else
			{
				manager.ChangePage(-1);
			}
		}
	}
}
