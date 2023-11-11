using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko;
using System;
using System.IO;
using System.Text;
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
			LoadButton.isEnabled = manager.CanLoad();
			DeleteButton.isEnabled = true;
			string text = "";
			if (GameSystem.Instance.UseEnglishText)
			{
				text = entry.Text;
				if (text == "")
				{
					text = entry.TextJp;
				}
			}
			else
			{
				text = entry.TextJp;
				if (text == "")
				{
					text = entry.Text;
				}
			}
			BottomLabel.text = CleanText(text);
			SaveTexture.mainTexture = AssetManager.Instance.LoadScreenshot(Path.GetFileNameWithoutExtension(entry.Path) + ".png");
		}

		public static string CleanText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			bool flag = false;
			bool flag2 = false;
			foreach (char c in text)
			{
				if (!flag && c == ' ')
				{
					continue;
				}
				switch (c)
				{
				case '\n':
					stringBuilder.Append(' ');
					continue;
				case '<':
					flag2 = true;
					continue;
				case '>':
					flag2 = false;
					continue;
				}
				if (!flag2)
				{
					flag = true;
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private void DisableButton()
		{
			SaveButton.isEnabled = manager.CanSave();
			LoadButton.isEnabled = false;
			DeleteButton.isEnabled = false;
			BottomLabel.text = "";
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
				return;
			}
			isEnabled = true;
			if (!(saveInfo.Time == lastDate))
			{
				lastDate = saveInfo.Time;
				EnableEntry(saveInfo);
				LoadButton.GetComponent<SaveLoadButton>().Prepare(slotnum, manager);
				DeleteButton.GetComponent<SaveLoadButton>().Prepare(slotnum, manager);
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
