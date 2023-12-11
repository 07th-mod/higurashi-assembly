using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Interfaces;
using Assets.Scripts.UI.SaveLoad;
using MOD.Scripts.Core;
using MOD.Scripts.Core.Audio;
using MOD.Scripts.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.Buriko
{
	public class BurikoScriptSystem : IScriptInterpreter
	{
		private Dictionary<string, BurikoScriptFile> scriptFiles = new Dictionary<string, BurikoScriptFile>();

		private Stack<BurikoStackEntry> callStack = new Stack<BurikoStackEntry>();

		private BurikoScriptFile currentScript;

		private BurikoMemory memoryManager;

		private BurikoSaveManager saveManager;

		private byte[] snapshotData;

		private byte[] tempSnapshotData;

		private byte[] autoSaveSnapshot;

		private System.Diagnostics.Stopwatch autoSaveStopwatch = new System.Diagnostics.Stopwatch();

		private bool hasSnapshot;

		private readonly string[] tempSnapshotText = new string[2];

		public bool FlowWasReached { get; private set; }

		public static BurikoScriptSystem Instance
		{
			get;
			private set;
		}

		public void Initialize(GameSystem gameSystem)
		{
			Logger.Log("Preparing BurikoScriptSystem.");
			LoadGameScripts();
			saveManager = new BurikoSaveManager();
			memoryManager = new BurikoMemory();
			Instance = this;
			JumpToScript("init");
		}

		private void LoadGameScripts()
		{
			string[] availableScriptNames = AssetManager.Instance.GetAvailableScriptNames();
			foreach (string text in availableScriptNames)
			{
				BurikoScriptFile value = new BurikoScriptFile(this, text);
				scriptFiles.Add(text, value);
			}
		}

		public BurikoScriptFile GetCurrentScript()
		{
			return currentScript;
		}

		public void CallBlock(string blockname)
		{
			CallScript(Path.GetFileNameWithoutExtension(currentScript.Filename), blockname);
		}

		public void JumpToBlock(string blockname)
		{
			JumpToScript(Path.GetFileNameWithoutExtension(currentScript.Filename), blockname);
		}

		public void JumpToScript(string scriptname, string blockname = "main")
		{
			Logger.Log((currentScript == null) ? $"Starting at script {scriptname} (block {blockname})" : $"Jumping from script {currentScript.Filename} to script {scriptname} (block {blockname})");
			callStack.Clear();
			scriptname = scriptname.ToLower();
			if (!scriptFiles.TryGetValue(scriptname + ".mg", out currentScript))
			{
				throw new KeyNotFoundException($"Could not JumpToScript {scriptname}, as the file was not found.");
			}
			if (!currentScript.IsInitialized)
			{
				currentScript.InitializeScript();
			}
			memoryManager.ResetScope();
			currentScript.JumpToBlock(blockname);
		}

		public void CallScript(string scriptname, string blockname = "main")
		{
			if(scriptname == "flow")
			{
				FlowWasReached = true;
			}

			Logger.Log($"{currentScript.Filename}: calling script {scriptname} (block {blockname})");
			callStack.Push(new BurikoStackEntry(currentScript, currentScript.Position, currentScript.LineNum));
			scriptname = scriptname.ToLower();
			if (!scriptFiles.TryGetValue(scriptname + ".mg", out currentScript))
			{
				throw new KeyNotFoundException($"Could not CallScript {scriptname}, as the file was not found.");
			}
			if (!currentScript.IsInitialized)
			{
				currentScript.InitializeScript();
			}
			memoryManager.AddScope();
			currentScript.JumpToBlock(blockname);
		}

		public void Return()
		{
			if (callStack.Count <= 0)
			{
				throw new Exception("Could not return from script, as the script is currently at the bottom of the call stack.");
			}
			BurikoStackEntry burikoStackEntry = callStack.Pop();
			Logger.Log($"Returning from script {currentScript.Filename} to script {burikoStackEntry.Script} LineNum {burikoStackEntry.LineNum}");
			if (!burikoStackEntry.Script.IsInitialized)
			{
				burikoStackEntry.Script.InitializeScript();
			}
			currentScript = burikoStackEntry.Script;
			currentScript.JumpToLineNum(burikoStackEntry.LineNum + 1);
			memoryManager.DropScope();
		}

		public void Advance()
		{
			currentScript.Next();
		}

		private bool AutoSaveEnabled()
		{
			for (int i = 0; i < 5; i++)
			{
				SaveEntry entry = saveManager.GetSaveInfoInSlot(i);
				if(entry != null)
				{
					// If any save in slot 0-4 is an autosave, disable autosave
					if(!entry.IsAutoSave)
					{
						return false;
					}
				}
			}

			return true;
		}

		public void AutoSaveUpdate()
		{
			// Start the autosave timer the first time AutoSaveUpdate() is called
			if(!autoSaveStopwatch.IsRunning)
			{
				autoSaveStopwatch.Start();
			}

			if(autoSaveStopwatch.Elapsed < TimeSpan.FromSeconds(15))
			{
				return;
			}
			else
			{
				autoSaveStopwatch.Reset();
				autoSaveStopwatch.Start();
			}

			try
			{
				if (!AutoSaveEnabled())
				{
					MODToaster.Show("Please delete save slots 1-5 to enable Autosave!");
					return;
				}

				// Delete save slot 5
				DeleteSave(5);

				// Move autosaves down by one, leaving slot 0 empty
				// Need to update save slot info if the slot was updated, or else
				// the save won't appear in the save/load menu
				for (int i = 3; i >= 0; i--)
				{
					MoveSave(i, i + 1, showErrorIfSourceMissing: false);
					saveManager.UpdateSaveSlot(i + 1);
				}

				// Save to slot 0
				int saveSlot = 0;
				SaveGame(saveSlot, saveAutoSave: true);
			}
			catch (Exception)
			{
				MODLogger.Log("Autosave failed", withContext: true);
			}
		}

		public void SaveQuickSave()
		{
			int num = memoryManager.GetGlobalFlag("GQsaveNum").IntValue();
			if (saveManager.IsSaveInSlot(100))
			{
				num++;
				if (num > 2)
				{
					num = 0;
				}
			}
			else
			{
				num = 0;
			}
			SaveGame(num + 100);
			memoryManager.SetGlobalFlag("GQsaveNum", num);
			GameSystem.Instance.MainUIController.ShowQuickSaveIcon();
		}

		public void DeleteSave(int slotnum)
		{
			saveManager.DeleteSave(slotnum);
			MODSteamCloudManager.ShowSteamCloudUsage();
		}

		public void MoveSave(int slot, int newSlot, bool showErrorIfSourceMissing)
		{
			saveManager.MoveSave(slot, newSlot, showErrorIfSourceMissing);
		}

		public bool IsSaveInSlot(int slotnum)
		{
			return saveManager.IsSaveInSlot(slotnum);
		}

		public int GetQSaveSlotByMostRecent(int num)
		{
			if (num > 2)
			{
				throw new Exception("Can't get QSave for slot above 2!");
			}
			int num2 = memoryManager.GetGlobalFlag("GQsaveNum").IntValue() - num;
			if (num2 < 0)
			{
				num2 += 3;
			}
			return num2 + 100;
		}

		public SaveEntry GetQSaveInfo(int qsavenum)
		{
			int num = memoryManager.GetGlobalFlag("GQsaveNum").IntValue() + qsavenum;
			if (num >= 3)
			{
				num -= 3;
			}
			return saveManager.GetSaveInfoInSlot(num + 100);
		}

		public SaveEntry GetSaveInfo(int savenum)
		{
			return saveManager.GetSaveInfoInSlot(savenum);
		}

		public SaveEntry GetQSaveInfo()
		{
			int num = memoryManager.GetGlobalFlag("GQsaveNum").IntValue();
			return saveManager.GetSaveInfoInSlot(num + 100);
		}

		public void LoadQuickSave()
		{
			int num = memoryManager.GetGlobalFlag("GQsaveNum").IntValue();
			LoadGame(num + 100);
		}

		public void CopyTempSnapshot()
		{
			Debug.Log("CopyTempSnapshot");
			if (snapshotData.Length > 0)
			{
				tempSnapshotData = snapshotData;
				tempSnapshotText[0] = GameSystem.Instance.TextController.GetFullText(0);
				tempSnapshotText[1] = GameSystem.Instance.TextController.GetFullText(1);
			}
		}

		public void RestoreTempSnapshot()
		{
			Debug.Log("RestoreTempSnapshot");
			if (tempSnapshotData.Length > 0)
			{
				snapshotData = tempSnapshotData;
				GameSystem.Instance.TextController.SetFullText(tempSnapshotText[0], 0);
				GameSystem.Instance.TextController.SetFullText(tempSnapshotText[1], 1);
			}
		}

		public void TakeSaveSnapshot(string text = "", bool isAutoSave = false)
		{
			if (GameSystem.Instance.CanSave)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						binaryWriter.Write("MGSV".ToCharArray(0, 4));
						binaryWriter.Write(1);
						binaryWriter.Write(DateTime.Now.ToBinary());
						if (text == string.Empty)
						{
							string fullText = GameSystem.Instance.TextController.GetFullText(0);
							string fullText2 = GameSystem.Instance.TextController.GetFullText(1);
							binaryWriter.Write(fullText);
							binaryWriter.Write(fullText2);
							string prevText = GameSystem.Instance.TextController.GetPrevText(0);
							string prevText2 = GameSystem.Instance.TextController.GetPrevText(1);
							binaryWriter.Write(prevText);
							binaryWriter.Write(prevText2);
							binaryWriter.Write(GameSystem.Instance.TextController.GetPrevAppendState());
						}
						else
						{
							binaryWriter.Write(text);
							binaryWriter.Write(text);
							binaryWriter.Write(string.Empty);
							binaryWriter.Write(string.Empty);
							binaryWriter.Write(value: false);
						}
						binaryWriter.Write(callStack.Count);
						foreach (BurikoStackEntry item in callStack.Reverse())
						{
							binaryWriter.Write(item.Script.Filename);
							binaryWriter.Write(item.LineNum);
						}
						binaryWriter.Write(currentScript.Filename);
						binaryWriter.Write(currentScript.LineNum);
						binaryWriter.Write(memoryManager.SaveMemory(isAutoSave));
						AudioController.Instance.SerializeCurrentAudio(memoryStream);
						GameSystem.Instance.SceneController.SerializeScene(memoryStream);
						if(isAutoSave)
						{
							autoSaveSnapshot = memoryStream.ToArray();
						}
						else
						{
							snapshotData = memoryStream.ToArray();
							hasSnapshot = true;
						}
					}
				}

				if(!isAutoSave)
				{
					// Take an autosave snapshot
					// TODO: only do this periodically (every 10 mins?)
					TakeSaveSnapshot(text, isAutoSave: true);
				}
			}
		}

		public void ModifySaveGame(int slot, string description)
		{
			Debug.Log("ModifySaveGame " + slot);
			SaveEntry saveInfoInSlot = saveManager.GetSaveInfoInSlot(slot);
			if (saveInfoInSlot != null)
			{
				byte[] array = File.ReadAllBytes(saveInfoInSlot.Path);
				MGHelper.KeyEncode(array);
				byte[] buffer = CLZF2.Decompress(array);
				MemoryStream memoryStream = new MemoryStream(buffer);
				MemoryStream memoryStream2 = new MemoryStream();
				BinaryReader binaryReader = new BinaryReader(memoryStream);
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream2);
				binaryWriter.Write(binaryReader.ReadBytes(16));
				binaryReader.ReadString();
				binaryWriter.Write(description);
				binaryWriter.Write(binaryReader.ReadBytes((int)(memoryStream.Length - memoryStream.Position)));
				byte[] inputBytes = memoryStream2.ToArray();
				memoryStream.Dispose();
				memoryStream2.Dispose();
				byte[] array2 = CLZF2.Compress(inputBytes);
				MGHelper.KeyEncode(array2);
				File.WriteAllBytes(saveInfoInSlot.Path, array2);
				saveManager.UpdateSaveSlot(slot);
			}
		}

		public void ModifySnapshotDescription(string newdescription)
		{
			Debug.Log("ModifySnapshotDescription: " + newdescription);
			MemoryStream memoryStream = new MemoryStream(snapshotData);
			MemoryStream memoryStream2 = new MemoryStream();
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream2);
			binaryWriter.Write(binaryReader.ReadBytes(16));
			binaryReader.ReadString();
			binaryWriter.Write(newdescription);
			binaryWriter.Write(binaryReader.ReadBytes((int)(memoryStream.Length - memoryStream.Position)));
			snapshotData = memoryStream2.ToArray();
			memoryStream.Dispose();
			memoryStream2.Dispose();
		}

		public void SaveGame(int slotnum)
		{
			SaveGame(slotnum, saveAutoSave: false);
		}

		public void SaveGame(int slotnum, bool saveAutoSave)
		{
			if (hasSnapshot)
			{
				byte[] snap;

				if(saveAutoSave)
				{
					if(autoSaveSnapshot != null)
					{
						snap = autoSaveSnapshot;
						autoSaveSnapshot = null;
					}
					else
					{
						Debug.Log(">>> Can't save autosave as no autosave snapshot");
						return;
					}
				}
				else
				{
					snap = snapshotData;
				}

				byte[] array = CLZF2.Compress(snap);
				MGHelper.KeyEncode(array);
				string str = (slotnum < 100) ? ("save" + slotnum.ToString("D3")) : ("qsave" + (slotnum - 100));
				File.WriteAllBytes(MGHelper.GetSavePath(str + ".dat", allowLegacyFallback: false), array);
				saveManager.UpdateSaveSlot(slotnum);
				GameSystem.Instance.SceneController.WriteScreenshot(MGHelper.GetSavePath(str + ".jpg", allowLegacyFallback: false));
				MODSteamCloudManager.ShowSteamCloudUsage();
			}
		}

		public void LoadGame(int slotnum)
		{
			SaveEntry saveInfoInSlot = saveManager.GetSaveInfoInSlot(slotnum);
			if (saveInfoInSlot != null)
			{
				GameSystem.Instance.TextController.ClearText();
				GameSystem.Instance.MainUIController.FadeOut(0f, isBlocking: true);
				byte[] array = File.ReadAllBytes(saveInfoInSlot.Path);
				MGHelper.KeyEncode(array);
				byte[] buffer = tempSnapshotData = (snapshotData = CLZF2.Decompress(array));
				hasSnapshot = true;
				GameSystem.Instance.ClearAllWaits();
				GameSystem.Instance.ClearActions();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
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
						binaryReader.ReadInt64();
						string text = binaryReader.ReadString();
						string text2 = binaryReader.ReadString();
						string text3 = binaryReader.ReadString();
						string text4 = binaryReader.ReadString();
						bool flag = binaryReader.ReadBoolean();
						GameSystem.Instance.TextController.SetPrevText(text3, 0);
						GameSystem.Instance.TextController.SetPrevText(text4, 1);
						if (flag)
						{
							if (GameSystem.Instance.UseEnglishText)
							{
								GameSystem.Instance.TextController.TypeTextImmediately(text4);
								GameSystem.Instance.TextController.SetFullText(text3, 0);
							}
							else
							{
								GameSystem.Instance.TextController.TypeTextImmediately(text3);
								GameSystem.Instance.TextController.SetFullText(text4, 1);
							}
							tempSnapshotText[0] = text3;
							tempSnapshotText[1] = text4;
							GameSystem.Instance.TextController.SetAppendState(append: true);
						}
						else
						{
							GameSystem.Instance.TextController.SetFullText(text, 0);
							GameSystem.Instance.TextController.SetFullText(text2, 1);
							tempSnapshotText[0] = text;
							tempSnapshotText[1] = text2;
							GameSystem.Instance.TextController.SetAppendState(append: false);
						}
						callStack.Clear();
						int num2 = binaryReader.ReadInt32();
						for (int i = 0; i < num2; i++)
						{
							string key = binaryReader.ReadString();
							int linenum = binaryReader.ReadInt32();
							BurikoScriptFile script = scriptFiles[key];
							callStack.Push(new BurikoStackEntry(script, 0, linenum));
						}
						string key2 = binaryReader.ReadString();
						int linenum2 = binaryReader.ReadInt32();
						currentScript = scriptFiles[key2];
						if (!currentScript.IsInitialized)
						{
							currentScript.InitializeScript();
						}
						currentScript.JumpToLineNum(linenum2);
						memoryManager.LoadMemory(memoryStream);
						AudioController.Instance.DeSerializeCurrentAudio(memoryStream);
						// Restoring mod audio state done here to avoid changes being overwritten by above DeSerializeCurrentAudio() call
						MODAudioTracking.Instance.RestoreState();
						GameSystem.Instance.SceneController.DeSerializeScene(memoryStream);
						GameSystem.Instance.ForceReturnNormalState();
						GameSystem.Instance.CloseChoiceIfExists();
						GameSystem.Instance.TextHistory.ClearHistory();
						GameSystem.Instance.IsSkipping = false;
						GameSystem.Instance.IsAuto = false;
						GameSystem.Instance.CanSkip = true;
						GameSystem.Instance.CanInput = true;
						GameSystem.Instance.CanSave = true;
						int flag2 = GetFlag("LTextFade");
						GameSystem.Instance.TextController.SetTextFade(flag2 == 1);
						if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1 && BurikoMemory.Instance.GetGlobalFlag("GLinemodeSp").IntValue() == 2 && BurikoMemory.Instance.GetFlag("NVL_in_ADV").IntValue() == 0)
						{
							MODActions.DisableNVLModeINADVMode();
						}
						if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1 && BurikoMemory.Instance.GetGlobalFlag("GLinemodeSp").IntValue() == 0 && BurikoMemory.Instance.GetFlag("NVL_in_ADV").IntValue() == 1)
						{
							MODActions.EnableNVLModeINADVMode();
						}
					}
				}
			}
		}

		public int GetFlag(string name)
		{
			return memoryManager.GetFlag(name).IntValue();
		}

		public void SetFlag(string name, int value)
		{
			memoryManager.SetFlag(name, value);
		}

		public void ShutDown()
		{
			throw new NotImplementedException();
		}
	}
}
