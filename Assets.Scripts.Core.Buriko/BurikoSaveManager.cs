using System;
using System.Collections.Generic;
using System.Diagnostics;
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
				string path = MGHelper.GetSavePath(string.Format("save{0}.dat", i.ToString("D3")), allowLegacyFallback: true);
				GetSaveData(i, path);
			}
			for (int j = 0; j < 3; j++)
			{
				string path2 = MGHelper.GetSavePath(string.Format("qsave{0}.dat", j.ToString("D1")), allowLegacyFallback: true);
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

		public void MoveSave(int slot, int newSlot, bool showErrorIfSourceMissing)
		{
			Logger.Log($"MoveSave: {slot} to {newSlot}");
			MoveSingleSave(slot, newSlot, ".dat", showErrorIfSourceMissing);
			MoveSingleSave(slot, newSlot, ".dat2", showErrorIfSourceMissing);
			MoveSingleSave(slot, newSlot, ".png", showErrorIfSourceMissing);
			MoveSingleSave(slot, newSlot, ".jpg", showErrorIfSourceMissing);
		}

		private string GetSaveFilePath(int slot, bool allowLegacyFallback, string extension)
		{
			string slotNumberAsString = slot.ToString("D3");
			string path = MGHelper.GetSavePath($"save{slotNumberAsString}{extension}", allowLegacyFallback);

			return path;
		}

		private void DeleteSingleSave(int slot, bool allowLegacyFallback, string extension)
		{
			string path = GetSaveFilePath(slot, allowLegacyFallback, extension);
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		private void DeleteSave(int slot, bool allowLegacyFallback)
		{
			DeleteSingleSave(slot, allowLegacyFallback, ".dat");
			DeleteSingleSave(slot, allowLegacyFallback, ".png");
			DeleteSingleSave(slot, allowLegacyFallback, ".jpg");

			saveList.Remove(slot);
		}

		// Note: moving legacy saves not supported.
		// Note: will not overwrite an existing save - make sure new slot is empty
		// If move fails, error will be printed but no exception raised
		private void MoveSingleSave(int slot, int newSlot, string extension, bool showErrorIfSourceMissing)
		{
			string sourcePath = GetSaveFilePath(slot, false, extension);
			string targetPath = GetSaveFilePath(newSlot, false, extension);

			try
			{
				if(File.Exists(sourcePath))
				{
					File.Move(sourcePath, targetPath);
				}
				else
				{
					if(showErrorIfSourceMissing)
					{
						Logger.Log($"MoveSingleSave({slot}, {newSlot}, {extension}) source path [{sourcePath}] does not exist");
					}
				}
			}
			catch(Exception e)
			{
				Logger.Log($"MoveSingleSave({slot}, {newSlot}, {extension}) failed from {sourcePath} to {targetPath}: {e.ToString()}");
			}
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
					byte[] array = File.ReadAllBytes(path);
					MGHelper.KeyEncode(array);
					byte[] buffer = CLZF2.Decompress(array);
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
							saveEntry2.IsModded = BurikoMemory.MODCheckMemoryIsModded(input, out bool isAutoSave);
							if(!saveEntry2.IsModded)
							{
								saveEntry2.Text = $"[UNMODDED] {saveEntry2.Text}";
								saveEntry2.TextJp = $"[UNMODDED] {saveEntry2.TextJp}";
							}

							saveEntry2.IsAutoSave = isAutoSave; //saveEntry2.Text.StartsWith("|AUTOSAVE|");
							if(isAutoSave)
							{
								saveEntry2.Text = $"AUTO{slot + 1}: {saveEntry2.Text}";
								saveEntry2.TextJp = $"AUTO{slot + 1}: {saveEntry2.TextJp}";
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
			GetSaveData(path: (slot >= 100) ? MGHelper.GetSavePath(string.Format("qsave{0}.dat", (slot - 100).ToString("D1")), allowLegacyFallback: true) : MGHelper.GetSavePath(string.Format("save{0}.dat", slot.ToString("D3")), allowLegacyFallback: true), slot: slot);
		}
	}
}
