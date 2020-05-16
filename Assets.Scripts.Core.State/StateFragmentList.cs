using Assets.Scripts.Core.Buriko;
using Assets.Scripts.UI.Fragments;
using UnityEngine;

namespace Assets.Scripts.Core.State
{
	internal class StateFragmentList : IGameState
	{
		private GameSystem gameSystem;

		private FragmentsManager fragmentManager;

		public StateFragmentList()
		{
			gameSystem = GameSystem.Instance;
			GameObject gameObject = Object.Instantiate(gameSystem.FragmentScreenPrefab);
			fragmentManager = gameObject.GetComponent<FragmentsManager>();
			GameSystem.Instance.IsForceSkip = false;
			GameSystem.Instance.IsSkipping = false;
			GameSystem.Instance.IsAuto = false;
			fragmentManager.Show();
		}

		public void OpenFragment(FragmentDataEntry entry)
		{
			bool hasPrereq = fragmentManager.FufilledPrereqs(entry);
			gameSystem.AudioController.FadeOutBGM(0, 500, waitForFade: false);
			gameSystem.TextController.ClearText();
			fragmentManager.Hide(delegate
			{
				gameSystem.PopStateStack();
				if (hasPrereq)
				{
					if (BurikoMemory.Instance.GetFragmentReadStatus(entry.Id) == 0)
					{
						int num = BurikoMemory.Instance.GetFlag("LFragmentRead").IntValue();
						BurikoMemory.Instance.SetFlag("LFragmentRead", num + 1);
					}
					BurikoMemory.Instance.SetFragmentReadStatus(entry.Id);
					BurikoMemory.Instance.SetFragmentValueStatus(entry.Id, 1);
					BurikoScriptSystem.Instance.CallScript(entry.Script);
				}
				else
				{
					BurikoMemory.Instance.SetFragmentValueStatus(entry.Id, 2);
					BurikoScriptSystem.Instance.CallScript("kakera_miss");
				}
			});
		}

		public void RequestLeave()
		{
			fragmentManager.Hide(delegate
			{
				gameSystem.PopStateStack();
			});
		}

		public void RequestLeaveImmediate()
		{
			Object.Destroy(fragmentManager.gameObject);
		}

		public void OnLeaveState()
		{
			if (fragmentManager != null)
			{
				Object.Destroy(fragmentManager.gameObject);
			}
		}

		public void OnRestoreState()
		{
		}

		public bool InputHandler()
		{
			return false;
		}

		public GameState GetStateType()
		{
			return GameState.FragmentScreen;
		}
	}
}
