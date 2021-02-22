using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOD.Scripts.Core.Audio
{
	public class MODAudio
	{
		private static MODAudio _instance;
		public static MODAudio Instance => _instance ?? (_instance = new MODAudio());

		// Dictionary of BGMFlow setting -> (Dictionary of channel -> last audio played on that channel by  MODPlayBGM functions))
		private Dictionary<int, AudioInfo>[] lastAltBGM;

		public bool LoggingEnabled { get; set; } = false;

		private void Log(string text)
		{
			if(LoggingEnabled)
			{
				Logger.Log(text);
			}
		}

		public MODAudio()
		{
			lastAltBGM = new Dictionary<int, AudioInfo>[8];
			for (int i = 0; i < lastAltBGM.Length; i++)
			{
				lastAltBGM[i] = new Dictionary<int, AudioInfo>();
			}
		}

		private bool flowInRange(int flow) => flow < lastAltBGM.Length;

		public void MODSaveLastBGM(AudioInfo info)
		{
			Log($"Saving bgm {info.Filename} channel {info.Channel} all flows");
			foreach (Dictionary<int, AudioInfo> channelDict in lastAltBGM)
			{
				channelDict[info.Channel] = info;
			}
		}

		public void MODForgetLastBGM(int channel)
		{
			Log($"Forgetting channel {channel} all flows");
			foreach (Dictionary<int, AudioInfo> channelDict in lastAltBGM)
			{
				channelDict.Remove(channel);
			}
		}

		public void MODSaveLastAltBGM(int altBGMFlow, AudioInfo info)
		{
			Log($"Saving bgm {info.Filename} channel {info.Channel} flow {altBGMFlow}");
			if (flowInRange(altBGMFlow))
			{
				lastAltBGM[altBGMFlow][info.Channel] = info;
			}
		}

		public void MODForgetLastAltBGM(int altBGMFlow, int channel)
		{
			Log($"Forgetting channel {channel} flow {altBGMFlow}");
			if (flowInRange(altBGMFlow))
			{
				lastAltBGM[altBGMFlow].Remove(channel);
			}
		}

		public void MODRestoreBGM(int oldBGMFlow, int newBGMFlow)
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
	}
}
