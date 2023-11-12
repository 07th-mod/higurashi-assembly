using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Core.Buriko
{
	public class BurikoSaveManager
	{
		private readonly Dictionary<int, SaveEntry> saveList;
		public static string lastSaveError;

		public BurikoSaveManager()
		{
			saveList = new Dictionary<int, SaveEntry>();
			lastSaveError = null;
			for (int i = 0; i < 100; i++)
			{
				string path = Path.Combine(MGHelper.GetSavePath(), string.Format("save{0}.dat", i.ToString("D3")));
				GetSaveData(i, path);
			}
			for (int j = 0; j < 3; j++)
			{
				string path2 = Path.Combine(MGHelper.GetSavePath(), string.Format("qsave{0}.dat", j.ToString("D1")));
				GetSaveData(j + 100, path2);
			}
		}

		public SaveEntry GetSaveInfoInSlot(int slot)
		{
			if (!saveList.ContainsKey(slot))
			{
				return null;
			}
			return saveList[slot];
		}

		public void DeleteSave(int slot)
		{
			string path = Path.Combine(MGHelper.GetSavePath(), string.Format("save{0}.dat", slot.ToString("D3")));
			string path2 = Path.Combine(MGHelper.GetSavePath(), string.Format("save{0}.png", slot.ToString("D3")));
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			if (File.Exists(path2))
			{
				File.Delete(path2);
			}
			saveList.Remove(slot);
		}

		public bool IsSaveInSlot(int slot)
		{
			return saveList.ContainsKey(slot);
		}

		private void GetSaveData(int slot, string path)
		{
			if (File.Exists(path))
			{
				try
				{
					SaveEntry saveEntry = new SaveEntry
					{
						Path = path
					};
					byte[] array = File.ReadAllBytes(path);
					MGHelper.KeyEncode(array);
					using (MemoryStream input = new MemoryStream(CLZF2.Decompress(array)))
					{
						using (BinaryReader binaryReader = new BinaryReader(input))
						{
							if (new string(binaryReader.ReadChars(4)) != "MGSV")
							{
								throw new FileLoadException("Save file does not appear to be valid! Invalid header.");
							}
							if (binaryReader.ReadInt32() != 1)
							{
								throw new FileLoadException("Save file does not appear to be valid! Invalid version number.");
							}
							saveEntry.Time = DateTime.FromBinary(binaryReader.ReadInt64());
							string textJp = binaryReader.ReadString();
							string text = saveEntry.Text = binaryReader.ReadString();
							string pattern = "[<](size)[=][+](.+)[<][/](size)[>]";
							// In Rei, text lines contains literal newline characters,
							// which are not matched by '.' unless using RegexOptions.Singleline
							textJp = Regex.Replace(textJp, pattern, string.Empty, RegexOptions.Singleline);
							text = Regex.Replace(text, pattern, string.Empty, RegexOptions.Singleline);
							saveEntry.Text = text;
							saveEntry.TextJp = textJp;
						}
					}
					if (saveList.ContainsKey(slot))
					{
						saveList.Remove(slot);
					}
					saveList.Add(slot, saveEntry);
				}
				catch (Exception ex)
				{
					lastSaveError = "Could not read from save file " + path + "\nException: " + ex;
					Logger.LogWarning(lastSaveError);
					throw;
				}
			}
		}

		public void UpdateSaveSlot(int slot)
		{
			GetSaveData(path: (slot >= 100) ? Path.Combine(MGHelper.GetSavePath(), string.Format("qsave{0}.dat", (slot - 100).ToString("D1"))) : Path.Combine(MGHelper.GetSavePath(), string.Format("save{0}.dat", slot.ToString("D3"))), slot: slot);
		}
	}
}
