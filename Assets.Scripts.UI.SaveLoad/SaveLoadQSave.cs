using Assets.Scripts.Core.Buriko;
using UnityEngine;

namespace Assets.Scripts.UI.SaveLoad
{
	public class SaveLoadQSave : MonoBehaviour
	{
		public UIButton SaveButton;

		public UIButton LoadButton;

		public UILabel BottomLabel;

		private SaveLoadManager manager;

		private bool isEnabled = true;

		public void EnableEntry(SaveEntry entry)
		{
			SaveButton.isEnabled = false;
			LoadButton.isEnabled = true;
			BottomLabel.text = entry.Time.ToString("MMM dd, yyyy h:mm tt");
			// Save button is never useful so hide it
			SaveButton.gameObject.SetActive(false);
		}

		private void DisableButton()
		{

			SaveButton.isEnabled = false;
			LoadButton.isEnabled = false;
			BottomLabel.text = string.Empty;
			// Save button is never useful so hide it
			SaveButton.gameObject.SetActive(false);
		}

		public void LoadSlot(int slotnum)
		{
			SaveEntry saveInfo = BurikoScriptSystem.Instance.GetSaveInfo(slotnum);
			SaveButton.GetComponent<SaveLoadButton>().Prepare(slotnum, manager);
			if (saveInfo == null)
			{
				if (isEnabled)
				{
					DisableButton();
					isEnabled = false;
				}
			}
			else
			{
				isEnabled = true;
				EnableEntry(saveInfo);
				LoadButton.GetComponent<SaveLoadButton>().Prepare(slotnum, manager);
			}
		}

		public void Prepare(SaveLoadManager mg)
		{
			manager = mg;
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
