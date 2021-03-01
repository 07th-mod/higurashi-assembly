using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko.Util;
using Assets.Scripts.Core.Buriko.VarTypes;
using Assets.Scripts.Core.Scene;
using Assets.Scripts.Core.State;
using Assets.Scripts.UI.Prompt;
using MOD.Scripts.Core;
using MOD.Scripts.Core.Audio;
using MOD.Scripts.Core.Scene;
using MOD.Scripts.Core.State;
using MOD.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.Buriko
{
	public class BurikoScriptFile
	{
		private readonly GameSystem gameSystem;

		private readonly BurikoScriptSystem scriptSystem;

		public string Filename;

		private Dictionary<string, int> blockLookup = new Dictionary<string, int>();

		private Dictionary<int, int> lineLookup = new Dictionary<int, int>();

		private byte[] dataSegment;

		private MemoryStream dataStream;

		private BinaryReader dataReader;

		private int dataSegmentLength;

		public int LineNum;

		private string opType = string.Empty;

		private int param;

		public bool IsInitialized;

		public int Position => (int)dataStream.Position;

		public BurikoScriptFile(BurikoScriptSystem system, string filename)
		{
			scriptSystem = system;
			gameSystem = GameSystem.Instance;
			Filename = filename;
		}

		public void InitializeScript()
		{
			byte[] scriptData = AssetManager.Instance.GetScriptData(Filename);
			MemoryStream memoryStream = new MemoryStream(scriptData);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			string a = new string(binaryReader.ReadChars(4));
			if (a != "MGSC")
			{
				throw new FileLoadException("The script file " + Filename + ".mg does not appear to be a valid script!");
			}
			int num = binaryReader.ReadInt32();
			if (num != 1)
			{
				throw new FileLoadException("The script file " + Filename + ".mg is an incompatible script version!");
			}
			int num2 = binaryReader.ReadInt32();
			int num3 = binaryReader.ReadInt32();
			dataSegmentLength = binaryReader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				blockLookup.Add(binaryReader.ReadString(), binaryReader.ReadInt32());
			}
			for (int j = 0; j < num3; j++)
			{
				lineLookup.Add(j, binaryReader.ReadInt32());
			}
			dataSegment = binaryReader.ReadBytes(dataSegmentLength);
			binaryReader.Close();
			memoryStream.Close();
			dataStream = new MemoryStream(dataSegment);
			dataReader = new BinaryReader(dataStream);
			JumpToBlock("main");
			IsInitialized = true;
			LineNum = 0;
		}

		public void Uninitialize()
		{
			if (IsInitialized)
			{
				blockLookup = new Dictionary<string, int>();
				lineLookup = new Dictionary<int, int>();
				dataSegment = null;
				if (dataReader != null)
				{
					dataReader.Close();
				}
				if (dataStream != null)
				{
					dataStream.Close();
				}
				dataReader = null;
				dataStream = null;
				IsInitialized = false;
			}
		}

		public void JumpToBlock(string blockname)
		{
			int value = 0;
			if (!blockLookup.TryGetValue(blockname, out value))
			{
				throw new KeyNotFoundException($"Unable to find a block segment with the name '{blockname}' within the script '{Filename}'.");
			}
			JumpToPosition(value);
		}

		public void JumpToPosition(int newposition)
		{
			if (newposition > dataSegmentLength)
			{
				throw new IndexOutOfRangeException($"Attempting to jump to script position {Position} within the script '{Filename}', but the script only has a length of '{dataSegmentLength}.");
			}
			dataStream.Seek(newposition, SeekOrigin.Begin);
		}

		public void JumpToLineNum(int linenum)
		{
			if (linenum > lineLookup.Count)
			{
				throw new IndexOutOfRangeException($"Attempting to jump to script line {linenum} within the script '{Filename}', but the script only has a length of '{lineLookup.Count}.");
			}
			dataStream.Seek(lineLookup[linenum], SeekOrigin.Begin);
			LineNum = linenum;
		}

		private void SetOperationType(string type)
		{
			opType = type;
			param = 0;
		}

		private BurikoVariable ReadVariable()
		{
			param++;
			BurikoVariable result = null;
			try
			{
				result = new BurikoVariable(dataReader);
				return result;
			}
			catch (Exception arg)
			{
				string message = $"ReadVariable: Operation {opType} Parameter {param} could not be read. Generated exception: {arg}";
				ScriptError(message);
				return result;
			}
		}

		private float D2BGetPosition(int posnum)
		{
			float result = 0f;
			switch (posnum)
			{
			case 2:
			case 4:
				result = 180f;
				break;
			case 3:
				result = -180f;
				break;
			case 6:
				result = 220f;
				break;
			case 7:
				result = -220f;
				break;
			case 9:
				result = 100f;
				break;
			case 10:
				result = -100f;
				break;
			case 11:
				result = -300f;
				break;
			case 12:
				result = 300f;
				break;
			}
			return result;
		}

		private BurikoVariable OperationStoreValueToLocalWork()
		{
			SetOperationType("StoreValueToLocalWork");
			string flagname = ReadVariable().VariableName();
			int val = ReadVariable().IntValue();
			BurikoMemory.Instance.SetFlag(flagname, val);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationLoadValueFromLocalWork()
		{
			SetOperationType("LoadValueFromLocalWork");
			string flagname = ReadVariable().VariableName();
			return BurikoMemory.Instance.GetFlag(flagname);
		}

		private BurikoVariable OperationCallScript()
		{
			SetOperationType("CallScript");
			string scriptname = ReadVariable().StringValue();
			scriptSystem.CallScript(scriptname);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationJumpScript()
		{
			SetOperationType("JumpScript");
			string scriptname = ReadVariable().StringValue();
			scriptSystem.JumpToScript(scriptname);
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationCallSection()
		{
			SetOperationType("CallSection");
			string blockname = ReadVariable().StringValue();
			scriptSystem.CallBlock(blockname);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationJumpSection()
		{
			SetOperationType("JumpSection");
			string blockname = ReadVariable().StringValue();
			scriptSystem.JumpToBlock(blockname);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetLocalFlag()
		{
			SetOperationType("SetLocalFlag");
			string flagname = ReadVariable().VariableName();
			int val = ReadVariable().IntValue();
			BurikoMemory.Instance.SetFlag(flagname, val);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetGlobalFlag()
		{
			SetOperationType("SetGlobalFlag");
			string flagname = ReadVariable().VariableName();
			int val = ReadVariable().IntValue();
			BurikoMemory.Instance.SetGlobalFlag(flagname, val);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationGetGlobalFlag()
		{
			SetOperationType("LoadValueFromLocalWork");
			string flagname = ReadVariable().VariableName();
			return BurikoMemory.Instance.GetGlobalFlag(flagname);
		}

		private BurikoVariable OperationWait()
		{
			SetOperationType("Wait");
			float length = (float)ReadVariable().IntValue() / 1000f;
			gameSystem.AddWait(new Wait(length, WaitTypes.WaitForTime, null));
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationWaitForInput()
		{
			SetOperationType("WaitForInput");
			gameSystem.TextController.AddWaitToFinishTyping();
			gameSystem.ExecuteActions();
			gameSystem.AddWait(new Wait(0f, WaitTypes.WaitForInput, null));
			if (gameSystem.IsAuto)
			{
				gameSystem.TextController.SetAutoTextWait();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetValidityOfInput()
		{
			SetOperationType("SetValidityOfInput");
			bool canInput = ReadVariable().BoolValue();
			gameSystem.CanInput = canInput;
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetValidityOfSaving()
		{
			SetOperationType("SetValidityOfSaving");
			bool canSave = ReadVariable().BoolValue();
			gameSystem.CanSave = canSave;
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetValidityOfSkipping()
		{
			SetOperationType("SetValidityOfSkipping");
			bool canSkip = ReadVariable().BoolValue();
			gameSystem.CanSkip = canSkip;
			if (gameSystem.IsSkipping)
			{
				gameSystem.IsSkipping = false;
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetValidityOfUserEffectSpeed()
		{
			SetOperationType("SetValidityOfUserEffectSpeed");
			ReadVariable().BoolValue();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationOutputLine()
		{
			SetOperationType("OutputLine");
			string text = string.Empty;
			string text2 = string.Empty;
			BurikoVariable burikoVariable = ReadVariable();
			if (burikoVariable.Type != BurikoValueType.Null)
			{
				text = burikoVariable.StringValue();
			}
			string text3 = ReadVariable().StringValue();
			BurikoVariable burikoVariable2 = ReadVariable();
			if (burikoVariable2.Type != BurikoValueType.Null)
			{
				text2 = burikoVariable2.StringValue();
			}
			string text4 = ReadVariable().StringValue();
			int num = ReadVariable().IntValue();
			BurikoTextModes textMode = (BurikoTextModes)num;
			text3 = text3.Replace("\\n", "\n");
			gameSystem.TextHistory.RegisterLine(text4, text3, text2, text);
			gameSystem.TextController.SetText(text, text3, textMode, 1);
			gameSystem.TextController.SetText(text2, text4, textMode, 2);
			gameSystem.ExecuteActions();
			scriptSystem.TakeSaveSnapshot(string.Empty);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationOutputLineAll()
		{
			SetOperationType("OutputLineAll");
			string text = string.Empty;
			BurikoVariable burikoVariable = ReadVariable();
			if (burikoVariable.Type != BurikoValueType.Null)
			{
				text = burikoVariable.StringValue();
			}
			string text2 = ReadVariable().StringValue();
			BurikoTextModes burikoTextModes = (BurikoTextModes)ReadVariable().IntValue();
			text2 = text2.Replace("\\n", "\n");
			gameSystem.TextHistory.RegisterLine(text2, text2, text, text);
			if (burikoTextModes == BurikoTextModes.Normal)
			{
				gameSystem.TextHistory.PushHistory();
			}
			gameSystem.TextController.SetText(text, text2, burikoTextModes, 0);
			gameSystem.ExecuteActions();
			scriptSystem.TakeSaveSnapshot(string.Empty);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationClearMessage()
		{
			SetOperationType("ClearMessage");
			gameSystem.TextController.ClearText();
			gameSystem.TextHistory.PushHistory();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetFontOfMessage()
		{
			SetOperationType("SetFontOfMessage");
			ReadVariable().IntValue();
			ReadVariable().IntValue();
			ReadVariable().IntValue();
			Debug.Log("SetFontOfMessage");
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetSpeedOfMessage()
		{
			SetOperationType("SetSpeedOfMessage");
			bool flag = ReadVariable().BoolValue();
			int num = ReadVariable().IntValue();
			if (!flag)
			{
				gameSystem.TextController.OverrideTextSpeed = -1;
			}
			else
			{
				gameSystem.TextController.OverrideTextSpeed = 50 * (num / 100);
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDisableWindow()
		{
			SetOperationType("DisableWindow");
			if (gameSystem.IsSkipping)
			{
				gameSystem.MainUIController.FadeOut(0f, isBlocking: false);
				gameSystem.SceneController.FadeFace(0f, isblocking: false);
			}
			else
			{
				gameSystem.MainUIController.FadeOut(0.5f, isBlocking: true);
				gameSystem.SceneController.FadeFace(0.5f, isblocking: true);
			}
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSpringText()
		{
			ReadVariable().IntValue();
			ReadVariable().IntValue();
			Logger.LogWarning("Operation SpringText not implemented!");
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetStyleOfMessageSwinging()
		{
			ReadVariable().IntValue();
			return BurikoVariable.Null;
		}

		private void PlayBGMCommon(out string filename, out int channel, out float volume, out float fadeInTime)
		{
			SetOperationType("PlayBGM");
			channel = ReadVariable().IntValue();
			filename = ReadVariable().StringValue() + ".ogg";
			volume = (float)ReadVariable().IntValue() / 128f;
			fadeInTime = (float)ReadVariable().IntValue();
		}

		private BurikoVariable OperationPlayBGM()
		{
			PlayBGMCommon(out string filename, out int channel, out float volume, out float fadeInTime);
			AudioController.Instance.PlayAudio(filename, Assets.Scripts.Core.Audio.AudioType.BGM, channel, volume, fadeInTime);
			return BurikoVariable.Null;
		}
		private BurikoVariable OperationMODPlayBGM()
		{
			PlayBGMCommon(out string filename, out int channel, out float volume, out float fadeInTime);

			// OperationMODPlayBGM accepts one extra argument - the GAltBGMflow setting where this bgm should be played
			int targetBGMFlow = ReadVariable().IntValue();

			MODAudioTracking.Instance.SaveLastAltBGM(targetBGMFlow, new AudioInfo(volume, filename, channel));

			if(BurikoMemory.Instance.GetGlobalFlag("GAltBGMflow").IntValue() == targetBGMFlow)
			{
				AudioController.Instance.PlayAudio(filename, Assets.Scripts.Core.Audio.AudioType.BGM, channel, volume, fadeInTime, noBGMTracking: true);
			}

			return BurikoVariable.Null;
		}

		private BurikoVariable OperationStopBGM()
		{
			SetOperationType("StopBGM");
			int channel = ReadVariable().IntValue();
			AudioController.Instance.StopBGM(channel);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationChangeVolumeOfBGM()
		{
			SetOperationType("ChangeVolumeOfBGM");
			int channel = ReadVariable().IntValue();
			float volume = (float)ReadVariable().IntValue() / 128f;
			int num = ReadVariable().IntValue();
			AudioController.Instance.ChangeVolumeOfBGM(channel, volume, (float)num);
			return BurikoVariable.Null;
		}

		private void FadeBGMCommon(out int channel, out int time, out bool waitForFade)
		{
			SetOperationType("FadeOutBGM");
			channel = ReadVariable().IntValue();
			time = ReadVariable().IntValue();
			waitForFade = ReadVariable().BoolValue();
		}

		private BurikoVariable OperationFadeOutBGM()
		{
			FadeBGMCommon(out int channel, out int time, out bool waitForFade);
			if (gameSystem.IsSkipping)
			{
				AudioController.Instance.StopBGM(channel);
			}
			else
			{
				AudioController.Instance.FadeOutBGM(channel, time, waitForFade);
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationMODFadeOutBGM()
		{
			FadeBGMCommon(out int channel, out int time, out bool waitForFade);
			int targetBGMFlow = ReadVariable().IntValue();

			MODAudioTracking.Instance.ForgetLastAltBGM(targetBGMFlow, channel);

			if (BurikoMemory.Instance.GetGlobalFlag("GAltBGMflow").IntValue() == targetBGMFlow)
			{
				if (gameSystem.IsSkipping)
				{
					AudioController.Instance.StopBGM(channel, noBGMTracking: true);
				}
				else
				{
					AudioController.Instance.FadeOutBGM(channel, time, waitForFade, noBGMTracking: true);
				}
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeOutMultiBGM()
		{
			SetOperationType("FadeOutMultiBGM");
			int channelstart = ReadVariable().IntValue();
			int channelend = ReadVariable().IntValue();
			int time = ReadVariable().IntValue();
			bool waitForFade = ReadVariable().BoolValue();
			AudioController.Instance.FadeOutMultiBGM(channelstart, channelend, time, waitForFade);
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationPlaySE()
		{
			SetOperationType("PlaySE");
			int channel = ReadVariable().IntValue();
			string filename = ReadVariable().StringValue() + ".ogg";
			float volume = (float)ReadVariable().IntValue() / 128f;
			float pan = (float)ReadVariable().IntValue() / 128f;
			AudioController.Instance.PlaySE(filename, channel, volume, pan);
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationStopSE()
		{
			SetOperationType("StopSE");
			int channel = ReadVariable().IntValue();
			AudioController.Instance.StopSE(channel);
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationFadeOutSE()
		{
			SetOperationType("FadeOutSE");
			int channel = ReadVariable().IntValue();
			float time = (float)ReadVariable().IntValue() / 1000f;
			AudioController.Instance.FadeOutSE(channel, time, waitForFade: false);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationPlayVoice()
		{
			SetOperationType("PlayVoice");
			int channel = ReadVariable().IntValue();
			string filename = ReadVariable().StringValue() + ".ogg";
			float volume = (float)ReadVariable().IntValue() / 128f;
			GameSystem.Instance.TextHistory.RegisterVoice(new AudioInfo(volume, filename, channel));
			AudioController.Instance.PlayVoice(filename, channel, volume);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationWaitToFinishSEPlaying()
		{
			SetOperationType("WaitToFinishSEPlaying");
			int channel = ReadVariable().IntValue();
			if (gameSystem.IsSkipping)
			{
				return new BurikoVariable(0);
			}
			if (gameSystem.AudioController.IsSEPlaying(channel))
			{
				float remainingSEPlayTime = gameSystem.AudioController.GetRemainingSEPlayTime(channel);
				gameSystem.AddWait(new Wait(remainingSEPlayTime, WaitTypes.WaitForAudio, null));
			}
			return new BurikoVariable(1);
		}

		private BurikoVariable OperationWaitToFinishVoicePlaying()
		{
			SetOperationType("WaitToFinishVoicePlaying");
			int channel = ReadVariable().IntValue();
			if (gameSystem.IsSkipping)
			{
				return new BurikoVariable(0);
			}
			if (gameSystem.AudioController.IsVoicePlaying(channel))
			{
				float remainingVoicePlayTime = gameSystem.AudioController.GetRemainingVoicePlayTime(channel);
				gameSystem.AddWait(new Wait(remainingVoicePlayTime, WaitTypes.WaitForAudio, null));
			}
			return new BurikoVariable(1);
		}

		private BurikoVariable OperationSelect()
		{
			int num = ReadVariable().IntValue();
			BurikoReference burikoReference = ReadVariable().VariableValue();
			BurikoString burikoString = BurikoMemory.Instance.GetMemory(burikoReference.Property) as BurikoString;
			if (burikoString == null)
			{
				throw new Exception("Unable to read BurikoString type from variable!");
			}
			List<string> stringList = burikoString.GetStringList();
			if (stringList.Count < num)
			{
				throw new Exception("Can't display options, two few options are present for the amount to be displayed!");
			}
			gameSystem.DisplayChoices(stringList, num);
			gameSystem.ExecuteActions();
			gameSystem.AddWait(new Wait(1f, WaitTypes.WaitForTime, null));
			return null;
		}

		private BurikoVariable OperationPreloadBitmap()
		{
			SetOperationType("PreloadBitmap");
			ReadVariable().StringValue();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationStartShakingOfWindow()
		{
			SetOperationType("StartShakingOfWindow");
			float speed = (float)ReadVariable().IntValue() / 1000f;
			int level = ReadVariable().IntValue();
			int attenuation = ReadVariable().IntValue();
			int vector = ReadVariable().IntValue();
			int loopcount = ReadVariable().IntValue();
			bool isblocking = ReadVariable().BoolValue();
			gameSystem.MainUIController.ShakeScene(speed, level, attenuation, vector, loopcount, isblocking);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationStartShakingOfAllObjects()
		{
			SetOperationType("StartShakingOfAllObjects");
			float speed = (float)ReadVariable().IntValue() / 1000f;
			int level = ReadVariable().IntValue();
			int attenuation = ReadVariable().IntValue();
			int vector = ReadVariable().IntValue();
			int loopcount = ReadVariable().IntValue();
			bool isblocking = ReadVariable().BoolValue();
			gameSystem.SceneController.ShakeScene(speed, level, attenuation, vector, loopcount, isblocking);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationStartShakingOfBustshot()
		{
			SetOperationType("StartShakingOfBustshot");
			int layer = ReadVariable().IntValue();
			float speed = (float)ReadVariable().IntValue() / 1000f;
			int level = ReadVariable().IntValue();
			int attenuation = ReadVariable().IntValue();
			int vector = ReadVariable().IntValue();
			int loopcount = ReadVariable().IntValue();
			bool isblocking = ReadVariable().BoolValue();
			gameSystem.SceneController.ShakeBustshot(layer, speed, level, attenuation, vector, loopcount, isblocking);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationTerminateShakingOfWindow()
		{
			SetOperationType("TerminateShakingOfWindow");
			ReadVariable().IntValue();
			ReadVariable().BoolValue();
			gameSystem.MainUIController.StopShake();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationTerminateShakingOfAllObjects()
		{
			SetOperationType("TerminateShakingOfAllObjects");
			ReadVariable().IntValue();
			ReadVariable().BoolValue();
			gameSystem.SceneController.StopSceneShake();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationTerminateShakingOfBustshot()
		{
			SetOperationType("TerminateShakingOfBustshot");
			int layer = ReadVariable().IntValue();
			ReadVariable().IntValue();
			ReadVariable().BoolValue();
			gameSystem.SceneController.StopBustshotShake(layer);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationShakeScreen()
		{
			SetOperationType("ShakeScreen");
			int vector = ReadVariable().IntValue();
			int level = ReadVariable().IntValue();
			float num = (float)ReadVariable().IntValue() / 1000f;
			int loopcount = ReadVariable().IntValue();
			int attenuation = ReadVariable().IntValue();
			num *= 2f;
			gameSystem.SceneController.ShakeScene(num, level, attenuation, vector, loopcount, isblocking: true);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationShakeScreenSx()
		{
			SetOperationType("ShakeScreenSx");
			int vector = ReadVariable().IntValue();
			int level = ReadVariable().IntValue();
			float speed = 1f;
			int loopcount = 30;
			int attenuation = 5;
			if (GameSystem.Instance.IsAuto)
			{
				loopcount = 2;
			}
			gameSystem.SceneController.ShakeScene(speed, level, attenuation, vector, loopcount, isblocking: true);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDrawBG()
		{
			SetOperationType("DrawBG");
			string texture = ReadVariable().StringValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.DrawBG(texture, wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeBG()
		{
			SetOperationType("FadeBG");
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.DrawBG("black", wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDrawScene()
		{
			SetOperationType("DrawScene");
			string text = ReadVariable().StringValue();
			float time = (float)ReadVariable().IntValue() / 1000f;
			MODSystem.instance.modSceneController.MODLipSyncDisableAll();
			MODSystem.instance.modTextureController.Initialize();
			if (gameSystem.IsSkipping)
			{
				time = 0f;
			}
			gameSystem.SceneController.DrawScene(text, time);
			gameSystem.ExecuteActions();
			BurikoMemory.Instance.SetCGFlag(text);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDrawSceneWithMask()
		{
			SetOperationType("DrawSceneWithMask");
			string text = ReadVariable().StringValue();
			string maskname = ReadVariable().StringValue();
			ReadVariable().IntValue();
			ReadVariable().IntValue();
			float time = (float)ReadVariable().IntValue() / 1000f;
			MODSystem.instance.modSceneController.MODLipSyncDisableAll();
			MODSystem.instance.modTextureController.Initialize();
			if (gameSystem.IsSkipping)
			{
				time = 0f;
			}
			gameSystem.SceneController.DrawSceneWithMask(text, maskname, time);
			gameSystem.ExecuteActions();
			BurikoMemory.Instance.SetCGFlag(text);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationChangeScene()
		{
			SetOperationType("ChangeScene");
			string text = ReadVariable().StringValue();
			ReadVariable().IntValue();
			float time = (float)ReadVariable().IntValue() / 1000f;
			MODSystem.instance.modSceneController.MODLipSyncDisableAll();
			if (gameSystem.IsSkipping)
			{
				time = 0f;
			}
			gameSystem.SceneController.DrawScene(text, time);
			gameSystem.ExecuteActions();
			BurikoMemory.Instance.SetCGFlag(text);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeScene()
		{
			SetOperationType("FadeScene");
			float time = (float)ReadVariable().IntValue() / 1000f;
			MODSystem.instance.modSceneController.MODLipSyncDisableAll();
			if (gameSystem.IsSkipping)
			{
				time = 0f;
			}
			gameSystem.SceneController.DrawScene("black", time);
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeSceneWithMask()
		{
			SetOperationType("FadeSceneWithMask");
			string maskname = ReadVariable().StringValue();
			ReadVariable().IntValue();
			ReadVariable().IntValue();
			float time = (float)ReadVariable().IntValue() / 1000f;
			MODSystem.instance.modSceneController.MODLipSyncDisableAll();
			if (gameSystem.IsSkipping)
			{
				time = 0f;
			}
			gameSystem.SceneController.DrawSceneWithMask("black", maskname, time);
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDrawBustshot()
		{
			SetOperationType("DrawBustshot");
			int num = ReadVariable().IntValue();
			string textureName = ReadVariable().StringValue();
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			int z = ReadVariable().IntValue();
			bool move = ReadVariable().BoolValue();
			int oldx = ReadVariable().IntValue();
			int oldy = ReadVariable().IntValue();
			int oldz = ReadVariable().IntValue();
			ReadVariable().IntValue();
			ReadVariable().IntValue();
			ReadVariable().IntValue();
			int type = ReadVariable().IntValue();
			int num2 = ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (num2 == 0)
			{
				num2 = num;
			}
			MODSystem.instance.modSceneController.MODLipSyncDisableCurrentLayer(num);
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.DrawBustshot(num, textureName, x, y, z, oldx, oldy, oldz, move, num2, type, wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationMoveBustshot()
		{
			SetOperationType("MoveBustshot");
			int layer = ReadVariable().IntValue();
			string textureName = ReadVariable().StringValue();
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			int z = ReadVariable().IntValue();
			ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.MoveBustshot(layer, textureName, x, y, z, wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeBustshot()
		{
			SetOperationType("FadeBustshot");
			int layer = ReadVariable().IntValue();
			bool flag = ReadVariable().BoolValue();
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			int z = ReadVariable().IntValue();
			int angle = ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag2 = ReadVariable().BoolValue();
			MODSystem.instance.modSceneController.MODLipSyncDisableCurrentLayer(layer);
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
				flag = false;
			}
			if (flag)
			{
				gameSystem.SceneController.MoveSprite(layer, x, y, z, angle, 0, 0f, wait, flag2);
			}
			else
			{
				gameSystem.SceneController.FadeSprite(layer, wait, flag2);
			}
			if (flag2)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDrawBustshotWithFiltering()
		{
			SetOperationType("DrawBustshotWithFiltering");
			int layer = ReadVariable().IntValue();
			string textureName = ReadVariable().StringValue();
			string mask = ReadVariable().StringValue();
			ReadVariable().IntValue();
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			bool move = ReadVariable().BoolValue();
			int oldx = ReadVariable().IntValue();
			int oldy = ReadVariable().IntValue();
			int oldz = ReadVariable().IntValue();
			int z = ReadVariable().IntValue();
			int originx = ReadVariable().IntValue();
			int priority = ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			MODSystem.instance.modSceneController.MODLipSyncDisableCurrentLayer(layer);
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.DrawBustshotWithFiltering(layer, textureName, mask, x, y, z, originx, 0, oldx, oldy, oldz, move, priority, 0, wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeBustshotWithFiltering()
		{
			int layer = ReadVariable().IntValue();
			string mask = ReadVariable().StringValue();
			int style = ReadVariable().IntValue();
			bool flag = ReadVariable().BoolValue();
			int num = ReadVariable().IntValue();
			int num2 = ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag2 = ReadVariable().BoolValue();
			MODSystem.instance.modSceneController.MODLipSyncDisableCurrentLayer(layer);
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.FadeBustshotWithFiltering(layer, mask, style, wait, flag2);
			if (flag2)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDrawFace()
		{
			SetOperationType("DrawFace");
			string texture = ReadVariable().StringValue();
			float num = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				num = 0f;
				flag = false;
			}
			gameSystem.SceneController.DrawFace(texture, num, flag);
			if (!gameSystem.MessageBoxVisible)
			{
				gameSystem.MainUIController.FadeIn(num);
			}
			if (flag || gameSystem.IsSkipping)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeFace()
		{
			SetOperationType("FadeFace");
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
				flag = false;
			}
			gameSystem.SceneController.FadeFace(wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationExecutePlannedControl()
		{
			SetOperationType("ExecutePlannedControl");
			if (ReadVariable().BoolValue())
			{
				Debug.LogWarning("ExecutePlannedControl called with true wait value, but this option is not implemented!");
			}
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDrawSprite()
		{
			SetOperationType("DrawSprite");
			int num = ReadVariable().IntValue();
			string text = ReadVariable().StringValue();
			string text2 = ReadVariable().StringValue();
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			int z = ReadVariable().IntValue();
			int originx = ReadVariable().IntValue();
			int originy = ReadVariable().IntValue();
			int angle = ReadVariable().IntValue();
			ReadVariable().BoolValue();
			ReadVariable().BoolValue();
			int style = ReadVariable().IntValue();
			float alpha = (float)ReadVariable().IntValue() / 256f;
			int priority = ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			MODSystem.instance.modSceneController.MODLipSyncDisableCurrentLayer(num);
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.DrawSprite(num, text, text2, x, y, z, originx, originy, angle, style, alpha, priority, wait, flag);
			if (text2 != string.Empty)
			{
				gameSystem.SceneController.SetLayerToDepthMasked(num);
			}
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			BurikoMemory.Instance.SetCGFlag(text);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDrawSpriteWithFiltering()
		{
			SetOperationType("DrawSpriteWithFiltering");
			int layer = ReadVariable().IntValue();
			string text = ReadVariable().StringValue();
			string mask = ReadVariable().StringValue();
			int style = ReadVariable().IntValue();
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			ReadVariable().BoolValue();
			ReadVariable().BoolValue();
			ReadVariable().IntValue();
			ReadVariable().IntValue();
			int priority = ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			MODSystem.instance.modSceneController.MODLipSyncDisableCurrentLayer(layer);
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.DrawSpriteWithFiltering(layer, text, mask, x, y, style, priority, wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			BurikoMemory.Instance.SetCGFlag(text);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeSprite()
		{
			SetOperationType("FadeSprite");
			int layer = ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			MODSystem.instance.modSceneController.MODLipSyncDisableCurrentLayer(layer);
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.FadeSprite(layer, wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeSpriteWithFiltering()
		{
			SetOperationType("FadeSpriteWithFiltering");
			int layer = ReadVariable().IntValue();
			string mask = ReadVariable().StringValue();
			int style = ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			MODSystem.instance.modSceneController.MODLipSyncDisableCurrentLayer(layer);
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.FadeSpriteWithFiltering(layer, mask, style, wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationMoveSprite()
		{
			SetOperationType("MoveSprite");
			int layer = ReadVariable().IntValue();
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			int z = ReadVariable().IntValue();
			int angle = ReadVariable().IntValue();
			float num = (float)ReadVariable().IntValue() / 256f;
			int easetype = ReadVariable().IntValue();
			ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.MoveSprite(layer, x, y, z, angle, easetype, 1f - num, wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationMoveSpriteEx()
		{
			SetOperationType("MoveSpriteEx");
			int layer = ReadVariable().IntValue();
			string filename = ReadVariable().StringValue();
			ReadVariable().IntValue();
			int count = ReadVariable().IntValue();
			BurikoReference burikoReference = ReadVariable().VariableValue();
			ReadVariable().IntValue();
			float alpha = (float)ReadVariable().IntValue() / 256f;
			ReadVariable().IntValue();
			ReadVariable().IntValue();
			float time = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			BurikoVector burikoVector = BurikoMemory.Instance.GetMemory(burikoReference.Property) as BurikoVector;
			if (burikoVector == null)
			{
				throw new Exception("Can't get ST_Vector object with name of " + burikoReference.Property);
			}
			gameSystem.SceneController.MoveSpriteEx(layer, filename, burikoVector.GetElements(count), alpha, time, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationControlMotionOfSprite()
		{
			SetOperationType("ControlMotionOfSprite");
			int layer = ReadVariable().IntValue();
			int num = ReadVariable().IntValue();
			BurikoReference burikoReference = ReadVariable().VariableValue();
			int num2 = ReadVariable().IntValue();
			if (num2 != 0)
			{
				throw new NotImplementedException("Style of ControlMotionOfSprite set to 1, but this type is not implemented!");
			}
			BurikoMtnCtrlElement burikoMtnCtrlElement = BurikoMemory.Instance.GetMemory(burikoReference.Property) as BurikoMtnCtrlElement;
			if (burikoMtnCtrlElement == null)
			{
				throw new Exception("Can't get MtnCtrlElement object with name of " + burikoReference.Property);
			}
			MtnCtrlElement[] allElements = burikoMtnCtrlElement.GetAllElements();
			if (allElements.Length < num)
			{
				throw new Exception("ControlMotionOfSprite call specified a larger number of MtnCtrlElements than was provided!");
			}
			MtnCtrlElement[] array = new MtnCtrlElement[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = allElements[i];
			}
			gameSystem.SceneController.ControlMotionOfSprite(layer, array, num2);
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationGetPositionOfSprite()
		{
			SetOperationType("GetPositionOfSprite");
			BurikoReference burikoReference = ReadVariable().VariableValue();
			int layer = ReadVariable().IntValue();
			Vector3 positionOfLayer = gameSystem.SceneController.GetPositionOfLayer(layer);
			BurikoVector burikoVector = BurikoMemory.Instance.GetMemory(burikoReference.Property) as BurikoVector;
			if (burikoVector == null)
			{
				Debug.LogError("OperationGetPositionOfSprite: Input variable is not ST_Vector!");
			}
			burikoVector.Elements[0] = positionOfLayer;
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationEnableHorizontalGradation()
		{
			SetOperationType("EnableHorizontalGradation");
			int targetpower = ReadVariable().IntValue();
			float length = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				length = 0f;
			}
			gameSystem.SceneController.CreateHorizontalGradation(targetpower, length, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationDisableGradation()
		{
			SetOperationType("DisableGradation");
			float time = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				time = 0f;
			}
			gameSystem.SceneController.HideFilmEffector(time, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationEnlargeScreen()
		{
			SetOperationType("EnlargeScreen");
			int num = ReadVariable().IntValue();
			int num2 = ReadVariable().IntValue();
			int num3 = ReadVariable().IntValue();
			int num4 = ReadVariable().IntValue();
			ReadVariable().BoolValue();
			float time = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				time = 0f;
			}
			gameSystem.SceneController.EnlargeScene((float)num, (float)num2, (float)num3, (float)num4, time, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDisableEffector()
		{
			SetOperationType("DisableEffector");
			int num = ReadVariable().IntValue();
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				num = 0;
			}
			if (num == 0)
			{
				gameSystem.SceneController.ResetViewportSize();
			}
			else
			{
				gameSystem.SceneController.EnlargeScene(0f, 0f, 800f, 600f, (float)num, flag);
			}
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDisableBlur()
		{
			SetOperationType("DisableBlur");
			float time = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				time = 0f;
			}
			gameSystem.SceneController.HideFilmEffector(time, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDrawFilm()
		{
			SetOperationType("DrawFilm");
			int effecttype = ReadVariable().IntValue();
			Color targetColor = new Color((float)ReadVariable().IntValue() / 255f, (float)ReadVariable().IntValue() / 255f, (float)ReadVariable().IntValue() / 255f);
			int targetpower = ReadVariable().IntValue();
			int style = ReadVariable().IntValue();
			float length = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				length = 0f;
			}
			gameSystem.SceneController.CreateFilmEffector(effecttype, targetColor, targetpower, style, length, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeFilm()
		{
			SetOperationType("OperationFadeFilm");
			float time = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				time = 0f;
			}
			gameSystem.SceneController.HideFilmEffector(time, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationSetValidityOfFilmToFace()
		{
			SetOperationType("SetValidityOfFilmToFace");
			bool flag = ReadVariable().BoolValue();
			gameSystem.SceneController.SetFaceToUpperLayer(!flag);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationNegative()
		{
			SetOperationType("Negative");
			int effecttype = 3;
			Color targetColor = new Color(1f, 1f, 1f);
			int targetpower = 255;
			int style = 0;
			float length = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				length = 0f;
			}
			gameSystem.SceneController.CreateFilmEffector(effecttype, targetColor, targetpower, style, length, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationPlaceViewTip()
		{
			SetOperationType("PlaceViewTip");
			int num = ReadVariable().IntValue();
			string texture = string.Empty;
			switch (num)
			{
			case 0:
				texture = "view_riho";
				break;
			case 1:
				texture = "view_shik";
				break;
			case 2:
				texture = "view_shoi";
				break;
			}
			MtnCtrlElement[] array = new MtnCtrlElement[3]
			{
				new MtnCtrlElement(),
				null,
				null
			};
			array[0].Points = 1;
			array[0].Route[0] = new Vector3(213f, 131f, 0f);
			array[0].Time = 200;
			array[0].Transparancy = 0;
			array[1] = new MtnCtrlElement();
			array[1].Points = 1;
			array[1].Route[0] = new Vector3(213f, 131f, 0f);
			array[1].Time = 4000;
			array[1].Transparancy = 0;
			array[2] = new MtnCtrlElement();
			array[2].Points = 1;
			array[2].Route[0] = new Vector3(213f, 131f, 0f);
			array[2].Time = 200;
			array[2].Transparancy = 256;
			gameSystem.SceneController.DrawSprite(50, texture, null, 213, 131, 0, 0, 0, 0, 0, 1f, 50, 10f, isblocking: false);
			gameSystem.SceneController.ControlMotionOfSprite(50, array, 0);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDrawStandgraphic()
		{
			SetOperationType("DrawStandgraphic");
			int num = ReadVariable().IntValue();
			string textureName = ReadVariable().StringValue();
			bool flag = ReadVariable().BoolValue();
			int oldx = ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag2 = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			int num2 = Mathf.RoundToInt(D2BGetPosition(num));
			if (!flag)
			{
				oldx = num2;
			}
			gameSystem.SceneController.DrawBustshot(num, textureName, num2, 0, 0, oldx, 0, 0, flag, num, 0, wait, flag2);
			if (flag2)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationDrawBustFace()
		{
			SetOperationType("DrawBustFace");
			int num = ReadVariable().IntValue();
			string text = ReadVariable().StringValue();
			bool flag = ReadVariable().BoolValue();
			int oldx = ReadVariable().IntValue();
			float num2 = 0.2f;
			if (gameSystem.IsSkipping)
			{
				num2 = 0f;
			}
			int num3 = Mathf.RoundToInt(D2BGetPosition(num));
			if (!flag)
			{
				oldx = num3;
			}
			gameSystem.SceneController.DrawBustshot(num, text, num3, 0, 0, oldx, 0, 0, flag, num, 0, num2, isblocking: false);
			string texture = "f" + text.Substring(3);
			if (gameSystem.IsSkipping)
			{
				gameSystem.SceneController.DrawFace(texture, 0f, isblocking: false);
			}
			else
			{
				gameSystem.SceneController.DrawFace(texture, num2, isblocking: true);
			}
			if (!gameSystem.MessageBoxVisible)
			{
				gameSystem.MainUIController.FadeIn(num2);
			}
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationPlusStandgraphic1()
		{
			int num = ReadVariable().IntValue();
			string textureName = ReadVariable().StringValue();
			float wait = 0.2f;
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			if (num == 2)
			{
				gameSystem.SceneController.DrawBustshot(num, textureName, 180, 0, 0, 160, 0, 0, /*move:*/ true, num, 0, wait, /*isblocking:*/ false);
			}
			if (num == 3)
			{
				gameSystem.SceneController.DrawBustshot(num, textureName, -180, 0, 0, -160, 0, 0, /*move:*/ true, num, 0, wait, /*isblocking:*/ false);
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationPlusStandgraphic2()
		{
			SetOperationType("PlusStandgraphic2");
			int num = ReadVariable().IntValue();
			string textureName = ReadVariable().StringValue();
			float wait = 0.2f;
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			if (num == 5)
			{
				gameSystem.SceneController.DrawBustshot(num, textureName, 0, 0, 0, 20, 0, 0, /*move:*/ true, num, 0, wait, /*isblocking:*/ false);
			}
			if (num == 6)
			{
				gameSystem.SceneController.DrawBustshot(num, textureName, 220, 0, 0, 200, 0, 0, /*move:*/ true, num, 0, wait, /*isblocking:*/ false);
			}
			if (num == 7)
			{
				gameSystem.SceneController.DrawBustshot(num, textureName, -220, 0, 0, -200, 0, 0, /*move:*/ true, num, 0, wait, /*isblocking:*/ false);
			}
			if (num == 8)
			{
				gameSystem.SceneController.DrawBustshot(num, textureName, 0, 0, 0, 20, 0, 0, /*move:*/ true, num, 0, wait, /*isblocking:*/ false);
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationPlusStandgraphic3()
		{
			SetOperationType("PlusStandgraphic3");
			int num = ReadVariable().IntValue();
			string textureName = ReadVariable().StringValue();
			float wait = 0.2f;
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			if (num == 5)
			{
				gameSystem.SceneController.DrawBustshot(5, textureName, 0, 0, 0, -20, 0, 0, /*move:*/ true, num, 0, wait, /*isblocking:*/ false);
			}
			if (num == 6)
			{
				gameSystem.SceneController.DrawBustshot(6, textureName, 220, 0, 0, 200, 0, 0, /*move:*/ true, num, 0, wait, /*isblocking:*/ false);
			}
			if (num == 7)
			{
				gameSystem.SceneController.DrawBustshot(7, textureName, -220, 0, 0, -200, 0, 0, /*move:*/ true, num, 0, wait, /*isblocking:*/ false);
			}
			if (num == 8)
			{
				gameSystem.SceneController.DrawBustshot(8, textureName, 0, 0, 0, -20, 0, 0, /*move:*/ true, num, 0, wait, /*isblocking:*/ false);
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeAllBustshots2()
		{
			SetOperationType("FadeAllBustshots2");
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.FadeSprite(2, wait, isblocking: false);
			gameSystem.SceneController.FadeSprite(3, wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeAllBustshots3()
		{
			SetOperationType("FadeAllBustshots3");
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool isblocking = ReadVariable().BoolValue();
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			gameSystem.SceneController.FadeSprite(5, wait, isblocking: false);
			gameSystem.SceneController.FadeSprite(6, wait, isblocking: false);
			gameSystem.SceneController.FadeSprite(7, wait, isblocking: false);
			gameSystem.SceneController.FadeSprite(8, wait, isblocking);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationFadeAllBustshots()
		{
			SetOperationType("FadeAllBustshots");
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			MODSystem.instance.modSceneController.MODLipSyncDisableAll();
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			for (int i = 1; i < 20; i++)
			{
				gameSystem.SceneController.FadeSprite(i, wait, flag);
			}
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationBlurOffOn()
		{
			SetOperationType("BlurOffOn");
			int targetpower = ReadVariable().IntValue() / 2;
			float length = (float)ReadVariable().IntValue() / 1000f;
			gameSystem.SceneController.HideFilmEffector(0f, isBlocking: false);
			gameSystem.SceneController.CreateFilmEffector(12, Color.white, targetpower, 0, length, isBlocking: true);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetDrawingPointOfMessage()
		{
			SetOperationType("SetDrawingPointOfMessage");
			gameSystem.TextController.SetTextPoint(ReadVariable().IntValue(), ReadVariable().IntValue());
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationTitleScreen()
		{
			SetOperationType("TitleScreen");
			gameSystem.PushStateObject(new StateTitle());
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationOpenGallery()
		{
			SetOperationType("OperationOpenGallery");
			gameSystem.SwitchToGallery();
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationCloseGallery()
		{
			SetOperationType("OperationCloseGallery");
			gameSystem.LeaveGalleryScreen(null);
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationHideGallery()
		{
			SetOperationType("OperationHideGallery");
			throw new NotImplementedException("OperationHideGallery");
		}

		private BurikoVariable OperationRevealGallery()
		{
			SetOperationType("OperationRevealGallery");
			throw new NotImplementedException("OperationRevealGallery");
		}

		private BurikoVariable OperationBreak()
		{
			SetOperationType("Break");
			Debug.Break();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationViewChapterScreen()
		{
			SetOperationType("ViewChapterScreen");
			gameSystem.AudioController.SaveRestoreTempAudio();
			gameSystem.PushStateObject(new StateChapterScreen());
			scriptSystem.RestoreTempSnapshot();
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationViewExtras()
		{
			SetOperationType("ViewExtras");
			gameSystem.PushStateObject(new StateExtraScreen());
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationViewTips()
		{
			SetOperationType("ViewTips");
			int tipsMode = ReadVariable().IntValue();
			gameSystem.AudioController.FadeOutBGM(0, 500, waitForFade: false);
			gameSystem.AudioController.FadeOutBGM(1, 500, waitForFade: false);
			gameSystem.AudioController.FadeOutBGM(2, 500, waitForFade: false);
			gameSystem.PushStateObject(new StateViewTips(tipsMode));
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationChapterPreview()
		{
			gameSystem.PushStateObject(new StateChapterPreview());
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSavePoint()
		{
			SetOperationType("SavePoint");
			string text = ReadVariable().StringValue();
			string text2 = ReadVariable().StringValue();
			gameSystem.TextController.SetFullText(text, 0);
			gameSystem.TextController.SetFullText(text2, 1);
			scriptSystem.TakeSaveSnapshot(string.Empty);
			scriptSystem.CopyTempSnapshot();
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationSetTextFade()
		{
			SetOperationType("SetValidityOfTextFade");
			bool flag = ReadVariable().BoolValue();
			BurikoMemory.Instance.SetFlag("LTextFade", flag ? 1 : 0);
			gameSystem.TextController.SetTextFade(flag);
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationSetValidityOfInterface()
		{
			SetOperationType("SetValidityOfInterface");
			if (ReadVariable().BoolValue())
			{
				gameSystem.ShowUIControls();
			}
			else
			{
				gameSystem.HideUIControls();
			}
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationEnableJumpingOfReturnIcon()
		{
			SetOperationType("EnableJumpingOfReturnIcon");
			Debug.LogWarning("Unhandled EnableJumpingOfReturnIcon");
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationSetValidityOfWindowDisablingWhenGraphicsControl()
		{
			SetOperationType("SetValidityOfWindowDisablingWhenGraphicsControl");
			bool flag = ReadVariable().BoolValue();
			Debug.LogWarning("Unhandled operation SetValidityOfWindowDisablingWhenGraphicsControl (value " + flag + ")");
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationLanguagePrompt()
		{
			SetOperationType("LanguagePrompt");
			gameSystem.PushStateObject(new StateDialogPrompt(PromptType.DialogLanguage, delegate
			{
				BurikoMemory.Instance.SetGlobalFlag("GLanguage", 1);
				GameSystem.Instance.UseEnglishText = true;
			}, delegate
			{
				BurikoMemory.Instance.SetGlobalFlag("GLanguage", 0);
				GameSystem.Instance.UseEnglishText = false;
			}));
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationGetAchievement()
		{
			SetOperationType("GetAchievement");
			gameSystem.SteamController.AddAchievement(ReadVariable().StringValue());
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationCheckTipsAchievements()
		{
			SetOperationType("CheckTipsAchievements");
			gameSystem.SteamController.CheckTipsAchievements();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetFontId()
		{
			SetOperationType("SetFontId");
			gameSystem.MainUIController.ChangeFontId(ReadVariable().IntValue());
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetCharSpacing()
		{
			SetOperationType("SetCharSpacing");
			gameSystem.MainUIController.SetCharSpacing(ReadVariable().IntValue());
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetLineSpacing()
		{
			SetOperationType("SetLineSpacing");
			gameSystem.MainUIController.SetLineSpacing(ReadVariable().IntValue());
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetFontSize()
		{
			SetOperationType("SetFontSize");
			gameSystem.MainUIController.SetFontSize(ReadVariable().IntValue());
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetWindowPos()
		{
			SetOperationType("SetWindowPos");
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			gameSystem.MainUIController.SetWindowPos(x, y);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetWindowSize()
		{
			SetOperationType("SetWindowSize");
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			gameSystem.MainUIController.SetWindowSize(x, y);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetWindowMargins()
		{
			SetOperationType("SetWindowMargins");
			int left = ReadVariable().IntValue();
			int top = ReadVariable().IntValue();
			int right = ReadVariable().IntValue();
			int bottom = ReadVariable().IntValue();
			gameSystem.MainUIController.SetWindowMargins(left, top, right, bottom);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetNameFormat()
		{
			SetOperationType("SetNameFormat");
			string nameFormat = ReadVariable().StringValue();
			gameSystem.TextController.NameFormat = nameFormat;
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationSetScreenAspect()
		{
			SetOperationType("SetScreenAspect");
			string s = ReadVariable().StringValue();
			float newratio = 1f / float.Parse(s, CultureInfo.InvariantCulture);
			gameSystem.UpdateAspectRatio(newratio);
			return BurikoVariable.Null;
		}

		// This has been modified so that we can save the 'default' UI position
		private BurikoVariable OperationSetGuiPosition()
		{
			SetOperationType("SetGUIPosition");
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			MODMainUIController mODMainUIController = new MODMainUIController();
			mODMainUIController.WideGuiPositionLoad(x, y);
			return BurikoVariable.Null;
		}
		private BurikoVariable OperationSetRyukishiGuiPosition()
		{
			SetOperationType("ModRyukishiGuiPositionLoad");
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			MODMainUIController mODMainUIController = new MODMainUIController();
			mODMainUIController.RyukishiGuiPositionLoad(x, y);
			return BurikoVariable.Null;
		}

		public int GetPositionByLineNumber(int linenum)
		{
			if (!lineLookup.ContainsKey(linenum))
			{
				throw new Exception("Could not load file!");
			}
			return lineLookup[linenum];
		}

		private void CommandLineNum()
		{
			LineNum = dataReader.ReadInt32();
			if (gameSystem.IsSkipping && !gameSystem.IsForceSkip && !BurikoMemory.Instance.IsLineRead(Filename, LineNum) && !gameSystem.SkipUnreadMessages)
			{
				gameSystem.IsSkipping = false;
			}
			BurikoMemory.Instance.MarkLineAsRead(Filename, LineNum);
		}

		public BurikoVariable ExecuteOperation(BurikoOperations op)
		{
			switch (op)
			{
			case BurikoOperations.CallScript:
				return OperationCallScript();
			case BurikoOperations.JumpScript:
				return OperationJumpScript();
			case BurikoOperations.CallSection:
				return OperationCallSection();
			case BurikoOperations.JumpSection:
				return OperationJumpSection();
			case BurikoOperations.StoreValueToLocalWork:
				return OperationStoreValueToLocalWork();
			case BurikoOperations.LoadValueFromLocalWork:
				return OperationLoadValueFromLocalWork();
			case BurikoOperations.SetLocalFlag:
				return OperationSetLocalFlag();
			case BurikoOperations.GetLocalFlag:
				return OperationLoadValueFromLocalWork();
			case BurikoOperations.SetGlobalFlag:
				return OperationSetGlobalFlag();
			case BurikoOperations.GetGlobalFlag:
				return OperationGetGlobalFlag();
			case BurikoOperations.Wait:
				return OperationWait();
			case BurikoOperations.WaitForInput:
				return OperationWaitForInput();
			case BurikoOperations.SetValidityOfInput:
				return OperationSetValidityOfInput();
			case BurikoOperations.SetValidityOfSaving:
				return OperationSetValidityOfSaving();
			case BurikoOperations.SetValidityOfSkipping:
				return OperationSetValidityOfSkipping();
			case BurikoOperations.SetValidityOfUserEffectSpeed:
				return OperationSetValidityOfUserEffectSpeed();
			case BurikoOperations.OutputLine:
				return OperationOutputLine();
			case BurikoOperations.OutputLineAll:
				return OperationOutputLineAll();
			case BurikoOperations.ClearMessage:
				return OperationClearMessage();
			case BurikoOperations.SetFontOfMessage:
				return OperationSetFontOfMessage();
			case BurikoOperations.SetSpeedOfMessage:
				return OperationSetSpeedOfMessage();
			case BurikoOperations.DisableWindow:
				return OperationDisableWindow();
			case BurikoOperations.SpringText:
				return OperationSpringText();
			case BurikoOperations.SetStyleOfMessageSwinging:
				return OperationSetStyleOfMessageSwinging();
			case BurikoOperations.Select:
				return OperationSelect();
			case BurikoOperations.PlayBGM:
				return OperationPlayBGM();
			case BurikoOperations.StopBGM:
				return OperationStopBGM();
			case BurikoOperations.ChangeVolumeOfBGM:
				return OperationChangeVolumeOfBGM();
			case BurikoOperations.FadeOutBGM:
				return OperationFadeOutBGM();
			case BurikoOperations.FadeOutMultiBGM:
				return OperationFadeOutMultiBGM();
			case BurikoOperations.PlaySE:
				return OperationPlaySE();
			case BurikoOperations.StopSE:
				return OperationStopSE();
			case BurikoOperations.FadeOutSE:
				return OperationFadeOutSE();
			case BurikoOperations.PlayVoice:
				return OperationPlayVoice();
			case BurikoOperations.WaitToFinishVoicePlaying:
				return OperationWaitToFinishVoicePlaying();
			case BurikoOperations.WaitToFinishSEPlaying:
				return OperationWaitToFinishSEPlaying();
			case BurikoOperations.PreloadBitmap:
				return OperationPreloadBitmap();
			case BurikoOperations.StartShakingOfAllObjects:
				return OperationStartShakingOfAllObjects();
			case BurikoOperations.TerminateShakingOfAllObjects:
				return OperationTerminateShakingOfAllObjects();
			case BurikoOperations.StartShakingOfWindow:
				return OperationStartShakingOfWindow();
			case BurikoOperations.TerminateShakingOfWindow:
				return OperationTerminateShakingOfWindow();
			case BurikoOperations.StartShakingOfBustshot:
				return OperationStartShakingOfBustshot();
			case BurikoOperations.TerminateShakingOfBustshot:
				return OperationTerminateShakingOfBustshot();
			case BurikoOperations.ShakeScreen:
				return OperationShakeScreen();
			case BurikoOperations.ShakeScreenSx:
				return OperationShakeScreenSx();
			case BurikoOperations.DrawBG:
				return OperationDrawBG();
			case BurikoOperations.FadeBG:
				return OperationFadeBG();
			case BurikoOperations.DrawScene:
				return OperationDrawScene();
			case BurikoOperations.DrawSceneWithMask:
				return OperationDrawSceneWithMask();
			case BurikoOperations.ChangeScene:
				return OperationChangeScene();
			case BurikoOperations.FadeScene:
				return OperationFadeScene();
			case BurikoOperations.FadeSceneWithMask:
				return OperationFadeSceneWithMask();
			case BurikoOperations.DrawBustshot:
				return OperationDrawBustshot();
			case BurikoOperations.MoveBustshot:
				return OperationMoveBustshot();
			case BurikoOperations.FadeBustshot:
				return OperationFadeBustshot();
			case BurikoOperations.DrawBustshotWithFiltering:
				return OperationDrawBustshotWithFiltering();
			case BurikoOperations.DrawFace:
				return OperationDrawFace();
			case BurikoOperations.FadeFace:
				return OperationFadeFace();
			case BurikoOperations.ExecutePlannedControl:
				return OperationExecutePlannedControl();
			case BurikoOperations.DrawSprite:
				return OperationDrawSprite();
			case BurikoOperations.DrawSpriteWithFiltering:
				return OperationDrawSpriteWithFiltering();
			case BurikoOperations.FadeSprite:
				return OperationFadeSprite();
			case BurikoOperations.FadeSpriteWithFiltering:
				return OperationFadeSpriteWithFiltering();
			case BurikoOperations.MoveSprite:
				return OperationMoveSprite();
			case BurikoOperations.MoveSpriteEx:
				return OperationMoveSpriteEx();
			case BurikoOperations.ControlMotionOfSprite:
				return OperationControlMotionOfSprite();
			case BurikoOperations.GetPositionOfSprite:
				return OperationGetPositionOfSprite();
			case BurikoOperations.EnableHorizontalGradation:
				return OperationEnableHorizontalGradation();
			case BurikoOperations.DisableGradation:
				return OperationDisableGradation();
			case BurikoOperations.EnlargeScreen:
				return OperationEnlargeScreen();
			case BurikoOperations.DisableEffector:
				return OperationDisableEffector();
			case BurikoOperations.DisableBlur:
				return OperationDisableBlur();
			case BurikoOperations.DrawFilm:
				return OperationDrawFilm();
			case BurikoOperations.FadeFilm:
				return OperationFadeFilm();
			case BurikoOperations.SetValidityOfFilmToFace:
				return OperationSetValidityOfFilmToFace();
			case BurikoOperations.FadeAllBustshots:
				return OperationFadeAllBustshots();
			case BurikoOperations.FadeBustshotWithFiltering:
				return OperationFadeBustshotWithFiltering();
			case BurikoOperations.SetDrawingPointOfMessage:
				return OperationSetDrawingPointOfMessage();
			case BurikoOperations.Negative:
				return OperationNegative();
			case BurikoOperations.PlaceViewTip:
				return OperationPlaceViewTip();
			case BurikoOperations.DrawStandgraphic:
				return OperationDrawStandgraphic();
			case BurikoOperations.DrawBustFace:
				return OperationDrawBustFace();
			case BurikoOperations.PlusStandgraphic1:
				return OperationPlusStandgraphic1();
			case BurikoOperations.PlusStandgraphic2:
				return OperationPlusStandgraphic2();
			case BurikoOperations.PlusStandgraphic3:
				return OperationPlusStandgraphic3();
			case BurikoOperations.FadeAllBustshots2:
				return OperationFadeAllBustshots2();
			case BurikoOperations.FadeAllBustshots3:
				return OperationFadeAllBustshots3();
			case BurikoOperations.BlurOffOn:
				return OperationBlurOffOn();
			case BurikoOperations.TitleScreen:
				return OperationTitleScreen();
			case BurikoOperations.Break:
				return OperationBreak();
			case BurikoOperations.OpenGallery:
				return OperationOpenGallery();
			case BurikoOperations.CloseGallery:
				return OperationCloseGallery();
			case BurikoOperations.HideGallery:
				return OperationHideGallery();
			case BurikoOperations.RevealGallery:
				return OperationRevealGallery();
			case BurikoOperations.ViewChapterScreen:
				return OperationViewChapterScreen();
			case BurikoOperations.ViewExtras:
				return OperationViewExtras();
			case BurikoOperations.ViewTips:
				return OperationViewTips();
			case BurikoOperations.ChapterPreview:
				return OperationChapterPreview();
			case BurikoOperations.SetValidityOfInterface:
				return OperationSetValidityOfInterface();
			case BurikoOperations.SavePoint:
				return OperationSavePoint();
			case BurikoOperations.EnableJumpingOfReturnIcon:
				return OperationEnableJumpingOfReturnIcon();
			case BurikoOperations.SetValidityOfWindowDisablingWhenGraphicsControl:
				return OperationSetValidityOfWindowDisablingWhenGraphicsControl();
			case BurikoOperations.LanguagePrompt:
				return OperationLanguagePrompt();
			case BurikoOperations.SetTextFade:
				return OperationSetTextFade();
			case BurikoOperations.GetAchievement:
				return OperationGetAchievement();
			case BurikoOperations.CheckTipsAchievements:
				return OperationCheckTipsAchievements();
			case BurikoOperations.SetFontId:
				return OperationSetFontId();
			case BurikoOperations.SetCharSpacing:
				return OperationSetCharSpacing();
			case BurikoOperations.SetLineSpacing:
				return OperationSetLineSpacing();
			case BurikoOperations.SetFontSize:
				return OperationSetFontSize();
			case BurikoOperations.SetWindowPos:
				return OperationSetWindowPos();
			case BurikoOperations.SetWindowSize:
				return OperationSetWindowSize();
			case BurikoOperations.SetWindowMargins:
				return OperationSetWindowMargins();
			case BurikoOperations.SetNameFormat:
				return OperationSetNameFormat();
			case BurikoOperations.SetScreenAspect:
				return OperationSetScreenAspect();
			case BurikoOperations.SetGuiPosition:
				return OperationSetGuiPosition();
			case BurikoOperations.ModEnableNVLModeInADVMode:
				return OperationMODenableNVLModeInADVMode();
			case BurikoOperations.ModDisableNVLModeInADVMode:
				return OperationMODdisableNVLModeInADVMode();
			case BurikoOperations.ModADVModeSettingLoad:
				return OperationMODADVModeSettingLoad();
			case BurikoOperations.ModNVLModeSettingLoad:
				return OperationMODNVLModeSettingLoad();
			case BurikoOperations.ModNVLADVModeSettingLoad:
				return OperationMODNVLADVModeSettingLoad();
			case BurikoOperations.ModCallScriptSection:
				return OperationMODCallScriptSection();
			case BurikoOperations.ModDrawCharacter:
				return OperationMODDrawCharacter();
			case BurikoOperations.ModDrawCharacterWithFiltering:
				return OperationMODDrawCharacterWithFiltering();
			case BurikoOperations.ModPlayVoiceLS:
				return OperationMODPlayVoiceLS();
			case BurikoOperations.ModPlayMovie:
				return OperationMODPlayMovie();
			case BurikoOperations.ModSetConfigFontSize:
				return OperationMODSetConfigFontSize();
			case BurikoOperations.ModSetChapterJumpFontSize:
				return OperationMODSetChapterJumpFontSize();
			case BurikoOperations.ModSetHighestChapterFlag:
				return OperationMODSetHighestChapterFlag();
			case BurikoOperations.ModGetHighestChapterFlag:
				return OperationMODGetHighestChapterFlag();
			case BurikoOperations.ModSetMainFontOutlineWidth:
				return OperationMODSetMainFontOutlineWidth();
			case BurikoOperations.ModSetLayerFilter:
				return OperationMODSetLayerFilter();
			case BurikoOperations.ModAddArtset:
				return OperationMODAddArtset();
			case BurikoOperations.ModClearArtsets:
				return OperationMODClearArtsets();
			case BurikoOperations.ModRyukishiModeSettingLoad:
				return OperationMODRyukishiModeSettingLoad();
			case BurikoOperations.ModRyukishiSetGuiPosition:
				return OperationSetRyukishiGuiPosition();
			case BurikoOperations.ModPlayBGM:
				return OperationMODPlayBGM();
			case BurikoOperations.ModFadeOutBGM:
				return OperationMODFadeOutBGM();
			case BurikoOperations.ModAddBGMset:
				return OperationMODAddBGMset();
			case BurikoOperations.ModAddSEset:
				return OperationMODAddSEset();
			case BurikoOperations.ModAddAudioset:
				return OperationMODAddAudioset();
			case BurikoOperations.ModGenericCall:
				return OperationMODGenericCall();
			default:
				ScriptError("Unhandled Operation : " + op);
				return BurikoVariable.Null;
			}
		}

		private void CommandOperation()
		{
			SetOperationType("Operation");
			BurikoOperations op = (BurikoOperations)dataReader.ReadInt16();
			var watch = System.Diagnostics.Stopwatch.StartNew();
			ExecuteOperation(op);
			watch.Stop();
			MODUtility.FlagMonitorOnlyLog("Executed " + opType + " in " + watch.ElapsedMilliseconds + "ms");
		}

		private void CommandDeclaration()
		{
			SetOperationType("Declaration");
			string text = dataReader.ReadString();
			string name = dataReader.ReadString();
			BurikoVariable burikoVariable = ReadVariable();
			int members = 1;
			if (burikoVariable.Type != BurikoValueType.Null)
			{
				members = burikoVariable.IntValue();
			}
			IBurikoObject burikoObject;
			switch (text)
			{
			case "char":
				burikoObject = new BurikoString();
				burikoObject.Create(members);
				break;
			case "ST_MtnCtrlElement":
				burikoObject = new BurikoMtnCtrlElement();
				burikoObject.Create(members);
				break;
			case "ST_Vector":
				burikoObject = new BurikoVector();
				burikoObject.Create(members);
				break;
			case "int":
				burikoObject = new BurikoInt();
				burikoObject.Create(members);
				break;
			default:
				throw new Exception("Unknown declaration variable type " + text);
			}
			BurikoMemory.Instance.AddMemory(name, burikoObject);
		}

		public void CommandAssignment()
		{
			SetOperationType("Assignment");
			BurikoReference burikoReference = BurikoVariable.ReadReference(dataReader);
			BurikoVariable var = ReadVariable();
			IBurikoObject memory = BurikoMemory.Instance.GetMemory(burikoReference.Property);
			memory.SetValue(burikoReference, var);
		}

		public void CommandIf()
		{
			SetOperationType("If");
			int num = ReadVariable().IntValue();
			int newposition = dataReader.ReadInt32();
			if (num != 1)
			{
				JumpToPosition(newposition);
			}
		}

		public void CommandJump()
		{
			SetOperationType("Jump");
			int newposition = dataReader.ReadInt32();
			JumpToPosition(newposition);
		}

		public void Next()
		{
			if (!IsInitialized)
			{
				InitializeScript();
			}
			BurikoCommands burikoCommands;
			try
			{
				burikoCommands = (BurikoCommands)dataReader.ReadInt16();
			}
			catch (Exception arg)
			{
				Debug.LogError($"{Filename}: Failed to read file! Position: {Position} Exception: {arg}");
				throw;
				IL_0046:;
			}
			switch (burikoCommands)
			{
			case BurikoCommands.LineNum:
				CommandLineNum();
				break;
			case BurikoCommands.Operation:
				CommandOperation();
				break;
			case BurikoCommands.Declaration:
				CommandDeclaration();
				break;
			case BurikoCommands.Assignment:
				CommandAssignment();
				break;
			case BurikoCommands.If:
				CommandIf();
				break;
			case BurikoCommands.Jump:
				CommandJump();
				break;
			case BurikoCommands.Return:
				BurikoScriptSystem.Instance.Return();
				break;
			default:
				ScriptError(string.Format("No handler for command " + burikoCommands + " (value " + (int)burikoCommands + ")"));
				break;
			}
		}

		public void ScriptError(string message)
		{
			string message2 = $"{Filename} ({LineNum}): {message}";
			Logger.LogError(message2);
			throw new Exception(message2);
		}

		private BurikoVariable OperationMODenableNVLModeInADVMode()
		{
			SetOperationType("ModEnableNVLModeInADVMode");
			MODActions.EnableNVLModeINADVMode();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationMODdisableNVLModeInADVMode()
		{
			SetOperationType("ModDisableNVLModeInADVMode");
			MODActions.DisableNVLModeINADVMode();
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODADVModeSettingLoad()
		{
			SetOperationType("ModADVModeSettingLoad");
			MODMainUIController mODMainUIController = new MODMainUIController();
			string name = ReadVariable().StringValue();
			int posx = ReadVariable().IntValue();
			int posy = ReadVariable().IntValue();
			int sizex = ReadVariable().IntValue();
			int sizey = ReadVariable().IntValue();
			int mleft = ReadVariable().IntValue();
			int mtop = ReadVariable().IntValue();
			int mright = ReadVariable().IntValue();
			int mbottom = ReadVariable().IntValue();
			int font = ReadVariable().IntValue();
			int cspace = ReadVariable().IntValue();
			int lspace = ReadVariable().IntValue();
			int fsize = ReadVariable().IntValue();
			mODMainUIController.ADVModeSettingLoad(name, posx, posy, sizex, sizey, mleft, mtop, mright, mbottom, font, cspace, lspace, fsize);
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODNVLModeSettingLoad()
		{
			SetOperationType("ModNVLModeSettingLoad");
			MODMainUIController mODMainUIController = new MODMainUIController();
			string name = ReadVariable().StringValue();
			int posx = ReadVariable().IntValue();
			int posy = ReadVariable().IntValue();
			int sizex = ReadVariable().IntValue();
			int sizey = ReadVariable().IntValue();
			int mleft = ReadVariable().IntValue();
			int mtop = ReadVariable().IntValue();
			int mright = ReadVariable().IntValue();
			int mbottom = ReadVariable().IntValue();
			int font = ReadVariable().IntValue();
			int cspace = ReadVariable().IntValue();
			int lspace = ReadVariable().IntValue();
			int fsize = ReadVariable().IntValue();
			mODMainUIController.NVLModeSettingLoad(name, posx, posy, sizex, sizey, mleft, mtop, mright, mbottom, font, cspace, lspace, fsize);
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODNVLADVModeSettingLoad()
		{
			SetOperationType("ModNVLADVModeSettingLoad");
			MODMainUIController mODMainUIController = new MODMainUIController();
			string name = ReadVariable().StringValue();
			int posx = ReadVariable().IntValue();
			int posy = ReadVariable().IntValue();
			int sizex = ReadVariable().IntValue();
			int sizey = ReadVariable().IntValue();
			int mleft = ReadVariable().IntValue();
			int mtop = ReadVariable().IntValue();
			int mright = ReadVariable().IntValue();
			int mbottom = ReadVariable().IntValue();
			int font = ReadVariable().IntValue();
			int cspace = ReadVariable().IntValue();
			int lspace = ReadVariable().IntValue();
			int fsize = ReadVariable().IntValue();
			mODMainUIController.NVLADVModeSettingLoad(name, posx, posy, sizex, sizey, mleft, mtop, mright, mbottom, font, cspace, lspace, fsize);
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODRyukishiModeSettingLoad()
		{
			SetOperationType("ModRyukishiModeSettingLoad");
			MODMainUIController mODMainUIController = new MODMainUIController();
			string name = ReadVariable().StringValue();
			int posx = ReadVariable().IntValue();
			int posy = ReadVariable().IntValue();
			int sizex = ReadVariable().IntValue();
			int sizey = ReadVariable().IntValue();
			int mleft = ReadVariable().IntValue();
			int mtop = ReadVariable().IntValue();
			int mright = ReadVariable().IntValue();
			int mbottom = ReadVariable().IntValue();
			int font = ReadVariable().IntValue();
			int cspace = ReadVariable().IntValue();
			int lspace = ReadVariable().IntValue();
			int fsize = ReadVariable().IntValue();
			mODMainUIController.RyukishiModeSettingLoad(name, posx, posy, sizex, sizey, mleft, mtop, mright, mbottom, font, cspace, lspace, fsize);
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODCallScriptSection()
		{
			SetOperationType("ModCallScriptSection");
			string scriptname = ReadVariable().StringValue();
			string blockname = ReadVariable().StringValue();
			scriptSystem.CallScript(scriptname, blockname);
			return BurikoVariable.Null;
		}

		public void MODOnlyRecompile()
		{
		}

		public BurikoVariable OperationMODDrawCharacter()
		{
			SetOperationType("ModDrawBustshot");
			int num3 = ReadVariable().IntValue();
			int character = ReadVariable().IntValue();
			string textureName = ReadVariable().StringValue();
			string str = ReadVariable().StringValue();
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			int z = ReadVariable().IntValue();
			bool move = ReadVariable().BoolValue();
			int oldx = ReadVariable().IntValue();
			int oldy = ReadVariable().IntValue();
			int oldz = ReadVariable().IntValue();
			ReadVariable().IntValue();
			ReadVariable().IntValue();
			ReadVariable().IntValue();
			int type = ReadVariable().IntValue();
			int num2 = ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			string textureName2 = textureName + "0";
			string text = textureName + str;
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(character);
			if (!MODSystem.instance.modSceneController.MODLipSyncIsEnabled())
			{
				textureName2 = text;
			}
			if (num2 == 0)
			{
				num2 = num3;
			}
			GameSystem.Instance.RegisterAction(delegate
			{
				MODSystem.instance.modSceneController.MODLipSyncStoreValue(num3, character, textureName, x, y, z, type, num2);
			});
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			MODSystem.instance.modTextureController.StoreLayerTexture(num3, text);
			gameSystem.SceneController.DrawBustshot(num3, textureName2, x, y, z, oldx, oldy, oldz, move, num2, type, wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODDrawCharacterWithFiltering()
		{
			SetOperationType("ModDrawCharacterWithFiltering");
			int layer = ReadVariable().IntValue();
			int character = ReadVariable().IntValue();
			string textureName = ReadVariable().StringValue();
			string str = ReadVariable().StringValue();
			string mask = ReadVariable().StringValue();
			ReadVariable().IntValue();
			int x = ReadVariable().IntValue();
			int y = ReadVariable().IntValue();
			bool move = ReadVariable().BoolValue();
			int oldx = ReadVariable().IntValue();
			int oldy = ReadVariable().IntValue();
			int oldz = ReadVariable().IntValue();
			int z = ReadVariable().IntValue();
			int originx = ReadVariable().IntValue();
			int priority = ReadVariable().IntValue();
			float wait = (float)ReadVariable().IntValue() / 1000f;
			bool flag = ReadVariable().BoolValue();
			string textureName2 = textureName + "0";
			string text = textureName + str;
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(character);
			if (!MODSystem.instance.modSceneController.MODLipSyncIsEnabled())
			{
				textureName2 = text;
			}
			if (priority == 0)
			{
				priority = layer;
			}
			gameSystem.RegisterAction(delegate
			{
				MODSystem.instance.modSceneController.MODLipSyncStoreValue(layer, character, textureName, x, y, z, 0, priority);
			});
			if (gameSystem.IsSkipping)
			{
				wait = 0f;
			}
			MODSystem.instance.modTextureController.StoreLayerTexture(layer, text);
			gameSystem.SceneController.DrawBustshotWithFiltering(layer, textureName2, mask, x, y, z, originx, 0, oldx, oldy, oldz, move, priority, 0, wait, flag);
			if (flag)
			{
				gameSystem.ExecuteActions();
			}
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODPlayVoiceLS()
		{
			SetOperationType("ModPlayVoiceLS");
			int channel = ReadVariable().IntValue();
			int character = ReadVariable().IntValue();
			string filename = ReadVariable().StringValue() + ".ogg";
			float volume = (float)ReadVariable().IntValue() / 128f;
			bool flag = ReadVariable().BoolValue();
			GameSystem.Instance.TextHistory.RegisterVoice(new AudioInfo(volume, filename, channel));
			if ((MODSystem.instance.modSceneController.MODLipSyncIsEnabled() && !gameSystem.IsSkipping) & flag)
			{
				MODSystem.instance.modSceneController.MODLipSyncPrepareVoice(character, channel);
				AudioController.Instance.MODPlayVoiceLS(filename, channel, volume, character);
			}
			else
			{
				AudioController.Instance.PlayVoice(filename, channel, volume);
			}
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODPlayMovie()
		{
			SetOperationType("MODPlayMovie");
			string moviename = ReadVariable().StringValue();
			GameSystem.Instance.PushStateObject(new StateMovie(moviename));
			gameSystem.ExecuteActions();
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationMODSetConfigFontSize()
		{
			SetOperationType("MODSetConfigFontSize");
			int size = ReadVariable().IntValue();
			GameSystem.Instance.ConfigMenuFontSize = size;
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationMODSetChapterJumpFontSize()
		{
			SetOperationType("MODSetChapterJumpFontSize");
			int japanese = ReadVariable().IntValue();
			int english = ReadVariable().IntValue();
			GameSystem.Instance.SetChapterJumpFontSize(japanese, english);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationMODSetHighestChapterFlag()
		{
			SetOperationType("MODSetHighestChapterFlag");
			int key = ReadVariable().IntValue();
			int value = ReadVariable().IntValue();
			BurikoMemory.Instance.SetHighestChapterFlag(key, value);
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationMODGetHighestChapterFlag()
		{
			SetOperationType("MODGetHighestChapterFlag");
			int key = ReadVariable().IntValue();
			return BurikoMemory.Instance.GetHighestChapterFlag(key);
		}

		private BurikoVariable OperationMODSetMainFontOutlineWidth()
		{
			SetOperationType("MODSetMainFontOutlineWidth");
			int width = ReadVariable().IntValue();
			GameSystem.Instance.OutlineWidth = width / 100f;
			GameSystem.Instance.MainUIController.TextWindow.outlineWidth = GameSystem.Instance.OutlineWidth;
			return BurikoVariable.Null;
		}

		private BurikoVariable OperationMODSetLayerFilter()
		{
			SetOperationType("MODSetLayerFilter");
			int layer = ReadVariable().IntValue();
			int alpha = ReadVariable().IntValue();
			string color = ReadVariable().StringValue();
			MODSceneController.Filter filter;
			switch (color.ToLower())
			{
				case "":
				case "none":
					filter = MODSceneController.Filter.Identity;
					break;
				case "grayscale":
					filter = MODSceneController.Filter.Grayscale;
					break;
				case "flashback":
					filter = MODSceneController.Filter.Flashback;
					break;
				case "night":
					filter = MODSceneController.Filter.Night;
					break;
				case "sunset":
					filter = MODSceneController.Filter.Sunset;
					break;
				default:
					try
					{
						var split = color.Split(',').Select(Int32.Parse).ToArray();
						if (split.Length == 3)
						{
							filter = new MODSceneController.Filter(split[0], 0, 0, 0, split[1], 0, 0, 0, split[2], 256);
							break;
						}
						else if (split.Length == 9)
						{
							filter = new MODSceneController.Filter(split[0], split[1], split[2], split[3], split[4], split[5], split[6], split[7], split[8], 256);
							break;
						}
					}
					catch (FormatException) { }
					throw new ArgumentException("Invalid color given to MODSetLayerFilter: " + color);
			}
			filter.a = (short)alpha;
			MODSceneController.SetLayerFilter(layer, filter);
			return BurikoVariable.Null;
		}

		private PathCascadeList ReadPathCascadeFromArgs()
		{
			string nameEN = ReadVariable().StringValue();
			string nameJP = ReadVariable().StringValue();
			string[] paths = ReadVariable().StringValue().Split(':');
			return new PathCascadeList(nameEN, nameJP, paths);
		}

		public BurikoVariable OperationMODAddArtset()
		{
			SetOperationType("MODAddArtset");
			AssetManager.Instance.AddArtset(ReadPathCascadeFromArgs());
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODAddBGMset()
		{
			SetOperationType("MODAddBGMset");
			MODAudioSet.Instance.AddBGMSet(ReadPathCascadeFromArgs());
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODAddSEset()
		{
			SetOperationType("MODAddSEset");
			MODAudioSet.Instance.AddSESet(ReadPathCascadeFromArgs());
			return BurikoVariable.Null;
		}

		/// <summary>
		/// Note: Tabs are not supported for descriptions and will be stripped out.
		/// This is so that you can write multiple line strings in the game script without tabs appearing in the output.
		/// </summary>
		public BurikoVariable OperationMODAddAudioset()
		{
			SetOperationType("MODAddAudioSet");
			string nameEN = ReadVariable().StringValue();
			string descriptionEN = ReadVariable().StringValue();
			string nameJP = ReadVariable().StringValue();
			string descriptionJP = ReadVariable().StringValue();
			int altBGM = ReadVariable().IntValue();
			int altBGMflow = ReadVariable().IntValue();
			int altSE = ReadVariable().IntValue();
			int altSEFlow = ReadVariable().IntValue();
			MODAudioSet.Instance.AddAudioSet(new AudioSet(nameEN, nameJP, MODUtility.StripTabs(descriptionEN), MODUtility.StripTabs(descriptionJP), altBGM, altBGMflow, altSE, altSEFlow));

			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODClearArtsets()
		{
			SetOperationType("MODClearArtsets");
			AssetManager.Instance.ClearArtsets();
			AssetManager.Instance.ShouldSerializeArtsets = true;
			return BurikoVariable.Null;
		}

		public BurikoVariable OperationMODGenericCall()
		{
			SetOperationType("MODGenericCall");
			string callID = ReadVariable().StringValue();
			string callParameters = ReadVariable().StringValue();
			switch(callID)
			{
				case "ShowAudioSetupMenu":
					if(MODAudioSet.Instance.HasAudioSets())
					{
						GameSystem.Instance.MainUIController.modMenu.SetMode(ModMenuMode.AudioSetup);
						GameSystem.Instance.MainUIController.modMenu.Show();
					}
					break;

				default:
					Logger.Log($"WARNING: Unknown ModGenericCall ID '{callID}'");
					break;
			}
			return BurikoVariable.Null;
		}
	}
}
