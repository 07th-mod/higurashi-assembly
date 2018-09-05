using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko.Util;
using Assets.Scripts.Core.Buriko.VarTypes;
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
		private Dictionary<string, BurikoMemoryEntry> memorylist = new Dictionary<string, BurikoMemoryEntry>();

		private Dictionary<string, int> variableReference = new Dictionary<string, int>();

		private Dictionary<int, int> flags = new Dictionary<int, int>();

		private Dictionary<int, int> globalFlags = new Dictionary<int, int>();

		private Dictionary<string, List<int>> readText = new Dictionary<string, List<int>>();

		private List<string> cgflags = new List<string>();

		private const int FlagCount = 11;

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
			variableReference.Add("s_jump", 3);
			variableReference.Add("ChapterNumber", 10);
			variableReference.Add("LOnikakushiDay", 30);
			variableReference.Add("LWatanagashiDay", 32);
			variableReference.Add("LTatarigoroshiDay", 33);
			variableReference.Add("LHimatsubushiDay", 33);
			variableReference.Add("LTextFade", 31);
			variableReference.Add("NewTipsStart", 50);
			variableReference.Add("NewTipsCount", 51);
			variableReference.Add("TipsCount", 52);
			variableReference.Add("GFlag_FirstPlay", 0);
			variableReference.Add("GFlag_GameClear", 1);
			variableReference.Add("GQsaveNum", 2);
			variableReference.Add("GOnikakushiDay", 3);
			variableReference.Add("GWatanagashiDay", 4);
			variableReference.Add("GTatarigoroshiDay", 5);
			variableReference.Add("GHimatsubushiDay", 6);
			variableReference.Add("redbox", 7);
			variableReference.Add("bluebox", 8);
			variableReference.Add("GTotalTips", 9);
			variableReference.Add("GHighestChapter", 10);
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
			variableReference.Add("GFlagMonitor", 519);
			variableReference.Add("DisableModHotkey", 520);
			variableReference.Add("GMOD_DEBUG_MODE", 521);
			variableReference.Add("GLipSync", 522);
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
			SetGlobalFlag("GArtStyle", 1);
			SetGlobalFlag("GHideButtons", 0);
			SetGlobalFlag("GLipSync", 1);
			SetFlag("LTextFade", 1);
			SetFlag("NVL_in_ADV", 0);
			SetFlag("DisableModHotkey", 0);
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

		public void SetGlobalFlag(string flagname, int val)
		{
			if (!variableReference.TryGetValue(flagname, out int value))
			{
				throw new Exception("Unable to set flag with the name " + flagname + ", flag not found.");
			}
			if (!globalFlags.ContainsKey(value))
			{
				globalFlags.Add(value, val);
			}
			else
			{
				globalFlags[value] = val;
			}
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
			if (!flags.ContainsKey(value))
			{
				return new BurikoVariable(0);
			}
			return new BurikoVariable(flags[value]);
		}

		public BurikoVariable GetGlobalFlag(string flagname)
		{
			if (!variableReference.TryGetValue(flagname, out int value))
			{
				throw new Exception("Unable to get global flag with the name " + flagname + ", flag not found.");
			}
			if (!globalFlags.ContainsKey(value))
			{
				return new BurikoVariable(0);
			}
			return new BurikoVariable(globalFlags[value]);
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
	}
}
