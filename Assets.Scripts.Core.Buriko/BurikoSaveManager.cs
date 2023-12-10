using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using static Assets.Scripts.Core.Buriko.BurikoScriptSystem;

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
				string path = MGHelper.GetSavePath(string.Format("save{0}.dat2", i.ToString("D3")), allowLegacyFallback: true);
				if(!File.Exists(path))
				{
					path = Path.ChangeExtension(path, ".dat");
				}
				GetSaveData(i, path);
			}
			for (int j = 0; j < 3; j++)
			{
				string path2 = MGHelper.GetSavePath(string.Format("qsave{0}.dat2", j.ToString("D1")), allowLegacyFallback: true);
				if (!File.Exists(path2))
				{
					path2 = Path.ChangeExtension(path2, ".dat");
				}
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
			// Delete the 'normal' save
			DeleteSave(slot, allowLegacyFallback: false);

			// If it exists, delete the 'legacy' save
			DeleteSave(slot, allowLegacyFallback: true);
		}

		private void DeleteSingleSave(int slot, bool allowLegacyFallback, string extension)
		{
			string slotNumberAsString = slot.ToString("D3");
			string path = MGHelper.GetSavePath($"save{slotNumberAsString}{extension}", allowLegacyFallback);
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		private void DeleteSave(int slot, bool allowLegacyFallback)
		{
			DeleteSingleSave(slot, allowLegacyFallback, ".dat");
			DeleteSingleSave(slot, allowLegacyFallback, ".dat2");
			DeleteSingleSave(slot, allowLegacyFallback, ".png");
			DeleteSingleSave(slot, allowLegacyFallback, ".jpg");

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
					SaveEntry saveEntry = new SaveEntry();
					saveEntry.Path = path;
					SaveEntry saveEntry2 = saveEntry;

					byte[] saveData = null;
					if (path.ToLower().EndsWith(".dat2"))
					{
						byte[] packedSave = File.ReadAllBytes(path);
						CombinedSaveData combinedData = BurikoScriptSystem.UnpackSave(packedSave);
						saveData = combinedData.save;
					}
					else
					{
						saveData = File.ReadAllBytes(path);
					}

					MGHelper.KeyEncode(saveData);
					byte[] buffer = CLZF2.Decompress(saveData);
					using (MemoryStream input = new MemoryStream(buffer))
					{
						using (BinaryReader binaryReader = new BinaryReader(input))
						{
							string a = new string(binaryReader.ReadChars(4));
							if (a != "MGSV")
							{
								throw new FileLoadException("Save file does not appear to be valid! Invalid header.");
							}
							int num = binaryReader.ReadInt32();
							if (num != 1)
							{
								throw new FileLoadException("Save file does not appear to be valid! Invalid version number.");
							}
							saveEntry2.Time = DateTime.FromBinary(binaryReader.ReadInt64());
							string textJp = binaryReader.ReadString();
							string text = saveEntry2.Text = binaryReader.ReadString();
							string pattern = "[<](size)[=][+](.+)[<][/](size)[>]";
							textJp = Regex.Replace(textJp, pattern, string.Empty);
							text = Regex.Replace(text, pattern, string.Empty);
							saveEntry.Text = text;
							saveEntry2.TextJp = textJp;

							// ------------------------ MOD SAVE DETECTION BEGIN ------------------------
							// This section was added so that the mod can tell whether a save is from the modded game or not
							// Note that extremely old mod saves (multiple years old?) may be detected incorrectly

							// Ignore serialized previous text
							binaryReader.ReadString(); // previous text
							binaryReader.ReadString(); // previous text
							binaryReader.ReadBoolean(); // flag?

							// Ignore serialiezd line number, call stack etc.
							int num2 = binaryReader.ReadInt32();
							for (int i = 0; i < num2; i++)
							{
								binaryReader.ReadString(); // key
								binaryReader.ReadInt32(); // line number
							}
							binaryReader.ReadString(); //key2
							binaryReader.ReadInt32(); //linenum2

							// Read serialized BurikoMemory only for the purpose of determining it contains mod-only variables
							saveEntry2.IsModded = BurikoMemory.MODCheckMemoryIsModded(input);
							if(!saveEntry2.IsModded)
							{
								saveEntry2.Text = $"[UNMODDED] {saveEntry2.Text}";
								saveEntry2.TextJp = $"[UNMODDED] {saveEntry2.TextJp}";
							}

							// ------------------------ MOD SAVE DETECTION END ------------------------
						}
					}
					if (saveList.ContainsKey(slot))
					{
						saveList.Remove(slot);
					}
					saveList.Add(slot, saveEntry2);
				}
				catch (Exception ex)
				{
					lastSaveError = "Could not read from save file " + path + "\nException: " + ex;
					Logger.LogWarning(lastSaveError);
					throw;
					IL_013f:;
				}
			}
		}

		public void UpdateSaveSlot(int slot)
		{
			string path = "";

			if(slot >= 100)
			{
				path = MGHelper.GetSavePath(string.Format("qsave{0}.dat2", (slot - 100).ToString("D1")), allowLegacyFallback: true);
			}
			else
			{
				path = MGHelper.GetSavePath(string.Format("save{0}.dat2", slot.ToString("D3")), allowLegacyFallback: true);
			}

			if (!File.Exists(path))
			{
				path = Path.ChangeExtension(path, ".dat");
			}

			GetSaveData(slot, path);
		}
	}
}
