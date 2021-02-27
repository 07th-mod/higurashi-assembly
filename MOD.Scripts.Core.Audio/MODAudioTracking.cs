using Assets.Scripts.Core;
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
		private Dictionary<int, AudioInfo>[] queuedState;

		public bool LoggingEnabled { get; set; } = false;

		public MODAudioTracking()
		{
			lastAltBGM = new Dictionary<int, AudioInfo>[BurikoMemory.Instance.GetGlobalFlag("GAltBGMflowMaxNum").IntValue() + 1];
			for (int i = 0; i < lastAltBGM.Length; i++)
			{
				lastAltBGM[i] = new Dictionary<int, AudioInfo>();
			}
		}

		public Dictionary<int, AudioInfo>[] SerializeableState()
		{
			return lastAltBGM;
		}

		public void QueueState(Dictionary<int, AudioInfo>[] state)
		{
			this.queuedState = state;
		}

		public void RestoreState()
		{
			if(queuedState != null)
			{
				for (int i = 0; i < queuedState.Length; i++)
				{
					if (flowInRange(i))
					{
						lastAltBGM[i] = queuedState[i];
					}
				}
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

		public void SetAndSaveBGMSE(int newBGMSEValue)
		{
			BurikoMemory.Instance.SetGlobalFlag("GAltBGM", newBGMSEValue);
			BurikoMemory.Instance.SetGlobalFlag("GAltSE", newBGMSEValue);
			BurikoMemory.Instance.SetGlobalFlag("GAltBGMflow", newBGMSEValue);
			BurikoMemory.Instance.SetGlobalFlag("GAltSEflow", newBGMSEValue);
			RestoreBGM(newBGMSEValue);
		}

		private void RestoreBGM(int newBGMFlow)
		{
			Log($"Begin BGM restore...");
			if (!flowInRange(newBGMFlow))
			{
				return;
			}

			// Stop all BGM (Currently game only uses BGM channels 0-5, see AudioController.StopAllAudio())
			Log($"Stopping all BGM");
			for (int i = 0; i < 6; i++)
			{
				AudioController.Instance.FadeOutBGM(i, 500, false, noBGMTracking: true);
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
			for (int i = 0; i < lastAltBGM.Length; i++)
			{
				sb.Append($"{GetBGMNameFromAltBGMFlag(i)} (flow = {i})\n");

				if (lastAltBGM[i].Count == 0)
				{
					sb.Append("    - Nothing playing\n");
					continue;
				}

				foreach (KeyValuePair<int, AudioInfo> entry in lastAltBGM[i])
				{
					sb.Append($"    - Ch {entry.Key}: {entry.Value.Filename}\n");
				}
			}

			return sb.ToString();
		}
	}
}
