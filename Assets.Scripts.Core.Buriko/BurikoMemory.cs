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
			memorylist = memorylist.Where((KeyValuePair<string, BurikoMemoryEntry> a) => a.Value.Scope <= scopeLevel).ToDictionary((KeyValuePair<string, BurikoMemoryEntry> a) => a.Key, (KeyValuePair<string, BurikoMemoryEntry> a) => a.Value);
			Debug.Log($"Dropping scope changed the number of objects in memory from {count} to {memorylist.Count}");
		}

		public void ResetScope()
		{
			scopeLevel = 0;
			memorylist = memorylist.Where((KeyValuePair<string, BurikoMemoryEntry> a) => a.Value.Scope <= scopeLevel).ToDictionary((KeyValuePair<string, BurikoMemoryEntry> a) => a.Key, (KeyValuePair<string, BurikoMemoryEntry> a) => a.Value);
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

		public void SetFragmentValueStatus(int fragmentId, int value)
		{
			SetFlag("FragmentStatus" + fragmentId.ToString("D2"), value);
		}

		public void SetFragmentReadStatus(int fragmentId)
		{
			SetFlag("FragmentRead" + fragmentId.ToString("D2"), 1);
		}

		public int GetFragmentValueStatus(int fragmentId)
		{
			return GetFlag("FragmentStatus" + fragmentId.ToString("D2")).IntValue();
		}

		public int GetFragmentReadStatus(int fragmentId)
		{
			return GetFlag("FragmentRead" + fragmentId.ToString("D2")).IntValue();
		}

		public void ResetFlags()
		{
			flags.Clear();
			Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			Instance.SetFlag("LTextColor", color.ToInt());
			Instance.SetFlag("LTextFade", 1);
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
					using (BsonWriter jsonWriter = new BsonWriter(memoryStream)
					{
						CloseOutput = false
					})
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
					throw new Exception("Cannot populate Buriko Object of type " + text);
				}
				burikoObject.DeSerialize(ms);
				memorylist.Add(key, new BurikoMemoryEntry(scope, burikoObject));
			}
			using (BsonReader reader = new BsonReader(ms)
			{
				CloseInput = false
			})
			{
				jsonSerializer.Deserialize<Dictionary<string, int>>(reader);
			}
			using (BsonReader reader2 = new BsonReader(ms)
			{
				CloseInput = false
			})
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
				return;
			}
			byte[] array = File.ReadAllBytes(path);
			MGHelper.KeyEncode(array);
			byte[] buffer = CLZF2.Decompress(array);
			try
			{
				JsonSerializer jsonSerializer = new JsonSerializer();
				using (MemoryStream stream = new MemoryStream(buffer))
				{
					using (BsonReader reader = new BsonReader(stream)
					{
						CloseInput = false
					})
					{
						globalFlags = jsonSerializer.Deserialize<Dictionary<int, int>>(reader);
					}
					using (BsonReader reader2 = new BsonReader(stream)
					{
						CloseInput = false,
						ReadRootValueAsArray = true
					})
					{
						cgflags = jsonSerializer.Deserialize<List<string>>(reader2);
					}
					using (BsonReader reader3 = new BsonReader(stream)
					{
						CloseInput = false
					})
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
			Debug.Log(string.Format("Saikoroshi: {0} Hirukowashi: {1} Batsukoishi: {2}", GetGlobalFlag("GSaikoroshi"), GetGlobalFlag("GHirukowashi"), GetGlobalFlag("GBatsukoishi")));
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

		public BurikoMemory()
		{
			memorylist = new Dictionary<string, BurikoMemoryEntry>();
			foreach (BurikoFlagInfo item in JsonConvert.DeserializeObject<List<BurikoFlagInfo>>(AssetManager.Instance.LoadTextDataString("localflags.txt")))
			{
				if (variableReference.ContainsKey(item.Name))
				{
					Debug.LogError("Local variable " + item.Name + " already exists!");
				}
				else
				{
					variableReference.Add(item.Name, item.Id);
				}
			}
			foreach (BurikoFlagInfo item2 in JsonConvert.DeserializeObject<List<BurikoFlagInfo>>(AssetManager.Instance.LoadTextDataString("globalflags.txt")))
			{
				if (variableReference.ContainsKey(item2.Name))
				{
					Debug.LogError("Local variable " + item2.Name + " already exists!");
				}
				else
				{
					variableReference.Add(item2.Name, item2.Id);
				}
			}
			SetGlobalFlag("GMessageSpeed", 50);
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
			SetGlobalFlag("GLastSavePage", 0);
			SetFlag("LTextFade", 1);
			SetFlag("LTextColor", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue).ToInt());
			Instance = this;
			LoadGlobals();
		}
	}
}
