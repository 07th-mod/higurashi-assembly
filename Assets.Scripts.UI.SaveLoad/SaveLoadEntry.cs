using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko;
using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.UI.SaveLoad
{
	public class SaveLoadEntry : MonoBehaviour
	{
		public UIButton SaveButton;

		public UIButton LoadButton;

		public UIButton DeleteButton;

		public UILabel BottomLabel;

		public UITexture SaveTexture;

		private SaveLoadManager manager;

		private DateTime lastDate = DateTime.Now;

		private bool isEnabled = true;

		public void EnableEntry(SaveEntry entry)
		{
			SaveButton.isEnabled = manager.CanSave();
			LoadButton.isEnabled = true;
			DeleteButton.isEnabled = true;
			string empty = string.Empty;
			if (GameSystem.Instance.UseEnglishText)
			{
				empty = entry.Text;
				if (empty == string.Empty)
				{
					empty = entry.TextJp;
				}
			}
			else
			{
				empty = entry.TextJp;
				if (empty == string.Empty)
				{
					empty = entry.Text;
				}
			}
			BottomLabel.text = empty.Replace("\n", " ").TrimStart(' ', '\n');
			SaveTexture.mainTexture = AssetManager.Instance.LoadScreenshot(Path.GetFileNameWithoutExtension(entry.Path) + ".png");
		}

		private void DisableButton()
		{
			SaveButton.isEnabled = manager.CanSave();
			LoadButton.isEnabled = false;
			DeleteButton.isEnabled = false;
			BottomLabel.text = string.Empty;
			SaveTexture.mainTexture = AssetManager.Instance.LoadTexture("no_data");
		}

		public void LoadSlot(int slotnum)
		{
			SaveEntry saveInfo = BurikoScriptSystem.Instance.GetSaveInfo(slotnum);
			SaveButton.GetComponent<SaveLoadButton>().Prepare(slotnum, manager);
			if (saveInfo == null)
			{
				if (isEnabled)
				{
					isEnabled = false;
					lastDate = DateTime.Now;
					DisableButton();
				}
			}
			else
			{
				isEnabled = true;
				if (!(saveInfo.Time == lastDate))
				{
					lastDate = saveInfo.Time;
					EnableEntry(saveInfo);
					LoadButton.GetComponent<SaveLoadButton>().Prepare(slotnum, manager);
					DeleteButton.GetComponent<SaveLoadButton>().Prepare(slotnum, manager);
				}
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
