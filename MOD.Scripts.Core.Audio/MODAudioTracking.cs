﻿using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOD.Scripts.Core.Audio
{
	public class MODAudioTracking
	{
		private static MODAudioTracking _instance;
		public static MODAudioTracking Instance => _instance ?? (_instance = new MODAudioTracking());

		// Dictionary of BGMFlow setting -> (Dictionary of channel -> last audio played on that channel by  MODPlayBGM functions))
		private Dictionary<int, AudioInfo>[] lastAltBGM;

		public bool LoggingEnabled { get; set; } = false;

		public MODAudioTracking()
		{
			lastAltBGM = new Dictionary<int, AudioInfo>[BurikoMemory.Instance.GetGlobalFlag("GAltBGMflowMaxNum").IntValue() + 1];
			for (int i = 0; i < lastAltBGM.Length; i++)
			{
				lastAltBGM[i] = new Dictionary<int, AudioInfo>();
			}
		}

		private void Log(string text)
		{
			if(LoggingEnabled)
			{
				Logger.Log($"[MODAudioTracking]: {text}");
			}
		}

		private bool flowInRange(int flow) => flow < lastAltBGM.Length;

		public void SaveLastBGM(AudioInfo info)
		{
			Log($"Saving bgm {info.Filename} channel {info.Channel} all flows");
			foreach (Dictionary<int, AudioInfo> channelDict in lastAltBGM)
			{
				channelDict[info.Channel] = info;
			}
		}

		public void ForgetLastBGM(int channel)
		{
			Log($"Forgetting channel {channel} all flows");
			foreach (Dictionary<int, AudioInfo> channelDict in lastAltBGM)
			{
				channelDict.Remove(channel);
			}
		}

		public void SaveLastAltBGM(int altBGMFlow, AudioInfo info)
		{
			Log($"Saving bgm {info.Filename} channel {info.Channel} flow {altBGMFlow}");
			if (flowInRange(altBGMFlow))
			{
				lastAltBGM[altBGMFlow][info.Channel] = info;
			}
		}

		public void ForgetLastAltBGM(int altBGMFlow, int channel)
		{
			Log($"Forgetting channel {channel} flow {altBGMFlow}");
			if (flowInRange(altBGMFlow))
			{
				lastAltBGM[altBGMFlow].Remove(channel);
			}
		}

		public void RestoreBGM(int oldBGMFlow, int newBGMFlow)
		{
			Log($"Begin BGM restore...");
			if (!flowInRange(oldBGMFlow) || !flowInRange(newBGMFlow))
			{
				return;
			}

			// Stop all audio at the current bgm flow
			foreach (int channel in lastAltBGM[oldBGMFlow].Keys)
			{
				Log($"Stop channel {channel}");
				AudioController.Instance.FadeOutBGM(channel, 500, false, noBGMTracking: true);
			}

			// Start all audio at the new bgm flow
			foreach (AudioInfo info in lastAltBGM[newBGMFlow].Values)
			{
				Log($"Start channel {info.Channel} file {info.Filename}");
				AudioController.Instance.PlayAudio(info.Filename, AudioType.BGM, info.Channel, info.Volume, 500, noBGMTracking: true);
			}
		}

		public static string GetBGMNameFromAltBGMFlag(int altBGMFlag)
		{
			switch(altBGMFlag)
			{
				case 0:
					return "New BGM/SE";

				case 1:
					return "Original BGM/SE";

				default:
					return $"BGM/SE {altBGMFlag}";
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < lastAltBGM.Length; i++)
			{
				if (lastAltBGM[i].Count == 0)
				{
					continue;
				}

				sb.Append($"{GetBGMNameFromAltBGMFlag(i)}\n");
				foreach (KeyValuePair<int, AudioInfo> entry in lastAltBGM[i])
				{
					sb.Append($"    {entry.Key}: {entry.Value.Filename}\n");
				}
			}

			return sb.ToString();
		}
	}
}
