using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko.Util;
using Assets.Scripts.Core.Buriko.VarTypes;
using MOD.Scripts.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.Buriko
{
	internal class BurikoMemory
	{
		private const int FlagCount = 11;

		private Dictionary<string, BurikoMemoryEntry> memorylist = new Dictionary<string, BurikoMemoryEntry>();

		private Dictionary<string, int> variableReference = new Dictionary<string, int>();

		private Dictionary<int, int> flags = new Dictionary<int, int>();

		private Dictionary<int, int> globalFlags = new Dictionary<int, int>();

		private Dictionary<string, List<int>> readText = new Dictionary<string, List<int>>();

		private List<string> cgflags = new List<string>();

		private int scopeLevel;

		public static BurikoMemory Instance
		{
			get;
			private set;
		}

		public BurikoMemory()
		{
			memorylist = new Dictionary<string, BurikoMemoryEntry>();
			variableReference.Add("LOCALWORK_NO_RESULT", 0);
			variableReference.Add("TipsMode", 1);
			variableReference.Add("SelectResult", 2);
			variableReference.Add("ChapterNumber", 10);
			variableReference.Add("LOnikakushiDay", 30);
			variableReference.Add("LTextFade", 31);
			variableReference.Add("GFlag_FirstPlay", 0);
			variableReference.Add("GFlag_GameClear", 1);
			variableReference.Add("GQsaveNum", 2);
			variableReference.Add("GOnikakushiDay", 3);
			variableReference.Add("GHighestChapter", 3); // For consistency between chapters
			variableReference.Add("GMessageSpeed", 10);
			variableReference.Add("GAutoSpeed", 11);
			variableReference.Add("GAutoAdvSpeed", 12);
			variableReference.Add("GUsePrompts", 13);
			variableReference.Add("GSlowSkip", 14);
			variableReference.Add("GSkipUnread", 15);
			variableReference.Add("GClickDuringAuto", 16);
			variableReference.Add("GRightClickMenu", 17);
			variableReference.Add("GWindowOpacity", 18);
			variableReference.Add("GVoiceVolume", 19);
			variableReference.Add("GBGMVolume", 20);
			variableReference.Add("GSEVolume", 21);
			variableReference.Add("GCutVoiceOnClick", 22);
			variableReference.Add("GUseSystemSound", 23);
			variableReference.Add("GLanguage", 24);
			variableReference.Add("GVChie", 30);
			variableReference.Add("GVEiji", 31);
			variableReference.Add("GVKana", 32);
			variableReference.Add("GVKira", 33);
			variableReference.Add("GVMast", 34);
			variableReference.Add("GVMura", 35);
			variableReference.Add("GVRiho", 36);
			variableReference.Add("GVRmn_", 37);
			variableReference.Add("GVSari", 38);
			variableReference.Add("GVTika", 39);
			variableReference.Add("GVYayo", 40);
			variableReference.Add("GVOther", 41);
			variableReference.Add("GArtStyle", 50);
			variableReference.Add("GHideButtons", 51);
			variableReference.Add("GADVMode", 500);
			variableReference.Add("GLinemodeSp", 501);
			variableReference.Add("GCensor", 502);
			variableReference.Add("GEffectExtend", 503);
			variableReference.Add("GAltBGM", 504);
			variableReference.Add("GAltSE", 505);
			variableReference.Add("GAltBGMflow", 506);
			variableReference.Add("GAltSEflow", 507);
			variableReference.Add("GAltVoice", 508);
			variableReference.Add("GAltVoicePriority", 509);
			variableReference.Add("GCensorMaxNum", 510);
			variableReference.Add("GEffectExtendMaxNum", 511);
			variableReference.Add("GAltBGMflowMaxNum", 512);
			variableReference.Add("GAltSEflowMaxNum", 513);
			variableReference.Add("GMOD_SETTING_LOADER", 514);
			variableReference.Add("GFlagForTest1", 515);
			variableReference.Add("GFlagForTest2", 516);
			variableReference.Add("GFlagForTest3", 517);
			variableReference.Add("NVL_in_ADV", 518);
			variableReference.Add("LFlagMonitor", 519);
			variableReference.Add("DisableModHotkey", 520);
			variableReference.Add("GMOD_DEBUG_MODE", 521);
			variableReference.Add("GLipSync", 522);
			variableReference.Add("GVideoOpening", 523);
			variableReference.Add("GADVTextbox", 526);
			variableReference.Add("GADVTextboxMaxNum", 527);
			// 611 - 619 used for additional chapter progress info
			SetGlobalFlag("GMessageSpeed", 60);
			SetGlobalFlag("GAutoSpeed", 50);
			SetGlobalFlag("GAutoAdvSpeed", 50);
			SetGlobalFlag("GWindowOpacity", 50);
			SetGlobalFlag("GUsePrompts", 1);
			SetGlobalFlag("GSlowSkip", 0);
			SetGlobalFlag("GSkipUnread", 0);
			SetGlobalFlag("GClickDuringAuto", 0);
			SetGlobalFlag("GRightClickMenu", 1);
			SetGlobalFlag("GVoiceVolume", 75);
			SetGlobalFlag("GBGMVolume", 50);
			SetGlobalFlag("GSEVolume", 50);
			SetGlobalFlag("GCutVoiceOnClick", 0);
			SetGlobalFlag("GUseSystemSound", 1);
			SetGlobalFlag("GLanguage", 1);
			SetGlobalFlag("GVChie", 1);
			SetGlobalFlag("GVEiji", 1);
			SetGlobalFlag("GVKana", 1);
			SetGlobalFlag("GVKira", 1);
			SetGlobalFlag("GVMast", 1);
			SetGlobalFlag("GVMura", 1);
			SetGlobalFlag("GVRiho", 1);
			SetGlobalFlag("GVRmn_", 1);
			SetGlobalFlag("GVSari", 1);
			SetGlobalFlag("GVTika", 1);
			SetGlobalFlag("GVYayo", 1);
			SetGlobalFlag("GVOther", 1);
			SetGlobalFlag("GArtStyle", 1);
			SetGlobalFlag("GHideButtons", 0);
			SetGlobalFlag("GLipSync", 1);
			SetFlag("LTextFade", 1);
			SetFlag("NVL_in_ADV", 0);
			SetFlag("DisableModHotkey", 0);
			SetFlag("LFlagMonitor", 0);
			Instance = this;
			LoadGlobals();
		}

		private void LoadFlags()
		{
			for (int i = 0; i < 11; i++)
			{
				flags.Add(i, 0);
			}
		}

		public void AddScope()
		{
			scopeLevel++;
		}

		public void DropScope()
		{
			int count = memorylist.Count;
			scopeLevel--;
			memorylist = (from a in memorylist
			where a.Value.Scope <= scopeLevel
			select a).ToDictionary((KeyValuePair<string, BurikoMemoryEntry> a) => a.Key, (KeyValuePair<string, BurikoMemoryEntry> a) => a.Value);
			Debug.Log($"Dropping scope changed the number of objects in memory from {count} to {memorylist.Count}");
		}

		public void ResetScope()
		{
			scopeLevel = 0;
			memorylist = (from a in memorylist
			where a.Value.Scope <= scopeLevel
			select a).ToDictionary((KeyValuePair<string, BurikoMemoryEntry> a) => a.Key, (KeyValuePair<string, BurikoMemoryEntry> a) => a.Value);
		}

		public bool SeenCG(string cg)
		{
			return cgflags.Contains(cg);
		}

		public void SetCGFlag(string cg)
		{
			if (!cgflags.Contains(cg))
			{
				cgflags.Add(cg);
			}
		}

		public void SetFlag(string flagname, int val)
		{
			if (!variableReference.TryGetValue(flagname, out int value))
			{
				throw new Exception("Unable to set flag with the name " + flagname + ", flag not found.");
			}
			if (!flags.ContainsKey(value))
			{
				flags.Add(value, val);
			}
			else
			{
				flags[value] = val;
			}
		}

		public void SetGlobalFlag(int key, int val)
		{
			if (!globalFlags.ContainsKey(key))
			{
				globalFlags.Add(key, val);
			}
			else
			{
				globalFlags[key] = val;
			}
		}

		public void SetGlobalFlag(string flagname, int val)
		{
			if (!variableReference.TryGetValue(flagname, out int key))
			{
				throw new Exception("Unable to set flag with the name " + flagname + ", flag not found.");
			}
			SetGlobalFlag(key, val);
			MODSyncState();
		}

		public void SetHighestChapterFlag(int arcNumber, int number)
		{
			if (arcNumber < 0 || arcNumber >= 10)
			{
				throw new Exception("Attempted to set highest chapter for chapter " + arcNumber + ", only 0-9 are allowed.");
			}
			if (arcNumber == 0)
			{
				SetGlobalFlag("GHighestChapter", number);
				return;
			}
			int target = arcNumber + 610;
			SetGlobalFlag(target, number);
		}

		public bool IsFlag(string name)
		{
			if (!variableReference.ContainsKey(name))
			{
				return false;
			}
			return true;
		}

		public BurikoVariable GetFlag(string flagname)
		{
			if (!variableReference.TryGetValue(flagname, out int value))
			{
				throw new Exception("Unable to get flag with the name " + flagname + ", flag not found.");
			}
			return new BurikoVariable(flags[value]);
		}

		private BurikoVariable GetGlobalFlag(int key)
		{
			if (!globalFlags.ContainsKey(key))
			{
				return new BurikoVariable(0);
			}
			return new BurikoVariable(globalFlags[key]);
		}

		public BurikoVariable GetGlobalFlag(string flagname)
		{
			if (!variableReference.TryGetValue(flagname, out int value))
			{
				throw new Exception("Unable to get global flag with the name " + flagname + ", flag not found.");
			}
			return GetGlobalFlag(value);
		}

		public BurikoVariable GetHighestChapterFlag(int arcNumber)
		{
			if (arcNumber < 0 || arcNumber >= 10)
			{
				throw new Exception("Attempted to set highest chapter for chapter " + arcNumber + ", only 0-9 are allowed.");
			}
			if (arcNumber == 0)
			{
				return GetGlobalFlag("GHighestChapter");
			}
			int target = arcNumber + 610;
			return GetGlobalFlag(target);
		}

		public int[] GetHighestChapterFlags()
		{
			return Enumerable.Range(0, 10).Select( arc => GetHighestChapterFlag(arc).IntValue() ).ToArray();
		}

		public void MarkLineAsRead(string scriptname, int line)
		{
			if (!readText.ContainsKey(scriptname))
			{
				readText.Add(scriptname, new List<int>());
			}
			List<int> list = readText[scriptname];
			if (!list.Contains(line))
			{
				list.Add(line);
			}
		}

		public bool IsLineRead(string scriptname, int line)
		{
			if (!readText.ContainsKey(scriptname))
			{
				return false;
			}
			if (readText[scriptname].Contains(line))
			{
				return true;
			}
			return false;
		}

		public bool HasReadScript(string scriptname)
		{
			return readText.ContainsKey(scriptname);
		}

		public bool IsMemory(string name)
		{
			if (!memorylist.ContainsKey(name))
			{
				return false;
			}
			return true;
		}

		public IBurikoObject GetMemory(string name)
		{
			if (!memorylist.TryGetValue(name, out BurikoMemoryEntry value))
			{
				throw new Exception("Unable to GetMemory the variable with the name " + name);
			}
			return value.Obj;
		}

		public void AddMemory(string name, IBurikoObject obj)
		{
			memorylist.Add(name, new BurikoMemoryEntry(scopeLevel, obj));
		}

		public byte[] SaveMemory()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					BsonWriter bsonWriter = new BsonWriter(memoryStream);
					bsonWriter.CloseOutput = false;
					using (BsonWriter jsonWriter = bsonWriter)
					{
						JsonSerializer jsonSerializer = new JsonSerializer();
						binaryWriter.Write(memorylist.Count);
						foreach (KeyValuePair<string, BurikoMemoryEntry> item in memorylist)
						{
							binaryWriter.Write(item.Key);
							binaryWriter.Write(item.Value.Scope);
							binaryWriter.Write(item.Value.Obj.GetObjectType());
							item.Value.Obj.Serialize(memoryStream);
						}
						jsonSerializer.Serialize(jsonWriter, variableReference);
						jsonSerializer.Serialize(jsonWriter, flags);
						return memoryStream.ToArray();
					}
				}
			}
		}

		public void LoadMemory(MemoryStream ms)
		{
			memorylist.Clear();
			flags.Clear();
			BinaryReader binaryReader = new BinaryReader(ms);
			JsonSerializer jsonSerializer = new JsonSerializer();
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string key = binaryReader.ReadString();
				int scope = binaryReader.ReadInt32();
				string text = binaryReader.ReadString();
				IBurikoObject burikoObject;
				switch (text)
				{
				case "String":
					burikoObject = new BurikoString();
					break;
				case "MtnCtrlElement":
					burikoObject = new BurikoMtnCtrlElement();
					break;
				case "Integer":
					burikoObject = new BurikoInt();
					break;
				case "Vector":
					burikoObject = new BurikoVector();
					break;
				default:
					throw new InvalidDataException("Cannot populate Buriko Object of type " + text);
				}
				burikoObject.DeSerialize(ms);
				memorylist.Add(key, new BurikoMemoryEntry(scope, burikoObject));
			}
			BsonReader bsonReader = new BsonReader(ms);
			bsonReader.CloseInput = false;
			using (BsonReader reader = bsonReader)
			{
				variableReference = jsonSerializer.Deserialize<Dictionary<string, int>>(reader);
			}
			bsonReader = new BsonReader(ms);
			bsonReader.CloseInput = false;
			using (BsonReader reader2 = bsonReader)
			{
				flags = jsonSerializer.Deserialize<Dictionary<int, int>>(reader2);
			}
		}

		public void LoadGlobals()
		{
			string path = Path.Combine(MGHelper.GetSavePath(), "global.dat");
			if (!File.Exists(path))
			{
				SetGlobalFlag("GUsePrompts", 1);
			}
			else
			{
				byte[] array = File.ReadAllBytes(path);
				MGHelper.KeyEncode(array);
				byte[] buffer = CLZF2.Decompress(array);
				try
				{
					JsonSerializer jsonSerializer = new JsonSerializer();
					using (MemoryStream stream = new MemoryStream(buffer))
					{
						BsonReader bsonReader = new BsonReader(stream);
						bsonReader.CloseInput = false;
						using (BsonReader reader = bsonReader)
						{
							// was: globalFlags = jsonSerializer.Deserialize<Dictionary<int, int>>(reader);
							// if global.dat exists but a new build introduced a new global with a default value, then the default value would be overwritten.
							// Replace each key-val pair instead
							var persistedGlobalFlags = jsonSerializer.Deserialize<Dictionary<int, int>>(reader);
							persistedGlobalFlags.ToList().ForEach(x => globalFlags[x.Key] = x.Value);
						}
						bsonReader = new BsonReader(stream);
						bsonReader.CloseInput = false;
						bsonReader.ReadRootValueAsArray = true;
						using (BsonReader reader2 = bsonReader)
						{
							cgflags = jsonSerializer.Deserialize<List<string>>(reader2);
						}
						bsonReader = new BsonReader(stream);
						bsonReader.CloseInput = false;
						using (BsonReader reader3 = bsonReader)
						{
							readText = jsonSerializer.Deserialize<Dictionary<string, List<int>>>(reader3);
						}
					}
				}
				catch (Exception arg)
				{
					Debug.LogWarning("Failed to load global data! Exception: " + arg);
					return;
					IL_0128:;
				}
				try
				{
					GameSystem.Instance.TextController.TextSpeed = GetGlobalFlag("GMessageSpeed").IntValue();
					GameSystem.Instance.TextController.AutoSpeed = GetGlobalFlag("GAutoSpeed").IntValue();
					GameSystem.Instance.TextController.AutoPageSpeed = GetGlobalFlag("GAutoAdvSpeed").IntValue();
					GameSystem.Instance.MessageWindowOpacity = (float)GetGlobalFlag("GWindowOpacity").IntValue() / 100f;
					GameSystem.Instance.UsePrompts = GetGlobalFlag("GUsePrompts").BoolValue();
					GameSystem.Instance.SkipModeDelay = GetGlobalFlag("GSlowSkip").BoolValue();
					GameSystem.Instance.SkipUnreadMessages = GetGlobalFlag("GSkipUnread").BoolValue();
					GameSystem.Instance.ClickDuringAuto = GetGlobalFlag("GClickDuringAuto").BoolValue();
					GameSystem.Instance.RightClickMenu = GetGlobalFlag("GRightClickMenu").BoolValue();
					GameSystem.Instance.AudioController.VoiceVolume = (float)GetGlobalFlag("GVoiceVolume").IntValue() / 100f;
					GameSystem.Instance.AudioController.BGMVolume = (float)GetGlobalFlag("GBGMVolume").IntValue() / 100f;
					GameSystem.Instance.AudioController.SoundVolume = (float)GetGlobalFlag("GSEVolume").IntValue() / 100f;
					GameSystem.Instance.AudioController.SystemVolume = (float)GetGlobalFlag("GSEVolume").IntValue() / 100f;
					GameSystem.Instance.StopVoiceOnClick = GetGlobalFlag("GCutVoiceOnClick").BoolValue();
					GameSystem.Instance.UseSystemSounds = GetGlobalFlag("GUseSystemSound").BoolValue();
					GameSystem.Instance.UseEnglishText = GetGlobalFlag("GLanguage").BoolValue();
					AssetManager.Instance.UseNewArt = GetGlobalFlag("GArtStyle").BoolValue();
					GameSystem.Instance.AudioController.RefreshLayerVolumes();
				}
				catch (Exception message)
				{
					Debug.LogWarning(message);
				}
			}
		}

		public void SaveGlobals()
		{
			byte[] inputBytes;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BsonWriter jsonWriter = new BsonWriter(memoryStream))
				{
					JsonSerializer jsonSerializer = new JsonSerializer();
					jsonSerializer.Serialize(jsonWriter, globalFlags);
					jsonSerializer.Serialize(jsonWriter, cgflags);
					jsonSerializer.Serialize(jsonWriter, readText);
					inputBytes = memoryStream.ToArray();
				}
			}
			byte[] array = CLZF2.Compress(inputBytes);
			MGHelper.KeyEncode(array);
			File.WriteAllBytes(Path.Combine(MGHelper.GetSavePath(), "global.dat"), array);
		}

		/// <summary>
		/// Syncs internal state with global flags.  We could technically add all the stuff in LoadGlobals() if we wanted to - just seemed like a lot of overhead.
		/// </summary>
		public void MODSyncState()
		{
			// Sync Art Style.  This is really set up to support only init.txt initialization
			AssetManager.Instance.UseNewArt = GetGlobalFlag("GArtStyle").BoolValue();
		}
	}
}
